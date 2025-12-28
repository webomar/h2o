using System;
using System.Collections.Generic;
using System.Text;
using WorkerService.Domain;

namespace WorkerService.Mapping
{
    public interface IMapperRegistry
    {
        IEnumerable<Employee> MapAll();
    }
}
