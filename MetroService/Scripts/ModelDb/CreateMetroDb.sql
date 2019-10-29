CREATE TABLE [dbo].[User] (
    [login]    NVARCHAR (50) NOT NULL,
    [password] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([login] ASC)
);

GO 

CREATE TABLE [dbo].[Document] (
    [Name]    NVARCHAR (50) NOT NULL,
    [header]  NVARCHAR (50) NOT NULL,
    [content] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Name] ASC)
);

GO

CREATE TABLE [dbo].[NotFamiliarDocuments] (
    [user_Login]          NVARCHAR (50) NOT NULL,
    [names_DocumentsList] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([user_Login] ASC),
    FOREIGN KEY ([user_Login]) REFERENCES [dbo].[User] ([login])
);

