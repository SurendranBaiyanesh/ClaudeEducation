-- Backlog Ticket Manager - initial schema.
-- Column names intentionally match the field names declared in Data/Schema/Xml/*.xsd
-- so DataTable columns line up 1:1 with SQL rows.

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BacklogTicketManager')
BEGIN
    CREATE DATABASE BacklogTicketManager;
END
GO

USE BacklogTicketManager;
GO

IF OBJECT_ID('dbo.Tickets', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tickets
    (
        Id                INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Title             NVARCHAR(200)      NOT NULL,
        Description       NVARCHAR(MAX)      NOT NULL DEFAULT N'',
        Status            INT                NOT NULL DEFAULT 0,
        Priority          INT                NOT NULL DEFAULT 1,
        AssignedTo        NVARCHAR(200)      NOT NULL DEFAULT N'',
        AssignedToEmail   NVARCHAR(320)      NOT NULL DEFAULT N'',
        CreatedDate       DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
        DueDate           DATETIME2          NOT NULL,
        CompletedDate     DATETIME2          NULL,
        LastNotifiedDate  DATETIME2          NULL
    );
END
GO

IF OBJECT_ID('dbo.TicketUpdates', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TicketUpdates
    (
        Id            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TicketId      INT                NOT NULL REFERENCES dbo.Tickets(Id) ON DELETE CASCADE,
        UpdateText    NVARCHAR(MAX)      NOT NULL,
        UpdatedBy     NVARCHAR(200)      NOT NULL,
        UpdatedDate   DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.EmailNotifications', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EmailNotifications
    (
        Id                INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TicketId          INT                NOT NULL REFERENCES dbo.Tickets(Id) ON DELETE CASCADE,
        NotificationType  INT                NOT NULL,
        RecipientEmail    NVARCHAR(320)      NOT NULL,
        Subject           NVARCHAR(400)      NOT NULL,
        Body              NVARCHAR(MAX)      NOT NULL,
        SentDate          DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
        Success           BIT                NOT NULL,
        ErrorMessage      NVARCHAR(MAX)      NULL
    );
END
GO
