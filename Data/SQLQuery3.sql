-- Step 1: Create a new database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'service-atlanta-db')
BEGIN
    CREATE DATABASE [service-atlanta-db];
END;
GO

-- Switch to the new database
USE [service-atlanta-db];
GO

-- Step 2: Create __EFMigrationsHistory table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='__EFMigrationsHistory' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory] (
        [MigrationId] NVARCHAR(300) NOT NULL,
        [ProductVersion] NVARCHAR(32) NOT NULL,
        PRIMARY KEY ([MigrationId])
    );
END;
GO

-- Step 3: Create AboutUs table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AboutUs' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[AboutUs] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Description] NVARCHAR(MAX) NOT NULL
    );
END;
GO

-- Step 4: Create BackgroundImages table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BackgroundImages' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[BackgroundImages] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [ImageData] VARBINARY(MAX) NOT NULL,
        [ContentType] NVARCHAR(100) NOT NULL
    );
END;
GO

-- Step 5: Create Content table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Content' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Content] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [PageName] NVARCHAR(200) NOT NULL,
        [Section] NVARCHAR(200) NOT NULL,
        [ContentText] NVARCHAR(MAX) NOT NULL,
        [CreatedAt] DATETIME DEFAULT GETDATE(),
        [UpdatedAt] DATETIME NULL,
        [Title] NVARCHAR(400) NULL,
        [MetaDescription] NVARCHAR(600) NULL,
        [Keywords] NVARCHAR(1000) NULL
    );
END;
GO

-- Step 6: Create CaptchaSettings table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CaptchaSettings' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[CaptchaSettings] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [SiteKey] NVARCHAR(200) NOT NULL,
        [SecretKey] NVARCHAR(200) NOT NULL
    );
END;
GO

-- Step 7: Create EmailVerifications table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmailVerifications' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[EmailVerifications] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Email] NVARCHAR(200) NOT NULL,
        [Code] NVARCHAR(50) NOT NULL,
        [CreatedAt] DATETIME DEFAULT GETDATE(),
        [IsVerified] BIT DEFAULT 0
    );
END;
GO

-- Step 8: Create Icons table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Icons' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Icons] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [ServiceName] NVARCHAR(200) NOT NULL,
        [IconData] VARBINARY(MAX) NOT NULL,
        [ContentType] NVARCHAR(100) NOT NULL
    );
END;
GO

-- Step 9: Create Questions table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Questions' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Questions] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [ServiceType] NVARCHAR(200) NOT NULL,
        [Text] NVARCHAR(MAX) NOT NULL,
        [InputType] NVARCHAR(50) NOT NULL,
        [IsMandatory] BIT NOT NULL DEFAULT 0,
        [ParentQuestionId] INT NULL,
        [ExpectedAnswer] NVARCHAR(MAX) NULL,
        [IsPrompt] BIT NOT NULL DEFAULT 0,
        [PromptMessage] NVARCHAR(MAX) NULL,
        [Page] NVARCHAR(200) NULL
    );
END;
GO

-- Step 10: Create SEOs table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SEOs' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[SEOs] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [PageName] NVARCHAR(200) NOT NULL,
        [Title] NVARCHAR(400) NOT NULL,
        [MetaDescription] NVARCHAR(600) NOT NULL,
        [Keywords] NVARCHAR(1000) NOT NULL
    );
END;
GO

-- Step 11: Create Users table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(200) NOT NULL,
        [Email] NVARCHAR(200) NOT NULL UNIQUE,
        [PasswordHash] NVARCHAR(MAX) NOT NULL,
        [CreatedAt] DATETIME DEFAULT GETDATE(),
        [Role] NVARCHAR(100) DEFAULT 'User'
    );
END;
GO

-- Step 12: Create PageVisit table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PageVisit' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[PageVisit] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [SessionInfoId] INT NOT NULL,
        [PageName] NVARCHAR(200) NULL,
        [VisitTimestamp] DATETIME DEFAULT GETDATE() NOT NULL
    );
END;
GO

-- Optional: Insert sample data for testing
INSERT INTO [dbo].[AboutUs] ([Description]) VALUES ('Mechanical Solutions Atlanta provides top-tier services in plumbing, HVAC, and water filtration.');

INSERT INTO [dbo].[SEOs] ([PageName], [Title], [MetaDescription], [Keywords])
VALUES ('Home', 'Mechanical Solutions Atlanta - Quality Services', 'Providing top-notch plumbing, HVAC, and water filtration services.', 'plumbing, HVAC, heating, air conditioning, water filtration');
GO
