using ApiService.DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
); //need package microsoft.aspnetcore.mvc.newtonsoftjson

// Add services to the container.
builder.Services.AddDbContext<DatabaseContextApi>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlDatabaseString")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO Hier moet een Authenticatie en Authorisatie service toegevoegd worden.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

// TODO UseAuthenticatie() implementatie moet hier.
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContextApi>();

    if (db.Database.CanConnect())
    {
        Console.WriteLine("✅ Database verbinding OK");
    }
    else
    {
        Console.WriteLine("❌ Kan geen verbinding maken met database");
    }
}

app.Run();
