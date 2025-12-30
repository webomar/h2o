using Microsoft.EntityFrameworkCore;
using WorkerService.DAL;
using WorkerService.DependencyInjection;
using WorkerService.Worker;

var builder = Host.CreateApplicationBuilder(args);

// Database
builder.Services.AddDbContext<DatabaseContextWorker>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlDatabaseString")));

// Import services (CSV, suppliers, mapping)
builder.Services.AddImportServices(builder.Configuration);

// Worker
builder.Services.AddHostedService<ImportWorker>();

var host = builder.Build();
host.Run();
