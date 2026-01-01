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

            // ======================================================
            // Youforce (HR)
            // ======================================================
            foreach (var record in _youforceSupplier.Fetch())
            {
                assembler.ApplyYouforce(
                    record.EmployeeNumber,
                    record.Achternaam,
                    new DienstverbandImport
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

            // ======================================================
            // ERP-X (RecordType gestuurd)
            // ======================================================
            foreach (var record in _erpXSupplier.Fetch())
            {
                switch (record.RecordType)
                {
                    case ErpXRecordType.CONTRACT:
                        if (!string.IsNullOrWhiteSpace(record.Crediteur))
                        {
                            assembler.ApplyErpXContract(
                                record.PersNr,
                                new ContractImport
                                {
                                    Crediteur = record.Crediteur,
                                    Rekening = record.Rekening,
                                    OrganisatorischeEenheidCode = record.OrganisatorischeEenheidCode
                                });
                        }
                        break;

                    case ErpXRecordType.BEGROTING:
                        if (record.BegrotingJaar.HasValue &&
                            record.Bedrag.HasValue)
                        {
                            assembler.ApplyErpXBegroting(
                                record.PersNr,
                                new BegrotingsregelImport
                                {
                                    BegrotingJaar = record.BegrotingJaar.Value,
                                    KostenplaatsCode = record.KostenplaatsCode,
                                    Bedrag = record.Bedrag.Value,
                                    Kostensoort = ParseKostensoort(record.Kostensoort)
                                });
                        }
                        break;

                    case ErpXRecordType.INHUUR:
                        if (record.InhuurJaar.HasValue &&
                            record.InhuurMaand.HasValue &&
                            record.InhuurBedrag.HasValue &&
                            !string.IsNullOrWhiteSpace(record.KostenplaatsCode))
                        {
                            assembler.ApplyErpXInhuurkosten(
                                record.PersNr,
                                new InhuurkostenImport
                                {
                                    Jaar = record.InhuurJaar.Value,
                                    Maand = record.InhuurMaand.Value,
                                    KostenplaatsCode = record.KostenplaatsCode,
                                    Bedrag = record.InhuurBedrag.Value,
                                    Periode = new PeriodeImport
                                    {
                                        Jaar = record.InhuurJaar.Value,
                                        Maand = record.InhuurMaand.Value,
                                        Verwerking = false,
                                        Label = $"{record.InhuurJaar}-{record.InhuurMaand:D2}"
                                    }
                                });
                        }
                        break;

                    case ErpXRecordType.TRANSACTIE:
                        if (record.TransactieDatum.HasValue &&
                            record.TransactieBedrag.HasValue &&
                            !string.IsNullOrWhiteSpace(record.Crediteur))
                        {
                            assembler.ApplyErpXTransactie(
                                record.PersNr,
                                new TransactieImport
                                {
                                    Crediteur = record.Crediteur,
                                    Rekening = record.Rekening,
                                    Datum = record.TransactieDatum.Value,
                                    Bedrag = record.TransactieBedrag.Value
                                });
                        }
                        break;
                }
            }

            var aggregates = assembler.GetComplete();

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContextWorker>();
            using var transaction = await dbContext.Database.BeginTransactionAsync(token);

            try
            {
                foreach (var agg in aggregates)
                {
                    PersistMedewerker(dbContext, agg);
                    PersistBegrotingsregels(dbContext, agg);
                    PersistInhuurkosten(dbContext, agg);
                    PersistTransacties(dbContext, agg);
                }

                await dbContext.SaveChangesAsync(token);
                await transaction.CommitAsync(token);

                _logger.LogInformation("Import succesvol afgerond");
            }
            catch
            {
                await transaction.RollbackAsync(token);
                throw;
            }
        }

        // ======================================================
        // Persist methods
        // ======================================================
        private static void PersistMedewerker(DatabaseContextWorker db, MedewerkerAggregate agg)
        {
            var medewerker = db.Medewerkers
                .Include(m => m.Dienstverband)
                .Include(m => m.Contracten)
                .FirstOrDefault(m => m.ExternalNummer == agg.Nummer);

            if (medewerker == null)
            {
                medewerker = new Medewerker { ExternalNummer = agg.Nummer };
                db.Medewerkers.Add(medewerker);
            }

            medewerker.Achternaam = agg.Achternaam!;

            if (medewerker.Dienstverband == null)
                medewerker.Dienstverband = new Dienstverband
                {
                    ExternalNummer = agg.Dienstverband!.Nummer
                };

            EnsureFunctieExists(db, agg.Dienstverband!.Functiecode, agg.Dienstverband.Functienaam);

            medewerker.Dienstverband.Functiecode = agg.Dienstverband.Functiecode;
            medewerker.Dienstverband.Type = agg.Dienstverband.Type;
            medewerker.Dienstverband.DatumInDienst = agg.Dienstverband.DatumInDienst;
            medewerker.Dienstverband.DatumUitDienst = agg.Dienstverband.DatumUitDienst;
            medewerker.Dienstverband.Ancienniteit = agg.Dienstverband.Ancienniteit;

            foreach (var c in agg.Contracten)
            {
                EnsureOrganisatorischeEenheidExists(db, c.OrganisatorischeEenheidCode);

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

        private static void PersistBegrotingsregels(DatabaseContextWorker db, MedewerkerAggregate agg)
        {
            foreach (var r in agg.Begrotingsregels)
            {
                EnsureBegrotingExists(db, r.BegrotingJaar);
                EnsureKostenplaatsExists(db, r.KostenplaatsCode);

                db.Begrotingsregels.Add(new Begrotingsregel
                {
                    BegrotingJaar = r.BegrotingJaar,
                    MedewerkerNummer = agg.Nummer,
                    KostenplaatsCode = r.KostenplaatsCode,
                    Bedrag = r.Bedrag,
                    Kostensoort = r.Kostensoort ?? Kostensoort.Lasten
                });
            }
        }

        private static void PersistInhuurkosten(DatabaseContextWorker db, MedewerkerAggregate agg)
        {
            foreach (var i in agg.Inhuurkosten)
            {
                var periode = EnsurePeriodeExists(db, i.Jaar, i.Maand, i.Periode);
                EnsureKostenplaatsExists(db, i.KostenplaatsCode);

                if (!db.Inhuurkosten.Any(x =>
                    x.PeriodeId == periode.Id &&
                    x.KostenplaatsCode == i.KostenplaatsCode &&
                    x.Bedrag == i.Bedrag))
                {
                    db.Inhuurkosten.Add(new Inhuurkosten
                    {
                        PeriodeId = periode.Id,
                        KostenplaatsCode = i.KostenplaatsCode,
                        Bedrag = i.Bedrag
                    });
                }
            }
        }

        private static void PersistTransacties(DatabaseContextWorker db, MedewerkerAggregate agg)
        {
            foreach (var t in agg.Transacties)
            {
                var contract = db.Contracten.FirstOrDefault(c =>
                    c.MedewerkerNummer == agg.Nummer &&
                    c.Crediteur == t.Crediteur &&
                    c.Rekening == t.Rekening);

                if (contract == null)
                    continue;

                if (!db.Transacties.Any(x =>
                    x.ContractId == contract.Id &&
                    x.Datum == t.Datum &&
                    x.Bedrag == t.Bedrag))
                {
                    db.Transacties.Add(new Transactie
                    {
                        ContractId = contract.Id,
                        Datum = t.Datum,
                        Bedrag = t.Bedrag
                    });
                }
            }
        }

        // ======================================================
        // Helpers
        // ======================================================
        private static Periode EnsurePeriodeExists(DatabaseContextWorker db, int jaar, int maand, PeriodeImport? import)
        {
            var periode = db.Periodes.FirstOrDefault(p => p.Jaar == jaar && p.Maand == maand);
            if (periode != null) return periode;

            periode = new Periode
            {
                Jaar = jaar,
                Maand = maand,
                Verwerking = import?.Verwerking ?? false,
                Label = import?.Label
            };

            db.Periodes.Add(periode);
            return periode;
        }

        private static void EnsureOrganisatorischeEenheidExists(DatabaseContextWorker db, string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return;

            if (db.OrganisatorischeEenheden.Local.Any(o => o.Code == code)) return;

            if (db.OrganisatorischeEenheden.AsNoTracking().Any(o => o.Code == code))
                db.OrganisatorischeEenheden.Attach(new OrganisatorischeEenheid { Code = code });
            else
                db.OrganisatorischeEenheden.Add(new OrganisatorischeEenheid
                {
                    Code = code,
                    Omschrijving = code
                });
        }

        private static void EnsureKostenplaatsExists(DatabaseContextWorker db, string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return;

            if (db.Kostenplaatsen.Local.Any(k => k.Code == code)) return;

            if (db.Kostenplaatsen.AsNoTracking().Any(k => k.Code == code))
                db.Kostenplaatsen.Attach(new Kostenplaats { Code = code });
            else
                db.Kostenplaatsen.Add(new Kostenplaats
                {
                    Code = code,
                    Omschrijving = code,
                    OrganisatorischeEenheidCode = "ONBEKEND"
                });
        }

        private static void EnsureFunctieExists(DatabaseContextWorker db, string code, string? naam)
        {
            if (db.Functies.Local.Any(f => f.Functiecode == code)) return;

            if (db.Functies.AsNoTracking().Any(f => f.Functiecode == code))
                db.Functies.Attach(new Functie { Functiecode = code });
            else
                db.Functies.Add(new Functie
                {
                    Functiecode = code,
                    Functienaam = naam ?? code
                });
        }

        private static void EnsureBegrotingExists(DatabaseContextWorker db, int jaar)
        {
            if (db.Begrotingen.Local.Any(b => b.Jaar == jaar)) return;

            if (db.Begrotingen.AsNoTracking().Any(b => b.Jaar == jaar))
                db.Begrotingen.Attach(new Begroting { Jaar = jaar });
            else
                db.Begrotingen.Add(new Begroting
                {
                    Jaar = jaar,
                    Status = BegrotingStatus.Primair,
                    Totaalbedrag = 0m
                });
        }

        private static Kostensoort? ParseKostensoort(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            return value.Trim().ToLower() switch
            {
                "lasten" => Kostensoort.Lasten,
                "baten" => Kostensoort.Baten,
                _ => null
            };
        }
    }
}
