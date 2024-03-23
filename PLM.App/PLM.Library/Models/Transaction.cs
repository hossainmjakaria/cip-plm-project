namespace PLM.Library.Models;
public record Transaction
{
    public string TagNumber { get; set; } = string.Empty;
    public DateTime CheckInTime { get; set; }
    public int ElapsedHours { get; set; }
}
