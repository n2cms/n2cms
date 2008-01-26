CREATE TABLE [n2DetailCollection]
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	CONSTRAINT [PK_DetailCollection] PRIMARY KEY CLUSTERED ( [ID] )
)

GO

ALTER TABLE dbo.n2Detail ADD
	DetailCollectionID int NULL,
	LinkValue int NULL

GO

-- FK n2DetailCollection n2Item
ALTER TABLE [n2DetailCollection]  WITH CHECK 
	ADD CONSTRAINT [FK_n2DetailCollection_n2Item] 
		FOREIGN KEY([ItemID])
		REFERENCES [n2Item] ([ID])

GO

-- FK n2Detail n2DetailCollection
ALTER TABLE [n2Detail]  WITH CHECK 
	ADD CONSTRAINT [FK_n2Detail_n2DetailCollection] 
		FOREIGN KEY([DetailCollectionID])
		REFERENCES [n2DetailCollection] ([ID])

GO

-- FK n2Detail n2Item link
ALTER TABLE [n2Detail] WITH CHECK 
	ADD CONSTRAINT [FK_n2Detail_n2Item_link] 
		FOREIGN KEY ([LinkValue])
		REFERENCES [n2Item] ([ID])
