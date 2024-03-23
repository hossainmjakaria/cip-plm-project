using PLM.Web.Infrustuctures;
using PLM.Web.Models;

namespace PLM.Web.Services;

public interface IParkingService
{
    public Task<Response<bool>> RegisterCarArrival(TagModel model);
    public Task<Response<decimal>> UpdateCarCheckOutTime(TagModel model);
    public Task<SnapshotViewModel> GetParkingSnapshot();
    public Task<ParkingStats> GetParkingStatistics();
}

public class ParkingService(IParkingRepository repository, AppSettings appSettings) : IParkingService
{
    public async Task<Response<bool>> RegisterCarArrival(TagModel model) => await repository.RegisterCarArrival(model.TagNumber);

    public async Task<Response<decimal>> UpdateCarCheckOutTime(TagModel model) => await repository.UpdateCarCheckOutTime(model.TagNumber, appSettings.HourlyFee);

    public async Task<SnapshotViewModel> GetParkingSnapshot() =>
        new SnapshotViewModel
        {
            Transactions = await repository.GetParkingSnapshot(),
            TotalSpots = appSettings.TotalSpots,
            HourlyFee = appSettings.HourlyFee
        };

    public async Task<ParkingStats> GetParkingStatistics() => await repository.GetParkingStatistics();
}