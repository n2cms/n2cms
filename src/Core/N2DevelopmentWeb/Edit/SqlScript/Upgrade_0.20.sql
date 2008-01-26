/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
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
CREATE TABLE dbo.Tmp_n2Item
	(
	ID int NOT NULL IDENTITY (1, 1),
	Name varchar(255) NULL,
	ZoneName varchar(50) NULL,
	ParentID int NULL,
	Type varchar(255) NULL,
	Title varchar(255) NULL,
	SortOrder int NOT NULL,
	Created datetime NOT NULL,
	Updated datetime NOT NULL,
	Published datetime NULL,
	Expires datetime NULL,
	Visible bit NULL
	)  ON [PRIMARY]
GO
SET IDENTITY_INSERT dbo.Tmp_n2Item ON
GO
IF EXISTS(SELECT * FROM dbo.n2Item)
	 EXEC('INSERT INTO dbo.Tmp_n2Item (ID, Name, ZoneName, ParentID, Type, Title, SortOrder, Created, Updated, Published, Expires, Visible)
		SELECT ID, Name, ZoneName, ParentID, Type, Title, SortOrder, Created, Updated, Published, Expires, Visible FROM dbo.n2Item WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_n2Item OFF
GO
ALTER TABLE dbo.n2Item
	DROP CONSTRAINT FK_ContentItem_ContentItem
GO
ALTER TABLE dbo.n2Detail
	DROP CONSTRAINT FK_Attribute_Item
GO
DROP TABLE dbo.n2Item
GO
EXECUTE sp_rename N'dbo.Tmp_n2Item', N'n2Item', 'OBJECT' 
GO
ALTER TABLE dbo.n2Item ADD CONSTRAINT
	PK_ContentItem PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.n2Item ADD CONSTRAINT
	FK_ContentItem_ContentItem FOREIGN KEY
	(
	ParentID
	) REFERENCES dbo.n2Item
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.n2Detail ADD CONSTRAINT
	FK_Attribute_Item FOREIGN KEY
	(
	ItemID
	) REFERENCES dbo.n2Item
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
