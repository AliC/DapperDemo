USE master;
GO
DROP DATABASE DapperDemo
GO
CREATE DATABASE DapperDemo
GO

USE DapperDemo;

CREATE TABLE dbo.Dog ( DogId INT IDENTITY PRIMARY KEY, Name NVARCHAR(128), Breed NVARCHAR(64), Age INT )
GO

SET IDENTITY_INSERT dbo.Dog ON;
INSERT dbo.Dog (DogId, Name, Breed, Age) SELECT 1, 'Fido', 'Mongrel', 2 UNION SELECT 2, 'Lassie', 'St. Bernard', 7 UNION SELECT 3, 'Shep', 'Sheepdog', 14 UNION SELECT 4, 'Lassie', 'Irish Wolfhound', 3
SET IDENTITY_INSERT dbo.Dog OFF;
GO

SELECT * FROM dbo.Dog
GO

USE master;
GO
