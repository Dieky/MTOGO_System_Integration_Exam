using Microsoft.Data.SqlClient;

namespace MTOGO_Restaurant_System.Model
{
    public class DBActions
    {
        public bool InsertRestaurant(Restaurant restaurant)
        {
            string hashedPassword = PasswordHelper.HashPassword(restaurant.Password);
            string query = "INSERT INTO Restaurants (Name, Email, Phone, Address, City, Password) " +
                       "VALUES (@Name, @Email, @Phone, @Address, @City, @Password);";
            bool insertSuccess = false;
            try
            {
                using (var connection = DatabaseConnection.Instance.Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Add parameters to the query to prevent SQL injection
                        command.Parameters.AddWithValue("@Name", restaurant.Name);
                        command.Parameters.AddWithValue("@Email", restaurant.Email);
                        command.Parameters.AddWithValue("@Phone", restaurant.Phone);
                        command.Parameters.AddWithValue("@Address", restaurant.Address);
                        command.Parameters.AddWithValue("@City", restaurant.City);
                        command.Parameters.AddWithValue("@Password", hashedPassword);

                        int rowsAffected = command.ExecuteNonQuery();
                        // Check if the insert was successful
                        if (rowsAffected > 0)
                        {
                            insertSuccess = true;
                        }
                        else
                        {
                            Console.WriteLine("Customer insertion failed.");
                        }
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
            return insertSuccess;
        }

        public Restaurant GetRestaurantById(int id)
        {
            Restaurant restaurant = new Restaurant();
            string query = "SELECT * FROM Restaurants WHERE RestaurantID = @RestaurantId";
            try
            {
                using (var connection = DatabaseConnection.Instance.Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Add parameters to the query to prevent SQL injection
                        command.Parameters.AddWithValue("@RestaurantId", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                restaurant.Id = reader.GetInt32(0);
                                restaurant.Name = reader.GetString(1);
                                restaurant.Email = reader.GetString(2);
                                restaurant.Phone = reader.GetString(3);
                                restaurant.Address = reader.GetString(4);
                                restaurant.City = reader.GetString(5);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DatabaseConnection.Instance.CloseConnection();
            }
            return restaurant;
        }

        public bool AuthenticateRestaurant(string email, string enteredPassword)
        {
            string query = "SELECT Password FROM Restaurants WHERE Email = @Email;";
            bool isSuccess = false;
            try
            {
                using (var connection = DatabaseConnection.Instance.Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Add parameters to the query to prevent SQL injection
                        command.Parameters.AddWithValue("@Email", email);

                        // Execute the query and retrieve the stored hashed password
                        string storedHashedPassword = (string)command.ExecuteScalar();

                        if (storedHashedPassword != null)
                        {
                            // Verify if the entered password matches the stored hashed password
                            if (PasswordHelper.VerifyPassword(enteredPassword, storedHashedPassword))
                            {
                                isSuccess = true;
                            }
                           
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
               throw new Exception("An error occurred: " + ex.Message);
            }
            finally
            {
                DatabaseConnection.Instance.CloseConnection();
            }

            return isSuccess;
        }

        public Restaurant LoginRestaurant(string email, string password)
        {
            if (!AuthenticateRestaurant(email, password))
            {
                throw new Exception("Incorrect password");
            }

            Restaurant restaurant = new Restaurant();
            string query = "SELECT * FROM Restaurants WHERE Email = @Email;";
            using (var connection = DatabaseConnection.Instance.Connection)
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                restaurant.Id = reader.GetInt32(0);
                                restaurant.Name = reader.GetString(1);
                                restaurant.Email = reader.GetString(2);
                                restaurant.Phone = reader.GetString(3);
                                restaurant.Address = reader.GetString(4);
                                restaurant.City = reader.GetString(5);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("An error occurred: " + ex.Message);
                    }
                    finally
                    {
                        DatabaseConnection.Instance.CloseConnection();
                    }
                }
            }
            return restaurant;
        }

        public bool UpdateRestaurant(Restaurant restaurant)
        {
            // SQL query to update only the non-null fields
            // Coalesce ensures that null values does not get updated
            string query = "UPDATE Restaurants SET " +
                           "Name = COALESCE(@Name, Name), " +
                           "Email = COALESCE(@Email, Email), " +
                           "Phone = COALESCE(@Phone, Phone), " +
                           "Address = COALESCE(@Address, Address), " +
                           "City = COALESCE(@City, City), " +
                           "Password = COALESCE(@Password, Password) " +
                           "WHERE RestaurantID = @ID";
            bool updateSuccess = false;
            string hashedPassword = null;
            if (restaurant.Password != null)
            {
                hashedPassword = PasswordHelper.HashPassword(restaurant.Password);
            }
            try
            {
                using (var connection = DatabaseConnection.Instance.Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Add parameters to the query
                        // ?? is a null-coalescing operator. If left side is null or empty the right side is provided to ensure null for the DB.
                        command.Parameters.AddWithValue("@ID", restaurant.Id);
                        command.Parameters.AddWithValue("@Name", (object)restaurant.Name ?? DBNull.Value); // Use DBNull for null values
                        command.Parameters.AddWithValue("@Email", (object)restaurant.Email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Phone", (object)restaurant.Phone ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Address", (object)restaurant.Address ?? DBNull.Value);
                        command.Parameters.AddWithValue("@City", (object)restaurant.City ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Password", (object)hashedPassword ?? DBNull.Value);

                        // Execute the query to update the customer record
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            updateSuccess = true;
                        }
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
            return updateSuccess;
        }

        public void AddMenuItem(MenuItem menuItem)
        {
            string query = "INSERT INTO MenuItems (Name, Description, Price, Category, FK_MenuItems_RestaurantID)" +
                "VALUES (@Name, @Description, @Price, @Category, @FK_MenuItems_RestaurantID)";
            try
            {
                using (var connection = DatabaseConnection.Instance.Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", menuItem.Name);
                        command.Parameters.AddWithValue("@Description", menuItem.Description);
                        command.Parameters.AddWithValue("@Price", menuItem.Price);
                        command.Parameters.AddWithValue("@Category", menuItem.Category);
                        command.Parameters.AddWithValue("@FK_MenuItems_RestaurantID", menuItem.RestaurantId);
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

        public void UpdateMenuItem(MenuItem menuItem)
        {
            string query = "UPDATE MenuItems SET " +
                           "Name = COALESCE(@Name, Name), " +
                           "Description = COALESCE(@Description, Description), " +
                           "Price = COALESCE(@Price, Price), " +
                           "Category = COALESCE(@Category, Category) " +
                           "WHERE MenuItemID = @MenuItemId";
            try
            {
                using (var connection = DatabaseConnection.Instance.Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", (object)menuItem.Name ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Description", (object)menuItem.Description ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Price", (object)menuItem.Price ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Category", (object)menuItem.Category ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MenuItemID", (object)menuItem.Id ?? DBNull.Value);
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

        public List<Order> GetOrdersLast24Hours(int restaurantId)
        {
            var orders = new List<Order>();
            string query = @"SELECT * FROM Orders
            WHERE OrderDate >= DATEADD(HOUR, -24, GETDATE()) AND FK_Orders_RestaurantID = @FK_Orders_RestaurantID;";

            try
            {
                using (var connection = DatabaseConnection.Instance.Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FK_Orders_RestaurantID", restaurantId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Order order = new Order();
                                order.Id = reader.GetInt32(0);
                                order.OrderDate = reader.GetDateTime(1);
                                order.OrderStatus = reader.GetString(2);
                                order.TotalPrice = reader.GetDecimal(3);
                                order.RestaurantId = reader.GetInt32(4);
                                orders.Add(order);
                            }
                        }
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
            return orders;

        }

        public void AcceptOrder(int orderId)
        {
            string query = "UPDATE Orders SET " +
                          "OrderStatus = 'Accepted' " +
                          "WHERE OrderID = @OrderId";
            try
            {
                using (var connection = DatabaseConnection.Instance.Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderId);
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

        public void PickupOrder(int orderId)
        {
            string query = "UPDATE Orders SET " +
                          "OrderStatus = 'Being delivered' " +
                          "WHERE OrderID = @OrderId";
            try
            {
                using (var connection = DatabaseConnection.Instance.Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderId);
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

        public void OrderDelivered(int orderId)
        {
            string query = "UPDATE Orders SET " +
                          "OrderStatus = 'Delivered' " +
                          "WHERE OrderID = @OrderId";
            try
            {
                using (var connection = DatabaseConnection.Instance.Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderId);
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
