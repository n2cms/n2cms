-- N2 CMS CREATE SCRIPTS FOR SQL SERVER 2005

-- n2Item
CREATE TABLE [n2Item](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
	[ZoneName] [varchar](50) NULL,
	[ParentID] [int] NULL,
	[Type] [varchar](255) NULL,
	[Title] [varchar](255) NULL,
	[SortOrder] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[Published] [datetime] NULL,
	[Expires] [datetime] NULL,
	[Visible] [bit] NULL,
	[VersionOfID] [int] NULL,
	[SavedBy] [varchar](50) NULL,
 CONSTRAINT [PK_ContentItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

-- n2Detail
CREATE TABLE [n2Detail](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[DetailCollectionID] [int] NULL,
	[Name] [varchar](50) NOT NULL,
	[Type] [varchar](50) NULL,
	[BoolValue] [bit] NULL,
	[IntValue] [int] NULL,
	[DateTimeValue] [datetime] NULL,
	[DoubleValue] [float] NULL,
	[StringValue] [varchar](max) NULL,
	[LinkValue] [int] NULL,
	[Value] [varbinary](max) NULL,
 CONSTRAINT [PK_ContentData] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

-- n2AllowedRole
CREATE TABLE [n2AllowedRole](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[Role] [varchar](50) NOT NULL,
 CONSTRAINT [PK_n2AllowedRole] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [n2DetailCollection]
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	CONSTRAINT [PK_DetailCollection] PRIMARY KEY CLUSTERED ( [ID] )
)

GO

-- FK n2DetailCollection n2Item
ALTER TABLE [n2DetailCollection]  WITH CHECK ADD  CONSTRAINT [FK_n2DetailCollection_n2Item] FOREIGN KEY([ItemID])
REFERENCES [n2Item] ([ID])

GO

-- FK n2Item Parent
ALTER TABLE [n2Item]  WITH CHECK ADD  CONSTRAINT [FK_n2Item_Parent] FOREIGN KEY([ParentID])
REFERENCES [n2Item] ([ID])

GO

-- FK n2Item VersionOf
ALTER TABLE [n2Item]  WITH CHECK ADD  CONSTRAINT [FK_n2Item_VersionOf] FOREIGN KEY([VersionOfID])
REFERENCES [n2Item] ([ID])

GO

-- FK n2Detail n2Item
ALTER TABLE [n2Detail]  WITH CHECK ADD  CONSTRAINT [FK_n2Detail_n2Item] FOREIGN KEY([ItemID])
REFERENCES [n2Item] ([ID])

GO

-- FK n2Detail n2DetailCollection
ALTER TABLE [n2Detail]  WITH CHECK ADD  CONSTRAINT [FK_n2Detail_n2DetailCollection] FOREIGN KEY([DetailCollectionID])
REFERENCES [n2DetailCollection] ([ID])

GO

-- FK n2AllowedRole n2Item
ALTER TABLE [n2AllowedRole]  WITH CHECK ADD  CONSTRAINT [FK_n2AllowedRole_n2Item] FOREIGN KEY([ItemID])
REFERENCES [n2Item] ([ID])
