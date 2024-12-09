using Microsoft.AspNetCore.Mvc;
using MTOGO_Courier_System.Model;

namespace MTOGO_Courier_System.Controllers
{
    public class CourierController : Controller
    {
        DBActions DBActions = new DBActions();

        [HttpPost("courier/add")]
        public ActionResult InsertCourier([FromBody] Courier courier)
        {
            try
            {
                DBActions.InsertCourier(courier);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("courier/assign")]
        public ActionResult AssignCourier([FromBody] AssignCourierRequest assignCourierRequest)
        {
            try
            {
                DBActions.AssignCourier(assignCourierRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
