namespace WorkerService.ImportDomain
{
    public class InhuurkostenImport
    {
        public int Jaar { get; set; }
        public int Maand { get; set; }
        public string KostenplaatsCode { get; set; } = null!;
        public decimal Bedrag { get; set; }
        public PeriodeImport Periode { get; set; } = null!;

    }
}
