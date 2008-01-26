create table n2Item (
  ID INT IDENTITY NOT NULL,
   Type NVARCHAR(255) not null,
   Updated DATETIME not null,
   Name NVARCHAR(255) null,
   ZoneName NVARCHAR(50) null,
   Title NVARCHAR(255) null,
   Created DATETIME not null,
   Published DATETIME null,
   Expires DATETIME null,
   SortOrder INT not null,
   Visible BIT not null,
   SavedBy NVARCHAR(50) null,
   VersionOfID INT null,
   ParentID INT null,
   primary key (ID)
)
create table n2Detail (
  ID INT IDENTITY NOT NULL,
   Type NVARCHAR(255) not null,
   ItemID INT not null,
   DetailCollectionID INT null,
   Name NVARCHAR(50) null,
   BoolValue BIT null,
   IntValue INT null,
   LinkValue INT null,
   DoubleValue DOUBLE PRECISION null,
   DateTimeValue DATETIME null,
   StringValue NTEXT null,
   Value IMAGE null,
   primary key (ID)
)
create table n2DetailCollection (
  ID INT IDENTITY NOT NULL,
   ItemID INT null,
   Name NVARCHAR(50) not null,
   primary key (ID)
)
create table n2AllowedRole (
  ID INT IDENTITY NOT NULL,
   ItemID INT not null,
   Role NVARCHAR(50) not null,
   primary key (ID)
)
alter table n2Item  add constraint FK18406FA09B59280D foreign key (ParentID) references n2Item 
alter table n2Item  add constraint FK18406FA0BAB0B0D0 foreign key (VersionOfID) references n2Item 
alter table n2Detail  add constraint FK1D14F83B7ECFB400 foreign key (LinkValue) references n2Item 
alter table n2Detail  add constraint FK1D14F83B4F20C9A5 foreign key (DetailCollectionID) references n2DetailCollection 
alter table n2Detail  add constraint FK1D14F83B2E687F77 foreign key (ItemID) references n2Item 
alter table n2DetailCollection  add constraint FKBE85C49A2E687F77 foreign key (ItemID) references n2Item 
alter table n2AllowedRole  add constraint FKB30F0672E687F77 foreign key (ItemID) references n2Item 
