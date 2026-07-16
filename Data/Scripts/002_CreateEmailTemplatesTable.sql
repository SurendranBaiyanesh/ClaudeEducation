-- Backlog Ticket Manager - Email Templates.
-- Column names intentionally match the field names declared in
-- Data/Schema/Xml/EmailTemplatesTable.xsd so DataTable columns line up 1:1 with SQL rows.

USE BacklogTicketManager;
GO

IF OBJECT_ID('dbo.EmailTemplates', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EmailTemplates
    (
        Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Name         NVARCHAR(200)      NOT NULL,
        Description  NVARCHAR(1000)     NOT NULL DEFAULT N'',
        Subject      NVARCHAR(400)      NOT NULL,
        Body         NVARCHAR(MAX)      NOT NULL,
        IsActive     BIT                NOT NULL DEFAULT 1,
        CreatedDate  DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedDate  DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME()
    );

    CREATE UNIQUE INDEX UX_EmailTemplates_Name ON dbo.EmailTemplates(Name);
END
GO

-- Seed a sample template (idempotent: only inserted if no template with this name exists).
IF NOT EXISTS (SELECT 1 FROM dbo.EmailTemplates WHERE Name = N'Ticket Assigned')
BEGIN
    INSERT INTO dbo.EmailTemplates (Name, Description, Subject, Body, IsActive)
    VALUES
    (
        N'Ticket Assigned',
        N'Sent to the assignee when a ticket is assigned to them.',
        N'[Backlog] Ticket #{{TicketId}} assigned to you: {{Title}}',
        N'<div style="font-family:Segoe UI,Arial,sans-serif;color:#1f2933;">
  <h2 style="color:#2563eb;margin:0 0 12px;">A ticket has been assigned to you</h2>
  <p>Hi {{AssignedTo}},</p>
  <p>You have been assigned ticket <strong>#{{TicketId}} - {{Title}}</strong>.</p>
  <table style="border-collapse:collapse;margin:12px 0;">
    <tr><td style="padding:4px 12px 4px 0;color:#6b7280;">Priority</td><td style="padding:4px 0;"><strong>{{Priority}}</strong></td></tr>
    <tr><td style="padding:4px 12px 4px 0;color:#6b7280;">Due date</td><td style="padding:4px 0;"><strong>{{DueDate}}</strong></td></tr>
  </table>
  <p>{{Description}}</p>
  <p style="color:#6b7280;font-size:12px;margin-top:24px;">Backlog Ticket Manager</p>
</div>',
        1
    );
END
GO
