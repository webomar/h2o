using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using WorkerService.Configuration;
using WorkerService.Csv;

public class CsvReader : ICsvReader
{
    public IEnumerable<T> Read<T>(SupplierCsvOptions options)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = options.Delimiter,
            HasHeaderRecord = options.HasHeader
        };

         
        using var reader = new StreamReader(
            options.FilePath,
            Encoding.GetEncoding(options.Encoding));

        using var csv = new CsvHelper.CsvReader(reader, config);

        return csv.GetRecords<T>().ToList();
    }
}

