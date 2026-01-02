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
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly YouforceSupplierService _youforceSupplier;
        private readonly ErpXContractSupplierService _contractSupplier;
        private readonly ErpXBegrotingSupplierService _begrotingSupplier;
        private readonly ErpXInhuurSupplierService _inhuurSupplier;
        private readonly ErpXTransactieSupplierService _transactieSupplier;

        public ImportWorker(
            ILogger<ImportWorker> logger,
            IServiceScopeFactory scopeFactory,
            YouforceSupplierService youforceSupplier,
            ErpXContractSupplierService contractSupplier,
            ErpXBegrotingSupplierService begrotingSupplier,
            ErpXInhuurSupplierService inhuurSupplier,
            ErpXTransactieSupplierService transactieSupplier)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _youforceSupplier = youforceSupplier;
            _contractSupplier = contractSupplier;
            _begrotingSupplier = begrotingSupplier;
            _inhuurSupplier = inhuurSupplier;
            _transactieSupplier = transactieSupplier;
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
            // YOUFORCE
            // ======================================================
            foreach (var r in _youforceSupplier.Fetch())
            {
                assembler.ApplyYouforce(
                    r.EmployeeNumber,
                    r.Achternaam,
                    new DienstverbandImport
                    {
                        Nummer = r.DienstverbandNummer,
                        Functiecode = r.FunctieCode,
                        Functienaam = r.FunctieNaam,
                        Type = r.Type,
                        DatumInDienst = r.DatumInDienst,
                        DatumUitDienst = r.DatumUitDienst,
                        Ancienniteit = r.Ancienniteit
                    });
            }

            // ======================================================
            // ERP-X CONTRACTEN
            // ======================================================
            foreach (var r in _contractSupplier.Fetch())
            {
                assembler.ApplyErpXContract(r.PersNr, new ContractImport
                {
                    Crediteur = r.Crediteur,
                    Rekening = r.Rekening,
                    OrganisatorischeEenheidCode = r.OrganisatorischeEenheidCode
                });
            }

            // ======================================================
            // ERP-X BEGROTING
            // ======================================================
            foreach (var r in _begrotingSupplier.Fetch())
            {
                assembler.ApplyErpXBegroting(r.PersNr, new BegrotingsregelImport
                {
                    BegrotingJaar = r.BegrotingJaar,
                    KostenplaatsCode = r.KostenplaatsCode,
                    Bedrag = r.Bedrag,
                    Kostensoort = ParseKostensoort(r.Kostensoort)
                });
            }

            // ======================================================
            // ERP-X INHUUR
            // ======================================================
            foreach (var r in _inhuurSupplier.Fetch())
            {
                assembler.ApplyErpXInhuurkosten(r.PersNr, new InhuurkostenImport
                {
                    Jaar = r.Jaar,
                    Maand = r.Maand,
                    KostenplaatsCode = r.KostenplaatsCode,
                    Bedrag = r.Bedrag,
                    Periode = new PeriodeImport
                    {
                        Jaar = r.Jaar,
                        Maand = r.Maand,
                        Verwerking = false,
                        Label = $"{r.Jaar}-{r.Maand:D2}"
                    }
                });
            }

            // ======================================================
            // ERP-X TRANSACTIES
            // ======================================================
            foreach (var r in _transactieSupplier.Fetch())
            {
                assembler.ApplyErpXTransactie(r.PersNr, new TransactieImport
                {
                    Crediteur = r.Crediteur,
                    Rekening = r.Rekening,
                    Datum = r.Datum,
                    Bedrag = r.Bedrag
                });
            }

            var aggregates = assembler.GetComplete();

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContextWorker>();
            using var tx = await db.Database.BeginTransactionAsync(token);

            try
            {
                EnsureOrganisatorischeEenheidExists(db, "ONBEKEND");

                foreach (var agg in aggregates)
                {
                    var medewerker = PersistMedewerker(db, agg);
                    PersistContracten(db, medewerker, agg);

                    await db.SaveChangesAsync(token);

                    PersistBegrotingsregels(db, agg, medewerker.Nummer);
                    PersistInhuurkosten(db, agg);
                    PersistTransacties(db, agg, medewerker.Nummer);
                }

                await db.SaveChangesAsync(token);
                await tx.CommitAsync(token);

                _logger.LogInformation("Import succesvol afgerond");
            }
            catch
            {
                await tx.RollbackAsync(token);
                throw;
            }
        }

        // ======================================================
        // PERSIST
        // ======================================================

        private static Medewerker PersistMedewerker(DatabaseContextWorker db, MedewerkerAggregate agg)
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

            medewerker.Achternaam = agg.Achternaam;

            if (agg.Dienstverband != null)
            {
                EnsureFunctieExists(db, agg.Dienstverband.Functiecode, agg.Dienstverband.Functienaam);

                if (medewerker.Dienstverband == null)
                    medewerker.Dienstverband = new Dienstverband();

                medewerker.Dienstverband.ExternalNummer = agg.Dienstverband.Nummer;
                medewerker.Dienstverband.Functiecode = agg.Dienstverband.Functiecode;
                medewerker.Dienstverband.Type = agg.Dienstverband.Type;
                medewerker.Dienstverband.DatumInDienst = agg.Dienstverband.DatumInDienst;
                medewerker.Dienstverband.DatumUitDienst = agg.Dienstverband.DatumUitDienst;
                medewerker.Dienstverband.Ancienniteit = agg.Dienstverband.Ancienniteit;
            }

            return medewerker;
        }

        private static void PersistContracten(DatabaseContextWorker db, Medewerker medewerker, MedewerkerAggregate agg)
        {
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

        private static void PersistBegrotingsregels(DatabaseContextWorker db, MedewerkerAggregate agg, int medewerkerId)
        {
            foreach (var r in agg.Begrotingsregels)
            {
                EnsureBegrotingExists(db, r.BegrotingJaar);
                EnsureKostenplaatsExists(db, r.KostenplaatsCode, "ONBEKEND");

                db.Begrotingsregels.Add(new Begrotingsregel
                {
                    BegrotingJaar = r.BegrotingJaar,
                    MedewerkerNummer = medewerkerId,
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

                db.SaveChanges();

                EnsureKostenplaatsExists(db, i.KostenplaatsCode, "ONBEKEND");

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


        private static void PersistTransacties(DatabaseContextWorker db, MedewerkerAggregate agg, int medewerkerId)
        {
            foreach (var t in agg.Transacties)
            {
                var contract = db.Contracten.FirstOrDefault(c =>
                    c.MedewerkerNummer == medewerkerId &&
                    c.Crediteur == t.Crediteur &&
                    c.Rekening == t.Rekening);

                if (contract == null) continue;

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
        // HELPERS
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

            if (!db.OrganisatorischeEenheden.Any(o => o.Code == code))
            {
                db.OrganisatorischeEenheden.Add(new OrganisatorischeEenheid
                {
                    Code = code,
                    Omschrijving = code
                });
            }
        }

        private static void EnsureKostenplaatsExists(DatabaseContextWorker db, string? code, string oe)
        {
            if (string.IsNullOrWhiteSpace(code)) return;

            EnsureOrganisatorischeEenheidExists(db, oe);

            if (!db.Kostenplaatsen.Any(k => k.Code == code))
            {
                db.Kostenplaatsen.Add(new Kostenplaats
                {
                    Code = code,
                    Omschrijving = code,
                    OrganisatorischeEenheidCode = oe
                });
            }
        }

        private static void EnsureFunctieExists(DatabaseContextWorker db, string code, string naam)
        {
            if (!db.Functies.Any(f => f.Functiecode == code))
            {
                db.Functies.Add(new Functie
                {
                    Functiecode = code,
                    Functienaam = naam
                });
            }
        }

        private static void EnsureBegrotingExists(DatabaseContextWorker db, int jaar)
        {
            if (!db.Begrotingen.Any(b => b.Jaar == jaar))
            {
                db.Begrotingen.Add(new Begroting
                {
                    Jaar = jaar,
                    Status = BegrotingStatus.Primair,
                    Totaalbedrag = 0m
                });
            }
        }

        private static Kostensoort? ParseKostensoort(string? value)
            => value?.Trim().ToLowerInvariant() switch
            {
                "lasten" => Kostensoort.Lasten,
                "baten" => Kostensoort.Baten,
                _ => null
            };
    }
}
