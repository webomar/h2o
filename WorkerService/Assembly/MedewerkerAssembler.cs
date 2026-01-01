using WorkerService.ImportDomain;

namespace WorkerService.Assembly
{
    public class MedewerkerAssembler
    {
        private readonly Dictionary<int, MedewerkerAggregate> _buffer = new();

        // 🔹 Stamdata (niet medewerker-gebonden)
        private readonly Dictionary<string, KostenplaatsImport> _kostenplaatsen = new();

        // ======================================================
        // Youforce (HR)
        // ======================================================
        public void ApplyYouforce(
            int medewerkerNummer,
            string achternaam,
            DienstverbandImport dienstverband)
        {
            var agg = GetOrCreate(medewerkerNummer);

            agg.Achternaam = achternaam;
            agg.Dienstverband = dienstverband;
        }

        // ======================================================
        // ERP-X (Contracten)
        // ======================================================
        public void ApplyErpXContract(
            int medewerkerNummer,
            ContractImport contract)
        {
            var agg = GetOrCreate(medewerkerNummer);
            agg.Contracten.Add(contract);
        }

        // ======================================================
        // ERP-X (Begroting)
        // ======================================================
        public void ApplyErpXBegroting(
            int medewerkerNummer,
            BegrotingsregelImport regel)
        {
            var agg = GetOrCreate(medewerkerNummer);
            agg.Begrotingsregels.Add(regel);
        }

        // ======================================================
        // ERP-X (Inhuurkosten)
        // ======================================================
        public void ApplyErpXInhuurkosten(
            int medewerkerNummer,
            InhuurkostenImport kosten)
        {
            var agg = GetOrCreate(medewerkerNummer);
            agg.Inhuurkosten.Add(kosten);
        }

        // ======================================================
        // ERP-X (Transacties)
        // ======================================================
        public void ApplyErpXTransactie(
            int medewerkerNummer,
            TransactieImport transactie)
        {
            var agg = GetOrCreate(medewerkerNummer);
            agg.Transacties.Add(transactie);
        }

        // ======================================================
        // ERP-X (Kostenplaats – STAMDATA)
        // ======================================================
        public void ApplyErpXKostenplaats(KostenplaatsImport kostenplaats)
        {
            if (string.IsNullOrWhiteSpace(kostenplaats.Code))
                return;

            // Deduplicatie op Code
            if (!_kostenplaatsen.ContainsKey(kostenplaats.Code))
            {
                _kostenplaatsen[kostenplaats.Code] = kostenplaats;
            }
        }

        // ======================================================
        // Resultaat
        // ======================================================
        public IReadOnlyCollection<MedewerkerAggregate> GetAll()
            => _buffer.Values.ToList();

        public IReadOnlyCollection<MedewerkerAggregate> GetComplete()
            => _buffer.Values.Where(x => x.IsComplete).ToList();

        public IReadOnlyCollection<MedewerkerAggregate> GetIncomplete()
            => _buffer.Values.Where(x => !x.IsComplete).ToList();

        // 🔹 Stamdata exports
        public IReadOnlyCollection<KostenplaatsImport> GetKostenplaatsen()
            => _kostenplaatsen.Values.ToList();

        // ======================================================
        // Intern
        // ======================================================
        private MedewerkerAggregate GetOrCreate(int nummer)
        {
            if (!_buffer.TryGetValue(nummer, out var agg))
            {
                agg = new MedewerkerAggregate
                {
                    Nummer = nummer
                };

                _buffer[nummer] = agg;
            }

            return agg;
        }
    }
}
