using WorkerService.Configuration;
using WorkerService.Conversion;
using WorkerService.Csv;
using WorkerService.Mapping;
using WorkerService.Suppliers.Abstractions;
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

            // Suppliers
            services.AddSingleton<ISupplierService<YouforceCsvRecord>, YouforceSupplierService>();
            services.AddSingleton<ISupplierService<ErpXCsvRecord>, ErpXSupplierService>();

            // Mappers
            services.AddSingleton<ISupplierMapper<YouforceCsvRecord>, YouforceMapper>();
            services.AddSingleton<ISupplierMapper<ErpXCsvRecord>, ErpXMapper>();

            services.AddSingleton<IMapperRegistry, MapperRegistry>();
            services.AddSingleton<IConversionService, ConversionService>();

            return services;
        }
    }
}
