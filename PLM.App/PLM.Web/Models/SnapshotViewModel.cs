
using PLM.Web.Models;


public record SnapshotViewModel
{
    public IEnumerable<Transaction> Transactions { get; set; } = new List<Transaction>();
    public AppSettings? AppSettings { get; set; }
}