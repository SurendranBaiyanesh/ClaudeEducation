-- Backlog Ticket Manager - application users (sign-in / sign-up).
-- Column names intentionally match the field names declared in
-- Data/Schema/Xml/UsersTable.xsd so DataTable columns line up 1:1 with SQL rows.
-- NOTE: passwords are never stored here in the clear - PasswordHash holds a salted
-- PBKDF2 hash produced by the Logic layer (see IPasswordHasher). A default admin user
-- is seeded on application startup (see Program.cs) so the hash matches the hasher.

USE BacklogTicketManager;
GO

IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id             INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Username       NVARCHAR(100)      NOT NULL,
        Email          NVARCHAR(320)      NOT NULL,
        DisplayName    NVARCHAR(200)      NOT NULL,
        PasswordHash   NVARCHAR(400)      NOT NULL,
        CreatedDate    DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
        LastLoginDate  DATETIME2          NULL
    );

    CREATE UNIQUE INDEX UX_Users_Username ON dbo.Users(Username);
    CREATE UNIQUE INDEX UX_Users_Email ON dbo.Users(Email);
END
GO
