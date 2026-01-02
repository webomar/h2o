namespace WorkerService.Suppliers.ErpX.Records
{
    public sealed class ErpXContractCsvRecord
    {
        public int PersNr { get; set; }

        public string Crediteur { get; set; } = null!;

        public string? Rekening { get; set; }

        public string? OrganisatorischeEenheidCode { get; set; }
    }
}
