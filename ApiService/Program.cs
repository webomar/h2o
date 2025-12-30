using ApiService.DAL;
using ApiService.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5050);
});


builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
); //need package microsoft.aspnetcore.mvc.newtonsoftjson

// Add services to the container.
builder.Services.AddDbContext<DatabaseContextApi>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlDatabaseString")));


// Kerberos/NTLM via Negotiate
builder.Services
    .AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddTransient<IClaimsTransformation, AdClaimsTransformer>();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});
// End Kerberos/NTLM


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable Authentication and Authorization using Kerberos/NTLM
app.UseAuthentication();
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
