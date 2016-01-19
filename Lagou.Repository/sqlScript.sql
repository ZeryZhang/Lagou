USE [HKLG]
GO

/****** Object:  Table [dbo].[Job]    Script Date: 01/16/2016 22:37:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Job](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Score] [nvarchar](100) NULL,
	[CreateTime] [datetime] NULL,
	[FormatCreateTime] [nvarchar](100) NULL,
	[PositionId] [nvarchar](100) NULL,
	[PositionName] [nvarchar](100) NULL,
	[PositionType] [nvarchar](100) NULL,
	[WorkYear] [nvarchar](100) NULL,
	[Education] [nvarchar](100) NULL,
	[JobNature] [nvarchar](100) NULL,
	[CompanyName] [nvarchar](100) NULL,
	[CompanyId] [nvarchar](100) NULL,
	[City] [nvarchar](100) NULL,
	[CompanyLogo] [nvarchar](100) NULL,
	[IndustryField] [nvarchar](100) NULL,
	[PositionAdvantag] [nvarchar](100) NULL,
	[Salary] [nvarchar](100) NULL,
	[PositionFirstType] [nvarchar](100) NULL,
	[LeaderName] [nvarchar](100) NULL,
	[CompanySize] [nvarchar](100) NULL,
	[FinanceStage] [nvarchar](100) NULL,
 CONSTRAINT [PK_Job] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


