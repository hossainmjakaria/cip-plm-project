using PLM.Library.Models;

namespace PLM.Web.Models;


public record SnapshotViewModel
{
    public IEnumerable<Transaction> Transactions { get; set; } = new List<Transaction>();
    public int HourlyFee { get; set; }
    public int TotalSpots { get; set; }
    public int AvailableSpots
    {
        get { return Transactions.Any() ? TotalSpots - Transactions.Count() : TotalSpots; }
    }
    public int SpotsTaken
    {
        get { return Transactions.Count(); }
    }
}