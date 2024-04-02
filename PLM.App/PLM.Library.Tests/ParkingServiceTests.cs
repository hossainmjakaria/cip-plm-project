using Moq;
using PLM.Library.Infrustuctures;
using PLM.Library.Models;
using PLM.Library.Services;

namespace PLM.Library.Tests;

public class ParkingServiceTests
{
    [Fact]
    public async Task RegisterCarArrival_ReturnsTrue_OnSuccess()
    {
        // Arrange
        var repositoryMock = new Mock<IParkingRepository>();
        repositoryMock.Setup(repo => repo.RegisterCarArrival(It.IsAny<string>())).ReturnsAsync(new Response<bool> { IsSuccess = true });

        var parkingService = new ParkingService(repositoryMock.Object);
        var tagModel = new TagModel { TagNumber = "ABC123" };

        // Act
        var result = await parkingService.RegisterCarArrival(tagModel);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("ABC123")]
    [InlineData("ABC124")]
    public async Task UpdateCarCheckOutTime_ReturnsAmount_OnSuccess(string tagNumber)
    {
        // Arrange
        var repositoryMock = new Mock<IParkingRepository>();
        repositoryMock.Setup(repo => repo.UpdateCarCheckOutTime(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new Response<decimal> { IsSuccess = true, Model = 50 });

        var parkingService = new ParkingService(repositoryMock.Object);
        var tagModel = new TagModel { TagNumber = tagNumber };
        int hourlyFee = 10;

        // Act
        var result = await parkingService.UpdateCarCheckOutTime(tagModel, hourlyFee);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(50, result.Model);
    }

    [Fact]
    public async Task GetParkingSnapshot_ReturnsTransactions()
    {
        // Arrange
        var today = DateTime.Today;
        var transactions = new List<Transaction> { new Transaction { TagNumber = "ABC123", CheckInTime = today, ElapsedHours = 5 } };
        var repositoryMock = new Mock<IParkingRepository>();
        repositoryMock.Setup(repo => repo.GetParkingSnapshot()).ReturnsAsync(transactions);

        var parkingService = new ParkingService(repositoryMock.Object);

        // Act
        var result = await parkingService.GetParkingSnapshot();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(5, result.First().ElapsedHours);
        Assert.Equal(today, result.First().CheckInTime);
        Assert.Equal("ABC123", result.First().TagNumber);
    }

    [Fact]
    public async Task GetParkingStatistics_ReturnsParkingStats()
    {
        // Arrange
        var parkingStats = new ParkingStats { SpotsAvailable = 10, TodayRevenue = 100, AvgCarsPerDayLast30Days = 50, AvgRevenuePerDayLast30Days = 500 };
        var repositoryMock = new Mock<IParkingRepository>();
        repositoryMock.Setup(repo => repo.GetParkingStatistics()).ReturnsAsync(parkingStats);

        var parkingService = new ParkingService(repositoryMock.Object);

        // Act
        var result = await parkingService.GetParkingStatistics();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.SpotsAvailable);
        Assert.Equal(100, result.TodayRevenue);
        Assert.Equal(50, result.AvgCarsPerDayLast30Days);
        Assert.Equal(500, result.AvgRevenuePerDayLast30Days);
    }
}