using ApiService.Models;
using Microsoft.EntityFrameworkCore;
using WorkerService.Assembly;
using WorkerService.DAL;
using WorkerService.ImportDomain;
using WorkerService.Suppliers.ErpX;
using WorkerService.Suppliers.Youforce;

namespace WorkerService.Worker
{
    public class ImportWorker : BackgroundService
    {
        private readonly ILogger<ImportWorker> _logger;
        private readonly YouforceSupplierService _youforceSupplier;
        private readonly ErpXSupplierService _erpXSupplier;
        private readonly IServiceScopeFactory _scopeFactory;

        public ImportWorker(
            ILogger<ImportWorker> logger,
            YouforceSupplierService youforceSupplier,
            ErpXSupplierService erpXSupplier,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _youforceSupplier = youforceSupplier;
            _erpXSupplier = erpXSupplier;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ImportWorker gestart");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunImport(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Onverwachte fout in ImportWorker");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task RunImport(CancellationToken token)
        {
            _logger.LogInformation("Start import-run");

            var assembler = new MedewerkerAssembler();

            // Youforce 
            foreach (var record in _youforceSupplier.Fetch())
            {
                assembler.ApplyYouforce(
                    medewerkerNummer: record.EmployeeNumber,
                    achternaam: record.Achternaam,
                    dienstverband: new DienstverbandImport
                    {
                        Nummer = record.DienstverbandNummer,
                        Functiecode = record.FunctieCode,
                        Functienaam = record.FunctieNaam,
                        Type = record.Type,
                        DatumInDienst = record.DatumInDienst,
                        DatumUitDienst = record.DatumUitDienst,
                        Ancienniteit = record.Ancienniteit
                    });
            }

            _logger.LogInformation("Youforce verwerkt");

            // ERP-X 
            foreach (var record in _erpXSupplier.Fetch())
            {
                assembler.ApplyErpXContract(
                    medewerkerNummer: record.PersNr,
                    new ContractImport
                    {
                        Crediteur = record.Crediteur,
                        Rekening = record.Rekening,
                        OrganisatorischeEenheidCode = record.OrganisatorischeEenheidCode
                    });

                if (record.BegrotingJaar.HasValue)
                {
                    assembler.ApplyErpXBegroting(
                        medewerkerNummer: record.PersNr,
                        new BegrotingsregelImport
                        {
                            Jaar = record.BegrotingJaar.Value,
                            KostenplaatsCode = record.KostenplaatsCode,
                            Bedrag = record.Bedrag,
                            Kostensoort = record.Kostensoort
                        });
                }
            }

            _logger.LogInformation("ERP-X verwerkt");

            // Alleen COMPLETE medewerkers
            var completeAggregates = assembler.GetComplete();

            _logger.LogInformation(
                "Aantal complete medewerkers: {Count}",
                completeAggregates.Count);

            // Persist naar database (scoped)
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider
                .GetRequiredService<DatabaseContextWorker>();

            using var transaction = await dbContext.Database
                .BeginTransactionAsync(token);

            try
            {
                foreach (var aggregate in completeAggregates)
                {
                    PersistMedewerker(dbContext, aggregate);
                }

                await dbContext.SaveChangesAsync(token);
                await transaction.CommitAsync(token);

                _logger.LogInformation("Database succesvol bijgewerkt");
            }
            catch
            {
                await transaction.RollbackAsync(token);
                throw;
            }
        }

        // Mapping ImportAggregate naar EF entities
        private static void PersistMedewerker(
            DatabaseContextWorker dbContext,
            MedewerkerAggregate agg)
        {
            // Zoek op ExternalNummer 
            var medewerker = dbContext.Medewerkers
                .Include(m => m.Dienstverband)
                .Include(m => m.Contracten)
                .FirstOrDefault(m => m.ExternalNummer == agg.Nummer);

            // Bestaat nog niet = aanmaken
            if (medewerker == null)
            {
                medewerker = new Medewerker
                {
                    ExternalNummer = agg.Nummer
                };

                dbContext.Medewerkers.Add(medewerker);
            }

            // Basisvelden updaten
            medewerker.Achternaam = agg.Achternaam!;

            // Dienstverband (1-op-1)
            if (medewerker.Dienstverband == null)
            {
                medewerker.Dienstverband = new Dienstverband
                {
                    ExternalNummer = agg.Dienstverband!.Nummer
                };
            }

            medewerker.Dienstverband.Functiecode = agg.Dienstverband!.Functiecode;
            medewerker.Dienstverband.Type = agg.Dienstverband.Type;
            medewerker.Dienstverband.DatumInDienst = agg.Dienstverband.DatumInDienst;
            medewerker.Dienstverband.DatumUitDienst = agg.Dienstverband.DatumUitDienst;
            medewerker.Dienstverband.Ancienniteit = agg.Dienstverband.Ancienniteit;

            // Contracten (optioneel idempotent maken)
            foreach (var c in agg.Contracten)
            {
                if (!medewerker.Contracten.Any(x =>
                    x.Crediteur == c.Crediteur &&
                    x.Rekening == c.Rekening))
                {
                    medewerker.Contracten.Add(new Contract
                    {
                        Crediteur = c.Crediteur,
                        Rekening = c.Rekening,
                        OrganisatorischeEenheidCode = c.OrganisatorischeEenheidCode
                    });
                }
            }
        }
    }
}
