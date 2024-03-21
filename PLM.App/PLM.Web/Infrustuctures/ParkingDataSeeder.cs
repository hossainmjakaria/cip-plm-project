using PLM.Web.Models;
using System.Data;
using System.Data.SqlClient;

namespace PLM.Web.Infrustuctures;

public interface IParkingDataSeederRepository
{
    public Task<bool> SeedAsync(int totalSpots);
}

public class ParkingDataSeederRepository(SqlConnection connection) : IParkingDataSeederRepository
{
    public async Task<bool> SeedAsync(int totalSpots)
    {

        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = Constants.SpEnsureParkingSpot;
                command.Parameters.AddWithValue("@TotalSpots", totalSpots);

                connection.Open();
                var result = await command.ExecuteScalarAsync();
                connection.Close();

                return Convert.ToInt32(result) == totalSpots;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return false;
        }
    }
}