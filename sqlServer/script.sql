USE master;
GO

- Drop the database if it exists
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'social_media')
 DROP DATABASE social_media;
GO
 - Create the database
CREATE DATABASE social_media;
GO
USE social_media;
GO
 - Table 'UserData'
IF OBJECT_ID('UserData', 'U') IS NOT NULL
 DROP TABLE UserData;
GO
CREATE TABLE UserData (
 id CHAR(36) NOT NULL PRIMARY KEY,
 data VARCHAR(160) NOT NULL
);
GO