using WorkerService.DAL;
using Microsoft.EntityFrameworkCore;
using WorkerService;

var builder = Host.CreateApplicationBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DatabaseContextWorker>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlDatabaseString")));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
