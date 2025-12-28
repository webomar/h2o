using WorkerService.Configuration;
using WorkerService.Domain;
using WorkerService.Mapping;


namespace WorkerService.Suppliers.Youforce
{
    public class YouforceMapper
        : ISupplierMapper<YouforceCsvRecord>
    {
        public string SupplierKey => SupplierKeys.Youforce;

        public Employee Map(YouforceCsvRecord r)
        {
            return new Employee
            {
                ExternalId = r.EmployeeNumber,
                Name = r.FullName,
                Email = r.Email,
                Source = SupplierKey
            };
        }
    }
}

