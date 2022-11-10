using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CCAAutomation.Lib
{
    public class SqlMethods
    {
        private static SqlConnection connection;
        private static string server;
        private static string database;
        private static string uid;
        private static string password;

        private static void SqlConnect(string db)
        {
            server = "xxxxxx";
            database = db;
            uid = "xxxxxx";
            password = "xxxxxx";

            string connectionString =
                "Data Source = " + server + ";" +
                "Initial Catalog = " + database + ";" +
                "User ID = " + uid + ";" +
                "Password = " + password + ";";
            connection = new SqlConnection(connectionString);
        }        
        public static bool SqlApprovalCheck(string plate_ID)
        {
            bool approved = false;

            SqlCommand command;
            SqlDataReader dataReader;

            string sql = "SELECT Status FROM dbo.Details WHERE Plate_# = '" + plate_ID + "'";

            try
            {
                SqlConnect("CCA");
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    if ((dataReader.GetString(dataReader.GetOrdinal("Plate_#")).EqualsString("approved")))
                    {
                        approved = true;
                    }
                }

                dataReader.Close();
                command.Dispose();
                connection.Close();

                SqlConnect("CCA-SS");
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    if ((dataReader.GetString(dataReader.GetOrdinal("Plate_#")).EqualsString("approved")))
                    {
                        approved = true;
                    }
                }

                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return approved;
        }

        public static LarModels.LARFinal SqlGetLarFinal(bool isSoftSurface, string plateId)
        {
            LarModels.LARFinal lfSql = new();

            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
            }
            else
            {
                SqlConnect("CCA");
            }

            SqlCommand command;
            SqlDataReader dataReader;

            string sql = "SELECT * FROM dbo.Details WHERE Plate_# = '" + plateId + "'";

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    lfSql.DetailsFinal.Plate_ID = (dataReader.GetString(dataReader.GetOrdinal("Plate_#")));
                }

                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            return lfSql;
        }

        public static List<string> GetRunJobs(bool isSoftSurface)
        {
            List<string> jobList = new();

            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
            }
            else
            {
                SqlConnect("CCA");
            }

            SqlCommand command;
            SqlDataReader dataReader;

            string sql = "SELECT DISTINCT Plate_# FROM dbo.Details WHERE (Output='1' AND Plate_# != '')";

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    jobList.Add(dataReader.GetString(dataReader.GetOrdinal("Plate_#")));
                }

                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return jobList;
        }
        public static int GetOutputStatus(string plateId, bool isSoftSurface)
        {
            int outputStatus = 0;
            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
            }
            else
            {
                SqlConnect("CCA");
            }

            SqlCommand command;
            SqlDataReader dataReader;

            string sql = "SELECT DISTINCT Output FROM dbo.Details WHERE Plate_# = '" + plateId + "')";

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    outputStatus = dataReader.GetInt32(dataReader.GetOrdinal("Output"));
                }

                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return outputStatus;
        }

        public static List<string> SqlSelectSwatchColors(string sampleId, bool isSoftSurface)
        {
            List<string> colorList = new();
            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
            }
            else
            {
                SqlConnect("CCA");
            }

            SqlCommand command;
            SqlDataReader dataReader;

            string sql = "select distinct dbo.Details.Merch_Color_Name, dbo.Details.Manufacturer_Feeler, Feeler from dbo.Sample inner join dbo.details on dbo.details.Sample_ID=dbo.sample.Sample_ID where dbo.Sample.Sample_ID='" + sampleId + "'";

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    if (dataReader.GetString(dataReader.GetOrdinal("Manufacturer_Feeler")).EqualsString("yes") || dataReader.GetString(dataReader.GetOrdinal("Merch_Color_Name")).EqualsString(dataReader.GetString(dataReader.GetOrdinal("Feeler"))))
                    {
                        colorList.Add("<b>" + dataReader.GetString(dataReader.GetOrdinal("Merch_Color_Name")) + "</b>");
                    }
                    else
                    {
                        colorList.Add(dataReader.GetString(dataReader.GetOrdinal("Merch_Color_Name")));
                    }
                }

                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return colorList;
        }        

        public static List<LarModels.WebTableItem> SqlSelectWebTableItem(bool isSoftSurface, int offset)
        {
            List<LarModels.WebTableItem> webTableItems = new();
            string sql;

            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
                sql = "SELECT DISTINCT dbo.Details.Plate_#, dbo.Details.Sample_ID, dbo.Details.Status, dbo.Details.Art_Type, Sample_Name, Shared_Card, Multiple_Color_Lines, dbo.Details.Change, dbo.Details.Output FROM dbo.Sample INNER JOIN dbo.Details ON dbo.Details.Sample_ID=dbo.Sample.Sample_ID";
            }
            else
            {
                SqlConnect("CCA");
                sql = "SELECT DISTINCT dbo.Details.Plate_#, dbo.Details.Sample_ID, dbo.Details.Status, dbo.Details.Art_Type, Sample_Name, dbo.Details.Change, dbo.Details.Output, dbo.Details.Size_Name, dbo.Details.Width, dbo.Details.Width_Measurement, dbo.Details.Length, dbo.Details.Length_Measurement FROM dbo.Sample INNER JOIN dbo.Details ON dbo.Details.Sample_ID=dbo.Sample.Sample_ID";
            }

            SqlCommand command;
            SqlDataReader dataReader;

            //string sql = "SELECT DISTINCT dbo.Details.Plate_#, dbo.Details.Sample_ID, dbo.Details.Status, dbo.Details.Art_Type, Sample_Name, Shared_Card, Multiple_Color_Lines, dbo.Details.Change FROM dbo.Sample INNER JOIN dbo.Details ON dbo.Details.Sample_ID=dbo.Sample.Sample_ID ORDER BY dbo.Details.Plate_# OFFSET " + offset + " ROWS FETCH NEXT 500 ROWS ONLY";

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    LarModels.WebTableItem webTableItem = new();
                    webTableItem.SearchTags += webTableItem.Plate_ID = dataReader.GetString(dataReader.GetOrdinal("Plate_#"));
                    webTableItem.SearchTags += webTableItem.Sample_ID = dataReader.GetString(dataReader.GetOrdinal("Sample_ID"));
                    webTableItem.SearchTags += webTableItem.Status = dataReader.GetString(dataReader.GetOrdinal("Status"));
                    webTableItem.SearchTags += webTableItem.Art_Type = dataReader.GetString(dataReader.GetOrdinal("Art_Type"));
                    webTableItem.SearchTags += webTableItem.Change = dataReader.GetString(dataReader.GetOrdinal("Change"));
                    webTableItem.SearchTags += webTableItem.Style = dataReader.GetString(dataReader.GetOrdinal("Sample_Name"));
                    webTableItem.Output = dataReader.GetInt32(dataReader.GetOrdinal("Output"));
                    if (isSoftSurface)
                    {
                        if (dataReader.GetString(dataReader.GetOrdinal("Shared_Card")).EqualsString("yes"))
                        {
                            webTableItem.SearchTags += webTableItem.Shared_Card = "Shared Card";
                        }
                        else
                        {
                            webTableItem.SearchTags += webTableItem.Shared_Card = "";
                        }
                        if (dataReader.GetString(dataReader.GetOrdinal("Multiple_Color_Lines")).EqualsString("yes"))
                        {
                            webTableItem.SearchTags += webTableItem.Multiple_Color_Lines = "Multiple Color Lines";
                        }
                        else
                        {
                            webTableItem.SearchTags += webTableItem.Multiple_Color_Lines = "";
                        }
                    }
                    else
                    {
                        webTableItem.Size_Name = dataReader.GetString(dataReader.GetOrdinal("Size_Name"));
                        webTableItem.Width = dataReader.GetString(dataReader.GetOrdinal("Width"));
                        webTableItem.Width_Measurement = dataReader.GetString(dataReader.GetOrdinal("Width_Measurement"));
                        webTableItem.Length = dataReader.GetString(dataReader.GetOrdinal("Length"));
                        webTableItem.Length_Measurement = dataReader.GetString(dataReader.GetOrdinal("Length_Measurement"));
                    }

                    webTableItems.Add(webTableItem);
                    webTableItem = new();
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return webTableItems;
        }

        public static LarModels.WebTableItem SqlSelectWebTableItem(string plateId, bool isSoftSurface)
        {
            LarModels.WebTableItem webTableItem = new();

            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
            }
            else
            {
                SqlConnect("CCA");
            }

            SqlCommand command;
            SqlDataReader dataReader;

            string sql = "SELECT TOP 1 Plate_#, Sample_ID, Status, Art_Type, Change, Output FROM dbo.Details WHERE Plate_#='" + plateId + "'";

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    webTableItem.Plate_ID = dataReader.GetString(dataReader.GetOrdinal("Plate_#"));
                    webTableItem.Sample_ID = dataReader.GetString(dataReader.GetOrdinal("Sample_ID"));
                    webTableItem.Status = dataReader.GetString(dataReader.GetOrdinal("Status"));
                    webTableItem.Art_Type = dataReader.GetString(dataReader.GetOrdinal("Art_Type"));
                    webTableItem.Change = dataReader.GetString(dataReader.GetOrdinal("Change"));
                    webTableItem.Output = dataReader.GetInt32(dataReader.GetOrdinal("Output"));
                }
                dataReader.Close();
                command.Dispose();

                sql = "SELECT Sample_Name, Shared_Card, Multiple_Color_Lines FROM dbo.Sample WHERE \"Sample_ID\"='" + webTableItem.Sample_ID + "'";
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    webTableItem.Style = dataReader.GetString(dataReader.GetOrdinal("Sample_Name"));
                    if (dataReader.GetString(dataReader.GetOrdinal("Shared_Card")).EqualsString("yes"))
                    {
                        webTableItem.Shared_Card = "Shared Card";
                    }
                    else
                    {
                        webTableItem.Shared_Card = "";
                    }
                    if (dataReader.GetString(dataReader.GetOrdinal("Multiple_Color_Lines")).EqualsString("yes"))
                    {
                        webTableItem.Multiple_Color_Lines = "Multiple Color Lines";
                    }
                    else
                    {
                        webTableItem.Multiple_Color_Lines = "";
                    }
                    
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return webTableItem;
        }

        public static List<LarModels.Details> SqlSelectDetails(string plateId, bool isSoftSurface)
        {
            List<LarModels.Details> detailsList = new();

            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
            }
            else
            {
                SqlConnect("CCA");
            }
            SqlCommand command;
            SqlDataReader dataReader;
            string sql = "SELECT * FROM dbo.Details WHERE \"Plate_#\"='" + plateId + "'";            

            if (plateId.Equals(""))
            {
                sql = "SELECT * FROM dbo.Details";

                if (isSoftSurface)
                {
                    sql = "SELECT * Plate_# FROM dbo.Details GROUP BY Plate_#";
                }
            }

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    LarModels.Details details = new();

                    details.Plate_ID = dataReader.GetString(dataReader.GetOrdinal("Plate_#")).Trim();
                    details.ArtType = dataReader.GetString(dataReader.GetOrdinal("Art_Type")).Trim();
                    details.Status = dataReader.GetString(dataReader.GetOrdinal("Status")).Trim();
                    details.Change = dataReader.GetString(dataReader.GetOrdinal("Change")).Trim();
                    details.Program = dataReader.GetString(dataReader.GetOrdinal("Program")).Trim();

                    details.Sample_ID = dataReader.GetString(dataReader.GetOrdinal("Sample_ID")).Trim();
                    details.Primary_Display = dataReader.GetString(dataReader.GetOrdinal("Primary_Display")).Trim();
                    details.Division_List = dataReader.GetString(dataReader.GetOrdinal("Division_List")).Trim();
                    details.Supplier_Name = dataReader.GetString(dataReader.GetOrdinal("Supplier_Name")).Trim();
                    //details.Child_Supplier = dataReader.GetString(dataReader.GetOrdinal("Child_Supplier")).Trim();
                    details.Taxonomy = dataReader.GetString(dataReader.GetOrdinal("Taxonomy")).Trim();
                    //details.Supplier_Product_Name = dataReader.GetString(dataReader.GetOrdinal("Supplier_Product_Name")).Trim();
                    //details.Merchandised_Product_ID = dataReader.GetString(dataReader.GetOrdinal("Merchandised_Product_ID")).Trim();
                    //details.Merch_Prod_Start_Date = dataReader.GetString(dataReader.GetOrdinal("Merch_Prod_Start_Date")).Trim();
                    details.Division_Product_Name = dataReader.GetString(dataReader.GetOrdinal("Division_Product_Name")).Trim();
                    //details.Division_Collection = dataReader.GetString(dataReader.GetOrdinal("Division_Collection")).Trim();
                    details.Division_Rating = dataReader.GetString(dataReader.GetOrdinal("Division_Rating")).Trim();
                    details.Product_Type = dataReader.GetString(dataReader.GetOrdinal("Product_Type")).Trim();
                    details.Product_Class = dataReader.GetString(dataReader.GetOrdinal("Product_Class")).Trim();
                    //details.Is_Web_Product = dataReader.GetString(dataReader.GetOrdinal("Is_Web_Product")).Trim();
                    //details.Sample_Box_Enabled = dataReader.GetString(dataReader.GetOrdinal("Sample_Box_Enabled")).Trim();
                    //details.Made_In = dataReader.GetString(dataReader.GetOrdinal("Made_In")).Trim();
                    details.Merchandise_Brand = dataReader.GetString(dataReader.GetOrdinal("Merchandise_Brand")).Trim();
                    //details.Stain_Treatment = dataReader.GetString(dataReader.GetOrdinal("Stain_Treatment")).Trim();
                    details.Match = dataReader.GetString(dataReader.GetOrdinal("Match")).Trim();
                    details.Match_Length = dataReader.GetString(dataReader.GetOrdinal("Match_Length")).Trim();
                    details.Match_Width = dataReader.GetString(dataReader.GetOrdinal("Match_Width")).Trim();
                    details.Backing = dataReader.GetString(dataReader.GetOrdinal("Backing")).Trim();
                    //details.Is_FHA_Certified = dataReader.GetString(dataReader.GetOrdinal("Is_FHA_Certified"));
                    //details.FHA_Type = dataReader.GetString(dataReader.GetOrdinal("FHA_Type")).Trim();
                    //details.FHA_Lab = dataReader.GetString(dataReader.GetOrdinal("FHA_Lab")).Trim();
                    //details.Commercial_Rating = dataReader.GetString(dataReader.GetOrdinal("Commercial_Rating")).Trim();
                    //details.Is_Green_Rated = dataReader.GetString(dataReader.GetOrdinal("Is_Green_Rated")).Trim();
                    //details.Green_Natural_Sustained = dataReader.GetString(dataReader.GetOrdinal("Green_Natural_Sustained")).Trim();
                    //details.Green_Recyclable_Content = dataReader.GetString(dataReader.GetOrdinal("Green_Recyclable_Content")).Trim();
                    //details.Green_Recycled_Content = dataReader.GetString(dataReader.GetOrdinal("Green_Recycled_Content")).Trim();
                    details.Size_Name = dataReader.GetString(dataReader.GetOrdinal("Size_Name")).Trim();
                    details.Manufacturer_Product_Color_ID = dataReader.GetString(dataReader.GetOrdinal("Manufacturer_Product_Color_ID")).Trim();
                    details.Mfg_Color_Name = dataReader.GetString(dataReader.GetOrdinal("Mfg_Color_Name")).Trim();
                    //details.Mfg_Color_Number = dataReader.GetString(dataReader.GetOrdinal("Mfg_Color_Number")).Trim();
                    //details.Sample_Box = dataReader.GetString(dataReader.GetOrdinal("Sample_Box")).Trim();
                    //details.Sample_Box_Availability = dataReader.GetString(dataReader.GetOrdinal("Sample_Box_Availability")).Trim();
                    details.Manufacturer_SKU_Number = dataReader.GetString(dataReader.GetOrdinal("Manufacturer_SKU_Number")).Trim();
                    details.Merchandised_Product_Color_ID = dataReader.GetString(dataReader.GetOrdinal("Merchandised_Product_Color_ID")).Trim();
                    //details.Merch_Color_Start_Date = dataReader.GetString(dataReader.GetOrdinal("Merch_Color_Start_Date")).Trim();
                    details.Merch_Color_Name = dataReader.GetString(dataReader.GetOrdinal("Merch_Color_Name")).Trim();
                    //details.Merch_Color_Number = dataReader.GetString(dataReader.GetOrdinal("Merch_Color_Number")).Trim();
                    details.Merchandised_SKU_Number = dataReader.GetString(dataReader.GetOrdinal("Merchandised_SKU_Number")).Trim();
                    details.CcaSkuId = dataReader.GetString(dataReader.GetOrdinal("CcaSkuId")).Trim();
                    details.Output = dataReader.GetInt32(dataReader.GetOrdinal("Output"));

                    if (!isSoftSurface)
                    {
                        details.Appearance = dataReader.GetString(dataReader.GetOrdinal("Appearance")).Trim();
                        details.Barcode = dataReader.GetString(dataReader.GetOrdinal("Barcode")).Trim();
                        //details.Construction = dataReader.GetString(dataReader.GetOrdinal("Construction")).Trim();
                        //details.Edge_Profile = dataReader.GetString(dataReader.GetOrdinal("Edge_Profile")).Trim();
                        //details.End_Profile = dataReader.GetString(dataReader.GetOrdinal("End_Profile")).Trim();
                        //details.Finish = dataReader.GetString(dataReader.GetOrdinal("Finish")).Trim();
                        //details.Glazed_Hardness = dataReader.GetString(dataReader.GetOrdinal("Glazed_Hardness")).Trim();
                        //details.Gloss_Level = dataReader.GetString(dataReader.GetOrdinal("Gloss_Level")).Trim();
                        //details.Grade = dataReader.GetString(dataReader.GetOrdinal("Grade")).Trim();
                        //details.Hardness_Rating = dataReader.GetString(dataReader.GetOrdinal("Hardness_Rating")).Trim();
                        //details.Installation_Method = dataReader.GetString(dataReader.GetOrdinal("Installation_Method")).Trim();
                        //details.Is_Recommended_Outdoors = dataReader.GetString(dataReader.GetOrdinal("Is_Recommended_Outdoors")).Trim();
                        //details.Is_Wall_Tile = dataReader.GetString(dataReader.GetOrdinal("Is_Wall_Tile")).Trim();
                        details.Length = dataReader.GetString(dataReader.GetOrdinal("Length")).Trim();
                        details.Length_Measurement = dataReader.GetString(dataReader.GetOrdinal("Length_Measurement")).Trim();
                        //details.Locking_Type = dataReader.GetString(dataReader.GetOrdinal("Locking_Type")).Trim();
                        //details.Radiant_Heat = dataReader.GetString(dataReader.GetOrdinal("Radiant_Heat")).Trim();
                        details.Roomscene = dataReader.GetString(dataReader.GetOrdinal("Roomscene")).Trim();
                        //details.Shade_Variation = dataReader.GetString(dataReader.GetOrdinal("Shade_Variation")).Trim();
                        details.Size_UC = dataReader.GetString(dataReader.GetOrdinal("Size_UC")).Trim();
                        details.Species = dataReader.GetString(dataReader.GetOrdinal("Species")).Trim();
                        details.Thickness_Fraction = dataReader.GetString(dataReader.GetOrdinal("Thickness_Fraction")).Trim();
                        details.Thickness = dataReader.GetString(dataReader.GetOrdinal("Thickness")).Trim();
                        details.Thickness_Measurement = dataReader.GetString(dataReader.GetOrdinal("Thickness_Measurement")).Trim();
                        details.Wear_Layer = dataReader.GetString(dataReader.GetOrdinal("Wear_Layer")).Trim();
                        details.Wear_Layer_Type = dataReader.GetString(dataReader.GetOrdinal("Wear_Layer_Type")).Trim();
                        details.Web_Product_Name = dataReader.GetString(dataReader.GetOrdinal("Web_Product_Name")).Trim();
                        details.Width = dataReader.GetString(dataReader.GetOrdinal("Width")).Trim();
                        details.Width_Measurement = dataReader.GetString(dataReader.GetOrdinal("Width_Measurement")).Trim();
                    }
                    if (isSoftSurface)
                    {
                        details.Number_of_Colors = dataReader.GetString(dataReader.GetOrdinal("Number_of_Colors")).Trim();
                        //details.Fiber_Company = dataReader.GetString(dataReader.GetOrdinal("Fiber_Company")).Trim();
                        //details.Fiber_Brand = dataReader.GetString(dataReader.GetOrdinal("Fiber_Brand")).Trim();
                        //details.Primary_Fiber = dataReader.GetString(dataReader.GetOrdinal("Primary_Fiber")).Trim();
                        //details.Primary_Fiber_Percentage = dataReader.GetString(dataReader.GetOrdinal("Primary_Fiber_Percentage")).Trim();
                        //details.Second_Fiber = dataReader.GetString(dataReader.GetOrdinal("Second_Fiber")).Trim();
                        //details.Second_Fiber_Percentage = dataReader.GetString(dataReader.GetOrdinal("Second_Fiber_Percentage")).Trim();
                        //details.Third_Fiber = dataReader.GetString(dataReader.GetOrdinal("Third_Fiber")).Trim();
                        //details.Third_Fiber_Percentage = dataReader.GetString(dataReader.GetOrdinal("Third_Fiber_Percentage")).Trim();
                        //details.Percent_BCF = dataReader.GetString(dataReader.GetOrdinal("Percent_BCF")).Trim();
                        //details.Percent_Spun = dataReader.GetString(dataReader.GetOrdinal("Percent_Spun")).Trim();
                        details.Pile_Line = dataReader.GetString(dataReader.GetOrdinal("Pile_Line")).Trim();
                        //details.Soil_Treatment = dataReader.GetString(dataReader.GetOrdinal("Soil_Treatment")).Trim();
                        //details.Dye_Method = dataReader.GetString(dataReader.GetOrdinal("Dye_Method")).Trim();
                        //details.Face_Weight = dataReader.GetString(dataReader.GetOrdinal("Face_Weight")).Trim();
                        //details.Yarn_Twist = dataReader.GetString(dataReader.GetOrdinal("Yarn_Twist")).Trim();
                        //details.Total_Weight = dataReader.GetString(dataReader.GetOrdinal("Total_Weight")).Trim();
                        //details.Density = dataReader.GetString(dataReader.GetOrdinal("Density")).Trim();
                        //details.Gauge = dataReader.GetString(dataReader.GetOrdinal("Gauge")).Trim();
                        //details.Pile_Height = dataReader.GetString(dataReader.GetOrdinal("Pile_Height")).Trim();
                        //details.Stitches = dataReader.GetString(dataReader.GetOrdinal("Stitches")).Trim();
                        //details.IAQ_Number = dataReader.GetString(dataReader.GetOrdinal("IAQ_Number")).Trim();
                        //details.FHA_Class = dataReader.GetString(dataReader.GetOrdinal("FHA_Class")).Trim();
                        //details.Durability_Rating = dataReader.GetString(dataReader.GetOrdinal("Durability_Rating")).Trim();
                        //details.Flammability = dataReader.GetString(dataReader.GetOrdinal("Flammability")).Trim();
                        //details.Static_AATCC134 = dataReader.GetString(dataReader.GetOrdinal("Static_AATCC134")).Trim();
                        //details.NBS_Smoke_Density_ASTME662 = dataReader.GetString(dataReader.GetOrdinal("NBS_Smoke_Density_ASTME662")).Trim();
                        //details.Radiant_Panel_ASTME648 = dataReader.GetString(dataReader.GetOrdinal("Radiant_Panel_ASTME648")).Trim();
                        //details.Installation_Pattern = dataReader.GetString(dataReader.GetOrdinal("Installation_Pattern")).Trim();
                        details.Manufacturer_Feeler = dataReader.GetString(dataReader.GetOrdinal("Manufacturer_Feeler")).Trim();
                        details.Color_Sequence = dataReader.GetString(dataReader.GetOrdinal("Color_Sequence")).Trim();
                        details.ADDNumber = dataReader.GetString(dataReader.GetOrdinal("ADDNumber")).Trim();
                        details.Roomscene = "";
                    }

                    detailsList.Add(details);
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return detailsList;
        }

        public static List<LarModels.Labels> SqlSelectLabels(string sampleId, bool isSoftSurface)
        {
            List<LarModels.Labels> labelsList = new();

            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
            }
            else
            {
                SqlConnect("CCA");
            }
            SqlCommand command;
            SqlDataReader dataReader;
            string sql = "SELECT DISTINCT(Division_Label_Name), Sample_ID, Division_Label_Type FROM dbo.Labels WHERE (Division_Label_Type = 'WBUG' OR Division_Label_Type = 'LOGO' OR Division_Label_Type = 'ICON') AND Sample_ID = '" + sampleId + "'";

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    LarModels.Labels labels = new();

                    labels.Division_Label_Name = dataReader.GetString(dataReader.GetOrdinal("Division_Label_Name"));
                    labels.Division_Label_Type = dataReader.GetString(dataReader.GetOrdinal("Division_Label_Type"));
                    labels.Merchandised_Product_ID = "";
                    //labels.Merchandised_Product_ID = dataReader.GetString(dataReader.GetOrdinal("Merchandised_Product_ID"));
                    labels.Sample_ID = dataReader.GetString(dataReader.GetOrdinal("Sample_ID"));

                    labelsList.Add(labels);
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return labelsList;
        }

        public static List<LarModels.Labels> SqlSelectLabels(string sampleId)
        {
            List<LarModels.Labels> labelsList = new();

            SqlConnect("CCA");
            SqlCommand command;
            SqlDataReader dataReader;
            string sql = "SELECT * FROM dbo.Labels WHERE Division_Label_Type = 'LOGO' OR Division_Label_Type = 'ICON'";

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    LarModels.Labels labels = new();

                    labels.Division_Label_Name = dataReader.GetString(dataReader.GetOrdinal("Division_Label_Name"));
                    labels.Division_Label_Type = dataReader.GetString(dataReader.GetOrdinal("Division_Label_Type"));
                    labels.Merchandised_Product_ID = dataReader.GetString(dataReader.GetOrdinal("Merchandised_Product_ID"));
                    labels.Sample_ID = dataReader.GetString(dataReader.GetOrdinal("Sample_ID"));

                    labelsList.Add(labels);
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return labelsList;
        }

        public static List<LarModels.Sample> SqlSelectSample(string sampleId, bool isSoftSurface)
        {
            List<LarModels.Sample> sampleList = new();

            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
            }
            else
            {
                SqlConnect("CCA");
            }
            SqlCommand command;
            SqlDataReader dataReader;
            string sql = "SELECT Sample_ID, Sample_Name, Feeler, Shared_Card FROM dbo.Sample WHERE Sample_ID = '" + sampleId + "'";
            if (isSoftSurface)
            {
                sql = "SELECT Sample_ID, Sample_Name, Feeler, Sample_Type, Shared_Card, Multiple_Color_Lines FROM dbo.Sample WHERE Sample_ID = '" + sampleId + "'";
            }
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    LarModels.Sample sample = new();

                    sample.Sample_ID = dataReader.GetString(dataReader.GetOrdinal("Sample_ID"));
                    sample.Sample_Name = dataReader.GetString(dataReader.GetOrdinal("Sample_Name"));
                    sample.Feeler = dataReader.GetString(dataReader.GetOrdinal("Feeler"));
                    sample.Shared_Card = dataReader.GetString(dataReader.GetOrdinal("Shared_Card"));
                    if (isSoftSurface)
                    {
                        sample.Multiple_Color_Lines = dataReader.GetString(dataReader.GetOrdinal("Multiple_Color_Lines"));
                        sample.Sample_Type = dataReader.GetString(dataReader.GetOrdinal("Sample_Type"));
                    }

                    sampleList.Add(sample);
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                
            }

            return sampleList;
        }

        public static string SqlUpdateStatus(string plateId, string theStatus, bool isSoftSurface)
        {
            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
            }
            else
            {
                SqlConnect("CCA");
            }
            SqlCommand command;
            SqlDataReader dataReader;
            string sql = "UPDATE dbo.Details SET Status='" + theStatus + "' WHERE \"Plate_#\"='" + plateId + "'";

            if (theStatus.Equals("Approved"))
            {
                sql = "UPDATE dbo.Details SET Status='" + theStatus + "', Change='' WHERE \"Plate_#\"='" + plateId + "'";
            }

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return theStatus;

        }

        public static string SqlUpdateChange(string plateId, string theChange, bool isSoftSurface)
        {
            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
            }
            else
            {
                SqlConnect("CCA");
            }
            SqlCommand command;
            SqlDataReader dataReader;
            string sql = "UPDATE dbo.Details SET Change='" + theChange + "' WHERE \"Plate_#\"='" + plateId + "'";

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return theChange;
        }

        public static int SqlSetToRun(string plateId, bool isSoftSurface, int run)
        {
            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
            }
            else
            {
                SqlConnect("CCA");
            }
            SqlCommand command;
            SqlDataReader dataReader;
            string sql = "UPDATE dbo.Details SET Output='" + run + "' WHERE \"Plate_#\"='" + plateId + "'";

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return run;
        }

        public static void SqlWebDBUpdate(LarModels.LARXlsSheet larModels, bool limited, bool isSoftSurface, bool resetInsite)
        {
            List<LarModels.MktSpreadsheetItem> mktSSI = SqlWebDBCleanup(larModels, isSoftSurface);

            string sql;
            if (!isSoftSurface)
            {
                foreach (LarModels.Details d in larModels.DetailsList)
                {
                    sql = "UPDATE dbo.Details " +
                        "SET " +
                        "Primary_Display = '" + d.Primary_Display + "', " +
                        "Division_List = '" + d.Division_List + "', " +
                        "Supplier_Name = '" + d.Supplier_Name.Replace("'", "''") + "', " +
                        "Child_Supplier = '" + d.Child_Supplier + "', " +
                        "Taxonomy = '" + d.Taxonomy + "', " +
                        "Supplier_Product_Name = '" + d.Supplier_Product_Name.Replace("'", "''") + "', " +
                        "Merchandised_Product_ID = '" + d.Merchandised_Product_ID + "', " +
                        "Merch_Prod_Start_Date = '" + d.Merch_Prod_Start_Date + "', " +
                        "Division_Product_Name = '" + d.Division_Product_Name.Replace("'", "''") + "', " +
                        "Web_Product_Name = '" + d.Web_Product_Name.Replace("'", "''") + "', " +
                        "Division_Collection = '" + d.Division_Collection + "', " +
                        "Division_Rating = '" + d.Division_Rating + "', " +
                        "Product_Type = '" + d.Product_Type + "', " +
                        "Product_Class = '" + d.Product_Class + "', " +
                        "Is_Web_Product = '" + d.Is_Web_Product + "', " +
                        "Sample_Box_Enabled = '" + d.Sample_Box_Enabled + "', " +
                        "Number_of_Colors = '" + d.Number_of_Colors + "', " +
                        "Made_In = '" + d.Made_In + "', " +
                        "Appearance = '" + d.Appearance + "', " +
                        "Backing = '" + d.Backing + "', " +
                        "Edge_Profile = '" + d.Edge_Profile + "', " +
                        "End_Profile = '" + d.End_Profile + "', " +
                        "FHA_Class = '" + d.FHA_Class + "', " +
                        "FHA_Lab = '" + d.FHA_Lab + "', " +
                        "FHA_Type = '" + d.FHA_Type + "', " +
                        "Finish = '" + d.Finish + "', " +
                        "Glazed_Hardness = '" + d.Glazed_Hardness + "', " +
                        "Grade = '" + d.Grade + "', " +
                        "Is_FHA_Certified = '" + d.Is_FHA_Certified + "', " +
                        "Is_Recommended_Outdoors = '" + d.Is_Recommended_Outdoors + "', " +
                        "Is_Wall_Tile = '" + d.Is_Wall_Tile + "', " +
                        "Locking_Type = '" + d.Locking_Type + "', " +
                        "Radiant_Heat = '" + d.Radiant_Heat + "', " +
                        "Shade_Variation = '" + d.Shade_Variation + "', " +
                        "Stain_Treatment = '" + d.Stain_Treatment + "', " +
                        "Wear_Layer = '" + d.Wear_Layer + "', " +
                        "Wear_Layer_Type = '" + d.Wear_Layer_Type + "', " +
                        "Construction = '" + d.Construction + "', " +
                        "Gloss_Level = '" + d.Gloss_Level + "', " +
                        "Hardness_Rating = '" + d.Hardness_Rating + "', " +
                        "Installation_Method = '" + d.Installation_Method + "', " +
                        "Match = '" + d.Match + "', " +
                        "Match_Length = '" + d.Match_Length + "', " +
                        "Match_Width = '" + d.Match_Width + "', " +
                        "Species = '" + d.Species + "', " +
                        "Merchandise_Brand = '" + d.Merchandise_Brand + "', " +
                        "Commercial_Rating = '" + d.Commercial_Rating + "', " +
                        "Is_Green_Rated = '" + d.Is_Green_Rated + "', " +
                        "Green_Natural_Sustained = '" + d.Green_Natural_Sustained + "', " +
                        "Green_Recyclable_Content = '" + d.Green_Recyclable_Content + "', " +
                        "Green_Recycled_Content = '" + d.Green_Recycled_Content + "', " +
                        "Size_Name = '" + d.Size_Name + "', " +
                        "Length = '" + d.Length + "', " +
                        "Length_Measurement = '" + d.Length_Measurement + "', " +
                        "Width = '" + d.Width + "', " +
                        "Width_Measurement = '" + d.Width_Measurement + "', " +
                        "Thickness = '" + d.Thickness + "', " +
                        "Thickness_Measurement = '" + d.Thickness_Measurement + "', " +
                        "Thickness_Fraction = '" + d.Thickness_Fraction + "', " +
                        "Manufacturer_Product_Color_ID = '" + d.Manufacturer_Product_Color_ID + "', " +
                        "Mfg_Color_Name = '" + d.Mfg_Color_Name + "', " +
                        "Mfg_Color_Number = '" + d.Mfg_Color_Number + "', " +
                        "Sample_Box = '" + d.Sample_Box + "', " +
                        "Sample_Box_Availability = '" + d.Sample_Box_Availability + "', " +
                        "Manufacturer_SKU_Number = '" + d.Manufacturer_SKU_Number + "', " +
                        "Merchandised_Product_Color_ID = '" + d.Merchandised_Product_Color_ID + "', " +
                        "Merch_Color_Start_Date = '" + d.Merch_Color_Start_Date + "', " +
                        "Merch_Color_Name = '" + d.Merch_Color_Name.Replace("'", "''") + "', " +
                        "Merch_Color_Number = '" + d.Merch_Color_Number + "', " +
                        "Merchandised_SKU_Number = '" + d.Merchandised_SKU_Number + "', " +
                        "Barcode = '" + d.Barcode + "', " +
                        "CCASKUID = '" + d.CcaSkuId + "', " +
                        "Size_UC = '" + d.Size_UC + "', " + 
                        "Output = '" + d.Output + "', ";
                    if (!limited)
                    {
                        sql += "Roomscene = '" + d.Roomscene + "', " +
                        "Style_Name_and_Color_Combo = '" + d.Division_Product_Name.Replace("'", "''") + " " + d.Merch_Color_Name.Replace("'", "''") + "', " +
                        "Art_Type = '" + d.ArtType + "', ";
                        if (resetInsite)
                        {
                            sql += "Status = 'Waiting for Approval', ";
                            sql += "Change = ' ', ";
                        }
                        sql += "Sample_ID = '" + d.Sample_ID + "', " +
                        "Program = '" + d.Program + "' " +
                        "WHERE Plate_# = '" + d.Plate_ID + "'";
                    }
                    else
                    {
                        //"Roomscene = '" + d.Roomscene + "', " +
                        //"Style_Name_and_Color_Combo = '" + d.Division_Product_Name.Replace("'", "''") + " " + d.Merch_Color_Name.Replace("'", "''") + "', " +
                        //"Art_Type = '" + d.ArtType + "', " +                    
                        if (resetInsite)
                        {
                            sql += "Status = 'Waiting for Approval', ";
                            sql += "Change = ' ' ";
                        }
                        sql += "WHERE Sample_ID = '" + d.Sample_ID + "'";
                    }

                    try
                    {
                        SqlConnect("CCA");
                        connection.Open();
                        SqlCommand command;
                        SqlDataReader dataReader;
                        command = new SqlCommand(sql, connection);
                        dataReader = command.ExecuteReader();
                        dataReader.Close();
                        command.Dispose();
                        connection.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else
            {
                foreach (LarModels.Details d in larModels.DetailsList)
                {
                    sql = "INSERT INTO dbo.Details (" +
                        "Sample_ID, Primary_Display, Division_List, " +
                        "Supplier_Name, Child_Supplier, Taxonomy, " +
                        "Supplier_Product_Name, Merchandised_Product_ID, Merch_Prod_Start_Date, " +
                        "Division_Product_Name, Division_Collection, Division_Rating, " +
                        "Product_Type, Product_Class, Is_Web_Product, Sample_Box_Enabled, " +
                        "Number_of_Colors, Made_In, Fiber_Company, Fiber_Brand, " +
                        "Merchandise_Brand, Primary_Fiber, Primary_Fiber_Percentage, " +
                        "Second_Fiber, Second_Fiber_Percentage, Third_Fiber, Third_Fiber_Percentage, " +
                        "Percent_BCF, Percent_Spun, Pile_Line, Stain_Treatment, " +
                        "Soil_Treatment, Dye_Method, Face_Weight, Yarn_Twist, " +
                        "Match, Match_Length, Match_Width, Total_Weight, " +
                        "Density, Gauge, Pile_Height, Stitches, Backing, " +
                        "IAQ_Number, Is_FHA_Certified, FHA_Type, FHA_Class, " +
                        "FHA_Lab, Durability_Rating, Flammability, " +
                        "Static_AATCC134, NBS_Smoke_Density_ASTME662, Radiant_Panel_ASTME648, " +
                        "Installation_Pattern, Commercial_Rating, Is_Green_Rated, " +
                        "Green_Natural_Sustained, Green_Recyclable_Content, Green_Recycled_Content, " +
                        "Size_Name, Manufacturer_Product_Color_ID, Mfg_Color_Name, " +
                        "Manufacturer_Feeler, Mfg_Color_Number, Sample_Box, Sample_Box_Availability, " +
                        "Manufacturer_SKU_Number, Merchandised_Product_Color_ID, Merch_Color_Start_Date, " +
                        "Merch_Color_Name, Merch_Color_Number, Merchandised_SKU_Number, " +
                        "CCASKUID, Art_Type, Plate_#, ADDNumber, Color_Sequence, Status, Change, Program, Output" +
                        ") VALUES (" +
                        "'" + d.Sample_ID + "', '" + d.Primary_Display + "', '" + d.Division_List + "', " +
                        "'" + d.Supplier_Name + "', '" + d.Child_Supplier + "', '" + d.Taxonomy + "', " +
                        "'" + d.Supplier_Product_Name + "', '" + d.Merchandised_Product_ID + "', '" + d.Merch_Prod_Start_Date + "', " +
                        "'" + d.Division_Product_Name + "', '" + d.Division_Collection + "', '" + d.Division_Rating + "', " +
                        "'" + d.Product_Type + "', '" + d.Product_Class + "', '" + d.Is_Web_Product + "', '" + d.Sample_Box_Enabled + "', " +
                        "'" + d.Number_of_Colors + "', '" + d.Made_In + "', '" + d.Fiber_Company + "', '" + d.Fiber_Brand + "', " +
                        "'" + d.Merchandise_Brand + "', '" + d.Primary_Fiber + "', '" + d.Primary_Fiber_Percentage + "', " +
                        "'" + d.Second_Fiber + "', '" + d.Second_Fiber_Percentage + "', '" + d.Third_Fiber + "', '" + d.Third_Fiber_Percentage + "', " +
                        "'" + d.Percent_BCF + "', '" + d.Percent_Spun + "', '" + d.Pile_Line + "', '" + d.Stain_Treatment + "', " +
                        "'" + d.Soil_Treatment + "', '" + d.Dye_Method + "', '" + d.Face_Weight + "', '" + d.Yarn_Twist + "', " +
                        "'" + d.Match + "', '" + d.Match_Length + "', '" + d.Match_Width + "', '" + d.Total_Weight + "', " +
                        "'" + d.Density + "', '" + d.Gauge + "', '" + d.Pile_Height + "', '" + d.Stitches + "', '" + d.Backing + "', " +
                        "'" + d.IAQ_Number + "', '" + d.Is_FHA_Certified + "', '" + d.FHA_Type + "', '" + d.FHA_Class + "', " +
                        "'" + d.FHA_Lab + "', '" + d.Durability_Rating + "', '" + d.Flammability + "', " +
                        "'" + d.Static_AATCC134 + "', '" + d.NBS_Smoke_Density_ASTME662 + "', '" + d.Radiant_Panel_ASTME648 + "', " +
                        "'" + d.Installation_Pattern + "', '" + d.Commercial_Rating + "', '" + d.Is_Green_Rated + "', " +
                        "'" + d.Green_Natural_Sustained + "', '" + d.Green_Recyclable_Content + "', '" + d.Green_Recycled_Content + "', " +
                        "'" + d.Size_Name + "', '" + d.Manufacturer_Product_Color_ID + "', '" + d.Mfg_Color_Name + "', " +
                        "'" + d.Manufacturer_Feeler + "', '" + d.Mfg_Color_Number + "', '" + d.Sample_Box + "', '" + d.Sample_Box_Availability + "', " +
                        "'" + d.Manufacturer_SKU_Number + "', '" + d.Merchandised_Product_Color_ID + "', '" + d.Merch_Color_Start_Date + "', " +
                        "'" + d.Merch_Color_Name + "', '" + d.Merch_Color_Number + "', '" + d.Merchandised_SKU_Number + "', " +
                        "'" + d.CcaSkuId + "', '" + d.ArtType + "', '" + d.Plate_ID + "', '" + d.ADDNumber + "', '" + d.Color_Sequence + "', " +
                        "'" + d.Status + "', '" + d.Change + "', '" + d.Program + "'" + "', '" + d.Output + "'" +
                        ")";
                    try
                    {
                        SqlConnect("CCA-SS");
                        connection.Open();
                        SqlCommand command;
                        SqlDataReader dataReader;
                        command = new SqlCommand(sql, connection);
                        dataReader = command.ExecuteReader();
                        dataReader.Close();
                        command.Dispose();
                        connection.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                
            }
            foreach (LarModels.MktSpreadsheetItem mkt in mktSSI)
            {
                if (isSoftSurface)
                {
                    SqlConnect("CCA-SS");
                }
                else
                {
                    SqlConnect("CCA");
                }
                SqlCommand commandU;
                SqlDataReader dataReaderU;
                sql = "UPDATE dbo.Details SET Status='" + mkt.Status + "', Program='" + mkt.Program + "' WHERE Sample_ID='" + mkt.Sample_ID + "'";

                try
                {
                    connection.Open();
                    commandU = new SqlCommand(sql, connection);
                    dataReaderU = commandU.ExecuteReader();
                    dataReaderU.Close();
                    commandU.Dispose();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            foreach (LarModels.Labels l in larModels.LabelList)
            {
                sql = "INSERT INTO dbo.Labels (Merchandised_Product_ID, Sample_ID, Division_Label_Type, Division_Label_Name) VALUES ('" + l.Merchandised_Product_ID + "', '" + l.Sample_ID + "', '" + l.Division_Label_Type + "', '" + l.Division_Label_Name + "')";

                try
                {
                    if (isSoftSurface)
                    {
                        SqlConnect("CCA-SS");
                    }
                    else
                    {
                        SqlConnect("CCA");
                    }
                    connection.Open();
                    SqlCommand command;
                    SqlDataReader dataReader;
                    command = new SqlCommand(sql, connection);
                    dataReader = command.ExecuteReader();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            foreach (LarModels.Sample s in larModels.SampleList)
            {
                if (!isSoftSurface)
                {
                    sql = "INSERT INTO dbo.Sample " +
                            "(Sample_ID, Sample_Name, Sample_Size, " + "Sample_Type, " +
                            "Sampled_Color_SKU, Shared_Card, Sampled_With_Merch_Product_ID, Quick_Ship, Binder, " +
                            "Border, Character_Rating_by_Color, Feeler, MSRP, MSRP_Canada, " +
                            "Our_Price, Our_Price_Canada, RRP_US, Sampling_Color_Description, Split_Board, " +
                            "Trade_Up, Wood_Imaging, Sample_Note) " +
                            "VALUES('" + s.Sample_ID + "', '" + s.Sample_Name.Replace("'", "''") + "', '" + s.Sample_Size + "', '" + s.Sample_Type + "', " +
                            "'" + s.Sampled_Color_SKU + "', '" + s.Shared_Card + "', '" + s.Sampled_With_Merch_Product_ID + "', '" + s.Quick_Ship + "', '" + s.Binder + "', " +
                            "'" + s.Border + "', '" + s.Character_Rating_by_Color + "', '" + s.Feeler.Replace("'", "''") + "', '" + s.MSRP + "', '" + s.MSRP_Canada + "', " +
                            "'" + s.Our_Price + "', '" + s.Our_Price_Canada + "', '" + s.RRP_US + "', '" + s.Sampling_Color_Description + "', '" + s.Split_Board + "', " +
                            "'" + s.Trade_Up + "', '" + s.Wood_Imaging + "', '" + s.Sample_Note + "')";
                }
                else
                {
                    sql = "INSERT INTO dbo.Sample " +
                            "(Sample_ID, Sample_Name, Sample_Size, " + "Sample_Type, " +
                            "Sampled_Color_SKU, Shared_Card, Multiple_Color_Lines, Sampled_With_Merch_Product_ID, Quick_Ship, Binder, " +
                            "Border, Character_Rating_by_Color, Feeler, MSRP, MSRP_Canada, " +
                            "Our_Price, Our_Price_Canada, RRP_US, Sampling_Color_Description, Split_Board, " +
                            "Trade_Up, Wood_Imaging, Sample_Note) " +
                            "VALUES('" + s.Sample_ID + "', '" + s.Sample_Name.Replace("'", "''") + "', '" + s.Sample_Size + "', '" + s.Sample_Type + "', " +
                            "'" + s.Sampled_Color_SKU + "', '" + s.Shared_Card + "', '" + s.Multiple_Color_Lines + "', '" + s.Sampled_With_Merch_Product_ID + "', '" + s.Quick_Ship + "', '" + s.Binder + "', " +
                            "'" + s.Border + "', '" + s.Character_Rating_by_Color + "', '" + s.Feeler.Replace("'", "''") + "', '" + s.MSRP + "', '" + s.MSRP_Canada + "', " +
                            "'" + s.Our_Price + "', '" + s.Our_Price_Canada + "', '" + s.RRP_US + "', '" + s.Sampling_Color_Description + "', '" + s.Split_Board + "', " +
                            "'" + s.Trade_Up + "', '" + s.Wood_Imaging + "', '" + s.Sample_Note + "')";
                }
                try
                {
                    if (isSoftSurface)
                    {
                        SqlConnect("CCA-SS");
                    }
                    else
                    {
                        SqlConnect("CCA");
                    }
                    connection.Open();
                    SqlCommand command;
                    SqlDataReader dataReader;
                    command = new SqlCommand(sql, connection);
                    dataReader = command.ExecuteReader();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            foreach (LarModels.Warranties w in larModels.WarrantiesList)
            {
                sql = "INSERT INTO dbo.Warranties " +
                        "(Merchandised_Product_ID,Sample_ID,Provider,Duration,Warranty_Period,Product_Warranty_Type_Code) " +
                        "VALUES ('" + w.Merchandised_Product_ID + "', '" + w.Sample_ID + "', '" + w.Provider + "', " +
                        "'" + w.Duration + "', '" + w.Warranty_Period + "', '" + w.Product_Warranty_Type_Code + "'); ";

                try
                {
                    if (isSoftSurface)
                    {
                        SqlConnect("CCA-SS");
                    }
                    else
                    {
                        SqlConnect("CCA");
                    }
                    connection.Open();
                    SqlCommand command;
                    SqlDataReader dataReader;
                    command = new SqlCommand(sql, connection);
                    dataReader = command.ExecuteReader();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }

        private static List<LarModels.MktSpreadsheetItem> SqlWebDBCleanup(LarModels.LARXlsSheet larModels, bool isSoftSurface)
        {
            List<LarModels.MktSpreadsheetItem> mktSpreadsheetItems = new();
            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
            }
            else
            {
                SqlConnect("CCA");
            }
            SqlCommand commandS;
            SqlDataReader dataReaderS;
            string sql = "SELECT Sample_ID, Status, Program FROM dbo.Details";

            try
            {
                connection.Open();
                commandS = new SqlCommand(sql, connection);
                dataReaderS = commandS.ExecuteReader();
                while (dataReaderS.Read())
                {
                    LarModels.MktSpreadsheetItem mktSpreadsheetItem = new();

                    mktSpreadsheetItem.Sample_ID = dataReaderS.GetString(dataReaderS.GetOrdinal("Sample_ID"));
                    mktSpreadsheetItem.Program = dataReaderS.GetString(dataReaderS.GetOrdinal("Program"));
                    mktSpreadsheetItem.Status = dataReaderS.GetString(dataReaderS.GetOrdinal("Status"));

                    mktSpreadsheetItems.Add(mktSpreadsheetItem);
                }
                dataReaderS.Close();
                commandS.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (isSoftSurface)
            {
                foreach (LarModels.Details d in larModels.DetailsList.Distinct())
                {
                    sql = "DELETE FROM dbo.Labels WHERE Sample_ID ='" + d.Sample_ID + "'";

                    try
                    {
                        if (isSoftSurface)
                        {
                            SqlConnect("CCA-SS");
                        }
                        else
                        {
                            SqlConnect("CCA");
                        }
                        connection.Open();
                        SqlCommand command;
                        SqlDataReader dataReader;
                        command = new SqlCommand(sql, connection);
                        dataReader = command.ExecuteReader();
                        dataReader.Close();
                        command.Dispose();
                        connection.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            foreach (LarModels.Labels l in larModels.LabelList.Distinct())
            {
                sql = "DELETE FROM dbo.Labels WHERE Sample_ID ='" + l.Sample_ID + "'";

                try
                {
                    if (isSoftSurface)
                    {
                        SqlConnect("CCA-SS");
                    }
                    else
                    {
                        SqlConnect("CCA");
                    }
                    connection.Open();
                    SqlCommand command;
                    SqlDataReader dataReader;
                    command = new SqlCommand(sql, connection);
                    dataReader = command.ExecuteReader();
                    dataReader.Close();
                    command.Dispose();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            foreach (LarModels.Sample s in larModels.SampleList.Distinct())
            {
                sql = "DELETE FROM dbo.Sample WHERE Sample_ID ='" + s.Sample_ID + "'";

                try
                {
                    if (isSoftSurface)
                    {
                        SqlConnect("CCA-SS");
                    }
                    else
                    {
                        SqlConnect("CCA");
                    }
                    connection.Open();
                    SqlCommand command;
                    SqlDataReader dataReader;
                    command = new SqlCommand(sql, connection);
                    dataReader = command.ExecuteReader();
                    dataReader.Close();
                    command.Dispose();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            foreach (LarModels.Warranties w in larModels.WarrantiesList.Distinct())
            {
                sql = "DELETE FROM dbo.Warranties WHERE Sample_ID ='" + w.Sample_ID + "'";

                try
                {
                    if (isSoftSurface)
                    {
                        SqlConnect("CCA-SS");
                    }
                    else
                    {
                        SqlConnect("CCA");
                    }
                    connection.Open();
                    SqlCommand command;
                    SqlDataReader dataReader;
                    command = new SqlCommand(sql, connection);
                    dataReader = command.ExecuteReader();
                    dataReader.Close();
                    command.Dispose();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return mktSpreadsheetItems;
        }

        public static string SqlGetStatus(string x, string program, bool isSoftSurface)
        {
            string sql = "";
            int count = 0;
            if (x.ToLower().Equals("waiting"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status = 'Waiting for Approval' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("waitingfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status='Waiting for Approval' and Art_Type Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("waitingbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status='Waiting for Approval' and Art_Type Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("rejected"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status = 'Rejected' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("rejectedfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status='Rejected' and Art_Type Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("rejectedbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status='Rejected' and Art_Type Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("approvedpend"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status = 'Approved Pending' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("approvedpendfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status='Approved Pending' and Art_Type Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("approvedpendbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status='Approved Pending' and Art_Type Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("approved"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status = 'Approved' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("approvedfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status='Approved' and Art_Type Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("approvedbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status='Approved' and Art_Type Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("questions"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status = 'Questions' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("questionsfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status='Questions' and Art_Type Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("questionsbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status='Questions' and Art_Type Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("notdone"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status = 'Not Done' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("notdonefl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status='Not Done' and Art_Type Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("notdonebl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type from dbo.Details where (Status='Not Done' and Art_Type Like '%BL%' and Program = '" + program + "')) t";
            }

            try
            {
                if (isSoftSurface)
                {
                    SqlConnect("CCA-SS");
                }
                else
                {
                    SqlConnect("CCA");
                }
                connection.Open();
                SqlCommand command;
                command = new SqlCommand(sql, connection);
                count = (int)command.ExecuteScalar();

                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return count.ToString();
        }

        public static List<string> SqlGetPrograms(bool isSoftSurface)
        {
            List<string> programList = new();
            
            string sql = "Select Distinct Program from dbo.Details";

            try
            {
                if (isSoftSurface)
                {
                    SqlConnect("CCA-SS");
                }
                else
                {
                    SqlConnect("CCA");
                }
                connection.Open();
                SqlCommand command;
                SqlDataReader dataReader;
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    programList.Add(dataReader.GetString(dataReader.GetOrdinal("Program")));
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return programList;
        }

    }
}
