using WorkerService.ImportDomain;

namespace WorkerService.Assembly
{
    public class MedewerkerAssembler
    {
        private readonly Dictionary<int, MedewerkerAggregate> _buffer = new();

        // Youforce
        public void ApplyYouforce(
            int medewerkerNummer,
            string achternaam,
            DienstverbandImport dienstverband)
        {
            var agg = GetOrCreate(medewerkerNummer);

            agg.Achternaam = achternaam;
            agg.Dienstverband = dienstverband;
        }

        // ERP-X
        public void ApplyErpXContract(
            int medewerkerNummer,
            ContractImport contract)
        {
            var agg = GetOrCreate(medewerkerNummer);
            agg.Contracten.Add(contract);
        }

        public void ApplyErpXBegroting(
            int medewerkerNummer,
            BegrotingsregelImport regel)
        {
            var agg = GetOrCreate(medewerkerNummer);
            agg.Begrotingsregels.Add(regel);
        }

        // Resultaat
        public IReadOnlyCollection<MedewerkerAggregate> GetAll()
            => _buffer.Values.ToList();

        public IReadOnlyCollection<MedewerkerAggregate> GetComplete()
            => _buffer.Values.Where(x => x.IsComplete).ToList();

        public IReadOnlyCollection<MedewerkerAggregate> GetIncomplete()
            => _buffer.Values.Where(x => !x.IsComplete).ToList();

        // Intern
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
