namespace WorkerService.ImportDomain
{
    public class MedewerkerAggregate
    {
        public int Nummer { get; set; }

        // Youforce (HR)
        public string? Achternaam { get; set; }
        public DienstverbandImport? Dienstverband { get; set; }

        // ERP-X (financieel)
        public List<ContractImport> Contracten { get; } = new();
        public List<BegrotingsregelImport> Begrotingsregels { get; } = new();

        public bool IsComplete =>
            Nummer > 0 &&
            !string.IsNullOrWhiteSpace(Achternaam) &&
            Dienstverband != null;
    }
}
