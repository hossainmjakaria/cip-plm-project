using Microsoft.AspNetCore.Mvc;
using PLM.Library.Models;
using PLM.Library.Services;
using PLM.Library.Utility;
using PLM.Web.Models;

namespace PLM.Web.Controllers
{
    public class HomeController(IParkingService service, AppSettings settings) : Controller
    {
        public IActionResult Index() => View();

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
                : Ok(await service.UpdateCarCheckOutTime(model, settings.HourlyFee));
        }

        [HttpGet("/parking-snapshot")]
        public async Task<IActionResult> GetParkingSnapshot()
        {
            var model = new SnapshotViewModel
            {
                Transactions = await service.GetParkingSnapshot(),
                TotalSpots = settings.TotalSpots,
                HourlyFee = settings.HourlyFee
            };

            return PartialView("_ParkingSnapshot", model);
        }

        [HttpGet("/parking-statistics")]
        public async Task<IActionResult> GetParkingStatistics()
        {
            return PartialView("_ParkingStatistics", await service.GetParkingStatistics());
        }
    }
}