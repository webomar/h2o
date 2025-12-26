using ApiService.DAL;
using ApiService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ApiService
{
    public class DataSeeder
    {
        public static async Task SeedAsync(IConfiguration configuration, bool clearExisting = false)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContextApi>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("SqlDatabaseString"));

            using var context = new DatabaseContextApi(optionsBuilder.Options);

            // Check if data already exists
            bool hasData = await context.Medewerkers.AnyAsync() || 
                          await context.Functies.AnyAsync() || 
                          await context.Begrotingen.AnyAsync();
            
            if (hasData)
            {
                if (clearExisting)
                {
                    Console.WriteLine("🗑️  Clearing existing data...");
                    // Delete in reverse order of dependencies
                    context.Transacties.RemoveRange(context.Transacties);
                    context.Inhuurkosten.RemoveRange(context.Inhuurkosten);
                    context.Begrotingsregels.RemoveRange(context.Begrotingsregels);
                    context.Contracten.RemoveRange(context.Contracten);
                    context.Dienstverbanden.RemoveRange(context.Dienstverbanden);
                    context.Kostenplaatsen.RemoveRange(context.Kostenplaatsen);
                    context.OrganisatorischeEenheden.RemoveRange(context.OrganisatorischeEenheden);
                    context.Periodes.RemoveRange(context.Periodes);
                    context.Begrotingen.RemoveRange(context.Begrotingen);
                    context.Medewerkers.RemoveRange(context.Medewerkers);
                    context.Functies.RemoveRange(context.Functies);
                    await context.SaveChangesAsync();
                    
                    // Reset identity seeds after clearing
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Medewerkers', RESEED, 0)");
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Dienstverbanden', RESEED, 0)");
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Begrotingen', RESEED, 0)");
                    
                    Console.WriteLine("✅ Existing data cleared and identity seeds reset.");
                }
                else
                {
                    Console.WriteLine("⚠️ Database already contains data. Skipping seed.");
                    Console.WriteLine("   To reseed, run with clearExisting=true or clear the database manually.");
                    return;
                }
            }

            Console.WriteLine("🌱 Seeding database with demo data...");

            // 1. Create Functies (Functions)
            var functies = new List<Functie>
            {
                new Functie { Functiecode = "DEV", Functienaam = "Software Developer", Schaall = "10" },
                new Functie { Functiecode = "MGR", Functienaam = "Manager", Schaall = "12" },
                new Functie { Functiecode = "HR", Functienaam = "HR Specialist", Schaall = "9" },
                new Functie { Functiecode = "FIN", Functienaam = "Financial Analyst", Schaall = "11" },
                new Functie { Functiecode = "ADM", Functienaam = "Administrator", Schaall = "8" }
            };
            context.Functies.AddRange(functies);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Created {0} Functies", functies.Count);

            // 2. Create Medewerkers (Employees)
            // Use raw SQL to ensure IDENTITY_INSERT works on the same connection
            var medewerkersSql = @"
                SET IDENTITY_INSERT Medewerkers ON;
                INSERT INTO Medewerkers (Nummer, Achternaam) VALUES
                (1, 'Jansen'),
                (2, 'De Vries'),
                (3, 'Bakker'),
                (4, 'Visser'),
                (5, 'Smit'),
                (6, 'Meijer'),
                (7, 'De Boer'),
                (8, 'Mulder');
                SET IDENTITY_INSERT Medewerkers OFF;";
            
            await context.Database.ExecuteSqlRawAsync(medewerkersSql);
            Console.WriteLine("✅ Created 8 Medewerkers");

            // 3. Create Dienstverbanden (Employment records)
            var dienstverbandenSql = @"
                SET IDENTITY_INSERT Dienstverbanden ON;
                INSERT INTO Dienstverbanden (Nummer, MedewerkerNummer, Functiecode, Type, DatumInDienst, DatumUitDienst, Ancienniteit) VALUES
                (1, 1, 'DEV', 'Vast', '2020-01-01', NULL, 5),
                (2, 2, 'DEV', 'Vast', '2021-03-15', NULL, 4),
                (3, 3, 'MGR', 'Vast', '2019-06-01', NULL, 6),
                (4, 4, 'HR', 'Vast', '2022-01-10', NULL, 3),
                (5, 5, 'FIN', 'Vast', '2021-09-01', NULL, 4),
                (6, 6, 'ADM', 'Tijdelijk', '2023-01-01', NULL, 2),
                (7, 7, 'DEV', 'Vast', '2022-05-01', NULL, 3),
                (8, 8, 'MGR', 'Vast', '2020-11-01', NULL, 5);
                SET IDENTITY_INSERT Dienstverbanden OFF;";
            
            await context.Database.ExecuteSqlRawAsync(dienstverbandenSql);
            Console.WriteLine("✅ Created 8 Dienstverbanden");

            // 4. Create OrganisatorischeEenheden (Gemeentes)
            var orgEenheden = new List<OrganisatorischeEenheid>
            {
                new OrganisatorischeEenheid { Code = "HEERDE", Omschrijving = "Gemeente Heerde", ParentCode = null },
                new OrganisatorischeEenheid { Code = "OLDEBROEK", Omschrijving = "Gemeente Oldebroek", ParentCode = null },
                new OrganisatorischeEenheid { Code = "HATTEM", Omschrijving = "Gemeente Hattem", ParentCode = null }
            };
            context.OrganisatorischeEenheden.AddRange(orgEenheden);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Created {0} OrganisatorischeEenheden (Gemeentes)", orgEenheden.Count);

            // 5. Create Kostenplaatsen (Cost Centers) per gemeente
            var kostenplaatsen = new List<Kostenplaats>
            {
                // Heerde
                new Kostenplaats { Code = "HEERDE-001", Omschrijving = "Heerde - IT & Digitalisering", OrganisatorischeEenheidCode = "HEERDE" },
                new Kostenplaats { Code = "HEERDE-002", Omschrijving = "Heerde - HR & Personeel", OrganisatorischeEenheidCode = "HEERDE" },
                new Kostenplaats { Code = "HEERDE-003", Omschrijving = "Heerde - Financiën", OrganisatorischeEenheidCode = "HEERDE" },
                new Kostenplaats { Code = "HEERDE-004", Omschrijving = "Heerde - Algemene Zaken", OrganisatorischeEenheidCode = "HEERDE" },
                // Oldebroek
                new Kostenplaats { Code = "OLDEBROEK-001", Omschrijving = "Oldebroek - IT & Digitalisering", OrganisatorischeEenheidCode = "OLDEBROEK" },
                new Kostenplaats { Code = "OLDEBROEK-002", Omschrijving = "Oldebroek - HR & Personeel", OrganisatorischeEenheidCode = "OLDEBROEK" },
                new Kostenplaats { Code = "OLDEBROEK-003", Omschrijving = "Oldebroek - Financiën", OrganisatorischeEenheidCode = "OLDEBROEK" },
                new Kostenplaats { Code = "OLDEBROEK-004", Omschrijving = "Oldebroek - Algemene Zaken", OrganisatorischeEenheidCode = "OLDEBROEK" },
                // Hattem
                new Kostenplaats { Code = "HATTEM-001", Omschrijving = "Hattem - IT & Digitalisering", OrganisatorischeEenheidCode = "HATTEM" },
                new Kostenplaats { Code = "HATTEM-002", Omschrijving = "Hattem - HR & Personeel", OrganisatorischeEenheidCode = "HATTEM" },
                new Kostenplaats { Code = "HATTEM-003", Omschrijving = "Hattem - Financiën", OrganisatorischeEenheidCode = "HATTEM" },
                new Kostenplaats { Code = "HATTEM-004", Omschrijving = "Hattem - Algemene Zaken", OrganisatorischeEenheidCode = "HATTEM" }
            };
            context.Kostenplaatsen.AddRange(kostenplaatsen);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Created {0} Kostenplaatsen", kostenplaatsen.Count);

            // 6. Create Periodes (Periods) - Q1 t/m Q4 2024
            var periodes = new List<Periode>
            {
                // Q1 2024
                new Periode { Jaar = 2024, Maand = 1, Verwerking = true, Label = "Januari 2024" },
                new Periode { Jaar = 2024, Maand = 2, Verwerking = true, Label = "Februari 2024" },
                new Periode { Jaar = 2024, Maand = 3, Verwerking = true, Label = "Maart 2024" },
                // Q2 2024
                new Periode { Jaar = 2024, Maand = 4, Verwerking = true, Label = "April 2024" },
                new Periode { Jaar = 2024, Maand = 5, Verwerking = true, Label = "Mei 2024" },
                new Periode { Jaar = 2024, Maand = 6, Verwerking = true, Label = "Juni 2024" },
                // Q3 2024
                new Periode { Jaar = 2024, Maand = 7, Verwerking = true, Label = "Juli 2024" },
                new Periode { Jaar = 2024, Maand = 8, Verwerking = true, Label = "Augustus 2024" },
                new Periode { Jaar = 2024, Maand = 9, Verwerking = true, Label = "September 2024" },
                // Q4 2024
                new Periode { Jaar = 2024, Maand = 10, Verwerking = false, Label = "Oktober 2024" },
                new Periode { Jaar = 2024, Maand = 11, Verwerking = false, Label = "November 2024" },
                new Periode { Jaar = 2024, Maand = 12, Verwerking = false, Label = "December 2024" }
            };
            context.Periodes.AddRange(periodes);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Created {0} Periodes", periodes.Count);

            // 7. Create Inhuurkosten (Hiring Costs) per gemeente
            // Mix van onder en boven budget voor realistische scenario's
            var inhuurkosten = new List<Inhuurkosten>
            {
                // Heerde - Q1 2024
                // HEERDE-001: begroot 115.000 (60k + 55k), gerealiseerd 35.000 (onder budget)
                new Inhuurkosten { PeriodeId = periodes[0].Id, KostenplaatsCode = "HEERDE-001", Bedrag = 35000.00m },
                // HEERDE-002: begroot 50.000, gerealiseerd 12.000 (onder budget)
                new Inhuurkosten { PeriodeId = periodes[0].Id, KostenplaatsCode = "HEERDE-002", Bedrag = 12000.00m },
                // HEERDE-003: begroot 65.000, gerealiseerd 70.000 (BOVEN budget - onverwachte kosten)
                new Inhuurkosten { PeriodeId = periodes[0].Id, KostenplaatsCode = "HEERDE-003", Bedrag = 70000.00m },
                
                // Oldebroek - Q1 2024
                // OLDEBROEK-001: begroot 80.000, gerealiseerd 45.000 (onder budget)
                new Inhuurkosten { PeriodeId = periodes[0].Id, KostenplaatsCode = "OLDEBROEK-001", Bedrag = 45000.00m },
                // OLDEBROEK-002: begroot 40.000, gerealiseerd 42.000 (BOVEN budget)
                new Inhuurkosten { PeriodeId = periodes[0].Id, KostenplaatsCode = "OLDEBROEK-002", Bedrag = 42000.00m },
                
                // Hattem - Q1 2024
                // HATTEM-001: begroot 143.000 (58k + 85k), gerealiseerd 155.000 (BOVEN budget - project overschrijding)
                new Inhuurkosten { PeriodeId = periodes[0].Id, KostenplaatsCode = "HATTEM-001", Bedrag = 155000.00m },
                
                // Heerde - Q2 2024
                // HEERDE-001: gerealiseerd 40.000 (onder budget)
                new Inhuurkosten { PeriodeId = periodes[1].Id, KostenplaatsCode = "HEERDE-001", Bedrag = 40000.00m },
                // HEERDE-003: gerealiseerd 18.000 (onder budget)
                new Inhuurkosten { PeriodeId = periodes[1].Id, KostenplaatsCode = "HEERDE-003", Bedrag = 18000.00m },
                // HEERDE-004: geen begroting, maar gerealiseerd 8.000 (extra kosten)
                new Inhuurkosten { PeriodeId = periodes[1].Id, KostenplaatsCode = "HEERDE-004", Bedrag = 8000.00m },
                
                // Oldebroek - Q2 2024
                // OLDEBROEK-001: gerealiseerd 50.000 (onder budget)
                new Inhuurkosten { PeriodeId = periodes[1].Id, KostenplaatsCode = "OLDEBROEK-001", Bedrag = 50000.00m },
                // OLDEBROEK-002: gerealiseerd 22.000 (onder budget)
                new Inhuurkosten { PeriodeId = periodes[1].Id, KostenplaatsCode = "OLDEBROEK-002", Bedrag = 22000.00m },
                
                // Hattem - Q2 2024
                // HATTEM-001: gerealiseerd 75.000 (onder budget)
                new Inhuurkosten { PeriodeId = periodes[1].Id, KostenplaatsCode = "HATTEM-001", Bedrag = 75000.00m },
                // HATTEM-002: geen begroting, maar gerealiseerd 15.000 (extra project)
                new Inhuurkosten { PeriodeId = periodes[1].Id, KostenplaatsCode = "HATTEM-002", Bedrag = 15000.00m },
                
                // Heerde - Q3 2024
                // HEERDE-001: gerealiseerd 45.000 (onder budget)
                new Inhuurkosten { PeriodeId = periodes[2].Id, KostenplaatsCode = "HEERDE-001", Bedrag = 45000.00m },
                // HEERDE-002: gerealiseerd 55.000 (BOVEN budget - extra werving)
                new Inhuurkosten { PeriodeId = periodes[2].Id, KostenplaatsCode = "HEERDE-002", Bedrag = 55000.00m },
                
                // Oldebroek - Q3 2024
                // OLDEBROEK-001: gerealiseerd 90.000 (BOVEN budget - project overschrijding)
                new Inhuurkosten { PeriodeId = periodes[2].Id, KostenplaatsCode = "OLDEBROEK-001", Bedrag = 90000.00m },
                // OLDEBROEK-003: geen begroting, maar gerealiseerd 12.000
                new Inhuurkosten { PeriodeId = periodes[2].Id, KostenplaatsCode = "OLDEBROEK-003", Bedrag = 12000.00m },
                
                // Hattem - Q3 2024
                // HATTEM-001: gerealiseerd 80.000 (onder budget)
                new Inhuurkosten { PeriodeId = periodes[2].Id, KostenplaatsCode = "HATTEM-001", Bedrag = 80000.00m },
                // HATTEM-003: geen begroting, maar gerealiseerd 10.000
                new Inhuurkosten { PeriodeId = periodes[2].Id, KostenplaatsCode = "HATTEM-003", Bedrag = 10000.00m },
                
                // Q4 2024 (oktober = periode index 9)
                // Heerde - Q4 2024
                new Inhuurkosten { PeriodeId = periodes[9].Id, KostenplaatsCode = "HEERDE-001", Bedrag = 38000.00m },
                new Inhuurkosten { PeriodeId = periodes[9].Id, KostenplaatsCode = "HEERDE-003", Bedrag = 20000.00m },
                // Oldebroek - Q4 2024
                new Inhuurkosten { PeriodeId = periodes[9].Id, KostenplaatsCode = "OLDEBROEK-001", Bedrag = 48000.00m },
                // Hattem - Q4 2024
                new Inhuurkosten { PeriodeId = periodes[9].Id, KostenplaatsCode = "HATTEM-001", Bedrag = 72000.00m }
            };
            context.Inhuurkosten.AddRange(inhuurkosten);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Created {0} Inhuurkosten", inhuurkosten.Count);

            // 8. Create Begrotingen (Budgets)
            var begrotingenSql = @"
                SET IDENTITY_INSERT Begrotingen ON;
                INSERT INTO Begrotingen (Jaar, Status, Totaalbedrag) VALUES
                (2024, 0, 500000.00),
                (2023, 1, 450000.00),
                (2025, 0, 550000.00);
                SET IDENTITY_INSERT Begrotingen OFF;";
            
            await context.Database.ExecuteSqlRawAsync(begrotingenSql);
            Console.WriteLine("✅ Created 3 Begrotingen");

            // 9. Create Begrotingsregels (Budget Line Items) per gemeente
            var begrotingsregels = new List<Begrotingsregel>
            {
                // Heerde - 2024 Lasten
                new Begrotingsregel { BegrotingJaar = 2024, MedewerkerNummer = 1, KostenplaatsCode = "HEERDE-001", Kostensoort = Kostensoort.Lasten, Bedrag = 60000.00m },
                new Begrotingsregel { BegrotingJaar = 2024, MedewerkerNummer = 2, KostenplaatsCode = "HEERDE-001", Kostensoort = Kostensoort.Lasten, Bedrag = 55000.00m },
                new Begrotingsregel { BegrotingJaar = 2024, MedewerkerNummer = 4, KostenplaatsCode = "HEERDE-002", Kostensoort = Kostensoort.Lasten, Bedrag = 50000.00m },
                new Begrotingsregel { BegrotingJaar = 2024, MedewerkerNummer = 5, KostenplaatsCode = "HEERDE-003", Kostensoort = Kostensoort.Lasten, Bedrag = 65000.00m },
                // Oldebroek - 2024 Lasten
                new Begrotingsregel { BegrotingJaar = 2024, MedewerkerNummer = 3, KostenplaatsCode = "OLDEBROEK-001", Kostensoort = Kostensoort.Lasten, Bedrag = 80000.00m },
                new Begrotingsregel { BegrotingJaar = 2024, MedewerkerNummer = 6, KostenplaatsCode = "OLDEBROEK-002", Kostensoort = Kostensoort.Lasten, Bedrag = 40000.00m },
                // Hattem - 2024 Lasten
                new Begrotingsregel { BegrotingJaar = 2024, MedewerkerNummer = 7, KostenplaatsCode = "HATTEM-001", Kostensoort = Kostensoort.Lasten, Bedrag = 58000.00m },
                new Begrotingsregel { BegrotingJaar = 2024, MedewerkerNummer = 8, KostenplaatsCode = "HATTEM-001", Kostensoort = Kostensoort.Lasten, Bedrag = 85000.00m },
                // Heerde - 2024 Baten
                new Begrotingsregel { BegrotingJaar = 2024, KostenplaatsCode = "HEERDE-001", Kostensoort = Kostensoort.Baten, Bedrag = 200000.00m },
                new Begrotingsregel { BegrotingJaar = 2024, KostenplaatsCode = "HEERDE-004", Kostensoort = Kostensoort.Baten, Bedrag = 50000.00m },
                // Oldebroek - 2024 Baten
                new Begrotingsregel { BegrotingJaar = 2024, KostenplaatsCode = "OLDEBROEK-001", Kostensoort = Kostensoort.Baten, Bedrag = 150000.00m },
                new Begrotingsregel { BegrotingJaar = 2024, KostenplaatsCode = "OLDEBROEK-003", Kostensoort = Kostensoort.Baten, Bedrag = 80000.00m },
                // Hattem - 2024 Baten
                new Begrotingsregel { BegrotingJaar = 2024, KostenplaatsCode = "HATTEM-001", Kostensoort = Kostensoort.Baten, Bedrag = 120000.00m },
                // 2025 Lasten
                new Begrotingsregel { BegrotingJaar = 2025, MedewerkerNummer = 1, KostenplaatsCode = "HEERDE-001", Kostensoort = Kostensoort.Lasten, Bedrag = 62000.00m },
                new Begrotingsregel { BegrotingJaar = 2025, MedewerkerNummer = 3, KostenplaatsCode = "OLDEBROEK-001", Kostensoort = Kostensoort.Lasten, Bedrag = 82000.00m },
                new Begrotingsregel { BegrotingJaar = 2025, MedewerkerNummer = 7, KostenplaatsCode = "HATTEM-001", Kostensoort = Kostensoort.Lasten, Bedrag = 60000.00m }
            };
            context.Begrotingsregels.AddRange(begrotingsregels);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Created {0} Begrotingsregels", begrotingsregels.Count);

            // 10. Create Contracten (Contracts) per gemeente
            var contracten = new List<Contract>
            {
                // Heerde
                new Contract { Crediteur = "Tech Solutions BV", MedewerkerNummer = 1, OrganisatorischeEenheidCode = "HEERDE", Rekening = "NL91ABNA0417164300" },
                new Contract { Crediteur = "Cloud Services Inc", MedewerkerNummer = 2, OrganisatorischeEenheidCode = "HEERDE", Rekening = "NL12INGB0001234567" },
                new Contract { Crediteur = "HR Consultancy Heerde", MedewerkerNummer = 4, OrganisatorischeEenheidCode = "HEERDE", Rekening = "NL34RABO0123456789" },
                new Contract { Crediteur = "Finance Partners Heerde", MedewerkerNummer = 5, OrganisatorischeEenheidCode = "HEERDE", Rekening = "NL56ABNA0987654321" },
                // Oldebroek
                new Contract { Crediteur = "IT Services Oldebroek", MedewerkerNummer = 3, OrganisatorischeEenheidCode = "OLDEBROEK", Rekening = "NL78RABO0123456789" },
                new Contract { Crediteur = "Administratie Oldebroek", MedewerkerNummer = 6, OrganisatorischeEenheidCode = "OLDEBROEK", Rekening = "NL90ABNA0123456789" },
                // Hattem
                new Contract { Crediteur = "Digital Solutions Hattem", MedewerkerNummer = 7, OrganisatorischeEenheidCode = "HATTEM", Rekening = "NL12RABO0123456789" },
                new Contract { Crediteur = "Management Services Hattem", MedewerkerNummer = 8, OrganisatorischeEenheidCode = "HATTEM", Rekening = "NL34INGB0123456789" },
                new Contract { Crediteur = "General Supplies Hattem", OrganisatorischeEenheidCode = "HATTEM", Rekening = "NL56ABNA0123456789" }
            };
            context.Contracten.AddRange(contracten);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Created {0} Contracten", contracten.Count);

            // 11. Create Transacties (Transactions)
            var transacties = new List<Transactie>
            {
                new Transactie { ContractId = contracten[0].Id, Datum = new DateTime(2024, 1, 15), Bedrag = 5000.00m },
                new Transactie { ContractId = contracten[0].Id, Datum = new DateTime(2024, 2, 15), Bedrag = 5000.00m },
                new Transactie { ContractId = contracten[0].Id, Datum = new DateTime(2024, 3, 15), Bedrag = 5000.00m },
                new Transactie { ContractId = contracten[1].Id, Datum = new DateTime(2024, 1, 20), Bedrag = 3500.00m },
                new Transactie { ContractId = contracten[1].Id, Datum = new DateTime(2024, 2, 20), Bedrag = 3500.00m },
                new Transactie { ContractId = contracten[2].Id, Datum = new DateTime(2024, 1, 10), Bedrag = 2500.00m },
                new Transactie { ContractId = contracten[2].Id, Datum = new DateTime(2024, 2, 10), Bedrag = 2500.00m },
                new Transactie { ContractId = contracten[3].Id, Datum = new DateTime(2024, 1, 5), Bedrag = 4000.00m },
                new Transactie { ContractId = contracten[4].Id, Datum = new DateTime(2024, 1, 25), Bedrag = 1200.00m },
                new Transactie { ContractId = contracten[4].Id, Datum = new DateTime(2024, 2, 25), Bedrag = 1200.00m }
            };
            context.Transacties.AddRange(transacties);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Created {0} Transacties", transacties.Count);

            Console.WriteLine("\n✅ Database seeding completed successfully!");
        }
    }
}

