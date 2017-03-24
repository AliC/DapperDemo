USE master;
DROP DATABASE DapperDemo
GO
CREATE DATABASE DapperDemo
GO

USE DapperDemo;

CREATE TABLE dbo.Wombat ( WombatId INT IDENTITY PRIMARY KEY, Name NVARCHAR(128), GeographicalAddress NVARCHAR(256), Rating INT )

SET IDENTITY_INSERT dbo.Wombat ON;
INSERT dbo.Wombat (WombatId, Name, GeographicalAddress, Rating) SELECT 1, 'Harry', 'Wombat State Forest', 3 UNION SELECT 2, 'Rosie', 'Wombat Hill', 8 UNION SELECT 3, 'McTavish', 'Wombat', 4 UNION SELECT 4, 'Sleepy', ' Adelaide Zoo', 10
SET IDENTITY_INSERT dbo.Wombat OFF;

SELECT * FROM dbo.Wombat
GO

USE master;
GO
