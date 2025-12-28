using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using WorkerService.Configuration;
using WorkerService.Csv;
using WorkerService.Suppliers.Abstractions;

namespace WorkerService.Suppliers.Youforce
{
    public class YouforceSupplierService
        : ISupplierService<YouforceCsvRecord>
    {
        private readonly ICsvReader _csvReader;
        private readonly SupplierCsvOptions _options;

        public string SupplierKey => SupplierKeys.Youforce;

        public YouforceSupplierService(
            ICsvReader csvReader,
            IOptionsMonitor<Dictionary<string, SupplierCsvOptions>> options)
        {
            _csvReader = csvReader;
            _options = options.CurrentValue[SupplierKey];
        }

        public IEnumerable<YouforceCsvRecord> Fetch()
            => _csvReader.Read<YouforceCsvRecord>(_options);
    }
}
