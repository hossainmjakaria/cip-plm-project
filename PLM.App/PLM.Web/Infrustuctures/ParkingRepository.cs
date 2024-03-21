using PLM.Web.Models;
using System.Data;
using System.Data.SqlClient;

namespace PLM.Web.Infrustuctures;

public interface IParkingRepository
{
    public Task<Response<Transaction>> RegisterCarArrival(string tagNumber);
    public Task<Response<Transaction>> UpdateCarCheckOutTime(string tagNumber, int hourlyFee);
    public Task<IEnumerable<Transaction>> GetParkingSnapshot();
}

public class ParkingRepository(SqlConnection connection) : IParkingRepository
{
    public async Task<Response<Transaction>> RegisterCarArrival(string tagNumber)
    {
        var response = new Response<Transaction>();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = Constants.SpRegisterCarArrival;
                command.Parameters.AddWithValue("@TagNumber", tagNumber);

                connection.Open();
                int rowsAffected = await command.ExecuteNonQueryAsync();
                connection.Close();

                response.IsSuccess = rowsAffected > 0;
                response.Message = Constants.CarCheckedInSuccessfully;
            }
        }
        catch (SqlException ex) when (ex.Number == 50000)
        {
            response.IsSuccess = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<Response<Transaction>> UpdateCarCheckOutTime(string tagNumber, int hourlyFee)
    {
        var response = new Response<Transaction>();

        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = Constants.SpUpdateCarCheckOutTime;
                command.Parameters.AddWithValue("@TagNumber", tagNumber);
                command.Parameters.AddWithValue("@HourlyFee", hourlyFee);

                connection.Open();
                int result = await command.ExecuteNonQueryAsync();
                connection.Close();

                response.IsSuccess = result > 0;
                response.Message = Constants.CarCheckedOutSuccessfully;
            }
        }
        catch (SqlException ex) when (ex.Number == 50000)
        {
            response.IsSuccess = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<IEnumerable<Transaction>> GetParkingSnapshot()
    {
        var entities = new List<Transaction>();

        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = Constants.SpGetSnapshot;
                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var parkingTransaction = MapParkingTransaction(reader);
                        entities.Add(parkingTransaction);
                    }
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}"); return entities;
        }

        return entities;
    }

    private Transaction MapParkingTransaction(SqlDataReader reader) =>
        new()
        {
            TagNumber = reader.GetString(reader.GetOrdinal("TagNumber")),
            CheckInTime = reader.GetDateTime(reader.GetOrdinal("CheckInTime")),
            ElapsedHours = reader.GetInt32(reader.GetOrdinal("ElapsedHours"))
        };
}