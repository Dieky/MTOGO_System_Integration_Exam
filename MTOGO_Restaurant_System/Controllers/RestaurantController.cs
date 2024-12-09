using Microsoft.AspNetCore.Mvc;
using MTOGO_Restaurant_System.Model;

namespace MTOGO_Restaurant_System.Controllers
{
    public class RestaurantController : Controller
    {
        DBActions DBActions = new DBActions();

        [HttpPost("restaurant/add")]
        public ActionResult InsertRestaurant([FromBody] Restaurant restaurant)
        {
            bool result = DBActions.InsertRestaurant(restaurant);
            return Ok();
        }

        [HttpGet("restaurant/{restaurantId}")]
        public ActionResult GetRestaurantById(int restaurantId)
        {
            Restaurant restaurant = DBActions.GetRestaurantById(restaurantId);
            if (restaurant != null)
            {
                return Ok(restaurant);
            }
            return BadRequest();
        }

        [HttpPost("restaurant/login")]
        public ActionResult LoginRestaurant([FromBody] LoginRequest login)
        {
            try
            {
                Restaurant restaurant = DBActions.LoginRestaurant(login.Email, login.Password);
                return Ok(restaurant);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut("restaurant/update")]
        public ActionResult UpdateRestaurant([FromBody] Restaurant restaurant)
        {
            bool status = DBActions.UpdateRestaurant(restaurant);
            if (status)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("restaurant/addmenu")]
        public ActionResult AddMenuItem([FromBody] MenuItem menuItem)
        {
            DBActions.AddMenuItem(menuItem);
            return Ok();
        }

        [HttpPut("restaurant/updatemenu")]
        public ActionResult UpdateMenuItem([FromBody] MenuItem menuItem)
        {
            DBActions.UpdateMenuItem(menuItem);
            return Ok();
        }

        [HttpGet("restaurant/orders/{restaurantId}")]
        public ActionResult<List<Order>> GetOrdersLast24Hours(int restaurantId)
        {
            List<Order> orders = DBActions.GetOrdersLast24Hours(restaurantId);
            return Ok(orders);
        }

        [HttpGet("restaurant/accept/{orderId}")]
        public ActionResult AcceptOrder(int orderId)
        {
            DBActions.AcceptOrder(orderId);
            return Ok();
        }

        [HttpGet("restaurant/pickup/{orderId}")]
        public ActionResult PickupOrder(int orderId)
        {
            DBActions.PickupOrder(orderId);
            return Ok();
        }

        [HttpGet("restaurant/delivered/{orderId}")]
        public ActionResult OrderDelivered(int orderId)
        {
            DBActions.OrderDelivered(orderId);
            return Ok();
        }

    }
}
