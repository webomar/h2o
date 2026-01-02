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
            HasHeaderRecord = options.HasHeader,

            HeaderValidated = null,
            MissingFieldFound = null,
            BadDataFound = null,
        };

        using var reader = new StreamReader(
            options.FilePath,
            Encoding.GetEncoding(options.Encoding));

        using var csv = new CsvHelper.CsvReader(reader, config);

        // 🔹 Registreer ClassMap automatisch
        RegisterMapIfExists<T>(csv);

        // 🔹 Lege cellen → null
        csv.Context.TypeConverterOptionsCache.GetOptions<int?>().NullValues.Add("");
        csv.Context.TypeConverterOptionsCache.GetOptions<decimal?>().NullValues.Add("");
        csv.Context.TypeConverterOptionsCache.GetOptions<DateTime?>().NullValues.Add("");

        return csv.GetRecords<T>().ToList();
    }

    private static void RegisterMapIfExists<T>(CsvHelper.CsvReader csv)
    {
        var mapType = typeof(T).Assembly
            .GetTypes()
            .FirstOrDefault(t =>
                typeof(ClassMap).IsAssignableFrom(t) &&
                t.Name == typeof(T).Name + "Map");

        if (mapType != null)
        {
            csv.Context.RegisterClassMap(mapType);
        }
    }
}
