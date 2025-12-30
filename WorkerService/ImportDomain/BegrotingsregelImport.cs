namespace WorkerService.ImportDomain
{
    public class BegrotingsregelImport
    {
        public int Jaar { get; set; }
        public string? KostenplaatsCode { get; set; }
        public decimal Bedrag { get; set; }
        public string Kostensoort { get; set; } = null!;
    }
}
