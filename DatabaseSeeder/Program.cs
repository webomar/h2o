using ApiService;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "ApiService"))
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var configuration = builder.Build();

// Check for command line argument to clear existing data
bool clearExisting = args.Length > 0 && (args[0].ToLower() == "--clear" || args[0].ToLower() == "-c");

try
{
    Console.WriteLine("Starting database seeding...\n");
    if (clearExisting)
    {
        Console.WriteLine("⚠️  Clear existing data mode enabled.\n");
    }
    await DataSeeder.SeedAsync(configuration, clearExisting);
    Console.WriteLine("\n✅ Seeding completed successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error seeding database: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"   Inner: {ex.InnerException.Message}");
    }
    Console.WriteLine($"\nStack trace: {ex.StackTrace}");
    Environment.Exit(1);
}
