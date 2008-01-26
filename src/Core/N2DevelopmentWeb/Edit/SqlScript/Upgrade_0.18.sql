ALTER TABLE dbo.n2Detail ADD
	BoolValue bit NULL,
	IntValue int NULL,
	DateTimeValue datetime NULL,
	StringValue varchar(MAX) NULL,
	Type varchar(50) NULL
GO
UPDATE dbo.n2Detail SET Type='Object'