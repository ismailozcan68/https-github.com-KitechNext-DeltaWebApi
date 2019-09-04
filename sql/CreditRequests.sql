USE [NODEDB]
GO

/****** Object:  Table [dbo].[CreditRequests]    Script Date: 3.09.2019 10:05:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CreditRequests](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestId] [uniqueidentifier] NOT NULL,
	[RequestDate] [datetime] NOT NULL,
	[CompanyCode] [varchar](50) NULL,
	[UserCode] [varchar](51) NULL,
	[WorkingYear] [smallint] NULL,
	[Request] [varchar](max) NULL,
	[HesapNo] [varchar](50) NULL,
	[MaasAdet] [int] NOT NULL,
	[KrediTutar] [decimal](18, 2) NOT NULL,
	[Status] [varchar](50) NULL,
	[StatusDescription] [varchar](250) NULL,
	[ResponseDate] [datetime] NULL,
	[TryCount] [smallint] NULL,
 CONSTRAINT [PK_CreditRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 60) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[CreditRequests] ADD  CONSTRAINT [DF_CreditRequests_Date]  DEFAULT (getdate()) FOR [RequestDate]
GO


