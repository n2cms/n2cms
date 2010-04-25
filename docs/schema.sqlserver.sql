
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKB30F0676AB29607]') AND parent_object_id = OBJECT_ID('n2AllowedRole'))
alter table n2AllowedRole  drop constraint FKB30F0676AB29607


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK1D14F83B6AB29607]') AND parent_object_id = OBJECT_ID('n2Detail'))
alter table n2Detail  drop constraint FK1D14F83B6AB29607


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK1D14F83B4F9855AA]') AND parent_object_id = OBJECT_ID('n2Detail'))
alter table n2Detail  drop constraint FK1D14F83B4F9855AA


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK1D14F83B3AF5DAB0]') AND parent_object_id = OBJECT_ID('n2Detail'))
alter table n2Detail  drop constraint FK1D14F83B3AF5DAB0


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK18406FA04B1A4E60]') AND parent_object_id = OBJECT_ID('n2Item'))
alter table n2Item  drop constraint FK18406FA04B1A4E60


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK18406FA018DD5AFD]') AND parent_object_id = OBJECT_ID('n2Item'))
alter table n2Item  drop constraint FK18406FA018DD5AFD


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKBE85C49A6AB29607]') AND parent_object_id = OBJECT_ID('n2DetailCollection'))
alter table n2DetailCollection  drop constraint FKBE85C49A6AB29607


    if exists (select * from dbo.sysobjects where id = object_id(N'n2AllowedRole') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table n2AllowedRole

    if exists (select * from dbo.sysobjects where id = object_id(N'n2Detail') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table n2Detail

    if exists (select * from dbo.sysobjects where id = object_id(N'n2Item') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table n2Item

    if exists (select * from dbo.sysobjects where id = object_id(N'n2DetailCollection') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table n2DetailCollection

    create table n2AllowedRole (
        ID INT IDENTITY NOT NULL,
       ItemID INT not null,
       Role NVARCHAR(50) not null,
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
       StringValue NVARCHAR(MAX) null,
       Value VARBINARY(MAX) null,
       primary key (ID)
    )

    create table n2Item (
        ID INT IDENTITY NOT NULL,
       Type NVARCHAR(255) not null,
       Created DATETIME not null,
       Published DATETIME null,
       Updated DATETIME not null,
       Expires DATETIME null,
       Name NVARCHAR(255) null,
       ZoneName NVARCHAR(50) null,
       Title NVARCHAR(255) null,
       SortOrder INT not null,
       Visible BIT not null,
       SavedBy NVARCHAR(50) null,
       State INT null,
       AncestralTrail NVARCHAR(100) null,
       VersionIndex INT null,
       AlteredPermissions INT null,
       VersionOfID INT null,
       ParentID INT null,
       primary key (ID)
    )

    create table n2DetailCollection (
        ID INT IDENTITY NOT NULL,
       ItemID INT null,
       Name NVARCHAR(50) not null,
       primary key (ID)
    )

    alter table n2AllowedRole 
        add constraint FKB30F0676AB29607 
        foreign key (ItemID) 
        references n2Item

    alter table n2Detail 
        add constraint FK1D14F83B6AB29607 
        foreign key (ItemID) 
        references n2Item

    alter table n2Detail 
        add constraint FK1D14F83B4F9855AA 
        foreign key (DetailCollectionID) 
        references n2DetailCollection

    alter table n2Detail 
        add constraint FK1D14F83B3AF5DAB0 
        foreign key (LinkValue) 
        references n2Item

    alter table n2Item 
        add constraint FK18406FA04B1A4E60 
        foreign key (VersionOfID) 
        references n2Item

    alter table n2Item 
        add constraint FK18406FA018DD5AFD 
        foreign key (ParentID) 
        references n2Item

    alter table n2DetailCollection 
        add constraint FKBE85C49A6AB29607 
        foreign key (ItemID) 
        references n2Item
