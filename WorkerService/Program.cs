//using WorkerService.DAL;
//using Microsoft.EntityFrameworkCore;
//using WorkerService;

//var builder = Host.CreateApplicationBuilder(args);

//// Add services to the container.
//builder.Services.AddDbContext<DatabaseContextWorker>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlDatabaseString")));

//builder.Services.AddHostedService<Worker>();

//var host = builder.Build();
//host.Run();

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
