-- ============================================
-- H2O Database Seed Data Script
-- ============================================
-- This script inserts all dummy data into an empty database
-- Run this script in SQL Server Management Studio (SSMS)
-- ============================================

-- Clear existing data (optional - uncomment if needed)
/*
DELETE FROM Transacties;
DELETE FROM Inhuurkosten;
DELETE FROM Begrotingsregels;
DELETE FROM Contracten;
DELETE FROM Dienstverbanden;
DELETE FROM Kostenplaatsen;
DELETE FROM OrganisatorischeEenheden;
DELETE FROM Periodes;
DELETE FROM Begrotingen;
DELETE FROM Medewerkers;
DELETE FROM Functies;
DBCC CHECKIDENT ('Medewerkers', RESEED, 0);
DBCC CHECKIDENT ('Dienstverbanden', RESEED, 0);
DBCC CHECKIDENT ('Begrotingen', RESEED, 0);
DBCC CHECKIDENT ('Periodes', RESEED, 0);
DBCC CHECKIDENT ('Contracten', RESEED, 0);
DBCC CHECKIDENT ('Transacties', RESEED, 0);
*/

-- ============================================
-- 1. Functies (Functions)
-- ============================================
INSERT INTO Functies (Functiecode, Functienaam, Schaall) VALUES
('DEV', 'Software Developer', '10'),
('MGR', 'Manager', '12'),
('HR', 'HR Specialist', '9'),
('FIN', 'Financial Analyst', '11'),
('ADM', 'Administrator', '8');

-- ============================================
-- 2. Medewerkers (Employees)
-- ============================================
-- Let identity column auto-generate IDs
INSERT INTO Medewerkers (Achternaam) VALUES
('Jansen'),
('De Vries'),
('Bakker'),
('Visser'),
('Smit'),
('Meijer'),
('De Boer'),
('Mulder');

-- ============================================
-- 3. Dienstverbanden (Employment records)
-- ============================================
-- Let identity column auto-generate IDs
-- Note: MedewerkerNummer assumes Medewerkers were inserted in order (1-8)
INSERT INTO Dienstverbanden (MedewerkerNummer, Functiecode, Type, DatumInDienst, DatumUitDienst, Ancienniteit) VALUES
(1, 'DEV', 'Vast', '2020-01-01', NULL, 5),
(2, 'DEV', 'Vast', '2021-03-15', NULL, 4),
(3, 'MGR', 'Vast', '2019-06-01', NULL, 6),
(4, 'HR', 'Vast', '2022-01-10', NULL, 3),
(5, 'FIN', 'Vast', '2021-09-01', NULL, 4),
(6, 'ADM', 'Tijdelijk', '2023-01-01', NULL, 2),
(7, 'DEV', 'Vast', '2022-05-01', NULL, 3),
(8, 'MGR', 'Vast', '2020-11-01', NULL, 5);

-- ============================================
-- 4. OrganisatorischeEenheden (Departments)
-- ============================================
-- Hattem (MunicipalityId = 1)
INSERT INTO OrganisatorischeEenheden (Code, Omschrijving, ParentCode, MunicipalityId) VALUES
('RUIMTE_HATTEM', 'Ruimte', NULL, 1),
('ONDERSTEUNING_HATTEM', 'Ondersteuning', NULL, 1),
('PUBLIEKSDIENST_HATTEM', 'Publieksdienst', NULL, 1),
('SOCIAAL_HATTEM', 'Sociaal', NULL, 1);

-- Oldebroek (MunicipalityId = 2)
INSERT INTO OrganisatorischeEenheden (Code, Omschrijving, ParentCode, MunicipalityId) VALUES
('SAMENLEVING_OLDEBROEK', 'Samenleving', NULL, 2),
('ZORG_EN_ONDERSTEUNING_OLDEBROEK', 'Zorg en Ondersteuning', NULL, 2),
('PARTICIPATIE_OLDEBROEK', 'Participatie', NULL, 2),
('RUIMTE_OLDEBROEK', 'Ruimte', NULL, 2),
('LEEFOMGEVING_OLDEBROEK', 'Leefomgeving', NULL, 2),
('BUITENDIENST_OLDEBROEK', 'Buitendienst', NULL, 2),
('BEDRIJFSVOERING_OLDEBROEK', 'Bedrijfsvoering', NULL, 2),
('DIENSTVERLENING_OLDEBROEK', 'Dienstverlening', NULL, 2),
('CIO_OFFICE_OLDEBROEK', 'CIO Office', NULL, 2);

-- Heerde (MunicipalityId = 3)
INSERT INTO OrganisatorischeEenheden (Code, Omschrijving, ParentCode, MunicipalityId) VALUES
('SOCIAAL_MAATSCHAPPELIJKE_VERBINDING_HEERDE', 'Sociaal Maatschappelijke Verbinding', NULL, 3),
('RUIMTE_ONDERNEMEN_EN_WONEN_HEERDE', 'Ruimte Ondernemen en Wonen', NULL, 3),
('REALISATIE_ONDERHOUD_EN_BEHEER_HEERDE', 'Realisatie Onderhoud en Beheer', NULL, 3),
('DIENSTVERLENING_EN_INFORMATIEBEHEER_HEERDE', 'Dienstverlening en Informatiebeheer', NULL, 3);

-- ============================================
-- 5. Kostenplaatsen (Cost Centers)
-- ============================================
-- Hattem - Ruimte
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('RUIMTE_HATTEM-001', 'Ruimtelijke Planning', 'RUIMTE_HATTEM'),
('RUIMTE_HATTEM-002', 'Bouw en Woningtoezicht', 'RUIMTE_HATTEM');

-- Hattem - Ondersteuning
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('ONDERSTEUNING_HATTEM-001', 'Administratieve Ondersteuning', 'ONDERSTEUNING_HATTEM'),
('ONDERSTEUNING_HATTEM-002', 'Financiële Ondersteuning', 'ONDERSTEUNING_HATTEM');

-- Hattem - Publieksdienst
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('PUBLIEKSDIENST_HATTEM-001', 'Klantcontact', 'PUBLIEKSDIENST_HATTEM'),
('PUBLIEKSDIENST_HATTEM-002', 'Informatieverstrekking', 'PUBLIEKSDIENST_HATTEM');

-- Hattem - Sociaal
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('SOCIAAL_HATTEM-001', 'Welzijn', 'SOCIAAL_HATTEM'),
('SOCIAAL_HATTEM-002', 'Jeugdzorg', 'SOCIAAL_HATTEM');

-- Oldebroek - Samenleving
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('SAMENLEVING_OLDEBROEK-001', 'Samenlevingsactiviteiten', 'SAMENLEVING_OLDEBROEK');

-- Oldebroek - Zorg en Ondersteuning
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('ZORG_ONDERSTEUNING_OLDEBROEK-001', 'Zorgverlening', 'ZORG_EN_ONDERSTEUNING_OLDEBROEK'),
('ZORG_ONDERSTEUNING_OLDEBROEK-002', 'Ondersteuningsdiensten', 'ZORG_EN_ONDERSTEUNING_OLDEBROEK');

-- Oldebroek - Participatie
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('PARTICIPATIE_OLDEBROEK-001', 'Participatieprojecten', 'PARTICIPATIE_OLDEBROEK');

-- Oldebroek - Ruimte
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('RUIMTE_OLDEBROEK-001', 'Ruimtelijke Ontwikkeling', 'RUIMTE_OLDEBROEK'),
('RUIMTE_OLDEBROEK-002', 'Woningbouw', 'RUIMTE_OLDEBROEK');

-- Oldebroek - Leefomgeving
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('LEEFOMGEVING_OLDEBROEK-001', 'Milieu en Duurzaamheid', 'LEEFOMGEVING_OLDEBROEK'),
('LEEFOMGEVING_OLDEBROEK-002', 'Groenbeheer', 'LEEFOMGEVING_OLDEBROEK');

-- Oldebroek - Buitendienst
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('BUITENDIENST_OLDEBROEK-001', 'Openbare Werken', 'BUITENDIENST_OLDEBROEK'),
('BUITENDIENST_OLDEBROEK-002', 'Onderhoud Openbare Ruimte', 'BUITENDIENST_OLDEBROEK');

-- Oldebroek - Bedrijfsvoering
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('BEDRIJFSVOERING_OLDEBROEK-001', 'HR en Personeel', 'BEDRIJFSVOERING_OLDEBROEK'),
('BEDRIJFSVOERING_OLDEBROEK-002', 'Financiën', 'BEDRIJFSVOERING_OLDEBROEK');

-- Oldebroek - Dienstverlening
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('DIENSTVERLENING_OLDEBROEK-001', 'Klantenservice', 'DIENSTVERLENING_OLDEBROEK'),
('DIENSTVERLENING_OLDEBROEK-002', 'Digitale Dienstverlening', 'DIENSTVERLENING_OLDEBROEK');

-- Oldebroek - CIO Office
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('CIO_OFFICE_OLDEBROEK-001', 'IT Beheer', 'CIO_OFFICE_OLDEBROEK'),
('CIO_OFFICE_OLDEBROEK-002', 'Digitale Innovatie', 'CIO_OFFICE_OLDEBROEK');

-- Heerde - Sociaal Maatschappelijke Verbinding
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('SOCIAAL_MAATSCHAPPELIJKE_VERBINDING_HEERDE-001', 'Welzijn en Zorg', 'SOCIAAL_MAATSCHAPPELIJKE_VERBINDING_HEERDE'),
('SOCIAAL_MAATSCHAPPELIJKE_VERBINDING_HEERDE-002', 'Participatie', 'SOCIAAL_MAATSCHAPPELIJKE_VERBINDING_HEERDE');

-- Heerde - Ruimte Ondernemen en Wonen
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('RUIMTE_ONDERNEMEN_EN_WONEN_HEERDE-001', 'Ruimtelijke Planning', 'RUIMTE_ONDERNEMEN_EN_WONEN_HEERDE'),
('RUIMTE_ONDERNEMEN_EN_WONEN_HEERDE-002', 'Economische Zaken', 'RUIMTE_ONDERNEMEN_EN_WONEN_HEERDE');

-- Heerde - Realisatie Onderhoud en Beheer
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('REALISATIE_ONDERHOUD_EN_BEHEER_HEERDE-001', 'Openbare Werken', 'REALISATIE_ONDERHOUD_EN_BEHEER_HEERDE'),
('REALISATIE_ONDERHOUD_EN_BEHEER_HEERDE-002', 'Groenbeheer', 'REALISATIE_ONDERHOUD_EN_BEHEER_HEERDE');

-- Heerde - Dienstverlening en Informatiebeheer
INSERT INTO Kostenplaatsen (Code, Omschrijving, OrganisatorischeEenheidCode) VALUES
('DIENSTVERLENING_EN_INFORMATIEBEHEER_HEERDE-001', 'Klantcontact', 'DIENSTVERLENING_EN_INFORMATIEBEHEER_HEERDE'),
('DIENSTVERLENING_EN_INFORMATIEBEHEER_HEERDE-002', 'Informatiebeheer', 'DIENSTVERLENING_EN_INFORMATIEBEHEER_HEERDE');

-- ============================================
-- 6. Periodes (Periods) - Q1 t/m Q4 2024
-- ============================================
-- Let identity column auto-generate IDs
INSERT INTO Periodes (Jaar, Maand, Verwerking, Label) VALUES
-- Q1 2024
(2024, 1, 1, 'Januari 2024'),
(2024, 2, 1, 'Februari 2024'),
(2024, 3, 1, 'Maart 2024'),
-- Q2 2024
(2024, 4, 1, 'April 2024'),
(2024, 5, 1, 'Mei 2024'),
(2024, 6, 1, 'Juni 2024'),
-- Q3 2024
(2024, 7, 1, 'Juli 2024'),
(2024, 8, 1, 'Augustus 2024'),
(2024, 9, 1, 'September 2024'),
-- Q4 2024
(2024, 10, 0, 'Oktober 2024'),
(2024, 11, 0, 'November 2024'),
(2024, 12, 0, 'December 2024');

-- ============================================
-- 7. Begrotingen (Budgets)
-- ============================================
-- Let identity column auto-generate IDs
-- Note: Id will be 1, 2, 3 for the three begrotingen
INSERT INTO Begrotingen (Jaar, Status, Totaalbedrag) VALUES
(2024, 0, 500000.00),
(2023, 1, 450000.00),
(2025, 0, 550000.00);

-- ============================================
-- 8. Begrotingsregels (Budget Line Items)
-- ============================================
-- Note: Kostensoort: 0 = Lasten (Expenses), 1 = Baten (Revenue)
-- Note: MedewerkerNummer assumes Medewerkers were inserted in order (1-8)
-- Note: BegrotingId assumes Begrotingen were inserted in order:
--       Id 1 = 2024, Id 2 = 2023, Id 3 = 2025
INSERT INTO Begrotingsregels (BegrotingId, MedewerkerNummer, KostenplaatsCode, Kostensoort, Bedrag) VALUES
-- Hattem - 2024 Lasten (BegrotingId = 1)
(1, 1, 'RUIMTE_HATTEM-001', 0, 50000.00),
(1, 2, 'ONDERSTEUNING_HATTEM-001', 0, 30000.00),
-- Oldebroek - 2024 Lasten
(1, 3, 'SAMENLEVING_OLDEBROEK-001', 0, 80000.00),
(1, 4, 'ZORG_ONDERSTEUNING_OLDEBROEK-001', 0, 40000.00),
-- Heerde - 2024 Lasten
(1, 5, 'SOCIAAL_MAATSCHAPPELIJKE_VERBINDING_HEERDE-001', 0, 65000.00),
-- Hattem - 2024 Baten
(1, NULL, 'RUIMTE_HATTEM-001', 1, 100000.00),
-- Oldebroek - 2024 Baten
(1, NULL, 'SAMENLEVING_OLDEBROEK-001', 1, 150000.00),
-- Heerde - 2024 Baten
(1, NULL, 'SOCIAAL_MAATSCHAPPELIJKE_VERBINDING_HEERDE-001', 1, 120000.00),
-- 2025 Lasten (BegrotingId = 3)
(3, 1, 'RUIMTE_HATTEM-001', 0, 52000.00),
(3, 3, 'SAMENLEVING_OLDEBROEK-001', 0, 82000.00),
(3, 5, 'SOCIAAL_MAATSCHAPPELIJKE_VERBINDING_HEERDE-001', 0, 67000.00);

-- ============================================
-- 9. Inhuurkosten (Hiring Costs)
-- ============================================
-- Note: PeriodeId assumes Periodes were inserted in order (1-12 for months Jan-Dec 2024)
INSERT INTO Inhuurkosten (PeriodeId, KostenplaatsCode, Bedrag) VALUES
-- Hattem - Q1 2024 (Januari = PeriodeId 1)
(1, 'RUIMTE_HATTEM-001', 25000.00),
(1, 'ONDERSTEUNING_HATTEM-001', 15000.00),
-- Oldebroek - Q1 2024
(1, 'SAMENLEVING_OLDEBROEK-001', 35000.00),
(1, 'ZORG_ONDERSTEUNING_OLDEBROEK-001', 12000.00),
-- Heerde - Q1 2024
(1, 'SOCIAAL_MAATSCHAPPELIJKE_VERBINDING_HEERDE-001', 28000.00),
-- Hattem - Q2 2024 (April = PeriodeId 4)
(4, 'PUBLIEKSDIENST_HATTEM-001', 30000.00),
(4, 'SOCIAAL_HATTEM-001', 20000.00),
-- Oldebroek - Q2 2024
(4, 'RUIMTE_OLDEBROEK-001', 40000.00),
-- Heerde - Q2 2024
(4, 'RUIMTE_ONDERNEMEN_EN_WONEN_HEERDE-001', 22000.00),
-- Hattem - Q3 2024 (Juli = PeriodeId 7)
(7, 'RUIMTE_HATTEM-002', 28000.00),
-- Oldebroek - Q3 2024
(7, 'LEEFOMGEVING_OLDEBROEK-001', 10000.00),
-- Heerde - Q3 2024
(7, 'REALISATIE_ONDERHOUD_EN_BEHEER_HEERDE-001', 18000.00);

-- ============================================
-- 10. Contracten (Contracts)
-- ============================================
-- Let identity column auto-generate IDs
-- Note: MedewerkerNummer assumes Medewerkers were inserted in order (1-8)
INSERT INTO Contracten (Crediteur, MedewerkerNummer, OrganisatorischeEenheidCode, Rekening) VALUES
-- Hattem
('Ruimte Advies BV', 1, 'RUIMTE_HATTEM', 'NL91ABNA0417164300'),
('Ondersteuning Services', 2, 'ONDERSTEUNING_HATTEM', 'NL12INGB0001234567'),
-- Oldebroek
('Samenleving Partners', 3, 'SAMENLEVING_OLDEBROEK', 'NL34RABO0123456789'),
('Zorg & Ondersteuning BV', 4, 'ZORG_EN_ONDERSTEUNING_OLDEBROEK', 'NL56ABNA0987654321'),
-- Heerde
('Sociaal Verbinding BV', 5, 'SOCIAAL_MAATSCHAPPELIJKE_VERBINDING_HEERDE', 'NL78RABO0123456789'),
('Ruimte & Wonen Partners', 6, 'RUIMTE_ONDERNEMEN_EN_WONEN_HEERDE', 'NL90ABNA0123456789'),
-- Hattem
('Publieksdienst BV', 7, 'PUBLIEKSDIENST_HATTEM', 'NL12RABO0123456789'),
-- Oldebroek
('Bedrijfsvoering Services', 8, 'BEDRIJFSVOERING_OLDEBROEK', 'NL34INGB0123456789'),
('CIO Office Partners', NULL, 'CIO_OFFICE_OLDEBROEK', 'NL56ABNA0123456789');

-- ============================================
-- 11. Transacties (Transactions)
-- ============================================
-- Let identity column auto-generate IDs
-- Note: ContractId assumes Contracten were inserted in order (1-9)
INSERT INTO Transacties (ContractId, Datum, Bedrag) VALUES
(1, '2024-01-15', 5000.00),
(1, '2024-02-15', 5000.00),
(1, '2024-03-15', 5000.00),
(2, '2024-01-20', 3500.00),
(2, '2024-02-20', 3500.00),
(3, '2024-01-10', 2500.00),
(3, '2024-02-10', 2500.00),
(4, '2024-01-05', 4000.00),
(5, '2024-01-25', 1200.00),
(5, '2024-02-25', 1200.00);

-- ============================================
-- Script completed successfully!
-- ============================================
PRINT '✅ Database seeding completed successfully!';
PRINT 'Created:';
PRINT '  - 5 Functies';
PRINT '  - 8 Medewerkers';
PRINT '  - 8 Dienstverbanden';
PRINT '  - 17 OrganisatorischeEenheden (4 Hattem, 9 Oldebroek, 4 Heerde)';
PRINT '  - 34 Kostenplaatsen';
PRINT '  - 12 Periodes';
PRINT '  - 9 Inhuurkosten';
PRINT '  - 3 Begrotingen';
PRINT '  - 10 Begrotingsregels';
PRINT '  - 9 Contracten';
PRINT '  - 10 Transacties';
