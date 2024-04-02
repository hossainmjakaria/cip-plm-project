using PLM.Library.Infrustuctures;
using PLM.Library.Services;
using System.Data.SqlClient;

namespace PLM.Web.Configurations;

public static class DependencyResolverExtension
{
    public static void ResolveDependencies(this IServiceCollection services)
    {
        services.AddTransient((factory) =>
        {
            var configuration = factory.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(connectionString);
        });

        services.AddTransient<IParkingDataSeederRepository, ParkingDataSeederRepository>();
        services.AddSingleton<IParkingSeederService, ParkingSeederService>();

        services.AddScoped<IParkingRepository, ParkingRepository>();
        services.AddScoped<IParkingService, ParkingService>();
    }
}
