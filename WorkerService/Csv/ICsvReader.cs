using WorkerService.Configuration;

namespace WorkerService.Csv
{
    public interface ICsvReader
    {
        IEnumerable<T> Read<T>(SupplierCsvOptions options);
    }
}
