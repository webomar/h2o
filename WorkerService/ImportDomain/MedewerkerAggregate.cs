namespace WorkerService.ImportDomain
{
    public class MedewerkerAggregate
    {
        public int Nummer { get; set; }
        public string? Achternaam { get; set; }

        public DienstverbandImport? Dienstverband { get; set; }

        public List<ContractImport> Contracten { get; } = new();
        public List<BegrotingsregelImport> Begrotingsregels { get; } = new();
        public List<InhuurkostenImport> Inhuurkosten { get; } = new();
        public List<TransactieImport> Transacties { get; } = new();
        public List<KostenplaatsImport> Kostenplaatsen { get; } = new();

        public bool IsComplete =>
            Achternaam != null &&
            Dienstverband != null;
    }
}
