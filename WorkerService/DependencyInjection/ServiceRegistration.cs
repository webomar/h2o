using WorkerService.Configuration;
using WorkerService.Csv;
using WorkerService.Suppliers.ErpX;
using WorkerService.Suppliers.Youforce;

namespace WorkerService.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddImportServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.Configure<Dictionary<string, SupplierCsvOptions>>(
                config.GetSection("Suppliers"));

            services.AddSingleton<ICsvReader, CsvReader>();

            // ✅ CONCRETE supplier services
            services.AddSingleton<YouforceSupplierService>();
            services.AddSingleton<ErpXSupplierService>();

            return services;
        }
    }
}
