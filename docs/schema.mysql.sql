
    
alter table n2AllowedRole  drop foreign key FKB30F0676AB29607


    
alter table n2Detail  drop foreign key FK1D14F83B6AB29607


    
alter table n2Detail  drop foreign key FK1D14F83B4F9855AA


    
alter table n2Detail  drop foreign key FK1D14F83B3AF5DAB0


    
alter table n2Item  drop foreign key FK18406FA04B1A4E60


    
alter table n2Item  drop foreign key FK18406FA018DD5AFD


    
alter table n2DetailCollection  drop foreign key FKBE85C49A6AB29607


    drop table if exists n2AllowedRole

    drop table if exists n2Detail

    drop table if exists n2Item

    drop table if exists n2DetailCollection

    create table n2AllowedRole (
        ID INTEGER NOT NULL AUTO_INCREMENT,
       ItemID INTEGER not null,
       Role VARCHAR(50) not null,
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
       StringValue MEDIUMTEXT,
       Value LONGBLOB,
       primary key (ID)
    )

    create table n2Item (
        ID INTEGER NOT NULL AUTO_INCREMENT,
       Type VARCHAR(255) not null,
       Created DATETIME not null,
       Published DATETIME,
       Updated DATETIME not null,
       Expires DATETIME,
       Name VARCHAR(255),
       ZoneName VARCHAR(50),
       Title VARCHAR(255),
       SortOrder INTEGER not null,
       Visible TINYINT(1) not null,
       SavedBy VARCHAR(50),
       State INTEGER,
       AncestralTrail VARCHAR(100),
       VersionIndex INTEGER,
       AlteredPermissions INTEGER,
       VersionOfID INTEGER,
       ParentID INTEGER,
       primary key (ID)
    )

    create table n2DetailCollection (
        ID INTEGER NOT NULL AUTO_INCREMENT,
       ItemID INTEGER,
       Name VARCHAR(50) not null,
       primary key (ID)
    )

    alter table n2AllowedRole 
        add index (ItemID), 
        add constraint FKB30F0676AB29607 
        foreign key (ItemID) 
        references n2Item (ID)

    alter table n2Detail 
        add index (ItemID), 
        add constraint FK1D14F83B6AB29607 
        foreign key (ItemID) 
        references n2Item (ID)

    alter table n2Detail 
        add index (DetailCollectionID), 
        add constraint FK1D14F83B4F9855AA 
        foreign key (DetailCollectionID) 
        references n2DetailCollection (ID)

    alter table n2Detail 
        add index (LinkValue), 
        add constraint FK1D14F83B3AF5DAB0 
        foreign key (LinkValue) 
        references n2Item (ID)

    alter table n2Item 
        add index (VersionOfID), 
        add constraint FK18406FA04B1A4E60 
        foreign key (VersionOfID) 
        references n2Item (ID)

    alter table n2Item 
        add index (ParentID), 
        add constraint FK18406FA018DD5AFD 
        foreign key (ParentID) 
        references n2Item (ID)

    alter table n2DetailCollection 
        add index (ItemID), 
        add constraint FKBE85C49A6AB29607 
        foreign key (ItemID) 
        references n2Item (ID)
