CREATE TABLE [dbo].[Log4Net](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Thread] [nvarchar](250) NOT NULL,
	[Level] [nvarchar](250) NOT NULL,
	[Logger] [nvarchar](250) NOT NULL,
	[Application] [nvarchar](255) NULL,
	[TraceID] [nvarchar](500) NULL,
	[EventID] [nvarchar](500) NULL,
	[User] [nvarchar](500) NULL,
	[Category] [nvarchar](255) NULL,
	[Callsite] [nvarchar](255) NULL,
	[Requesturl] [nvarchar](255) NULL,
	[Referrerurl] [nvarchar](255) NULL,
	[Method] [nvarchar](15) NULL,
	[Action] [nvarchar](255) NULL,
	[Properties] [nvarchar](50) NULL,
	[Parameters] [varchar](4000) NULL,
	[CustomData] [nvarchar](max) NULL,
	[ExecutionDuration] [nvarchar](50) NULL,
	[Message] [nvarchar](max) NULL,
	[Exception] [nvarchar](2000) NULL,
	[Stacktrace] [nvarchar](max) NULL,
	[Amount] [nvarchar](255) NULL,
	[ClientIP] [nvarchar](150) NULL,
	[ClientName] [nvarchar](150) NULL,
	[BrowserInfo] [nvarchar](150) NULL,
	[OperatingAddress] [nvarchar](255) NULL,
	[OperatingTime] [datetime] NULL,
	[OS] [nvarchar](50) NULL,
	[CreateTime] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Log4Net] ADD  CONSTRAINT [DF_Log4_Operatingtime]  DEFAULT (getdate()) FOR [OperatingTime]
GO

ALTER TABLE [dbo].[Log4Net] ADD  CONSTRAINT [DF_Log4_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO