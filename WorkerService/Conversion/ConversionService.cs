using System;
using System.Collections.Generic;
using System.Text;
using WorkerService.Domain;
using WorkerService.Mapping;

namespace WorkerService.Conversion
{
    public class ConversionService : IConversionService
    {
        private readonly IMapperRegistry _mapperRegistry;

        public ConversionService(IMapperRegistry mapperRegistry)
        {
            _mapperRegistry = mapperRegistry;
        }

        public IEnumerable<Employee> Convert()
        {
            return _mapperRegistry
                .MapAll()
                .GroupBy(e => e.ExternalId)
                .Select(g => g.First());
        }
    }
}
