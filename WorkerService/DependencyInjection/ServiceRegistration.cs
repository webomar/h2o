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
            // CSV configuraties (per supplier key)
            services.Configure<Dictionary<string, SupplierCsvOptions>>(
                config.GetSection("Suppliers"));

            // CSV reader
            services.AddSingleton<ICsvReader, CsvReader>();

            // YOUFORCE
            services.AddSingleton<YouforceSupplierService>();

            // ERP-X (losse CSV’s)
            services.AddSingleton<ErpXContractSupplierService>();
            services.AddSingleton<ErpXBegrotingSupplierService>();
            services.AddSingleton<ErpXInhuurSupplierService>();
            services.AddSingleton<ErpXTransactieSupplierService>();

            return services;
        }
    }
}
