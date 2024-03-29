USE [master]
GO
/****** Object:  Database [CCA]    Script Date: 2/27/2023 11:33:39 AM ******/
CREATE DATABASE [CCA]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CCA', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\CCA.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'CCA_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\CCA_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [CCA] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CCA].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CCA] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CCA] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CCA] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CCA] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CCA] SET ARITHABORT OFF 
GO
ALTER DATABASE [CCA] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CCA] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CCA] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CCA] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CCA] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CCA] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CCA] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CCA] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CCA] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CCA] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CCA] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CCA] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CCA] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CCA] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CCA] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CCA] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CCA] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CCA] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [CCA] SET  MULTI_USER 
GO
ALTER DATABASE [CCA] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CCA] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CCA] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CCA] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CCA] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [CCA] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [CCA] SET QUERY_STORE = OFF
GO
USE [CCA]
GO
/****** Object:  Table [dbo].[Details]    Script Date: 2/27/2023 11:33:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Details](
	[Sample_ID] [varchar](10) NULL,
	[Primary_Display] [varchar](512) NULL,
	[Division_List] [varchar](512) NULL,
	[Supplier_Name] [varchar](512) NULL,
	[Child_Supplier] [varchar](512) NULL,
	[Taxonomy] [varchar](512) NULL,
	[Supplier_Product_Name] [varchar](512) NULL,
	[Merchandised_Product_ID] [varchar](512) NULL,
	[Merch_Prod_Start_Date] [varchar](512) NULL,
	[Division_Product_Name] [varchar](512) NULL,
	[Web_Product_Name] [varchar](512) NULL,
	[Division_Collection] [varchar](512) NULL,
	[Division_Rating] [varchar](512) NULL,
	[Product_Type] [varchar](512) NULL,
	[Product_Class] [varchar](512) NULL,
	[Is_Web_Product] [varchar](512) NULL,
	[Sample_Box_Enabled] [varchar](512) NULL,
	[Number_of_Colors] [varchar](512) NULL,
	[Made_In] [varchar](512) NULL,
	[Appearance] [varchar](512) NULL,
	[Backing] [varchar](512) NULL,
	[Edge_Profile] [varchar](512) NULL,
	[End_Profile] [varchar](512) NULL,
	[FHA_Class] [varchar](512) NULL,
	[FHA_Lab] [varchar](512) NULL,
	[FHA_Type] [varchar](512) NULL,
	[Finish] [varchar](512) NULL,
	[Glazed_Hardness] [varchar](512) NULL,
	[Grade] [varchar](512) NULL,
	[Is_FHA_Certified] [varchar](512) NULL,
	[Is_Recommended_Outdoors] [varchar](512) NULL,
	[Is_Wall_Tile] [varchar](512) NULL,
	[Locking_Type] [varchar](512) NULL,
	[Radiant_Heat] [varchar](512) NULL,
	[Shade_Variation] [varchar](512) NULL,
	[Stain_Treatment] [varchar](512) NULL,
	[Wear_Layer] [varchar](512) NULL,
	[Wear_Layer_Type] [varchar](512) NULL,
	[Construction] [varchar](512) NULL,
	[Gloss_Level] [varchar](512) NULL,
	[Hardness_Rating] [varchar](512) NULL,
	[Installation_Method] [varchar](512) NULL,
	[Match] [varchar](512) NULL,
	[Match_Length] [varchar](512) NULL,
	[Match_Width] [varchar](512) NULL,
	[Species] [varchar](512) NULL,
	[Merchandise_Brand] [varchar](512) NULL,
	[Commercial_Rating] [varchar](512) NULL,
	[Is_Green_Rated] [varchar](512) NULL,
	[Green_Natural_Sustained] [varchar](512) NULL,
	[Green_Recyclable_Content] [varchar](512) NULL,
	[Green_Recycled_Content] [varchar](512) NULL,
	[Size_Name] [varchar](512) NULL,
	[Length] [varchar](512) NULL,
	[Length_Measurement] [varchar](512) NULL,
	[Width] [varchar](512) NULL,
	[Width_Measurement] [varchar](512) NULL,
	[Thickness] [varchar](512) NULL,
	[Thickness_Measurement] [varchar](512) NULL,
	[Thickness_Fraction] [varchar](512) NULL,
	[Manufacturer_Product_Color_ID] [varchar](512) NULL,
	[Mfg_Color_Name] [varchar](512) NULL,
	[Mfg_Color_Number] [varchar](512) NULL,
	[Sample_Box] [varchar](512) NULL,
	[Sample_Box_Availability] [varchar](512) NULL,
	[Manufacturer_SKU_Number] [varchar](512) NULL,
	[Merchandised_Product_Color_ID] [varchar](512) NULL,
	[Merch_Color_Start_Date] [varchar](512) NULL,
	[Merch_Color_Name] [varchar](512) NULL,
	[Merch_Color_Number] [varchar](512) NULL,
	[Merchandised_SKU_Number] [varchar](512) NULL,
	[Barcode] [varchar](512) NULL,
	[CCASKUID] [varchar](512) NULL,
	[Size_UC] [varchar](512) NULL,
	[Roomscene] [varchar](512) NULL,
	[Style_Name_and_Color_Combo] [varchar](512) NULL,
	[Plate] [varchar](512) NULL,
	[Back_Label_Plate] [varchar](512) NULL,
	[Face_Label_Plate] [varchar](512) NULL,
	[Art_Type] [varchar](512) NULL,
	[Art_Type_BL] [varchar](512) NULL,
	[Art_Type_FL] [varchar](512) NULL,
	[Status] [varchar](512) NULL,
	[Status_FL] [varchar](512) NULL,
	[Change] [varchar](512) NULL,
	[Change_FL] [varchar](512) NULL,
	[Program] [varchar](512) NULL,
	[Output] [int] NULL,
	[Output_FL] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Labels]    Script Date: 2/27/2023 11:33:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Labels](
	[Merchandised_Product_ID] [varchar](300) NULL,
	[Sample_ID] [varchar](300) NULL,
	[Division_Label_Type] [varchar](300) NULL,
	[Division_Label_Name] [varchar](300) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sample]    Script Date: 2/27/2023 11:33:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sample](
	[Sample_ID] [nvarchar](255) NULL,
	[Sample_Name] [nvarchar](255) NULL,
	[Sample_Size] [nvarchar](255) NULL,
	[Sample_Type] [nvarchar](255) NULL,
	[Sampled_Color_SKU] [nvarchar](255) NULL,
	[Shared_Card] [nvarchar](255) NULL,
	[Sampled_With_Merch_Product_ID] [nvarchar](255) NULL,
	[Quick_Ship] [nvarchar](255) NULL,
	[Binder] [nvarchar](255) NULL,
	[Border] [nvarchar](255) NULL,
	[Character_Rating_by_Color] [nvarchar](255) NULL,
	[Feeler] [nvarchar](255) NULL,
	[MSRP] [nvarchar](255) NULL,
	[MSRP_Canada] [nvarchar](255) NULL,
	[Our_Price] [nvarchar](255) NULL,
	[Our_Price_Canada] [nvarchar](255) NULL,
	[RRP_US] [nvarchar](255) NULL,
	[Sampling_Color_Description] [nvarchar](255) NULL,
	[Split_Board] [nvarchar](255) NULL,
	[Trade_Up] [nvarchar](255) NULL,
	[Wood_Imaging] [nvarchar](255) NULL,
	[Sample_Note] [nvarchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Warranties]    Script Date: 2/27/2023 11:33:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Warranties](
	[Merchandised_Product_ID] [nvarchar](255) NULL,
	[Sample_ID] [nvarchar](255) NULL,
	[Provider] [nvarchar](255) NULL,
	[Duration] [nvarchar](255) NULL,
	[Warranty_Period] [nvarchar](255) NULL,
	[Product_Warranty_Type_Code] [nvarchar](255) NULL
) ON [PRIMARY]
GO
USE [master]
GO
ALTER DATABASE [CCA] SET  READ_WRITE 
GO
