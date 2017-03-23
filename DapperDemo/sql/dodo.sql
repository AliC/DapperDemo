USE master;
DROP DATABASE DapperDemo
GO
CREATE DATABASE DapperDemo
GO

USE DapperDemo;
CREATE TABLE dbo.Dodo ( DodoId INT IDENTITY PRIMARY KEY, Name NVARCHAR(128))
GO
CREATE TABLE dbo.GameStatistics (Id INT IDENTITY PRIMARY KEY, DodoId INT REFERENCES Dodo(DodoId), Bitepower INT, Cuteness INT, Speed INT)
GO

SET IDENTITY_INSERT dbo.Dodo ON;
INSERT dbo.Dodo (DodoId, Name) SELECT 1, 'Anthony' UNION SELECT 2, 'Samantha' UNION SELECT 3, 'Norris' UNION SELECT 4, 'Tommy'
SET IDENTITY_INSERT dbo.Dodo OFF;
GO

INSERT dbo.GameStatistics (DodoId, BitePower, Cuteness, Speed) SELECT 1, 19, 1, 8 UNION SELECT 2, 2, 10, 20 UNION SELECT 3, 9, 18, 12 UNION SELECT 4, 16, 14, 15
GO

SELECT * FROM dbo.Dodo d LEFT OUTER JOIN dbo.GameStatistics g ON d.DodoId = g.DodoId

