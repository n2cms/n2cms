-- N2 CMS CREATE SCRIPTS FOR MySQL

-- n2Item
CREATE TABLE n2Item 
(
	ID INT NOT NULL AUTO_INCREMENT PRIMARY KEY ,
	Name VARCHAR (255) NOT NULL ,
	ParentID INT NULL ,
	Type VARCHAR (255) NULL ,
	Title VARCHAR (255) NULL ,
	SortOrder INT NOT NULL ,
	Created DATETIME NOT NULL ,
	Updated DATETIME NOT NULL ,
	Published DATETIME NOT NULL ,
	Expires DATETIME NULL ,
	Visible BOOL NULL ,
	ZoneName VARCHAR (50) NULL ,
	VersionOfID INT NULL ,
	SavedBy VARCHAR (50) NULL ,
	FOREIGN KEY FK_n2Item_Parent (ParentID) REFERENCES n2Item (ID) ,
	FOREIGN KEY FK_n2Item_VersionOf (VersionOfID) REFERENCES n2Item (ID)
) ENGINE=INNODB

GO

-- n2Detail
CREATE TABLE n2Detail 
(
	ID INT NOT NULL AUTO_INCREMENT PRIMARY KEY ,
	ItemID INT NOT NULL ,
	DetailCollectionID INT NULL ,
	Type VARCHAR (50) NULL ,
	Name VARCHAR (50) NOT NULL ,
	BoolValue BOOL NULL ,
	IntValue INT NULL ,
	DoubleValue DOUBLE NULL ,  
	DateTimeValue DATETIME NULL ,
	StringValue LONGTEXT NULL ,
	LinkValue int NULL ,
	Value LONGBLOB NULL ,
	FOREIGN KEY FK_n2Detail_n2Item (ItemID) REFERENCES n2Item (ID),
	FOREIGN KEY FK_n2Detail_n2Item_link (LinkValue) REFERENCES n2Item (ID),
	FOREIGN KEY FK_n2Detail_n2DetailCollection (DetailCollectionID) REFERENCES n2DetailCollection (ID)
) ENGINE=INNODB

GO

-- n2AllowedRole
CREATE TABLE n2AllowedRole 
(
	ID INT NOT NULL AUTO_INCREMENT PRIMARY KEY ,
	ItemID INT NOT NULL ,
	Role VARCHAR (50) NOT NULL ,
	FOREIGN KEY FK_n2AllowedRole_n2Item (ItemID) REFERENCES n2Item (ID)
) ENGINE=INNODB

GO

-- n2DetailCollection
CREATE TABLE n2DetailCollection
(
	ID INT NOT NULL AUTO_INCREMENT PRIMARY KEY ,
	ItemID int NOT NULL,
	Name varchar (50) NOT NULL ,
	FOREIGN KEY FK_n2DetailCollection_n2Item (ItemID) REFERENCES n2Item (ID)
)
