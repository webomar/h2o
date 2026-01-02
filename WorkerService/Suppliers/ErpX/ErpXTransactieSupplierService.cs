using Microsoft.Extensions.Options;
using WorkerService.Configuration;
using WorkerService.Csv;
using WorkerService.Suppliers.Abstractions;
using WorkerService.Suppliers.ErpX.Records;

namespace WorkerService.Suppliers.ErpX
{
    public class ErpXTransactieSupplierService
        : ISupplierService<ErpXTransactieCsvRecord>
    {
        public string SupplierKey => SupplierKeys.ErpXTransactie;

        private readonly ICsvReader _csvReader;
        private readonly SupplierCsvOptions _options;

        public ErpXTransactieSupplierService(
            ICsvReader csvReader,
            IOptionsMonitor<Dictionary<string, SupplierCsvOptions>> options)
        {
            _csvReader = csvReader;
            _options = options.CurrentValue[SupplierKey];
        }

        public IEnumerable<ErpXTransactieCsvRecord> Fetch()
            => _csvReader.Read<ErpXTransactieCsvRecord>(_options);
    }
}
