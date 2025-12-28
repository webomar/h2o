using Microsoft.Extensions.Options;
using WorkerService.Configuration;
using WorkerService.Csv;
using WorkerService.Suppliers.Abstractions;

namespace WorkerService.Suppliers.ErpX
{
    public class ErpXSupplierService
        : ISupplierService<ErpXCsvRecord>
    {
        private readonly ICsvReader _csvReader;
        private readonly SupplierCsvOptions _options;

        public string SupplierKey => SupplierKeys.ErpX;

        public ErpXSupplierService(
            ICsvReader csvReader,
            IOptionsMonitor<Dictionary<string, SupplierCsvOptions>> options)
        {
            _csvReader = csvReader;
            _options = options.CurrentValue[SupplierKey];
        }

        public IEnumerable<ErpXCsvRecord> Fetch()
        {
            return _csvReader.Read<ErpXCsvRecord>(_options);
        }
    }
}
