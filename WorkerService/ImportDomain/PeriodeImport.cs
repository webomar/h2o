namespace WorkerService.ImportDomain
{
    public class PeriodeImport
    {
        public int Jaar { get; set; }
        public int Maand { get; set; }
        public bool Verwerking { get; set; }
        public string? Label { get; set; }

        public string Key => $"{Jaar}-{Maand:D2}";
    }
}
