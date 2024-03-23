namespace PLM.Web.Models;

public static class Constants
{
    #region StoredProcedures
    public const string SpEnsureParkingSpot = "sp_EnsureParkingSpot";
    public const string SpRegisterCarArrival = "sp_RegisterCarArrival";
    public const string SpUpdateCarCheckOutTime = "sp_UpdateCarCheckOutTime";
    public const string SpGetSnapshot = "sp_GetSnapshot";
    public const string SpGetParkingStats = "sp_GetParkingStats";
    #endregion

    #region Messages
    public const string CarCheckedInSuccessfully = "Checked in successfully.";
    public const string CarCheckedOutSuccessfully = "Checked out successfully.";
    #endregion

}
