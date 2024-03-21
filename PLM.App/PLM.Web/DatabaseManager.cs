using System.Data;
using System.Data.SqlClient;

namespace PLM.Web
{

    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection");

        public void AddCar(string tagNumber)
        {
            using SqlConnection connection = new(_connectionString);
            string query = "INSERT INTO Cars (TagNumber) VALUES (@TagNumber)";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TagNumber", tagNumber);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine("Error adding car: " + ex.Message);
            }
        }

        public void UpdateCarCheckOutTime(string tagNumber, DateTime checkOutTime)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Transactions SET CheckOutTime = @CheckOutTime WHERE CarID = (SELECT CarID FROM Cars WHERE TagNumber = @TagNumber)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CheckOutTime", checkOutTime);
                command.Parameters.AddWithValue("@TagNumber", tagNumber);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error updating check-out time: " + ex.Message);
                }
            }
        }

        public DataTable GetParkingSnapshot()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT SpotID, TagNumber FROM ParkingSpots LEFT JOIN Cars ON ParkingSpots.SpotID = Cars.SpotID";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    // Handle exception
                    Console.WriteLine("Error getting parking snapshot: " + ex.Message);
                }
            }

            return dt;

        }


    }

}
