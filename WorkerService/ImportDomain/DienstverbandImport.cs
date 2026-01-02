namespace WorkerService.ImportDomain
{
    public class DienstverbandImport
    {
        public int Nummer { get; set; }
        public string Functiecode { get; set; } = null!;
        public string Functienaam { get; set; } = null!;
        public string Type { get; set; } = null!;
        public DateTime? DatumInDienst { get; set; }
        public DateTime? DatumUitDienst { get; set; }
        public int? Ancienniteit { get; set; }
    }
}
