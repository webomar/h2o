using Microsoft.Extensions.Options;
using WorkerService.Configuration;
using WorkerService.Csv;
using WorkerService.Suppliers.Abstractions;
using WorkerService.Suppliers.ErpX.Records;

namespace WorkerService.Suppliers.ErpX
{
    public class ErpXContractSupplierService
        : ISupplierService<ErpXContractCsvRecord>
    {
        public string SupplierKey => SupplierKeys.ErpXContract;

        private readonly ICsvReader _csvReader;
        private readonly SupplierCsvOptions _options;

        public ErpXContractSupplierService(
            ICsvReader csvReader,
            IOptionsMonitor<Dictionary<string, SupplierCsvOptions>> options)
        {
            _csvReader = csvReader;
            _options = options.CurrentValue[SupplierKey];
        }

        public IEnumerable<ErpXContractCsvRecord> Fetch()
            => _csvReader.Read<ErpXContractCsvRecord>(_options);
    }
}


