namespace WorkerService.ImportDomain
{
    public class TransactieImport
    {
        public string Crediteur { get; set; } = null!;
        public string? Rekening { get; set; }
        public DateTime Datum { get; set; }
        public decimal Bedrag { get; set; }
    }
}
