# Personnel Costs API Endpoints

Deze API endpoints bieden data voor Power BI rapporten die begrote vs gerealiseerde personeelskosten tonen.

## Endpoints

### 1. GET `/api/PersonnelCosts/budget-vs-realized`
Haalt begrote vs gerealiseerde personeelskosten op, gegroepeerd per dimensie.

**Query Parameters:**
- `jaar` (int, optioneel): Filter op jaar
- `kwartaal` (int, optioneel): Filter op kwartaal (1-4)
- `functiecode` (string, optioneel): Filter op functiecode (bijv. "DEV", "MGR")
- `organisatorischeEenheidCode` (string, optioneel): Filter op organisatorische eenheid
- `kostenplaatsCode` (string, optioneel): Filter op kostenplaats

**Response:**
```json
[
  {
    "jaar": 2024,
    "kwartaal": null,
    "maand": null,
    "functiecode": "DEV",
    "functienaam": "Software Developer",
    "organisatorischeEenheidCode": "IT-DEV",
    "organisatorischeEenheidOmschrijving": "Development Team",
    "kostenplaatsCode": "KP001",
    "kostenplaatsOmschrijving": "IT Development",
    "begrootBedrag": 195000.00,
    "gerealiseerdBedrag": 15000.00,
    "verschil": -180000.00,
    "verschilPercentage": -92.31
  }
]
```

### 2. GET `/api/PersonnelCosts/summary`
Haalt een samenvatting op van begrote vs gerealiseerde kosten, gegroepeerd per dimensie.

**Query Parameters:**
- `jaar` (int, optioneel): Filter op jaar
- `kwartaal` (int, optioneel): Filter op kwartaal
- `groupBy` (string, optioneel): Groepeer op "jaar", "functie", "organisatorischeEenheid", of "kostenplaats"

**Response:**
```json
[
  {
    "jaar": 2024,
    "kwartaal": null,
    "maand": null,
    "functiecode": "DEV",
    "functienaam": "Software Developer",
    "organisatorischeEenheidCode": null,
    "organisatorischeEenheidOmschrijving": null,
    "kostenplaatsCode": null,
    "kostenplaatsOmschrijving": null,
    "totaalBegroot": 195000.00,
    "totaalGerealiseerd": 15000.00,
    "totaalVerschil": -180000.00,
    "totaalVerschilPercentage": -92.31
  }
]
```

### 3. GET `/api/PersonnelCosts/budget`
Haalt alleen begrote kosten op per dimensie.

**Query Parameters:**
- `jaar` (int, optioneel)
- `functiecode` (string, optioneel)
- `organisatorischeEenheidCode` (string, optioneel)
- `kostenplaatsCode` (string, optioneel)

### 4. GET `/api/PersonnelCosts/realized`
Haalt alleen gerealiseerde kosten op (van Inhuurkosten).

**Query Parameters:**
- `jaar` (int, optioneel)
- `kwartaal` (int, optioneel)
- `functiecode` (string, optioneel)
- `organisatorischeEenheidCode` (string, optioneel)
- `kostenplaatsCode` (string, optioneel)

## Power BI Integratie

### Voorbeeld queries:

**Alle data voor 2024:**
```
GET /api/PersonnelCosts/budget-vs-realized?jaar=2024
```

**Data per kwartaal:**
```
GET /api/PersonnelCosts/budget-vs-realized?jaar=2024&kwartaal=1
```

**Data per functie:**
```
GET /api/PersonnelCosts/summary?jaar=2024&groupBy=functie
```

**Data per organisatorische eenheid:**
```
GET /api/PersonnelCosts/summary?jaar=2024&groupBy=organisatorischeEenheid
```

**Data per kostenplaats:**
```
GET /api/PersonnelCosts/summary?jaar=2024&groupBy=kostenplaats
```

## Opmerkingen

- **Gerealiseerde kosten** worden momenteel berekend op basis van `Inhuurkosten` (hiring costs)
- Voor volledige gerealiseerde kosten zou je ook de werkelijke salariskosten moeten berekenen op basis van `Dienstverband` records
- De berekening van salariskosten vereist aanvullende business logic (salarisschalen, anciënniteit, etc.)

## Toekomstige uitbreidingen

- Toevoegen van werkelijke salariskosten berekening
- Ondersteuning voor maandelijkse rapportage
- Export functionaliteit (Excel, CSV)
- Caching voor betere performance

