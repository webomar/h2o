using Microsoft.Extensions.Options;
using WorkerService.Configuration;
using WorkerService.Csv;
using WorkerService.Suppliers.Abstractions;
using WorkerService.Suppliers.ErpX.Records;

namespace WorkerService.Suppliers.ErpX
{
    public class ErpXBegrotingSupplierService
        : ISupplierService<ErpXBegrotingCsvRecord>
    {
        public string SupplierKey => SupplierKeys.ErpXBegroting;

        private readonly ICsvReader _csvReader;
        private readonly SupplierCsvOptions _options;

        public ErpXBegrotingSupplierService(
            ICsvReader csvReader,
            IOptionsMonitor<Dictionary<string, SupplierCsvOptions>> options)
        {
            _csvReader = csvReader;
            _options = options.CurrentValue[SupplierKey];
        }

        public IEnumerable<ErpXBegrotingCsvRecord> Fetch()
            => _csvReader.Read<ErpXBegrotingCsvRecord>(_options);
    }
}
