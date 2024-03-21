using Microsoft.AspNetCore.Mvc;
using PLM.Web.Models;
using PLM.Web.Services;
using System.Reflection;

namespace PLM.Web.Controllers
{
    public class HomeController(IParkingService service) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/checkin")]
        public async Task<IActionResult> CheckIn([FromBody] TagModel model)
        {
            return model == null || string.IsNullOrEmpty(model.TagNumber)
                ? BadRequest("Tag number is required.")
                : Ok(await service.RegisterCarArrival(model));
        }

        [HttpPost("/checkout")]
        public async Task<IActionResult> CheckOut([FromBody] TagModel model)
        {
            return model == null || string.IsNullOrEmpty(model.TagNumber)
                ? BadRequest("Tag number is required.")
                : Ok(await service.UpdateCarCheckOutTime(model));
        }

        [HttpGet("/parking-snapshot")]
        public async Task<IActionResult> GetParkingSnapshot()
        {
            var snapshots = await service.GetParkingSnapshot();
            return PartialView("_ParkingSnapshot", snapshots);
        }
    }
}