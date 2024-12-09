using Microsoft.AspNetCore.Mvc;
using MTOGO_Customer_System.Model;
using MTOGO_Customer_System.Model.DTO;
using System.Text.Json;

namespace MTOGO_Customer_System.Controllers
{
    public class CustomerController : Controller
    {
        DBActions DBActions = new DBActions();

        [HttpPost("customer/add")]
        public ActionResult InsertCustomer([FromBody] Customer customer)
        {
            bool result = DBActions.InsertCustomer(customer);
            return Ok("Customer was added");
        }

        [HttpGet("customer/{customerId}")]
        public ActionResult GetCustomerById(int customerId)
        {
            Customer customer = DBActions.GetCustomerById(customerId);
            if (customer != null)
            {
                return Ok(customer);
            }
            return BadRequest();
        }

        [HttpPost("customer/login")]
        public ActionResult LoginCustomer([FromBody] Customer customer)
        {
            Customer loggedInCustomer = DBActions.LoginCustomer(customer.Email, customer.Password);
            if (customer != null)
            {
                return Ok(loggedInCustomer);
            }
            return BadRequest();
        }

        [HttpPut("customer/update")]
        public ActionResult UpdateCustomer([FromBody] Customer customer)
        {
            bool status = DBActions.UpdateCustomer(customer);
            if (status)
            {
                return Ok("Customer was updated");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("customer/placeorder/{customerId}")]
        public ActionResult PlaceOrder(int customerId, [FromBody] List<OrderLine> orderLines)
        {
            int orderId = DBActions.PlaceOrder(customerId, orderLines);
            DBActions.UpdateOrderPrice(orderId);
            return Ok(orderId);
        }

        [HttpGet("customer/orders/{customerId}")]
        public ActionResult<List<OrderDTO>> GetOrdersByCustomerId(int customerId)
        {
            List<OrderDTO> orders = DBActions.GetOrdersByCustomerId(customerId);
            return Ok(orders);
        }

        
        [HttpGet("customer/order/{orderId}")]
        public ActionResult<OrderDTO> GetOrderByOrderId(int orderId)
        {
            OrderDTO orderDTO = DBActions.GetOrderById(orderId);
            return Ok(orderDTO);
        }


    }
}
