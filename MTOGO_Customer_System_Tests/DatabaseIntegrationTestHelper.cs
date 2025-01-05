using Castle.Core.Resource;
using Microsoft.Data.SqlClient;
using MTOGO_Customer_System.Model;
using MTOGO_Customer_System_Tests;
using System.Collections.Generic;

public class DatabaseIntegrationTestHelper
{
    private const string ConnectionString = "data source=Diekmann-Laptop;trusted_connection=true;TrustServerCertificate=True;";
    private static readonly string SQLFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mtogo DB schema recreate.sql");

    public static void RecreateDatabase()
    {
        // Drop the database if it exists
        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "IF EXISTS (SELECT * FROM sys.databases WHERE name = 'MTOGO_TEST') " +
                                  "BEGIN DROP DATABASE MTOGO_TEST END";
            command.ExecuteNonQuery();
        }

        // Create the database again
        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "CREATE DATABASE MTOGO_TEST";
            command.ExecuteNonQuery();
        }

        // Run your SQL script to create tables
        string script = File.ReadAllText(SQLFilePath);

        using (var connection = new SqlConnection(ConnectionString + "Initial Catalog=MTOGO_TEST;"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = script;
            command.ExecuteNonQuery();
        }

        // Adding dummy data for tests
        using (var connection = new SqlConnection(ConnectionString + "Initial Catalog=MTOGO_TEST;"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            Customer customer = new Customer
            {
                Name = "Paul Smith",
                Email = "paulsmith@gmail.com",
                Phone = "10203040",
                Address = "Smith Lane 54",
                City = "Dallas",
                Password = "smith"
            };

            string hashedPassword = PasswordHelper.HashPassword(customer.Password);
            command.CommandText = "INSERT INTO Customers (Name, Email, Phone, Address, City, Password) " +
                       "VALUES (@Name, @Email, @Phone, @Address, @City, @Password);";
            command.Parameters.AddWithValue("@Name", customer.Name);
            command.Parameters.AddWithValue("@Email", customer.Email);
            command.Parameters.AddWithValue("@Phone", customer.Phone);
            command.Parameters.AddWithValue("@Address", customer.Address);
            command.Parameters.AddWithValue("@City", customer.City);
            command.Parameters.AddWithValue("@Password", hashedPassword);
            command.ExecuteNonQuery();
        }

        // Restaurant dummy data
        using (var connection = new SqlConnection(ConnectionString + "Initial Catalog=MTOGO_TEST;"))
        {
            connection.Open();
            var command = connection.CreateCommand();

            string Name = "Paul Smith";
            string Email = "paulsmith@gmail.com";
            string Phone = "10203040";
            string Address = "Smith Lane 54";
            string City = "Dallas";
            string Password = "smith";

            string hashedPassword = PasswordHelper.HashPassword(Password);
            command.CommandText = "INSERT INTO Restaurants (Name, Email, Phone, Address, City, Password) " +
                       "VALUES (@Name, @Email, @Phone, @Address, @City, @Password);";

           
            command.Parameters.AddWithValue("@Name", Name);
            command.Parameters.AddWithValue("@Email", Email);
            command.Parameters.AddWithValue("@Phone", Phone);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@City", City);
            command.Parameters.AddWithValue("@Password", hashedPassword);
            command.ExecuteNonQuery();
        }

        // MenuItem dummy data
        using (var connection = new SqlConnection(ConnectionString + "Initial Catalog=MTOGO_TEST;"))
        {
            connection.Open();
            var command = connection.CreateCommand();

            string Name = "Hamburger";
            string Description = "Classic burger";
            int Price = 50;
            string Category = "Burger";
            int RestaurantId = 1;

            command.CommandText = "INSERT INTO MenuItems (Name, Description, Price, Category, FK_MenuItems_RestaurantID)" +
                "VALUES (@Name, @Description, @Price, @Category, @FK_MenuItems_RestaurantID);";


            command.Parameters.AddWithValue("@Name", Name);
            command.Parameters.AddWithValue("@Description", Description);
            command.Parameters.AddWithValue("@Price", Price);
            command.Parameters.AddWithValue("@Category", Category);
            command.Parameters.AddWithValue("@FK_MenuItems_RestaurantID", RestaurantId);
            command.ExecuteNonQuery();
        }

        // PlaceOrder dummy data
        DBActions DBActions = new DBActions();
        List<OrderLine> orderLines = new List<OrderLine>();
        int customerId = 1;
        OrderLine orderLine = new OrderLine
        {
            RestaurantId = 1,
            Quantity = 1,
            MenuItemId = 1
        };
        orderLines.Add(orderLine);
        int orderId = DBActions.PlaceOrder(1, orderLines, "MTOGO_TEST");

    }
}
