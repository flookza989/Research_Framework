
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/15/2023 23:05:27
-- Generated from EDMX file: C:\Users\FLOOK\source\repos\Research_Framework\Research_Framework\ApplicationDB\ContextDB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ResearchDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[branch]', 'U') IS NOT NULL
    DROP TABLE [dbo].[branch];
GO
IF OBJECT_ID(N'[dbo].[faculty]', 'U') IS NOT NULL
    DROP TABLE [dbo].[faculty];
GO
IF OBJECT_ID(N'[dbo].[log]', 'U') IS NOT NULL
    DROP TABLE [dbo].[log];
GO
IF OBJECT_ID(N'[dbo].[process]', 'U') IS NOT NULL
    DROP TABLE [dbo].[process];
GO
IF OBJECT_ID(N'[dbo].[process_path]', 'U') IS NOT NULL
    DROP TABLE [dbo].[process_path];
GO
IF OBJECT_ID(N'[dbo].[research]', 'U') IS NOT NULL
    DROP TABLE [dbo].[research];
GO
IF OBJECT_ID(N'[dbo].[research_member]', 'U') IS NOT NULL
    DROP TABLE [dbo].[research_member];
GO
IF OBJECT_ID(N'[dbo].[research_status]', 'U') IS NOT NULL
    DROP TABLE [dbo].[research_status];
GO
IF OBJECT_ID(N'[dbo].[user]', 'U') IS NOT NULL
    DROP TABLE [dbo].[user];
GO
IF OBJECT_ID(N'[ResearchDBModelStoreContainer].[View_research]', 'U') IS NOT NULL
    DROP TABLE [ResearchDBModelStoreContainer].[View_research];
GO
IF OBJECT_ID(N'[ResearchDBModelStoreContainer].[View_research_member]', 'U') IS NOT NULL
    DROP TABLE [ResearchDBModelStoreContainer].[View_research_member];
GO
IF OBJECT_ID(N'[ResearchDBModelStoreContainer].[View_user]', 'U') IS NOT NULL
    DROP TABLE [ResearchDBModelStoreContainer].[View_user];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'View_user'
CREATE TABLE [dbo].[View_user] (
    [id] int  NOT NULL,
    [username] nvarchar(50)  NOT NULL,
    [password] nvarchar(50)  NOT NULL,
    [name] nvarchar(50)  NOT NULL,
    [lname] nvarchar(50)  NOT NULL,
    [faculty_name] nvarchar(50)  NULL,
    [branch_name] nvarchar(50)  NULL,
    [permission] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'branch'
CREATE TABLE [dbo].[branch] (
    [id] int IDENTITY(1,1) NOT NULL,
    [branch_name] nvarchar(50)  NULL,
    [faculty_id] int  NOT NULL
);
GO

-- Creating table 'faculty'
CREATE TABLE [dbo].[faculty] (
    [id] int IDENTITY(1,1) NOT NULL,
    [faculty_name] nvarchar(50)  NULL
);
GO

-- Creating table 'log'
CREATE TABLE [dbo].[log] (
    [id] int IDENTITY(1,1) NOT NULL,
    [user_id] int  NULL,
    [log_name] nvarchar(50)  NULL,
    [log_time] datetime  NULL
);
GO

-- Creating table 'process'
CREATE TABLE [dbo].[process] (
    [id] int IDENTITY(1,1) NOT NULL,
    [process1] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'process_path'
CREATE TABLE [dbo].[process_path] (
    [id] int IDENTITY(1,1) NOT NULL,
    [process_id] int  NOT NULL,
    [research_id] int  NOT NULL,
    [path_student] nvarchar(max)  NULL,
    [path_teacher] nvarchar(max)  NULL,
    [status] bit  NULL
);
GO

-- Creating table 'research_member'
CREATE TABLE [dbo].[research_member] (
    [id] int IDENTITY(1,1) NOT NULL,
    [user_id] int  NOT NULL,
    [research_id] int  NOT NULL
);
GO

-- Creating table 'research_status'
CREATE TABLE [dbo].[research_status] (
    [id] int IDENTITY(1,1) NOT NULL,
    [status_inprogress1] bit  NOT NULL,
    [status_estimate1] bit  NOT NULL,
    [status_inprogress2] bit  NOT NULL,
    [status_estimate2] bit  NOT NULL,
    [status_inprogress3] bit  NOT NULL,
    [status_complete] bit  NOT NULL,
    [research_name] nvarchar(100)  NOT NULL
);
GO

-- Creating table 'user'
CREATE TABLE [dbo].[user] (
    [id] int IDENTITY(1,1) NOT NULL,
    [username] nvarchar(50)  NOT NULL,
    [password] nvarchar(50)  NOT NULL,
    [name] nvarchar(50)  NOT NULL,
    [lname] nvarchar(50)  NOT NULL,
    [faculty_id] int  NOT NULL,
    [branch_id] int  NOT NULL,
    [permission] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'View_research'
CREATE TABLE [dbo].[View_research] (
    [id] int  NOT NULL,
    [research_name] nvarchar(100)  NOT NULL,
    [user_id] int  NOT NULL,
    [username] nvarchar(50)  NOT NULL,
    [password] nvarchar(50)  NOT NULL,
    [name] nvarchar(50)  NOT NULL,
    [lname] nvarchar(50)  NOT NULL,
    [faculty_name] nvarchar(50)  NULL,
    [branch_name] nvarchar(50)  NULL,
    [permission] nvarchar(50)  NOT NULL,
    [approve] bit  NOT NULL
);
GO

-- Creating table 'View_research_member'
CREATE TABLE [dbo].[View_research_member] (
    [id] int  NOT NULL,
    [user_id] int  NOT NULL,
    [username] nvarchar(50)  NOT NULL,
    [password] nvarchar(50)  NOT NULL,
    [name] nvarchar(50)  NOT NULL,
    [lname] nvarchar(50)  NOT NULL,
    [faculty_name] nvarchar(50)  NULL,
    [branch_name] nvarchar(50)  NULL,
    [permission] nvarchar(50)  NOT NULL,
    [research_id] int  NOT NULL,
    [research_name] nvarchar(100)  NOT NULL,
    [teacher_id] int  NOT NULL
);
GO

-- Creating table 'research'
CREATE TABLE [dbo].[research] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(100)  NOT NULL,
    [user_id] int  NOT NULL,
    [approve] bit  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id], [username], [password], [name], [lname], [permission] in table 'View_user'
ALTER TABLE [dbo].[View_user]
ADD CONSTRAINT [PK_View_user]
    PRIMARY KEY CLUSTERED ([id], [username], [password], [name], [lname], [permission] ASC);
GO

-- Creating primary key on [id], [faculty_id] in table 'branch'
ALTER TABLE [dbo].[branch]
ADD CONSTRAINT [PK_branch]
    PRIMARY KEY CLUSTERED ([id], [faculty_id] ASC);
GO

-- Creating primary key on [id] in table 'faculty'
ALTER TABLE [dbo].[faculty]
ADD CONSTRAINT [PK_faculty]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'log'
ALTER TABLE [dbo].[log]
ADD CONSTRAINT [PK_log]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [process1] in table 'process'
ALTER TABLE [dbo].[process]
ADD CONSTRAINT [PK_process]
    PRIMARY KEY CLUSTERED ([process1] ASC);
GO

-- Creating primary key on [process_id], [research_id] in table 'process_path'
ALTER TABLE [dbo].[process_path]
ADD CONSTRAINT [PK_process_path]
    PRIMARY KEY CLUSTERED ([process_id], [research_id] ASC);
GO

-- Creating primary key on [user_id], [research_id] in table 'research_member'
ALTER TABLE [dbo].[research_member]
ADD CONSTRAINT [PK_research_member]
    PRIMARY KEY CLUSTERED ([user_id], [research_id] ASC);
GO

-- Creating primary key on [research_name] in table 'research_status'
ALTER TABLE [dbo].[research_status]
ADD CONSTRAINT [PK_research_status]
    PRIMARY KEY CLUSTERED ([research_name] ASC);
GO

-- Creating primary key on [username] in table 'user'
ALTER TABLE [dbo].[user]
ADD CONSTRAINT [PK_user]
    PRIMARY KEY CLUSTERED ([username] ASC);
GO

-- Creating primary key on [id], [research_name], [user_id], [username], [password], [name], [lname], [permission], [approve] in table 'View_research'
ALTER TABLE [dbo].[View_research]
ADD CONSTRAINT [PK_View_research]
    PRIMARY KEY CLUSTERED ([id], [research_name], [user_id], [username], [password], [name], [lname], [permission], [approve] ASC);
GO

-- Creating primary key on [id], [user_id], [username], [password], [name], [lname], [permission], [research_id], [research_name], [teacher_id] in table 'View_research_member'
ALTER TABLE [dbo].[View_research_member]
ADD CONSTRAINT [PK_View_research_member]
    PRIMARY KEY CLUSTERED ([id], [user_id], [username], [password], [name], [lname], [permission], [research_id], [research_name], [teacher_id] ASC);
GO

-- Creating primary key on [id] in table 'research'
ALTER TABLE [dbo].[research]
ADD CONSTRAINT [PK_research]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------