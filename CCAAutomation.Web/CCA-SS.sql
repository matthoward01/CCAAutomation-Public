USE [master]
GO
/****** Object:  Database [CCA-SS]    Script Date: 2/27/2023 11:34:12 AM ******/
CREATE DATABASE [CCA-SS]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CCA', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\CCA-SS.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'CCA_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\CCA-SS_log.ldf' , SIZE = 139264KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [CCA-SS] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CCA-SS].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CCA-SS] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CCA-SS] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CCA-SS] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CCA-SS] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CCA-SS] SET ARITHABORT OFF 
GO
ALTER DATABASE [CCA-SS] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CCA-SS] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CCA-SS] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CCA-SS] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CCA-SS] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CCA-SS] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CCA-SS] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CCA-SS] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CCA-SS] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CCA-SS] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CCA-SS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CCA-SS] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CCA-SS] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CCA-SS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CCA-SS] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CCA-SS] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CCA-SS] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CCA-SS] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [CCA-SS] SET  MULTI_USER 
GO
ALTER DATABASE [CCA-SS] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CCA-SS] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CCA-SS] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CCA-SS] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CCA-SS] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [CCA-SS] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [CCA-SS] SET QUERY_STORE = OFF
GO
USE [CCA-SS]
GO
/****** Object:  Table [dbo].[Details]    Script Date: 2/27/2023 11:34:12 AM ******/
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
	[Division_Collection] [varchar](512) NULL,
	[Division_Rating] [varchar](512) NULL,
	[Product_Type] [varchar](512) NULL,
	[Product_Class] [varchar](512) NULL,
	[Is_Web_Product] [varchar](512) NULL,
	[Sample_Box_Enabled] [varchar](512) NULL,
	[Number_of_Colors] [varchar](512) NULL,
	[Made_In] [varchar](512) NULL,
	[Fiber_Company] [varchar](512) NULL,
	[Fiber_Brand] [varchar](512) NULL,
	[Merchandise_Brand] [varchar](512) NULL,
	[Primary_Fiber] [varchar](512) NULL,
	[Primary_Fiber_Percentage] [varchar](512) NULL,
	[Second_Fiber] [varchar](512) NULL,
	[Second_Fiber_Percentage] [varchar](512) NULL,
	[Third_Fiber] [varchar](512) NULL,
	[Third_Fiber_Percentage] [varchar](512) NULL,
	[Percent_BCF] [varchar](512) NULL,
	[Percent_Spun] [varchar](512) NULL,
	[Pile_Line] [varchar](512) NULL,
	[Stain_Treatment] [varchar](512) NULL,
	[Soil_Treatment] [varchar](512) NULL,
	[Dye_Method] [varchar](512) NULL,
	[Face_Weight] [varchar](512) NULL,
	[Yarn_Twist] [varchar](512) NULL,
	[Match] [varchar](512) NULL,
	[Match_Length] [varchar](512) NULL,
	[Match_Width] [varchar](512) NULL,
	[Total_Weight] [varchar](512) NULL,
	[Density] [varchar](512) NULL,
	[Gauge] [varchar](512) NULL,
	[Pile_Height] [varchar](512) NULL,
	[Stitches] [varchar](512) NULL,
	[Backing] [varchar](512) NULL,
	[IAQ_Number] [varchar](512) NULL,
	[Is_FHA_Certified] [varchar](512) NULL,
	[FHA_Type] [varchar](512) NULL,
	[FHA_Class] [varchar](512) NULL,
	[FHA_Lab] [varchar](512) NULL,
	[Durability_Rating] [varchar](512) NULL,
	[Flammability] [varchar](512) NULL,
	[Static_AATCC134] [varchar](512) NULL,
	[NBS_Smoke_Density_ASTME662] [varchar](512) NULL,
	[Radiant_Panel_ASTME648] [varchar](512) NULL,
	[Installation_Pattern] [varchar](512) NULL,
	[Commercial_Rating] [varchar](512) NULL,
	[Is_Green_Rated] [varchar](512) NULL,
	[Green_Natural_Sustained] [varchar](512) NULL,
	[Green_Recyclable_Content] [varchar](512) NULL,
	[Green_Recycled_Content] [varchar](512) NULL,
	[Size_Name] [varchar](512) NULL,
	[Manufacturer_Product_Color_ID] [varchar](512) NULL,
	[Mfg_Color_Name] [varchar](512) NULL,
	[Manufacturer_Feeler] [varchar](512) NULL,
	[Mfg_Color_Number] [varchar](512) NULL,
	[Sample_Box] [varchar](512) NULL,
	[Sample_Box_Availability] [varchar](512) NULL,
	[Manufacturer_SKU_Number] [varchar](512) NULL,
	[Merchandised_Product_Color_ID] [varchar](512) NULL,
	[Merch_Color_Start_Date] [varchar](512) NULL,
	[Merch_Color_Name] [varchar](512) NULL,
	[Merch_Color_Number] [varchar](512) NULL,
	[Merchandised_SKU_Number] [varchar](512) NULL,
	[CCASKUID] [varchar](512) NULL,
	[Style_Color_Combo_From_AP] [varchar](512) NULL,
	[Art_Type] [varchar](512) NULL,
	[Art_Type_BL] [varchar](512) NULL,
	[Art_Type_FL] [varchar](512) NULL,
	[Plate] [varchar](512) NULL,
	[Back_Label_Plate] [varchar](512) NULL,
	[Face_Label_Plate] [varchar](512) NULL,
	[ADDNumber] [varchar](512) NULL,
	[Divison_Style_Name_and_Color] [varchar](512) NULL,
	[Color_Sequence] [varchar](512) NULL,
	[Layout] [varchar](512) NULL,
	[Status] [varchar](512) NULL,
	[Status_FL] [varchar](512) NULL,
	[Change] [varchar](512) NULL,
	[Change_FL] [varchar](512) NULL,
	[Program] [varchar](512) NULL,
	[Output] [int] NULL,
	[Output_FL] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ClusteredIndex-20230126-130852]    Script Date: 2/27/2023 11:34:12 AM ******/
CREATE CLUSTERED INDEX [ClusteredIndex-20230126-130852] ON [dbo].[Details]
(
	[Sample_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Labels]    Script Date: 2/27/2023 11:34:12 AM ******/
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
/****** Object:  Table [dbo].[Sample]    Script Date: 2/27/2023 11:34:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sample](
	[Sample_ID] [varchar](300) NULL,
	[Sample_Name] [varchar](300) NULL,
	[Sample_Size] [varchar](300) NULL,
	[Sample_Type] [varchar](300) NULL,
	[Sampled_Color_SKU] [varchar](300) NULL,
	[Shared_Card] [varchar](300) NULL,
	[Multiple_Color_Lines] [varchar](300) NULL,
	[Sampled_With_Merch_Product_ID] [varchar](300) NULL,
	[Quick_Ship] [varchar](300) NULL,
	[Binder] [varchar](300) NULL,
	[Border] [varchar](300) NULL,
	[Character_Rating_by_Color] [varchar](300) NULL,
	[Feeler] [varchar](300) NULL,
	[MSRP] [varchar](300) NULL,
	[MSRP_Canada] [varchar](300) NULL,
	[Our_Price] [varchar](300) NULL,
	[Our_Price_Canada] [varchar](300) NULL,
	[RRP_US] [varchar](300) NULL,
	[Sampling_Color_Description] [varchar](300) NULL,
	[Split_Board] [varchar](300) NULL,
	[Trade_Up] [varchar](300) NULL,
	[Wood_Imaging] [varchar](300) NULL,
	[Sample_Note] [varchar](300) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Warranties]    Script Date: 2/27/2023 11:34:12 AM ******/
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
ALTER DATABASE [CCA-SS] SET  READ_WRITE 
GO
