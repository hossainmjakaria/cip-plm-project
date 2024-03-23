using PLM.Library.Infrustuctures;
using PLM.Library.Models;

namespace PLM.Library.Services;

public interface IParkingService
{
    public Task<Response<bool>> RegisterCarArrival(TagModel model);
    public Task<Response<decimal>> UpdateCarCheckOutTime(TagModel model, int hourlyFee);
    public Task<IEnumerable<Transaction>> GetParkingSnapshot();
    public Task<ParkingStats> GetParkingStatistics();
}

public class ParkingService(IParkingRepository repository) : IParkingService
{
    public async Task<Response<bool>> RegisterCarArrival(TagModel model) => await repository.RegisterCarArrival(model.TagNumber);

    public async Task<Response<decimal>> UpdateCarCheckOutTime(TagModel model, int hourlyFee) => await repository.UpdateCarCheckOutTime(model.TagNumber, hourlyFee);

    public async Task<IEnumerable<Transaction>> GetParkingSnapshot() => await repository.GetParkingSnapshot();

    public async Task<ParkingStats> GetParkingStatistics() => await repository.GetParkingStatistics();
}