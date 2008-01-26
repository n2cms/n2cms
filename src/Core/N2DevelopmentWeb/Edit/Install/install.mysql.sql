create table n2Item (
  ID INTEGER NOT NULL AUTO_INCREMENT,
   Type VARCHAR(255) not null,
   Updated DATETIME not null,
   Name VARCHAR(255),
   ZoneName VARCHAR(50),
   Title VARCHAR(255),
   Created DATETIME not null,
   Published DATETIME,
   Expires DATETIME,
   SortOrder INTEGER not null,
   Visible TINYINT(1) not null,
   SavedBy VARCHAR(50),
   VersionOfID INTEGER,
   ParentID INTEGER,
   primary key (ID)
)
create table n2Detail (
  ID INTEGER NOT NULL AUTO_INCREMENT,
   Type VARCHAR(255) not null,
   ItemID INTEGER not null,
   DetailCollectionID INTEGER,
   Name VARCHAR(50),
   BoolValue TINYINT(1),
   IntValue INTEGER,
   LinkValue INTEGER,
   DoubleValue DOUBLE,
   DateTimeValue DATETIME,
   StringValue LONGTEXT,
   Value LONGBLOB,
   primary key (ID)
)
create table n2DetailCollection (
  ID INTEGER NOT NULL AUTO_INCREMENT,
   ItemID INTEGER,
   Name VARCHAR(50) not null,
   primary key (ID)
)
create table n2AllowedRole (
  ID INTEGER NOT NULL AUTO_INCREMENT,
   ItemID INTEGER not null,
   Role VARCHAR(50) not null,
   primary key (ID)
)
alter table n2Item  add index (ParentID), add constraint FK18406FA09B59280D foreign key (ParentID) references n2Item (ID) 
alter table n2Item  add index (VersionOfID), add constraint FK18406FA0BAB0B0D0 foreign key (VersionOfID) references n2Item (ID) 
alter table n2Detail  add index (LinkValue), add constraint FK1D14F83B7ECFB400 foreign key (LinkValue) references n2Item (ID) 
alter table n2Detail  add index (DetailCollectionID), add constraint FK1D14F83B4F20C9A5 foreign key (DetailCollectionID) references n2DetailCollection (ID) 
alter table n2Detail  add index (ItemID), add constraint FK1D14F83B2E687F77 foreign key (ItemID) references n2Item (ID) 
alter table n2DetailCollection  add index (ItemID), add constraint FKBE85C49A2E687F77 foreign key (ItemID) references n2Item (ID) 
alter table n2AllowedRole  add index (ItemID), add constraint FKB30F0672E687F77 foreign key (ItemID) references n2Item (ID) 
