using WorkerService.Domain;
using WorkerService.Mapping;

namespace WorkerService.Suppliers.ErpX
{
    public class ErpXMapper
        : ISupplierMapper<ErpXCsvRecord>
    {
        public string SupplierKey => "ErpX";

        public Employee Map(ErpXCsvRecord r)
        {
            return new Employee
            {
                ExternalId = r.PersNr,
                Name = $"{r.Voornaam} {r.Achternaam}",
                Source = SupplierKey
            };
        }
    }
}
