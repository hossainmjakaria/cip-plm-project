USE [master]
GO
/****** Object:  Database [ParkingLotManagementDB]    Script Date: 3/22/2024 2:08:52 PM ******/
CREATE DATABASE [ParkingLotManagementDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ParkingLotManagementDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\ParkingLotManagementDB.mdf' , SIZE = 3264KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'ParkingLotManagementDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\ParkingLotManagementDB_log.ldf' , SIZE = 816KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [ParkingLotManagementDB] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ParkingLotManagementDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ParkingLotManagementDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ParkingLotManagementDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ParkingLotManagementDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [ParkingLotManagementDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ParkingLotManagementDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET RECOVERY FULL 
GO
ALTER DATABASE [ParkingLotManagementDB] SET  MULTI_USER 
GO
ALTER DATABASE [ParkingLotManagementDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ParkingLotManagementDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ParkingLotManagementDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ParkingLotManagementDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [ParkingLotManagementDB] SET DELAYED_DURABILITY = DISABLED 
GO
USE [ParkingLotManagementDB]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_getElapsedTime]    Script Date: 3/22/2024 2:08:52 PM ******/
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
/****** Object:  Table [dbo].[ParkingSpots]    Script Date: 3/22/2024 2:08:52 PM ******/
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
/****** Object:  Table [dbo].[Transactions]    Script Date: 3/22/2024 2:08:52 PM ******/
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
/****** Object:  StoredProcedure [dbo].[sp_EnsureParkingSpot]    Script Date: 3/22/2024 2:08:52 PM ******/
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
/****** Object:  StoredProcedure [dbo].[sp_GetParkingStats]    Script Date: 3/22/2024 2:08:52 PM ******/
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
/****** Object:  StoredProcedure [dbo].[sp_GetSnapshot]    Script Date: 3/22/2024 2:08:52 PM ******/
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
/****** Object:  StoredProcedure [dbo].[sp_IsCarCheckedIn]    Script Date: 3/22/2024 2:08:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_IsCarCheckedIn]
	@TagNumber NVARCHAR(50) = ''
AS
BEGIN
    SELECT 
		COUNT(*) CheckedIn
	FROM 
		Transactions t
	WHERE 
		t.CheckOutTime IS NOT NULL 
		AND t.CheckOutTime IS NULL 
		AND t.TagNumber = @TagNumber
	
	RETURN;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_RegisterCarArrival]    Script Date: 3/22/2024 2:08:52 PM ******/
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
/****** Object:  StoredProcedure [dbo].[sp_UpdateCarCheckOutTime]    Script Date: 3/22/2024 2:08:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UpdateCarCheckOutTime]
    @TagNumber NVARCHAR(50),
	@HourlyFee INT,
	@RowsAffected INT OUTPUT 
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

	SET @RowsAffected = @@ROWCOUNT;
END;
GO
USE [master]
GO
ALTER DATABASE [ParkingLotManagementDB] SET  READ_WRITE 
GO
