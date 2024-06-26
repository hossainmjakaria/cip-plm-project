CREATE DATABASE [ParkingLotManagementDB];
 GO
USE [ParkingLotManagementDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_getElapsedTime] 
(
	@StartTime datetime,
	@EndTime datetime
)

RETURNS int
AS
BEGIN
	DECLARE @DurationInMinutes int;

    SET @DurationInMinutes = DATEDIFF(MINUTE, @StartTime, @EndTime);
    SET @DurationInMinutes = CAST(CEILING(@DurationInMinutes / 60 + 0.000000001) AS int);

	RETURN @DurationInMinutes

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParkingSpots](
	[SpotID] [int] NOT NULL,
	[IsOccupied] [bit] NOT NULL,
 CONSTRAINT [PK__ParkingS__61645FE74CDC68D9] PRIMARY KEY CLUSTERED 
(
	[SpotID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transactions](
	[TransactionID] [int] IDENTITY(1,1) NOT NULL,
	[SpotID] [int] NOT NULL,
	[TagNumber] [nvarchar](50) NOT NULL,
	[CheckInTime] [datetime] NOT NULL,
	[CheckOutTime] [datetime] NULL,
	[TotalAmount] [decimal](10, 2) NULL,
 CONSTRAINT [PK__Transact__55433A4B444374D5] PRIMARY KEY CLUSTERED 
(
	[TransactionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ParkingSpots] ADD  CONSTRAINT [DF__ParkingSp__IsOcc__108B795B]  DEFAULT ((0)) FOR [IsOccupied]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_Transactions_ParkingSpots] FOREIGN KEY([SpotID])
REFERENCES [dbo].[ParkingSpots] ([SpotID])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_Transactions_ParkingSpots]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_EnsureParkingSpot]
    @TotalSpots INT = 5
AS
BEGIN
    DECLARE @CurrentTotalSpot INT;

    SELECT @CurrentTotalSpot = COUNT(*) FROM ParkingSpots;

    IF @CurrentTotalSpot >= @TotalSpots
    BEGIN
        SELECT @TotalSpots AS TotalSpots; 
		RETURN;
    END

    DECLARE @i INT = @CurrentTotalSpot + 1;

    WHILE @i <= @TotalSpots
    BEGIN
        INSERT INTO ParkingSpots (SpotID, IsOccupied) VALUES (@i, 0);
        SET @i = @i + 1;
    END

	SELECT @TotalSpots AS TotalSpots; 
	RETURN;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetParkingStats]
AS
BEGIN
    DECLARE @SpotsAvailable INT;
    SELECT @SpotsAvailable = (SELECT COUNT(*) FROM ParkingSpots WHERE IsOccupied = 0);

    DECLARE @TodayRevenue DECIMAL(10, 2);
    SELECT @TodayRevenue = ISNULL(SUM(TotalAmount), 0) FROM Transactions WHERE CONVERT(DATE, CheckOutTime) = CONVERT(DATE, GETDATE());

    DECLARE @AvgCarsPerDayLast30Days DECIMAL(10, 2);
    SELECT @AvgCarsPerDayLast30Days = ISNULL(AVG(CarsPerDay), 0)
    FROM (
        SELECT COUNT(*) AS CarsPerDay
        FROM Transactions
        WHERE CheckInTime >= DATEADD(DAY, -30, GETDATE())
        GROUP BY CONVERT(DATE, CheckInTime)
    ) AS AvgCarsPerDay;

    DECLARE @AvgRevenuePerDayLast30Days DECIMAL(10, 2);
    SELECT @AvgRevenuePerDayLast30Days = ISNULL(AVG(RevenuePerDay), 0)
    FROM (
        SELECT SUM(TotalAmount) AS RevenuePerDay
        FROM Transactions
        WHERE CheckInTime >= DATEADD(DAY, -30, GETDATE())
        GROUP BY CONVERT(DATE, CheckInTime)
    ) AS AvgRevenuePerDay;

    SELECT 
        @SpotsAvailable AS SpotsAvailable,
        @TodayRevenue AS TodayRevenue,
        @AvgCarsPerDayLast30Days AS AvgCarsPerDayLast30Days,
        @AvgRevenuePerDayLast30Days AS AvgRevenuePerDayLast30Days;
END


--EXEC sp_GetParkingStats
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetSnapshot]
AS
BEGIN
    SET NOCOUNT ON;

	SELECT 
		TagNumber, 
		CheckInTime, 
		dbo.fn_getElapsedTime(CheckInTime, GETDATE()) AS ElapsedHours
	FROM 
		Transactions
	WHERE CheckOutTime IS NULL;


	RETURN;
END

--EXEC sp_GetSnapshot
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_RegisterCarArrival]
    @TagNumber NVARCHAR(50),
	@RowsAffected INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;

	-- Any spots available?
	IF NOT EXISTS (SELECT 1 FROM ParkingSpots WHERE IsOccupied = 0)
    BEGIN
        RAISERROR('Parking slot is not avaiable.', 16, 1);
		RETURN;
    END

	DECLARE @SpotID INT = 0;
	SELECT TOP 1 
		@SpotID = ps.SpotID
	FROM 
		ParkingSpots ps
	WHERE ps.IsOccupied = 0;

	--Is the car already in the parking lot?
	DECLARE @AlreadyCheckedIn INT = 0;

	SELECT 
		@AlreadyCheckedIn = COUNT(*)
	FROM 
		Transactions t
	WHERE 
		t.CheckInTime IS NOT NULL 
		AND t.CheckOutTime IS NULL 
		AND t.TagNumber = @TagNumber;


	IF @AlreadyCheckedIn > 0
    BEGIN
        RAISERROR('Car is already checked in.', 16, 1);
		RETURN;
    END

	INSERT INTO Transactions (TagNumber, SpotID, CheckInTime)
    VALUES (@TagNumber, @SpotID, GETDATE());

	UPDATE ParkingSpots
	   SET [IsOccupied] = 1
	WHERE SpotID = @SpotID;

	SET @RowsAffected = @@ROWCOUNT;
END;


--exec sp_RegisterCarArrival 'DDD'
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UpdateCarCheckOutTime]
    @TagNumber NVARCHAR(50),
	@HourlyFee INT,
	@RowsAffected INT OUTPUT,
	@ChargedAmount DECIMAL(18, 2) OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;

	--Is the car registered in the parking lot
	DECLARE @CheckInTime DATETIME;
	DECLARE @SpotID INT;
	SELECT 
		@CheckInTime = t.CheckInTime,
		@SpotID = t.SpotID
	FROM 
		Transactions t
	WHERE 
		t.CheckInTime IS NOT NULL 
		AND t.CheckOutTime IS NULL 
		AND t.TagNumber = @TagNumber;


	IF @CheckInTime IS NULL
    BEGIN
        RAISERROR('Car is not registered.', 16, 1);
		RETURN;
    END

    DECLARE @TotalAmount DECIMAL(18, 2);
	DECLARE @CheckOutTime DATETIME = GETDATE();


    SET @TotalAmount = dbo.fn_getElapsedTime(@CheckInTime, @CheckOutTime) * @HourlyFee;

	UPDATE 
		Transactions
	SET 
		CheckOutTime = @CheckOutTime,
		TotalAmount = @TotalAmount
	WHERE 
		CheckInTime IS NOT NULL 
		AND CheckOutTime IS NULL 
		AND TagNumber = @TagNumber

	UPDATE ParkingSpots
	   SET [IsOccupied] = 0
	WHERE SpotID = @SpotID;

	SET @ChargedAmount = @TotalAmount;
	SET @RowsAffected = @@ROWCOUNT;
END;
GO
USE [master]
GO
ALTER DATABASE [ParkingLotManagementDB] SET  READ_WRITE 
GO
