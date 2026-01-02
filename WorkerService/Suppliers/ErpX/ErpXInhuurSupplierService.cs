using Microsoft.Extensions.Options;
using WorkerService.Configuration;
using WorkerService.Csv;
using WorkerService.Suppliers.Abstractions;
using WorkerService.Suppliers.ErpX.Records;

namespace WorkerService.Suppliers.ErpX
{
    public class ErpXInhuurSupplierService
        : ISupplierService<ErpXInhuurCsvRecord>
    {
        public string SupplierKey => SupplierKeys.ErpXInhuur;

        private readonly ICsvReader _csvReader;
        private readonly SupplierCsvOptions _options;

        public ErpXInhuurSupplierService(
            ICsvReader csvReader,
            IOptionsMonitor<Dictionary<string, SupplierCsvOptions>> options)
        {
            _csvReader = csvReader;
            _options = options.CurrentValue[SupplierKey];
        }

        public IEnumerable<ErpXInhuurCsvRecord> Fetch()
            => _csvReader.Read<ErpXInhuurCsvRecord>(_options);
    }
}
