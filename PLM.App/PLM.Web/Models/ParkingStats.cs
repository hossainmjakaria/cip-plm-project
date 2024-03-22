namespace PLM.Web.Models;

public record ParkingStats
{
    public int SpotsAvailable { get; set; }
    public decimal TodayRevenue { get; set; }
    public decimal AvgCarsPerDayLast30Days { get; set; }
    public decimal AvgRevenuePerDayLast30Days { get; set; }
}
