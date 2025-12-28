using System;
using System.Collections.Generic;
using System.Text;
using WorkerService.Domain;
using WorkerService.Suppliers.Abstractions;
using WorkerService.Suppliers.ErpX;
using WorkerService.Suppliers.Youforce;

namespace WorkerService.Mapping
{
    public class MapperRegistry : IMapperRegistry
    {
        private readonly IServiceProvider _provider;

        public MapperRegistry(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<Employee> MapAll()
        {
            var employees = new List<Employee>();

            employees.AddRange(Map<YouforceCsvRecord>());
            employees.AddRange(Map<ErpXCsvRecord>());

            return employees;
        }

        private IEnumerable<Employee> Map<T>()
        {
            var supplier = _provider.GetService<ISupplierService<T>>();
            var mapper = _provider.GetService<ISupplierMapper<T>>();

            if (supplier == null || mapper == null)
                return Enumerable.Empty<Employee>();

            return supplier.Fetch().Select(mapper.Map);
        }
    }

}
