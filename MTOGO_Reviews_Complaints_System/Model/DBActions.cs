using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace MTOGO_Reviews_Complaints_System.Model
{
    public class DBActions
    {
        public void InsertCustomerReview(CustomerReview review)
        {   
            string query = @"
            INSERT INTO CustomerReviews (Text, Rating, FK_CustomerReviews_OrderID)
            VALUES (@Text, @Rating, @OrderId);";
            try
            {
                using (var connection = DatabaseConnection.Instance.Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Text", review.Text);
                        command.Parameters.AddWithValue("@Rating", review.Rating);
                        command.Parameters.AddWithValue("@OrderId", review.OrderId);
                        command.ExecuteScalar();
                       
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                DatabaseConnection.Instance.CloseConnection();
            }

        }

        public void InsertRestaurantComplaint(RestaurantComplaint complaint)
        {
            string query = @"
            INSERT INTO RestaurantComplaints (Text, FK_RestaurantComplaints_OrderID)
            VALUES (@Text, @OrderId);";
            try
            {
                using (var connection = DatabaseConnection.Instance.Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Text", complaint.Text);
                        command.Parameters.AddWithValue("@OrderId", complaint.OrderId);
                        command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                DatabaseConnection.Instance.CloseConnection();
            }

        }
    }
}
