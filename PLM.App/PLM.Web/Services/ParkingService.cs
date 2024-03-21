using PLM.Web.Infrustuctures;
using PLM.Web.Models;

namespace PLM.Web.Services;

public interface IParkingService
{
    public Task<Response<Transaction>> RegisterCarArrival(TagModel model);
    public Task<Response<Transaction>> UpdateCarCheckOutTime(TagModel model);
    public Task<SnapshotViewModel> GetParkingSnapshot();
}

public class ParkingService(IParkingRepository repository, AppSettings appSettings) : IParkingService
{
    public async Task<Response<Transaction>> RegisterCarArrival(TagModel model) => await repository.RegisterCarArrival(model.TagNumber);

    public async Task<Response<Transaction>> UpdateCarCheckOutTime(TagModel model) => await repository.UpdateCarCheckOutTime(model.TagNumber, appSettings.HourlyFee);

    public async Task<SnapshotViewModel> GetParkingSnapshot() =>
        new SnapshotViewModel
        {
            Transactions = await repository.GetParkingSnapshot(),
            AppSettings = appSettings
        };
}