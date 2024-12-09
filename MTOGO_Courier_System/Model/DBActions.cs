using Microsoft.Data.SqlClient;
using MTOGO_Courier_System.Model;
using System.Diagnostics.Metrics;

namespace MTOGO_Courier_System.Model
{
    public class DBActions
    {
        public bool InsertCourier(Courier courier)
        {
            string query = "INSERT INTO Couriers (Name, Phone, City) " +
                       "VALUES (@Name, @Phone, @City);";
            bool insertSuccess = false;
            try
            {
                using (var connection = DatabaseConnection.Instance.Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Add parameters to the query to prevent SQL injection
                        command.Parameters.AddWithValue("@Name", courier.Name);
                        command.Parameters.AddWithValue("@Phone", courier.Phone);
                        command.Parameters.AddWithValue("@City", courier.City);

                        int rowsAffected = command.ExecuteNonQuery();
                        // Check if the insert was successful
                        if (rowsAffected > 0)
                        {
                            insertSuccess = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Invalid data: The request did not contain the information required");
            }
            finally
            {
                DatabaseConnection.Instance.CloseConnection();
            }
            return insertSuccess;
        }

        public void AssignCourier(AssignCourierRequest assignCourierRequest)
        {
            string query = "INSERT INTO CourierOrders (FK_CourierOrders_OrderID, FK_CourierOrders_CourierID) " +
                      "VALUES (@orderId, @courierId);";
            try
            {
                using (var connection = DatabaseConnection.Instance.Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Add parameters to the query to prevent SQL injection
                        command.Parameters.AddWithValue("@orderId", assignCourierRequest.OrderId);
                        command.Parameters.AddWithValue("@courierId", assignCourierRequest.CourierId);

                        int rowsAffected = command.ExecuteNonQuery();
                        
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid data: The orderId is missing or doesnt match an order");
            }
            finally
            {
                DatabaseConnection.Instance.CloseConnection();
            }
        }
    }
}
