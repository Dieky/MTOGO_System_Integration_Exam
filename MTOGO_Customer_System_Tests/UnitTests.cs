using Microsoft.AspNetCore.Mvc;
using Moq;
using MTOGO_Customer_System.Controllers;
using MTOGO_Customer_System.Model.Interfaces;
using MTOGO_Customer_System.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Resource;
using MTOGO_Customer_System.Model.DTO;
using System.Numerics;

namespace MTOGO_Customer_System_Tests
{

    public class UnitTests
    {

        [Fact]
        public void InsertCustomer_ReturnsOk_WhenCustomerIsInserted()
        {
            // Arrange
            var mockDbActions = new Mock<ICustomerDBActions>();
            var customer = new Customer
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Phone = "123456789",
                Address = "123 Main St",
                City = "Anytown",
                Password = "SecurePassword"
            };

            mockDbActions.Setup(db => db.InsertCustomer(It.IsAny<Customer>(), "MTOGO")).Returns(true);
            var controller = new CustomerController(mockDbActions.Object);

            // Act
            var result = controller.InsertCustomer(customer);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Customer was added", actionResult.Value);
            mockDbActions.Verify(db => db.InsertCustomer(It.IsAny<Customer>(), "MTOGO"), Times.Once);
        }

        [Fact]
        public void InsertCustomer_ReturnsBadRequest_WhenInsertFails()
        {
            // Arrange
            var mockDbActions = new Mock<ICustomerDBActions>();
            var customer = new Customer
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Phone = "123456789",
                Address = "123 Main St",
                City = "Anytown",
                Password = "SecurePassword"
            };

            mockDbActions.Setup(db => db.InsertCustomer(It.IsAny<Customer>(), "MTOGO")).Returns(false);
            var controller = new CustomerController(mockDbActions.Object);

            // Act
            var result = controller.InsertCustomer(customer);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Customer insertion failed", actionResult.Value);
            mockDbActions.Verify(db => db.InsertCustomer(It.IsAny<Customer>(), "MTOGO"), Times.Once);
        }

        [Fact]
        public void GetCustomerById_ReturnsOk_WhenCustomerExists()
        {
            // Arrange
            var mockDbActions = new Mock<ICustomerDBActions>();
            int customerId = 1;

            var customer = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Phone = "123456789",
                Address = "123 Main St",
                City = "Anytown",
                Password = "SecurePassword"
            };

            mockDbActions.Setup(db => db.GetCustomerById(customerId, "MTOGO")).Returns(customer);
            var controller = new CustomerController(mockDbActions.Object);

            // Act
            var result = controller.GetCustomerById(customerId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result); 
            var returnValue = Assert.IsType<Customer>(actionResult.Value);
            Assert.Equal(customerId, returnValue.Id); 
            Assert.Equal("John Doe", returnValue.Name);
            mockDbActions.Verify(db => db.GetCustomerById(customerId, "MTOGO"), Times.Once);
        }

        [Fact]
        public void GetCustomerById_ReturnsBadRequest_WhenCustomerDoesNotExist()
        {
            // Arrange
            var mockDbActions = new Mock<ICustomerDBActions>();
            int customerId = 1; 

            mockDbActions.Setup(db => db.GetCustomerById(customerId, "MTOGO")).Returns((Customer)null);
            var controller = new CustomerController(mockDbActions.Object);

            // Act
            var result = controller.GetCustomerById(customerId);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Found no customer matching the id", actionResult.Value);
            mockDbActions.Verify(db => db.GetCustomerById(customerId, "MTOGO"), Times.Once);
        }

        [Fact]
        public void AuthenticateCustomer_ReturnsTrue_WhenCredentialsAreValid()
        {
            // Arrange
            var mockDbActions = new Mock<ICustomerDBActions>();

            var customer = new Customer
            {
                Id = 1,
                Email = "john.doe@example.com",
                Password = "SecurePassword"
            };

            mockDbActions.Setup(db => db.AuthenticateCustomer(customer.Email, customer.Password, "MTOGO")).Returns(true);

            // Act
            var isAuthenticated = mockDbActions.Object.AuthenticateCustomer(customer.Email, customer.Password);

            // Assert
            Assert.True(isAuthenticated);
            mockDbActions.Verify(db => db.AuthenticateCustomer(customer.Email, customer.Password, "MTOGO"), Times.Once);
        }


        [Fact]
        public void AuthenticateCustomer_ReturnsFalse_WhenCredentialsAreInvalid()
        {
            // Arrange
            var mockDbActions = new Mock<ICustomerDBActions>();

            var customer = new Customer
            {
                Id = 1,
                Email = "john.doe@example.com",
                Password = "SecurePassword" 
            };

            mockDbActions.Setup(db => db.AuthenticateCustomer(customer.Email, "WrongPassword", "MTOGO")).Returns(false);

            // Act
            var isAuthenticated = mockDbActions.Object.AuthenticateCustomer(customer.Email, "WrongPassword");

            // Assert
            Assert.False(isAuthenticated); 
            mockDbActions.Verify(db => db.AuthenticateCustomer(customer.Email, "WrongPassword", "MTOGO"), Times.Once);
        }

        [Fact]
        public void LoginCustomer_ReturnsCustomer_WhenCredentialsAreValid()
        {
            // Arrange
            var mockDbActions = new Mock<ICustomerDBActions>();

            var customer = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Phone = "123456789",
                Address = "123 Main St",
                City = "Anytown",
                Password = "SecurePassword"
            };

            mockDbActions.Setup(db => db.LoginCustomer(customer.Email, customer.Password, "MTOGO")).Returns(customer);
            var controller = new CustomerController(mockDbActions.Object);

            // Act
            var result = controller.LoginCustomer(customer);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Customer>(actionResult.Value); 
            Assert.Equal(customer.Id, returnValue.Id); 
            Assert.Equal(customer.Name, returnValue.Name); 
            mockDbActions.Verify(db => db.LoginCustomer(customer.Email, customer.Password, "MTOGO"), Times.Once);
        }

        [Fact]
       public void UpdateCustomer_ReturnsTrue_WhenCustomerUpdated()
        {
            // Arrange
            var mockDbActions = new Mock<ICustomerDBActions>();

            var customer = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Phone = "123456789",
                Address = "123 Main St",
                City = "Anytown",
                Password = "SecurePassword"
            };

            mockDbActions.Setup(db => db.UpdateCustomer(customer, "MTOGO")).Returns(true);
            var controller = new CustomerController(mockDbActions.Object);

            // Act
            var result = controller.UpdateCustomer(customer);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<String>(actionResult.Value);

            Assert.Equal("Customer was updated", returnValue);
            mockDbActions.Verify(db => db.UpdateCustomer(customer, "MTOGO"), Times.Once);
        }

        [Fact]
        public void UpdateCustomer_ReturnsFalse_WhenCustomerUpdated()
        {
            // Arrange
            var mockDbActions = new Mock<ICustomerDBActions>();

            var customer = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Phone = "123456789",
                Address = "123 Main St",
                City = "Anytown",
                Password = "SecurePassword"
            };

            mockDbActions.Setup(db => db.UpdateCustomer(customer, "MTOGO")).Returns(false);
            var controller = new CustomerController(mockDbActions.Object);

            // Act
            var result = controller.UpdateCustomer(customer);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = Assert.IsType<String>(actionResult.Value);
            Assert.Equal("Customer was not updated", returnValue);
            mockDbActions.Verify(db => db.UpdateCustomer(customer, "MTOGO"), Times.Once);
        }

        [Fact]
        public void PlaceOrder_ReturnsOk_WhenPlaceorderIsValid()
        {
            // Arrange
            var mockDbActions = new Mock<ICustomerDBActions>();
            int customerId = 1;
            List<OrderLine> orderLines = new List<OrderLine>();
            OrderLine orderLine = new OrderLine
            {
                Quantity = 1,
                MenuItemId = 5,
                RestaurantId = 4,
            };
            orderLines.Add(orderLine);

            mockDbActions.Setup(db => db.PlaceOrder(customerId,orderLines, "MTOGO")).Returns(1);
            var controller = new CustomerController(mockDbActions.Object);

            // Act
            var result = controller.PlaceOrder(customerId, orderLines);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<int>(actionResult.Value);
            Assert.Equal(1, returnValue);
            mockDbActions.Verify(db => db.PlaceOrder(customerId, orderLines, "MTOGO"), Times.Once);

        }

        [Fact]
        public void PlaceOrder_ReturnsBadRequest_WhenPlaceorderIsInvalid()
        {
            // Arrange
            var mockDbActions = new Mock<ICustomerDBActions>();
            int customerId = 1;
            List<OrderLine> orderLines = new List<OrderLine>();
          
            mockDbActions.Setup(db => db.PlaceOrder(customerId, orderLines, "MTOGO")).Returns(0);
            var controller = new CustomerController(mockDbActions.Object);

            // Act
            var result = controller.PlaceOrder(customerId, orderLines);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = Assert.IsType<string>(actionResult.Value);
            Assert.Equal("Order was not placed", returnValue);
            mockDbActions.Verify(db => db.PlaceOrder(customerId, orderLines, "MTOGO"), Times.Once);

        }

        [Fact]
        public void GetOrdersByCustomerId_ReturnsOk_WhenCustomerIdIsValid()
        {
            // Arrange
            var mockDbActions = new Mock<ICustomerDBActions>();
            int customerId = 1;
            List<OrderDTO> orders = new List<OrderDTO>();

            mockDbActions.Setup(db => db.GetOrdersByCustomerId(customerId, "MTOGO")).Returns(orders);
            var controller = new CustomerController(mockDbActions.Object);

            // Act
            var result = controller.GetOrdersByCustomerId(customerId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<OrderDTO>>(actionResult.Value);
            Assert.Equal(orders, returnValue);
            mockDbActions.Verify(db => db.GetOrdersByCustomerId(customerId, "MTOGO"), Times.Once);
        }

        [Fact]
        public void GetOrderByOrderId_ReturnsOk_WhenOrderIdIsValid()
        {
            // Arrange
            var mockDbActions = new Mock<ICustomerDBActions>();
            int orderId = 1;
            OrderDTO order = new OrderDTO();

            mockDbActions.Setup(db => db.GetOrderById(orderId, "MTOGO")).Returns(order);
            var controller = new CustomerController(mockDbActions.Object);

            // Act
            var result = controller.GetOrderByOrderId(orderId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<OrderDTO>(actionResult.Value);
            Assert.Equal(order, returnValue);
            mockDbActions.Verify(db => db.GetOrderById(orderId, "MTOGO"), Times.Once);
        }

        [Fact]
        public void GetOrderByOrderId_ReturnsBadRequest_WhenOrderIdIsInvalid()
        {
            // Arrange
            var mockDbActions = new Mock<ICustomerDBActions>();
            int orderId = 1;
            OrderDTO order = new OrderDTO();

            mockDbActions.Setup(db => db.GetOrderById(orderId, "MTOGO")).Returns((OrderDTO)null);
            var controller = new CustomerController(mockDbActions.Object);

            // Act
            var result = controller.GetOrderByOrderId(orderId);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = Assert.IsType<string>(actionResult.Value);
            Assert.Equal("No order was found found", returnValue);
            mockDbActions.Verify(db => db.GetOrderById(orderId, "MTOGO"), Times.Once);
        }


    }
}
