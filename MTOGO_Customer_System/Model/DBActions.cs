using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using MTOGO_Customer_System.Model.DTO;
using MTOGO_Customer_System.Model.Interfaces;

namespace MTOGO_Customer_System.Model
{
    public class DBActions : ICustomerDBActions
    {
        public bool InsertCustomer(Customer customer, string catalog = "MTOGO")
        {
            if (string.IsNullOrEmpty(customer.Password))
            {
                return false;
            }
            string hashedPassword = PasswordHelper.HashPassword(customer.Password);
            string query = "INSERT INTO Customers (Name, Email, Phone, Address, City, Password) " +
                       "VALUES (@Name, @Email, @Phone, @Address, @City, @Password);";
            bool insertSuccess = false;
            try
            {
                using (var connection = DatabaseConnection.Instance(catalog).Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Add parameters to the query to prevent SQL injection
                        command.Parameters.AddWithValue("@Name", customer.Name);
                        command.Parameters.AddWithValue("@Email", customer.Email);
                        command.Parameters.AddWithValue("@Phone", customer.Phone);
                        command.Parameters.AddWithValue("@Address", customer.Address);
                        command.Parameters.AddWithValue("@City", customer.City);
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
                //throw new Exception("Missing data");
            }
            finally
            {
                DatabaseConnection.Instance(catalog).CloseConnection();
            }
            return insertSuccess;
        }

        public Customer GetCustomerById(int id, string catalog = "MTOGO")
        {
            Customer customer = new Customer();
            string query = "SELECT * FROM Customers WHERE CustomerID = @CustomerId";
            try
            {
                using (var connection = DatabaseConnection.Instance(catalog).Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Add parameters to the query to prevent SQL injection
                        command.Parameters.AddWithValue("@CustomerId", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                customer.Id = reader.GetInt32(0);
                                customer.Name = reader.GetString(1);
                                customer.Email = reader.GetString(2);
                                customer.Phone = reader.GetString(3);
                                customer.Address = reader.GetString(4);
                                customer.City = reader.GetString(5);
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
                DatabaseConnection.Instance(catalog).CloseConnection();
            }
            return customer;
        }

        public bool AuthenticateCustomer(string email, string enteredPassword, string catalog = "MTOGO")
        {
            string query = "SELECT Password FROM Customers WHERE Email = @Email;";
            try
            {
                using (var connection = DatabaseConnection.Instance(catalog).Connection)
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
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Customer not found.");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
            finally
            {
                DatabaseConnection.Instance(catalog).CloseConnection();
            }
        }

        public Customer LoginCustomer(string email, string password, string catalog = "MTOGO")
        {
            if (!AuthenticateCustomer(email, password))
            {
                throw new Exception("Incorrect password");
            }

            Customer customer = new Customer();
            string query = "SELECT * FROM Customers WHERE Email = @Email;";
            using (var connection = DatabaseConnection.Instance(catalog).Connection)
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
                                customer.Id = reader.GetInt32(0);
                                customer.Name = reader.GetString(1);
                                customer.Email = reader.GetString(2);
                                customer.Phone = reader.GetString(3);
                                customer.Address = reader.GetString(4);
                                customer.City = reader.GetString(5);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                    finally
                    {
                        DatabaseConnection.Instance(catalog).CloseConnection();
                    }
                }
            }
            return customer;
        }

        public bool UpdateCustomer(Customer customer, string catalog = "MTOGO")
        {
            // SQL query to update only the non-null fields
            // Coalesce ensures that null values does not get updated
            string query = "UPDATE Customers SET " +
                           "Name = COALESCE(@Name, Name), " +
                           "Email = COALESCE(@Email, Email), " +
                           "Phone = COALESCE(@Phone, Phone), " +
                           "Address = COALESCE(@Address, Address), " +
                           "City = COALESCE(@City, City), " +
                           "Password = COALESCE(@Password, Password) " +
                           "WHERE CustomerID = @ID";
            bool updateSuccess = false;
            string hashedPassword = null;
            if (customer.Password != null)
            {
                hashedPassword = PasswordHelper.HashPassword(customer.Password);
            }
            try
            {
                using (var connection = DatabaseConnection.Instance(catalog).Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Add parameters to the query
                        // ?? is a null-coalescing operator. If left side is null or empty the right side is provided to ensure null for the DB.
                        command.Parameters.AddWithValue("@ID", customer.Id);
                        command.Parameters.AddWithValue("@Name", (object)customer.Name ?? DBNull.Value); // Use DBNull for null values
                        command.Parameters.AddWithValue("@Email", (object)customer.Email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Phone", (object)customer.Phone ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);
                        command.Parameters.AddWithValue("@City", (object)customer.City ?? DBNull.Value);
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
                DatabaseConnection.Instance(catalog).CloseConnection();
            }
            return updateSuccess;
        }

        public int PlaceOrder(int customerId, List<OrderLine> orderLines, string catalog = "MTOGO")
        {
            string query = @"
            INSERT INTO Orders (OrderDate, OrderStatus, Total, FK_Orders_RestaurantID, FK_Orders_CustomerID)
            OUTPUT INSERTED.OrderID
            VALUES (GETDATE(), 'Pending', 0.00, @FK_Orders_RestaurantID, @FK_Orders_CustomerID);";
            int orderID = 0;
            try
            {
                using (var connection = DatabaseConnection.Instance(catalog).Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FK_Orders_RestaurantID", orderLines[0].RestaurantId);
                        command.Parameters.AddWithValue("@FK_Orders_CustomerID", customerId);
                        orderID = (int)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                DatabaseConnection.Instance(catalog).CloseConnection();
            }

            foreach (OrderLine orderLine in orderLines)
            {
                AddOrderLine(orderID, orderLine, catalog);
            }

            return orderID;

        }

        public void AddOrderLine(int orderId, OrderLine orderLine, string catalog = "MTOGO")
        {
            string query = @"
            INSERT INTO OrderLines (Quantity, FK_OrderLines_OrderID, FK_OrderLines_MenuItemID, FK_OrderLines_RestaurantID)
            VALUES (@Quantity, @FK_OrderLines_OrderID, @FK_OrderLines_MenuItemID, @FK_OrderLines_RestaurantID);";

            try
            {
                using (var connection = DatabaseConnection.Instance(catalog).Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Quantity", orderLine.Quantity);
                        command.Parameters.AddWithValue("@FK_OrderLines_OrderID", orderId);
                        command.Parameters.AddWithValue("@FK_OrderLines_MenuItemID", orderLine.MenuItemId);
                        command.Parameters.AddWithValue("@FK_OrderLines_RestaurantID", orderLine.RestaurantId);
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
                DatabaseConnection.Instance(catalog).CloseConnection();
            }
        }

        public void UpdateOrderPrice(int orderId, string catalog = "MTOGO")
        {
            string query = @"
            WITH TotalCalc AS (
            SELECT ol.FK_OrderLines_OrderID, SUM(ol.Quantity * m.Price) AS OrderTotal
            FROM OrderLines ol
            JOIN MenuItems m ON ol.FK_OrderLines_MenuItemID = m.MenuItemID
            WHERE ol.FK_OrderLines_OrderID = @OrderID
            GROUP BY ol.FK_OrderLines_OrderID
            )
            UPDATE o
            SET o.Total = t.OrderTotal
            FROM Orders o
            JOIN TotalCalc t ON o.OrderID = t.FK_OrderLines_OrderID
            WHERE o.OrderID = @OrderID;
            ";

            try
            {
                using (var connection = DatabaseConnection.Instance(catalog).Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderID", orderId);
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
                DatabaseConnection.Instance(catalog).CloseConnection();
            }

        }

        public List<OrderDTO> GetOrdersByCustomerId(int customerId, string catalog = "MTOGO")
        {
            List<OrderDTO> orders = new List<OrderDTO>();
            string query = @"
            SELECT OrderID,
            OrderDate,
            Total,
            FK_Orders_RestaurantID,
            MenuItems.Name,
            MenuItems.Price
            FROM Orders
            RIGHT JOIN OrderLines ON Orders.OrderID = OrderLines.FK_OrderLines_OrderID
            RIGHT JOIN MenuItems ON OrderLines.FK_OrderLines_MenuItemID = MenuItems.MenuItemID
            WHERE FK_Orders_CustomerID = @CustomerId;
            ";

            try
            {
                using (var connection = DatabaseConnection.Instance(catalog).Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", customerId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrderDTO order = new OrderDTO();
                                order.Id = reader.GetInt32(0);
                                order.OrderDate = reader.GetDateTime(1);
                                order.TotalPrice = reader.GetDecimal(2);
                                order.RestaurantId = reader.GetInt32(3);

                                OrderLineDTO line = new OrderLineDTO();
                                line.Name = reader.GetString(4);
                                line.Price = reader.GetDecimal(5);

                                order.OrderLineDTO = line;
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
                DatabaseConnection.Instance(catalog).CloseConnection();
            }
            orders = RemoveRedundantInfo(orders);
            return orders;
        }

        // The query result from getordersbycustomerid returns rows that can be almost identical.
        // This method returns a clean list to avoid redundancy
        public List<OrderDTO> RemoveRedundantInfo(List<OrderDTO> orders)
        {
            if (orders.IsNullOrEmpty())
            {
                Console.WriteLine("No results were found");
            }

            List<OrderDTO> cleanOrders = new List<OrderDTO>();
            int currentOrderID = 0;
            List<OrderLineDTO> lines = new List<OrderLineDTO>();

            foreach (OrderDTO order in orders)
            {
                if (order.Id == currentOrderID)
                {
                    OrderLineDTO line = new OrderLineDTO();
                    line.Name = order.OrderLineDTO.Name;
                    line.Price = order.OrderLineDTO.Price;
                    lines.Add(line);

                }

                if (order.Id != currentOrderID)
                {
                    OrderDTO orderDTO = new OrderDTO();
                    orderDTO.Id = order.Id;
                    orderDTO.OrderDate = order.OrderDate;
                    orderDTO.TotalPrice = order.TotalPrice;
                    orderDTO.RestaurantId = order.RestaurantId;

                    lines = new List<OrderLineDTO>();
                    OrderLineDTO line = new OrderLineDTO();
                    line.Name = order.OrderLineDTO.Name;
                    line.Price = order.OrderLineDTO.Price;
                    lines.Add(line);
                    orderDTO.OrderLines = lines;

                    currentOrderID = order.Id;
                    cleanOrders.Add(orderDTO);
                }

            }
            return cleanOrders;

        }

        public OrderDTO GetOrderById(int orderId, string catalog = "MTOGO")
        {
            OrderDTO orderDTO = new OrderDTO();
            List<OrderLineDTO> lines = new List<OrderLineDTO>();
            string query = @"
            SELECT OrderID,
            OrderDate,
            Total,
            FK_Orders_RestaurantID,
            MenuItems.Name,
            MenuItems.Price
            FROM Orders
            RIGHT JOIN OrderLines ON Orders.OrderID = OrderLines.FK_OrderLines_OrderID
            RIGHT JOIN MenuItems ON OrderLines.FK_OrderLines_MenuItemID = MenuItems.MenuItemID
            WHERE OrderID = @orderId;
            ";

            try
            {
                using (var connection = DatabaseConnection.Instance(catalog).Connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@orderId", orderId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                orderDTO.Id = reader.GetInt32(0);
                                orderDTO.OrderDate = reader.GetDateTime(1);
                                orderDTO.TotalPrice = reader.GetDecimal(2);
                                orderDTO.RestaurantId = reader.GetInt32(3);

                                OrderLineDTO line = new OrderLineDTO();
                                line.Name = reader.GetString(4);
                                line.Price = reader.GetDecimal(5);

                                lines.Add(line);
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
                DatabaseConnection.Instance(catalog).CloseConnection();
            }
            orderDTO.OrderLines = lines;
            return orderDTO;
        }
    }
}
