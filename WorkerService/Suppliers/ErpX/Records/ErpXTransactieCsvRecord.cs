namespace WorkerService.Suppliers.ErpX.Records
{
    public class ErpXTransactieCsvRecord
    {
        public int PersNr { get; set; }
        public string Crediteur { get; set; } = null!;
        public string Rekening { get; set; } = null!;
        public DateTime Datum { get; set; }
        public decimal Bedrag { get; set; }
    }
}
