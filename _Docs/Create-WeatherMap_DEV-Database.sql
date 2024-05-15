-- Create WeatherMap_DEV Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'WeatherMap_DEV')
BEGIN
    CREATE DATABASE [WeatherMap_DEV];
END

GO
	USE [WeatherMap_DEV];

GO
	-- Create AppSettings Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[AppSettings]') AND type in (N'U'))
BEGIN
    CREATE TABLE [AppSettings] (
        [ID] INT IDENTITY(1, 1) PRIMARY KEY,
        [Name] NVARCHAR(255),
        [Code] NVARCHAR(MAX),
        [Type] NVARCHAR(50),
        [Description] NVARCHAR(500),
        [Category] NVARCHAR(50),
        [IsActive] BIT,
        [Version] NVARCHAR(20),
        [Permission] NVARCHAR(50),
        [CreatedBy] NVARCHAR(100),
        [CreatedDate] DATETIME,
        [UpdatedBy] NVARCHAR(100),
        [UpdatedDate] DATETIME
    );
END

GO
  -- Create LogsExceptions Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LogsExceptions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [LogsExceptions] (
        [ID] INT IDENTITY(1, 1) PRIMARY KEY,
        [Name] NVARCHAR(255),
        [Code] NVARCHAR(255),
        [Type] NVARCHAR(50),
        [Description] NVARCHAR(500),
        [Message] NVARCHAR(2048),
        [StackTrace] NVARCHAR(MAX),
        [Source] NVARCHAR(255),
        [TargetSite] NVARCHAR(255),
        [ExceptionType] NVARCHAR(255),
        [InnerException] NVARCHAR(MAX),
        [Version] NVARCHAR(20),
        [Permission] NVARCHAR(50),
        [CreatedBy] NVARCHAR(100),
        [CreatedDate] DATETIME,
        [UpdatedBy] NVARCHAR(100),
        [UpdatedDate] DATETIME
    );
END

GO
  -- Create LogsWebRequests Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LogsWebRequests]') AND type in (N'U'))
BEGIN
    CREATE TABLE [LogsWebRequests] (
        [ID] INT IDENTITY(1, 1) PRIMARY KEY,
        [Name] NVARCHAR(50),
        [Code] NVARCHAR(50),
        [Type] NVARCHAR(50),
        [Body] NVARCHAR(MAX),
        [RequestMethod] NVARCHAR(10),
        [ResponseCode] INT,
        [UserAgent] NVARCHAR(512),
        [RequestURL] NVARCHAR(2048),
        [Permission] NVARCHAR(50),
        [CreatedBy] NVARCHAR(100),
        [CreatedDate] DATETIME,
        [UpdatedBy] NVARCHAR(100),
        [UpdatedDate] DATETIME
    );
END

GO

--Insert required AppSettings
IF NOT EXISTS (SELECT * FROM [AppSettings] WHERE [Name] = 'apiBaseUrl_NWSforecasts')
BEGIN
    INSERT INTO [AppSettings] ([Name], [Code], [Type], [Description], [Category], [IsActive], [CreatedBy], [CreatedDate])
    VALUES ('apiBaseUrl_NWSforecasts', 'https://api.weather.gov', 'string', 'API Base URL for Natl Weather Service Forecasts and Office Location endpoints', 'URL', 1, 'Admin', GETDATE());
END

GO

IF NOT EXISTS (SELECT * FROM [AppSettings] WHERE [Name] = 'userAgent_NWS')
BEGIN
    INSERT INTO [AppSettings] ([Name], [Code], [Type], [Description], [Category], [IsActive], [CreatedBy], [CreatedDate])
    VALUES ('userAgent_NWS', 'WeatherMap.com, contact@weathermap.com', 'string', 'User Agent for Natl Weather Service API request header', 'User Agent', 1, 'Admin', GETDATE());
END

GO
