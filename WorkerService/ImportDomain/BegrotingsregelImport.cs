using ApiService.Models;

namespace WorkerService.ImportDomain
{
    public class BegrotingsregelImport
    {
        public int BegrotingJaar { get; set; }
        public string? KostenplaatsCode { get; set; }
        public Kostensoort? Kostensoort { get; set; }
        public decimal Bedrag { get; set; }
    }
}
