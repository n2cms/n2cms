ALTER TABLE dbo.n2Item ADD
	VersionOfID int NULL,
	SavedBy varchar(50) NULL
GO
CREATE TABLE [dbo].[n2AllowedRole](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[Role] [varchar](50) NOT NULL,
 CONSTRAINT [PK_n2AllowedRole] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[n2AllowedRole]  WITH CHECK ADD  CONSTRAINT [FK_n2AllowedRole_n2Item] FOREIGN KEY([ItemID])
REFERENCES [dbo].[n2Item] ([ID])
