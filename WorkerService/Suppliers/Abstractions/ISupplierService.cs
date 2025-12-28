using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerService.Suppliers.Abstractions
{
    public interface ISupplierService<T>
    {
        string SupplierKey { get; }
        IEnumerable<T> Fetch();
    }
}
