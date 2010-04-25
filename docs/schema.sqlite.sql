
    drop table if exists n2AllowedRole

    drop table if exists n2Detail

    drop table if exists n2Item

    drop table if exists n2DetailCollection

    create table n2AllowedRole (
        ID  integer,
       ItemID INTEGER not null,
       Role TEXT not null,
       primary key (ID)
    )

    create table n2Detail (
        ID  integer,
       Type TEXT not null,
       ItemID INTEGER not null,
       DetailCollectionID INTEGER,
       Name TEXT,
       BoolValue INTEGER,
       IntValue INTEGER,
       LinkValue INTEGER,
       DoubleValue NUMERIC,
       DateTimeValue DATETIME,
       StringValue TEXT,
       Value BLOB,
       primary key (ID)
    )

    create table n2Item (
        ID  integer,
       Type TEXT not null,
       Created DATETIME not null,
       Published DATETIME,
       Updated DATETIME not null,
       Expires DATETIME,
       Name TEXT,
       ZoneName TEXT,
       Title TEXT,
       SortOrder INTEGER not null,
       Visible INTEGER not null,
       SavedBy TEXT,
       State INTEGER,
       AncestralTrail TEXT,
       VersionIndex INTEGER,
       AlteredPermissions INTEGER,
       VersionOfID INTEGER,
       ParentID INTEGER,
       primary key (ID)
    )

    create table n2DetailCollection (
        ID  integer,
       ItemID INTEGER,
       Name TEXT not null,
       primary key (ID)
    )
