using Microsoft.AspNetCore.Mvc;
using MTOGO_Reviews_Complaints_System.Model;

namespace MTOGO_Reviews_Complaints_System.Controllers
{
    public class ReviewsComplaintsController : Controller
    {
        DBActions DBActions = new DBActions();

        [HttpPost("review/add")]
        public ActionResult InsertCustomerReview([FromBody] CustomerReview review)
        {
            try
            {
                DBActions.InsertCustomerReview(review);
                return Ok("Review was inserted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/complaint/add")]
        public ActionResult InsertRestaurantComplaint([FromBody] RestaurantComplaint complaint)
        {
            try
            {
                DBActions.InsertRestaurantComplaint(complaint);
                return Ok("Complaint was inserted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
