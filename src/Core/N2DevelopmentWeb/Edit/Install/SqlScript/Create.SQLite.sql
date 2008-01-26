-- N2 CMS CREATE SCRIPTS FOR SQLite

-- n2Item
CREATE TABLE n2Item 
(
	ID int NOT NULL PRIMARY KEY AUTOINCREMENT,
	Name varchar (255) NOT NULL ,
	ParentID int NULL ,
	Type varchar (255) NULL ,
	Title varchar (255) NULL ,
	SortOrder int NOT NULL ,
	Created datetime NOT NULL ,
	Updated datetime NOT NULL ,
	Published datetime NULL ,
	Expires datetime NULL ,
	Visible bit NULL ,
	ZoneName varchar (50) NULL ,
	VersionOfID int NULL ,
	SavedBy varchar (50) NULL 
)
GO

-- n2Detail
CREATE TABLE n2Detail 
(
	ID int NOT NULL PRIMARY KEY AUTOINCREMENT,
	ItemID int NOT NULL ,
	DetailCollectionID INT NULL ,
	Type varchar (50) NULL ,
	Name varchar (50) NOT NULL ,
	BoolValue bit NULL ,
	IntValue int NULL ,
	DoubleValue float NULL ,  
	DateTimeValue datetime NULL ,
	StringValue text NULL,
	LinkValue int NULL,
	Value blob NULL
)
GO

-- n2AllowedRole
CREATE TABLE n2AllowedRole 
(
	ID int NOT NULL PRIMARY KEY AUTOINCREMENT,
	ItemID int NOT NULL ,
	Role varchar (50) NOT NULL 
)

GO

CREATE TABLE n2DetailCollection
(
	ID int NOT NULL PRIMARY KEY AUTOINCREMENT,
	ItemID int NOT NULL,
	Name varchar (50) NOT NULL
)
