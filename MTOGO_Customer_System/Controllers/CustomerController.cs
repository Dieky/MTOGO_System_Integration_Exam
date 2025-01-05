using Microsoft.AspNetCore.Mvc;
using MTOGO_Customer_System.Model;
using MTOGO_Customer_System.Model.DTO;
using MTOGO_Customer_System.Model.Interfaces;

namespace MTOGO_Customer_System.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerDBActions _dbActions;

        // Inject ICustomerDBActions via the constructor
        public CustomerController(ICustomerDBActions dbActions)
        {
            _dbActions = dbActions;
        }

        [HttpPost("customer/add")]
        public ActionResult InsertCustomer([FromBody] Customer customer)
        {
            bool result = _dbActions.InsertCustomer(customer);
            if (result)
            {
                return Ok("Customer was added");
            }
            else
            {
                return BadRequest("Customer insertion failed");
            }
        }

        [HttpGet("customer/{customerId}")]
        public ActionResult GetCustomerById(int customerId)
        {
            Customer customer = _dbActions.GetCustomerById(customerId);
            if (customer != null)
            {
                return Ok(customer);
            }
            return BadRequest("Found no customer matching the id");
        }

        [HttpPost("customer/login")]
        public ActionResult LoginCustomer([FromBody] Customer customer)
        {
            Customer loggedInCustomer = _dbActions.LoginCustomer(customer.Email, customer.Password);
            if (customer != null)
            {
                return Ok(loggedInCustomer);
            }
            return BadRequest("Wrong email or password");
        }

        [HttpPut("customer/update")]
        public ActionResult UpdateCustomer([FromBody] Customer customer)
        {
            bool status = _dbActions.UpdateCustomer(customer);
            if (status)
            {
                return Ok("Customer was updated");
            }
            else
            {
                return BadRequest("Customer was not updated");
            }
        }

        [HttpPost("customer/placeorder/{customerId}")]
        public ActionResult PlaceOrder(int customerId, [FromBody] List<OrderLine> orderLines)
        {
            int orderId = _dbActions.PlaceOrder(customerId, orderLines);
            if (orderId != 0)
            {
                _dbActions.UpdateOrderPrice(orderId);
                return Ok(orderId);
            }
            else
            {
                return BadRequest("Order was not placed");
            }
        }

        [HttpGet("customer/orders/{customerId}")]
        public ActionResult GetOrdersByCustomerId(int customerId)
        {
            List<OrderDTO> orders = _dbActions.GetOrdersByCustomerId(customerId);
            return Ok(orders);
        }


        [HttpGet("customer/order/{orderId}")]
        public ActionResult GetOrderByOrderId(int orderId)
        {
            OrderDTO orderDTO = _dbActions.GetOrderById(orderId);
            if (orderDTO != null)
            {
                return Ok(orderDTO);
            }
            else
            {
                return BadRequest("No order was found found");
            }
        }


    }
}
