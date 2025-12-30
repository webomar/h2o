namespace WorkerService.Suppliers.Youforce
{
    public class YouforceCsvRecord
    {
        public int EmployeeNumber { get; set; }

        public string Achternaam { get; set; } = null!;

        public int DienstverbandNummer { get; set; }

        public string FunctieCode { get; set; } = null!;

        public string FunctieNaam { get; set; } = null!;

        public string Type { get; set; } = null!;

        public DateTime? DatumInDienst { get; set; }

        public DateTime? DatumUitDienst { get; set; }

        public int? Ancienniteit { get; set; }
    }
}
