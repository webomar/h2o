namespace WorkerService.Configuration
{
    public class SupplierCsvOptions
    {
        public string FilePath { get; set; } = string.Empty;
        public string Delimiter { get; set; } = ";";
        public string Encoding { get; set; } = "utf-8";
        public bool HasHeader { get; set; } = true;
    }

}
