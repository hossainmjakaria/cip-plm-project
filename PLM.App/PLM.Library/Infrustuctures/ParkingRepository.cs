using PLM.Library.Models;
using PLM.Library.Utility;
using System.Data;
using System.Data.SqlClient;

namespace PLM.Library.Infrustuctures;

public interface IParkingRepository
{
    public Task<Response<bool>> RegisterCarArrival(string tagNumber);
    public Task<Response<decimal>> UpdateCarCheckOutTime(string tagNumber, int hourlyFee);
    public Task<IEnumerable<Transaction>> GetParkingSnapshot();
    public Task<ParkingStats> GetParkingStatistics();
}

public class ParkingRepository(SqlConnection connection) : IParkingRepository
{
    public async Task<Response<bool>> RegisterCarArrival(string tagNumber)
    {
        var response = new Response<bool>();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = Constants.SpRegisterCarArrival;
                command.Parameters.AddWithValue("@TagNumber", tagNumber);
                SqlParameter outParameter = new("@RowsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(outParameter);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                int rowsAffected = Convert.ToInt32(outParameter.Value);
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

    public async Task<Response<decimal>> UpdateCarCheckOutTime(string tagNumber, int hourlyFee)
    {
        var response = new Response<decimal>();

        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = Constants.SpUpdateCarCheckOutTime;

                SqlParameter rowsAffected = new("@RowsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output };
                SqlParameter chargedAmount = new("@ChargedAmount", SqlDbType.Int) { Direction = ParameterDirection.Output };

                command.Parameters.AddWithValue("@TagNumber", tagNumber);
                command.Parameters.AddWithValue("@HourlyFee", hourlyFee);
                command.Parameters.Add(rowsAffected);
                command.Parameters.Add(chargedAmount);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                int rows = Convert.ToInt32(rowsAffected.Value);
                response.Model = Convert.ToDecimal(chargedAmount.Value);
                connection.Close();

                response.IsSuccess = rows > 0;
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

    public async Task<ParkingStats> GetParkingStatistics()
    {
        ParkingStats parkingStats = new();

        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = Constants.SpGetParkingStats;
                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        parkingStats.SpotsAvailable = reader.GetInt32(reader.GetOrdinal("SpotsAvailable"));
                        parkingStats.TodayRevenue = reader.GetDecimal(reader.GetOrdinal("TodayRevenue"));
                        parkingStats.AvgCarsPerDayLast30Days = reader.GetDecimal(reader.GetOrdinal("AvgCarsPerDayLast30Days"));
                        parkingStats.AvgRevenuePerDayLast30Days = reader.GetDecimal(reader.GetOrdinal("AvgRevenuePerDayLast30Days"));
                    }
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}"); return parkingStats;
        }

        return parkingStats;
    }

    private Transaction MapParkingTransaction(SqlDataReader reader) =>
        new()
        {
            TagNumber = reader.GetString(reader.GetOrdinal("TagNumber")),
            CheckInTime = reader.GetDateTime(reader.GetOrdinal("CheckInTime")),
            ElapsedHours = reader.GetInt32(reader.GetOrdinal("ElapsedHours"))
        };
}