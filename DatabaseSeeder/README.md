# Database Seeder

This project contains a script to seed the H2oDatabase with demo data.

## Usage

### Option 1: Using the shell script (Recommended)
From the project root directory:
```bash
./seed-database.sh
```

### Option 2: Using dotnet run
From the DatabaseSeeder directory:
```bash
cd DatabaseSeeder
dotnet run
```

## What gets seeded?

The seeder creates demo data for all entities in the domain model:

- **5 Functies** (Functions): DEV, MGR, HR, FIN, ADM
- **8 Medewerkers** (Employees): Various Dutch surnames
- **8 Dienstverbanden** (Employment records): Linking employees to functions
- **6 OrganisatorischeEenheden** (Organizational Units): With hierarchical structure (ROOT → IT/HR/FIN → IT-DEV/IT-OPS)
- **5 Kostenplaatsen** (Cost Centers): Linked to organizational units
- **5 Periodes** (Periods): For 2024 (January-May)
- **6 Inhuurkosten** (Hiring Costs): For different periods and cost centers
- **3 Begrotingen** (Budgets): For 2023, 2024, and 2025
- **9 Begrotingsregels** (Budget Line Items): Mix of Lasten (expenses) and Baten (revenues)
- **5 Contracten** (Contracts): With creditors, employees, and organizational units
- **10 Transacties** (Transactions): Linked to contracts

## Notes

- The seeder checks if data already exists and will skip seeding if the database already contains data
- All amounts are in euros (decimal format)
- The seeder uses the connection string from `ApiService/appsettings.json`

