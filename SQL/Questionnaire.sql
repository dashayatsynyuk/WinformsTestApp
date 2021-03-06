use NevlabsTest
Go

/*
   29 марта 2018 г.19:54:43
   User: 
   Server: DESKTOP-SD21DGS
   Database: NevlabsTest
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
DROP TABLE dbo.Questionnaire

BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Questionnaire
	(
	QuestionnaireId int identity NOT NULL,
	Name nchar(100) NOT NULL,
	BirthDate datetime NOT NULL,
	Email nchar(100) NOT NULL,
	Phone nchar(20) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Questionnaire ADD CONSTRAINT
	PK_Questionnaire PRIMARY KEY CLUSTERED 
	(
	QuestionnaireId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Questionnaire SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
