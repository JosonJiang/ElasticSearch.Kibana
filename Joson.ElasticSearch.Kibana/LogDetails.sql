CREATE TABLE [dbo].[LogDetails] (

[LogID] int NOT NULL IDENTITY(1,1) ,
[LogDate] datetime NOT NULL ,
[LogThread] nvarchar(100) NOT NULL ,
[LogLevel] nvarchar(200) NOT NULL ,
[LogLogger] nvarchar(500) NOT NULL ,
[LogMessage] nvarchar(3000) NOT NULL ,
[LogActionClick] nvarchar(4000) NULL ,
[UserName] nvarchar(30) NULL ,
[UserIP] varchar(20) NULL 

)



CREATE TABLE [dbo].[Log4]
 (
    [Id] [int] IDENTITY (1, 1) NOT NULL,
    [Date] [datetime] NOT NULL,
    [Thread] [varchar] (255) NOT NULL,
    [Level] [varchar] (50) NOT NULL,
    [Logger] [varchar] (255) NOT NULL,
    [Message] [varchar] (4000) NOT NULL,
    [Exception] [varchar] (2000) NULL
)


CREATE TABLE [dbo].[NLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Application] [nvarchar](2550) NOT NULL,
	[Level] [nvarchar](500) NOT NULL,
	[TraceID] [nvarchar](500) NOT NULL,
	[EventID] [nvarchar](500) NOT NULL,
	[User] [nvarchar](500) NOT NULL,
	[Operatingtime] [datetime] NOT NULL,
	[Operatingaddress] [nvarchar](255) NOT NULL,
	[Category] [nvarchar](255) NOT NULL,
	[Logger] [nvarchar](255) NOT NULL,
	[Callsite] [nvarchar](255) NOT NULL,
	[Requesturl] [nvarchar](255) NOT NULL,
	[Referrerurl] [nvarchar](255) NOT NULL,
	[Action] [nvarchar](255) NOT NULL,
	[Properties] [varchar](255) NOT NULL,
	[Message] [nvarchar](4000) NOT NULL,
	[Exception] [nvarchar](4000) NULL,
	[Stacktrace] [nvarchar](max) NULL,
	[Amount] [nvarchar](255) NULL,
	[ClientIP] [nvarchar](50) NOT NULL,
	[CreateTime] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


 
 Create Procedure [dbo].[SP_CreateNLog](
    @application nvarchar(450),
    @level nvarchar(8),
    @traceId nvarchar(32),
    @eventId int,
    @user nvarchar(450),
	@operatingtime datetime,
	@operatingaddress nvarchar(450),
    @category nvarchar(450),
    @message nvarchar(max),
    @logger nvarchar(max),
	@callSite nvarchar(max),
	@requesturl nvarchar(max),
	@referrerurl nvarchar(max),
	@action nvarchar(max),
    @properties nvarchar(max),
    @exception nvarchar(max),
    @stacktrace nvarchar(max),
    @amount nvarchar(max),
    @clientIP nvarchar(450),
    @addTime datetime
)
as
begin


insert into NLog ([Application],[Level],TraceID,EventID,[User],Operatingtime,Operatingaddress,Category,Logger,Callsite,Requesturl,Referrerurl,[Action],[Message],Exception,[Stacktrace],Amount,Properties,ClientIP,CreateTime)
values (@application,@traceId,@level,@eventId,@user,@operatingtime,@operatingaddress,@category,@logger,@callSite,@requesturl,@referrerurl,@action,@message,@exception,@stacktrace,@amount,@properties,@clientIP,@addTime);

end









create table Log4NetNotNullable (
	[Date] datetime not null,
	[Thread] nvarchar(255) not null, 
	[Level]  nvarchar(50) not null, 
	[Logger] nvarchar(255) not null, 
	[Number] nvarchar(20) not null, 
	[Message]   nvarchar(4000) not null, 
	[Exception] nvarchar(4000) not null)
GO
create table Log4NetNullable (
	[Date] datetime null,
	[Thread] nvarchar(255) null, 
	[Level]  nvarchar(50) null, 
	[Logger] nvarchar(255) null, 
	[Number] nvarchar(20) null, 
	[Message]   nvarchar(4000) null, 
	[Exception] nvarchar(4000) null)
GO
create table Log4NetBuffering25 (
	[Date] datetime not null,
	[Thread] nvarchar(255) not null, 
	[Level]  nvarchar(50) not null, 
	[Logger] nvarchar(255) not null, 
	[Number] nvarchar(20) not null, 
	[Message]   nvarchar(4000) not null, 
	[Exception] nvarchar(4000) not null)
GO