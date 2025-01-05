using MTOGO_Customer_System.Model;
using MTOGO_Customer_System.Model.DTO;

namespace MTOGO_Customer_System_Tests
{
    public class IntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly DBActions dbActions;
        public IntegrationTests(DatabaseFixture fixture)
        {
            // Fixture will automatically call RecreateDatabase once
            dbActions = new DBActions();
        }

        [Fact]
        public void InsertCustomer_DuplicateEmail_ReturnsFalse()
        {
            // Arrange
            Customer customer = new Customer
            {
                Name = "Paul Smith",
                Email = "paulsmith@gmail.com", // Email already in use
                Phone = "10203040",
                Address = "Smith Lane 54",
                City = "Dallas",
                Password = "smith"
            };

            // Act
            bool result = dbActions.InsertCustomer(customer, "MTOGO_TEST");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void InsertCustomer_Success_ReturnsTrue()
        {
            // Arrange
            Customer customer = new Customer
            {
                Name = "Kage Kaj",
                Email = "kagekaj@gmail.com",
                Phone = "12345678",
                Address = "Chokolade Vej 15",
                City = "Marabou",
                Password = "kagekaj"
            };

            // Act
            bool result = dbActions.InsertCustomer(customer, "MTOGO_TEST");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void InsertCustomer_MissingOrNullPassowrd_ReturnsFalse()
        {
            // Arrange
            Customer customer = new Customer
            {
                Name = "Kage Kaj",
                Email = "kagekaj2@gmail.com",
                Phone = "12345678",
                Address = "Chokolade Vej 15",
                City = "Marabou",
                Password = null
            };

            // Act
            bool result = dbActions.InsertCustomer(customer, "MTOGO_TEST");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetCustomerById_Success_ReturnsCustomer()
        {
            // Arrange
            int id = 1;

            // Act
            Customer customer = dbActions.GetCustomerById(id, "MTOGO_TEST");

            // Assert
            Assert.Equal(id, customer.Id);
        }

        [Fact]
        public void GetCustomerById_NoMatchingId_ReturnsEmptyCustomer()
        {
            // Arrange
            int id = 100; // set a high ID to avoid colliding with other tests

            // Act
            Customer customer = dbActions.GetCustomerById(id);

            // Assert
            Assert.NotEqual(id, customer.Id);
        }

        [Fact]
        public void AuthenticateCustomer_MatchingPassword_ReturnsTrue()
        {

            // Arrange
            string email = "paulsmith@gmail.com";
            string password = "smith";

            // Act
            bool result = dbActions.AuthenticateCustomer(email, password, "MTOGO_TEST");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AuthenticateCustomer_WrongPassword_ReturnsFalse()
        {
            // Arrange
            string email = "paulsmith@gmail.com";
            string password = "wrongpassword";

            // Act
            bool result = dbActions.AuthenticateCustomer(email, password, "MTOGO_TEST");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void LoginCustomer_CorrectCredentials_ReturnsCustomer()
        {
            // Arrange
            Customer customer = new Customer
            {
                Name = "Paul Smith",
                Email = "paulsmith@gmail.com",
                Phone = "10203040",
                Address = "Smith Lane 54",
                City = "Dallas",
                Password = "smith"
            };

            // Act
            Customer loggedInCustomer = dbActions.LoginCustomer(customer.Email, customer.Password, "MTOGO_TEST");

            // Assert
            Assert.Equal(customer.Name,loggedInCustomer.Name);
            Assert.Equal(customer.Address,loggedInCustomer.Address);
        }

        [Fact]
        public void LoginCustomer_IncorrectCredentials_ThrowsException()
        {
            // Arrange
            Customer customer = new Customer
            {
                Email = "paulsmith@gmail.com",
                Password = "wrongpassword"
            };

            // Act
            var exception = Assert.Throws<Exception>(() => dbActions.LoginCustomer(customer.Email, customer.Password, "MTOGO_TEST"));

            // Assert
            Assert.Equal("Incorrect password", exception.Message);
           
        }

        [Fact]
        public void UpdateCustomer_Success_ReturnsTrue()
        {
            // Arrange
            Customer customer = new Customer
            {
                Id = 1,
                Name = "Paul Smith",
                Email = "paulsmith@gmail.com",
                Phone = "10203040",
                Address = "Smith Lane 54",
                City = "New York",
                Password = "smith"
            };

            // Act
            bool result = dbActions.UpdateCustomer(customer, "MTOGO_TEST");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UpdateCustomer_InvalidId_ReturnsFalse()
        {
            // Arrange
            Customer customer = new Customer
            {
                Id = 100,
                Name = "Paul Smith",
                Email = "paulsmith@gmail.com",
                Phone = "10203040",
                Address = "Smith Lane 54",
                City = "New York",
                Password = "smith"
            };

            // Act
            bool result = dbActions.UpdateCustomer(customer, "MTOGO_TEST");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void PlaceOrder_Success_ReturnsOrderId()
        {
            // Arrange
            List<OrderLine> orderLines = new List<OrderLine>();
            int customerId = 1;
            OrderLine orderLine = new OrderLine
            {
                RestaurantId = 1,
                Quantity = 1,
                MenuItemId = 1
            };
            orderLines.Add(orderLine);

            // Act
            int orderId = dbActions.PlaceOrder(customerId, orderLines, "MTOGO_TEST");

            // Assert
            Assert.NotEqual(0,orderId);
        }

        [Fact]
        public void PlaceOrder_InvalidRestaurantId_Returns0()
        {
            List<OrderLine> orderLines = new List<OrderLine>();
            int customerId = 1;
            OrderLine orderLine = new OrderLine
            {
                RestaurantId = 5,
                Quantity = 1,
                MenuItemId = 1
            };
            orderLines.Add(orderLine);

            // Act
            int orderId = dbActions.PlaceOrder(customerId, orderLines, "MTOGO_TEST");

            // Assert
            Assert.Equal(0, orderId);
        }

        [Fact]
        public void UpdateOrderPrice_Success()
        {
            // Arrange
            int orderId = 1;

            // Act
            dbActions.UpdateOrderPrice(orderId, "MTOGO_TEST");
            OrderDTO order = dbActions.GetOrderById(orderId, "MTOGO_TEST");

            // Assert
            Assert.NotEqual(0, order.TotalPrice);
        }

        [Fact]
        public void GetOrdersByCustomerId_ValidCustomerId_ReturnsOrders()
        {
            // Arrange
            int customerId = 1;

            // Act
            List<OrderDTO> orders = dbActions.GetOrdersByCustomerId(customerId, "MTOGO_TEST");

            // Assert
            Assert.True(orders.Count() > 0);
        }

        [Fact]
        public void GetOrdersByCustomerId_InvalidCustomerId_ReturnsEmptyOrdersList()
        {
            // Arrange
            int customerId = 100;

            // Act
            List<OrderDTO> orders = dbActions.GetOrdersByCustomerId(customerId, "MTOGO_TEST");

            // Assert
            Assert.True(orders.Count() == 0);
        }

        // lav de sidste tests og tilføj en restaurant samt menuitems i recreateDB


    }
}