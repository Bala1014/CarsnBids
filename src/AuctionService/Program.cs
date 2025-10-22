using AuctionService.Data;
using AuctionService.RequestHelper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// AppDomain.CurrentDomain.GetAssemblies() to get all assemblies in the current domain that inherites the Profile 
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);

var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

try
{
    DbInitializer.InitDb(app);
}
catch (Exception ex)
{
    throw ex;
}

app.Run();
