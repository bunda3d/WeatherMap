-- Create WeatherMap_DEV Database
CREATE DATABASE WeatherMap_DEV;

GO
  USE WeatherMap_DEV;

GO
  -- Create AppSettings Table
  CREATE TABLE AppSettings (
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

GO
  -- Create LogsExceptions Table
  CREATE TABLE LogsExceptions (
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

GO
  -- Create LogsWebRequests Table
  CREATE TABLE LogsWebRequests (
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

GO