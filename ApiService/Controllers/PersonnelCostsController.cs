using ApiService.DAL;
using ApiService.Helpers;
using ApiService.Models;
using ApiService.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PersonnelCostsController : ControllerBase
    {
        private readonly DatabaseContextApi _context;
        private readonly ILogger<PersonnelCostsController> _logger;

        public PersonnelCostsController(DatabaseContextApi context, ILogger<PersonnelCostsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Haalt de gemeentenaam op uit de user claims voor filtering
        /// </summary>
        private string? GetUserMunicipality()
        {
            return MunicipalityHelper.GetMunicipalityFromClaims(User);
        }

        /// <summary>
        /// Haalt begrote vs gerealiseerde personeelskosten op, gegroepeerd per jaar, functie, organisatorische eenheid en kostenplaats
        /// </summary>
        [HttpGet("budget-vs-realized")]
        public async Task<ActionResult<IEnumerable<PersonnelCostsDto>>> GetBudgetVsRealized(
            [FromQuery] int? jaar = null,
            [FromQuery] int? kwartaal = null,
            [FromQuery] string? functiecode = null,
            [FromQuery] string? organisatorischeEenheidCode = null,
            [FromQuery] string? kostenplaatsCode = null)
        {
            try
            {
                var result = await GetPersonnelCostsData(
                    jaar, 
                    kwartaal, 
                    null, 
                    functiecode, 
                    organisatorischeEenheidCode, 
                    kostenplaatsCode);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving budget vs realized costs");
                return StatusCode(500, new { error = "An error occurred while retrieving data" });
            }
        }

        /// <summary>
        /// Haalt samenvatting op van begrote vs gerealiseerde kosten, gegroepeerd per dimensie
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<IEnumerable<PersonnelCostsSummaryDto>>> GetSummary(
            [FromQuery] int? jaar = null,
            [FromQuery] int? kwartaal = null,
            [FromQuery] string? groupBy = null) // "jaar", "functie", "organisatorischeEenheid", "kostenplaats"
        {
            try
            {
                var data = await GetPersonnelCostsData(jaar, kwartaal, null, null, null, null);

                var summary = data
                    .GroupBy(d => new
                    {
                        d.Jaar,
                        d.Kwartaal,
                        d.Maand,
                        Functiecode = groupBy == "functie" ? d.Functiecode : null,
                        Functienaam = groupBy == "functie" ? d.Functienaam : null,
                        OrganisatorischeEenheidCode = groupBy == "organisatorischeEenheid" ? d.OrganisatorischeEenheidCode : null,
                        OrganisatorischeEenheidOmschrijving = groupBy == "organisatorischeEenheid" ? d.OrganisatorischeEenheidOmschrijving : null,
                        KostenplaatsCode = groupBy == "kostenplaats" ? d.KostenplaatsCode : null,
                        KostenplaatsOmschrijving = groupBy == "kostenplaats" ? d.KostenplaatsOmschrijving : null
                    })
                    .Select(g => new PersonnelCostsSummaryDto
                    {
                        Jaar = g.Key.Jaar,
                        Kwartaal = g.Key.Kwartaal,
                        Maand = g.Key.Maand,
                        Functiecode = g.Key.Functiecode,
                        Functienaam = g.Key.Functienaam,
                        OrganisatorischeEenheidCode = g.Key.OrganisatorischeEenheidCode,
                        OrganisatorischeEenheidOmschrijving = g.Key.OrganisatorischeEenheidOmschrijving,
                        KostenplaatsCode = g.Key.KostenplaatsCode,
                        KostenplaatsOmschrijving = g.Key.KostenplaatsOmschrijving,
                        TotaalBegroot = g.Sum(x => x.BegrootBedrag),
                        TotaalGerealiseerd = g.Sum(x => x.GerealiseerdBedrag),
                        TotaalVerschil = g.Sum(x => x.Verschil),
                        TotaalVerschilPercentage = g.Sum(x => x.BegrootBedrag) != 0
                            ? (g.Sum(x => x.Verschil) / g.Sum(x => x.BegrootBedrag)) * 100
                            : 0
                    })
                    .ToList();

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving summary");
                return StatusCode(500, new { error = "An error occurred while retrieving data" });
            }
        }

        /// <summary>
        /// Haalt begrote kosten op per dimensie
        /// </summary>
        [HttpGet("budget")]
        public async Task<ActionResult> GetBudget(
            [FromQuery] int? jaar = null,
            [FromQuery] string? functiecode = null,
            [FromQuery] string? organisatorischeEenheidCode = null,
            [FromQuery] string? kostenplaatsCode = null)
        {
            try
            {
                var municipality = GetUserMunicipality();

                var query = _context.Begrotingsregels
                    .Include(br => br.Begroting)
                    .Include(br => br.Medewerker)
                        .ThenInclude(m => m!.Dienstverband!)
                            .ThenInclude(d => d.Functie)
                    .Include(br => br.Kostenplaats)
                        .ThenInclude(kp => kp!.OrganisatorischeEenheid)
                    .Where(br => br.Kostensoort == Kostensoort.Lasten);

                if (jaar.HasValue)
                    query = query.Where(br => br.BegrotingJaar == jaar.Value);

                if (!string.IsNullOrEmpty(functiecode))
                    query = query.Where(br => br.Medewerker != null && 
                                             br.Medewerker.Dienstverband != null && 
                                             br.Medewerker.Dienstverband!.Functiecode == functiecode);

                if (!string.IsNullOrEmpty(organisatorischeEenheidCode))
                    query = query.Where(br => br.Kostenplaats != null && 
                                             br.Kostenplaats.OrganisatorischeEenheidCode == organisatorischeEenheidCode);

                if (!string.IsNullOrEmpty(kostenplaatsCode))
                    query = query.Where(br => br.KostenplaatsCode == kostenplaatsCode);

                var result = await query
                    .Select(br => new
                    {
                        Jaar = br.BegrotingJaar,
                        Functiecode = br.Medewerker != null && br.Medewerker.Dienstverband != null 
                            ? br.Medewerker.Dienstverband.Functiecode 
                            : null,
                        Functienaam = br.Medewerker != null && br.Medewerker.Dienstverband != null 
                            ? br.Medewerker.Dienstverband.Functie.Functienaam 
                            : null,
                        OrganisatorischeEenheidCode = br.Kostenplaats != null 
                            ? br.Kostenplaats.OrganisatorischeEenheidCode 
                            : null,
                        OrganisatorischeEenheidOmschrijving = br.Kostenplaats != null 
                            ? br.Kostenplaats.OrganisatorischeEenheid.Omschrijving 
                            : null,
                        KostenplaatsCode = br.KostenplaatsCode,
                        KostenplaatsOmschrijving = br.Kostenplaats != null 
                            ? br.Kostenplaats.Omschrijving 
                            : null,
                        Bedrag = br.Bedrag
                    })
                    .GroupBy(x => new
                    {
                        x.Jaar,
                        x.Functiecode,
                        x.Functienaam,
                        x.OrganisatorischeEenheidCode,
                        x.OrganisatorischeEenheidOmschrijving,
                        x.KostenplaatsCode,
                        x.KostenplaatsOmschrijving
                    })
                    .Select(g => new
                    {
                        g.Key.Jaar,
                        g.Key.Functiecode,
                        g.Key.Functienaam,
                        g.Key.OrganisatorischeEenheidCode,
                        g.Key.OrganisatorischeEenheidOmschrijving,
                        g.Key.KostenplaatsCode,
                        g.Key.KostenplaatsOmschrijving,
                        TotaalBedrag = g.Sum(x => x.Bedrag)
                    })
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving budget data");
                return StatusCode(500, new { error = "An error occurred while retrieving data" });
            }
        }

        /// <summary>
        /// Haalt gerealiseerde kosten op (van dienstverbanden en inhuurkosten)
        /// </summary>
        [HttpGet("realized")]
        public async Task<ActionResult> GetRealized(
            [FromQuery] int? jaar = null,
            [FromQuery] int? kwartaal = null,
            [FromQuery] string? functiecode = null,
            [FromQuery] string? organisatorischeEenheidCode = null,
            [FromQuery] string? kostenplaatsCode = null)
        {
            try
            {
                var municipality = GetUserMunicipality();

                // Get realized costs from Dienstverband (employment records)
                // Note: This is a simplified calculation. In reality, you'd calculate actual salary costs
                // based on Dienstverband data, salary scales, etc.
                var dienstverbandQuery = _context.Dienstverbanden
                    .Include(d => d.Medewerker)
                    .Include(d => d.Functie)
                    .Where(d => d.DatumUitDienst == null || (jaar.HasValue && d.DatumUitDienst.Value.Year >= jaar.Value));

                if (!string.IsNullOrEmpty(functiecode))
                    dienstverbandQuery = dienstverbandQuery.Where(d => d.Functiecode == functiecode);

                // Get Inhuurkosten (hiring costs)
                var inhuurkostenQuery = _context.Inhuurkosten
                    .Include(i => i.Periode)
                    .Include(i => i.Kostenplaats)
                        .ThenInclude(kp => kp.OrganisatorischeEenheid)
                    .AsQueryable();

                if (jaar.HasValue)
                    inhuurkostenQuery = inhuurkostenQuery.Where(i => i.Periode.Jaar == jaar.Value);

                if (kwartaal.HasValue)
                {
                    var startMaand = (kwartaal.Value - 1) * 3 + 1;
                    var endMaand = kwartaal.Value * 3;
                    inhuurkostenQuery = inhuurkostenQuery.Where(i => 
                        i.Periode.Maand >= startMaand && i.Periode.Maand <= endMaand);
                }

                if (!string.IsNullOrEmpty(organisatorischeEenheidCode))
                    inhuurkostenQuery = inhuurkostenQuery.Where(i => 
                        i.Kostenplaats.OrganisatorischeEenheidCode == organisatorischeEenheidCode);

                if (!string.IsNullOrEmpty(kostenplaatsCode))
                    inhuurkostenQuery = inhuurkostenQuery.Where(i => i.KostenplaatsCode == kostenplaatsCode);

                var inhuurkosten = await inhuurkostenQuery
                    .Select(i => new
                    {
                        Jaar = i.Periode.Jaar,
                        Kwartaal = (i.Periode.Maand - 1) / 3 + 1,
                        Maand = i.Periode.Maand,
                        Functiecode = (string?)null,
                        Functienaam = (string?)null,
                        OrganisatorischeEenheidCode = i.Kostenplaats.OrganisatorischeEenheidCode,
                        OrganisatorischeEenheidOmschrijving = i.Kostenplaats.OrganisatorischeEenheid.Omschrijving,
                        KostenplaatsCode = i.KostenplaatsCode,
                        KostenplaatsOmschrijving = i.Kostenplaats.Omschrijving,
                        Bedrag = i.Bedrag
                    })
                    .ToListAsync();

                // For Dienstverband, we need to estimate costs or use a calculation
                // This is a placeholder - you'd need actual salary calculation logic
                var dienstverbandCosts = await dienstverbandQuery
                    .Select(d => new
                    {
                        Jaar = jaar ?? DateTime.Now.Year,
                        Kwartaal = (int?)null,
                        Maand = (int?)null,
                        Functiecode = d.Functiecode,
                        Functienaam = d.Functie.Functienaam,
                        OrganisatorischeEenheidCode = (string?)null, // Would need to link via Kostenplaats
                        OrganisatorischeEenheidOmschrijving = (string?)null,
                        KostenplaatsCode = (string?)null,
                        KostenplaatsOmschrijving = (string?)null,
                        Bedrag = 0m // Placeholder - would need actual salary calculation
                    })
                    .ToListAsync();

                // For now, just return inhuurkosten since dienstverbandCosts calculation needs actual salary logic
                var result = inhuurkosten
                    .GroupBy(x => new
                    {
                        x.Jaar,
                        x.Kwartaal,
                        x.Maand,
                        x.Functiecode,
                        x.Functienaam,
                        x.OrganisatorischeEenheidCode,
                        x.OrganisatorischeEenheidOmschrijving,
                        x.KostenplaatsCode,
                        x.KostenplaatsOmschrijving
                    })
                    .Select(g => new
                    {
                        g.Key.Jaar,
                        g.Key.Kwartaal,
                        g.Key.Maand,
                        g.Key.Functiecode,
                        g.Key.Functienaam,
                        g.Key.OrganisatorischeEenheidCode,
                        g.Key.OrganisatorischeEenheidOmschrijving,
                        g.Key.KostenplaatsCode,
                        g.Key.KostenplaatsOmschrijving,
                        TotaalBedrag = g.Sum(x => x.Bedrag)
                    })
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving realized costs");
                return StatusCode(500, new { error = "An error occurred while retrieving data" });
            }
        }

        private async Task<List<PersonnelCostsDto>> GetPersonnelCostsData(
            int? jaar,
            int? kwartaal,
            int? maand,
            string? functiecode,
            string? organisatorischeEenheidCode,
            string? kostenplaatsCode)
        {
            var municipality = GetUserMunicipality();

            // Get budgeted costs
            var budgetQuery = _context.Begrotingsregels
                .Include(br => br.Begroting)
                .Include(br => br.Medewerker)
                    .ThenInclude(m => m!.Dienstverband!)
                        .ThenInclude(d => d.Functie)
                .Include(br => br.Kostenplaats)
                    .ThenInclude(kp => kp!.OrganisatorischeEenheid)
                .Where(br => br.Kostensoort == Kostensoort.Lasten);

            if (jaar.HasValue)
                budgetQuery = budgetQuery.Where(br => br.BegrotingJaar == jaar.Value);

                if (!string.IsNullOrEmpty(functiecode))
                    budgetQuery = budgetQuery.Where(br => 
                        br.Medewerker != null && 
                        br.Medewerker.Dienstverband != null && 
                        br.Medewerker.Dienstverband!.Functiecode == functiecode);

            if (!string.IsNullOrEmpty(organisatorischeEenheidCode))
                budgetQuery = budgetQuery.Where(br => 
                    br.Kostenplaats != null && 
                    br.Kostenplaats.OrganisatorischeEenheidCode == organisatorischeEenheidCode);

            if (!string.IsNullOrEmpty(kostenplaatsCode))
                budgetQuery = budgetQuery.Where(br => br.KostenplaatsCode == kostenplaatsCode);

            var budgetData = await budgetQuery
                .Select(br => new
                {
                    Jaar = br.BegrotingJaar,
                    Functiecode = br.Medewerker != null && br.Medewerker.Dienstverband != null 
                        ? br.Medewerker.Dienstverband.Functiecode 
                        : null,
                    Functienaam = br.Medewerker != null && br.Medewerker.Dienstverband != null 
                        ? br.Medewerker.Dienstverband.Functie.Functienaam 
                        : null,
                    OrganisatorischeEenheidCode = br.Kostenplaats != null 
                        ? br.Kostenplaats.OrganisatorischeEenheidCode 
                        : null,
                    OrganisatorischeEenheidOmschrijving = br.Kostenplaats != null 
                        ? br.Kostenplaats.OrganisatorischeEenheid.Omschrijving 
                        : null,
                    KostenplaatsCode = br.KostenplaatsCode,
                    KostenplaatsOmschrijving = br.Kostenplaats != null 
                        ? br.Kostenplaats.Omschrijving 
                        : null,
                    Bedrag = br.Bedrag
                })
                .GroupBy(x => new
                {
                    x.Jaar,
                    x.Functiecode,
                    x.Functienaam,
                    x.OrganisatorischeEenheidCode,
                    x.OrganisatorischeEenheidOmschrijving,
                    x.KostenplaatsCode,
                    x.KostenplaatsOmschrijving
                })
                .Select(g => new
                {
                    g.Key.Jaar,
                    Kwartaal = (int?)null,
                    Maand = (int?)null,
                    g.Key.Functiecode,
                    g.Key.Functienaam,
                    g.Key.OrganisatorischeEenheidCode,
                    g.Key.OrganisatorischeEenheidOmschrijving,
                    g.Key.KostenplaatsCode,
                    g.Key.KostenplaatsOmschrijving,
                    BegrootBedrag = g.Sum(x => x.Bedrag)
                })
                .ToListAsync();

            // Get realized costs from Inhuurkosten
            IQueryable<Inhuurkosten> realizedQuery = _context.Inhuurkosten
                .Include(i => i.Periode)
                .Include(i => i.Kostenplaats)
                    .ThenInclude(kp => kp.OrganisatorischeEenheid);

            if (jaar.HasValue)
                realizedQuery = realizedQuery.Where(i => i.Periode.Jaar == jaar.Value);

            if (kwartaal.HasValue)
            {
                var startMaand = (kwartaal.Value - 1) * 3 + 1;
                var endMaand = kwartaal.Value * 3;
                realizedQuery = realizedQuery.Where(i => 
                    i.Periode.Maand >= startMaand && i.Periode.Maand <= endMaand);
            }

            if (maand.HasValue)
                realizedQuery = realizedQuery.Where(i => i.Periode.Maand == maand.Value);

            if (!string.IsNullOrEmpty(organisatorischeEenheidCode))
                realizedQuery = realizedQuery.Where(i => 
                    i.Kostenplaats.OrganisatorischeEenheidCode == organisatorischeEenheidCode);

            if (!string.IsNullOrEmpty(kostenplaatsCode))
                realizedQuery = realizedQuery.Where(i => i.KostenplaatsCode == kostenplaatsCode);

            var realizedData = await realizedQuery
                .GroupBy(i => new
                {
                    i.Periode.Jaar,
                    Kwartaal = (i.Periode.Maand - 1) / 3 + 1,
                    i.Periode.Maand,
                    i.Kostenplaats.OrganisatorischeEenheidCode,
                    OrganisatorischeEenheidOmschrijving = i.Kostenplaats.OrganisatorischeEenheid.Omschrijving,
                    i.KostenplaatsCode,
                    KostenplaatsOmschrijving = i.Kostenplaats.Omschrijving
                })
                .Select(g => new
                {
                    g.Key.Jaar,
                    g.Key.Kwartaal,
                    g.Key.Maand,
                    Functiecode = (string?)null,
                    Functienaam = (string?)null,
                    g.Key.OrganisatorischeEenheidCode,
                    g.Key.OrganisatorischeEenheidOmschrijving,
                    g.Key.KostenplaatsCode,
                    g.Key.KostenplaatsOmschrijving,
                    GerealiseerdBedrag = g.Sum(i => i.Bedrag)
                })
                .ToListAsync();

            // Combine budget and realized data
            var combined = budgetData
                .Select(b => new PersonnelCostsDto
                {
                    Jaar = b.Jaar,
                    Kwartaal = b.Kwartaal,
                    Maand = b.Maand,
                    Functiecode = b.Functiecode,
                    Functienaam = b.Functienaam,
                    OrganisatorischeEenheidCode = b.OrganisatorischeEenheidCode,
                    OrganisatorischeEenheidOmschrijving = b.OrganisatorischeEenheidOmschrijving,
                    KostenplaatsCode = b.KostenplaatsCode,
                    KostenplaatsOmschrijving = b.KostenplaatsOmschrijving,
                    BegrootBedrag = b.BegrootBedrag,
                    GerealiseerdBedrag = realizedData
                        .Where(r => r.Jaar == b.Jaar &&
                                   r.OrganisatorischeEenheidCode == b.OrganisatorischeEenheidCode &&
                                   r.KostenplaatsCode == b.KostenplaatsCode)
                        .Sum(r => r.GerealiseerdBedrag),
                    Verschil = 0,
                    VerschilPercentage = 0
                })
                .ToList();

            // Calculate differences
            foreach (var item in combined)
            {
                item.Verschil = item.GerealiseerdBedrag - item.BegrootBedrag;
                item.VerschilPercentage = item.BegrootBedrag != 0
                    ? (item.Verschil / item.BegrootBedrag) * 100
                    : 0;
            }

            return combined;
        }
    }
}

