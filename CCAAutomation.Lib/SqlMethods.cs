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
            server = "xxxxxxxxx";
            database = db;
            uid = "xxxxxxxxx";
            password = "xxxxxxxxx";

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

            string sql = "SELECT Plate_# FROM dbo.Details WHERE Plate_# = '" + plateId + "'";

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

            string sql = "select distinct dbo.Details.Merch_Color_Name, dbo.Details.Color_Sequence, dbo.Details.Manufacturer_Feeler, Feeler from dbo.Sample inner join dbo.details on dbo.details.Sample_ID=dbo.sample.Sample_ID where dbo.Sample.Sample_ID='" + sampleId + "'";

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    string sequence = dataReader.GetString(dataReader.GetOrdinal("Color_Sequence")).PadLeft(2, '0');
                    if (dataReader.GetString(dataReader.GetOrdinal("Manufacturer_Feeler")).EqualsString("yes") || dataReader.GetString(dataReader.GetOrdinal("Merch_Color_Name")).EqualsString(dataReader.GetString(dataReader.GetOrdinal("Feeler"))))
                    {
                        colorList.Add(sequence + " | " + "<b>" + dataReader.GetString(dataReader.GetOrdinal("Merch_Color_Name")) + "</b>");
                    }
                    else
                    {
                        colorList.Add(sequence + " | " + dataReader.GetString(dataReader.GetOrdinal("Merch_Color_Name")));
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
            colorList.Sort();
            return colorList;
        }        

        public static List<LarModels.WebTableItem> SqlSelectWebTableItem(bool isSoftSurface, int offset)
        {
            List<LarModels.WebTableItem> webTableItems = new();
            string sql;

            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
                sql = "SELECT DISTINCT dbo.Details.Supplier_Name, dbo.Details.Plate_#, dbo.Details.Face_Label_Plate_#, dbo.Details.Back_Label_Plate_#, dbo.Details.Sample_ID, dbo.Details.Status, dbo.Details.Status_FL, dbo.Details.Art_Type, dbo.Details.Art_Type_BL, dbo.Details.Art_Type_FL, Sample_Name, Shared_Card, Multiple_Color_Lines, dbo.Details.Change, dbo.Details.Change_FL, dbo.Details.Output, dbo.Details.Output_FL FROM dbo.Sample INNER JOIN dbo.Details ON dbo.Details.Sample_ID=dbo.Sample.Sample_ID";
            }
            else
            {
                SqlConnect("CCA");
                sql = "SELECT DISTINCT dbo.Details.Supplier_Name, dbo.Details.Plate_#, dbo.Details.Face_Label_Plate_#, dbo.Details.Back_Label_Plate_#, dbo.Details.Sample_ID, dbo.Details.Status, dbo.Details.Status_FL, dbo.Details.Art_Type, dbo.Details.Art_Type_BL, dbo.Details.Art_Type_FL, Sample_Name, dbo.Details.Change, dbo.Details.Change_FL, dbo.Details.Output, dbo.Details.Output_FL, dbo.Details.Size_Name, dbo.Details.Width, dbo.Details.Width_Measurement, dbo.Details.Length, dbo.Details.Length_Measurement, dbo.Sample.Feeler FROM dbo.Sample INNER JOIN dbo.Details ON dbo.Details.Sample_ID=dbo.Sample.Sample_ID";
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
                    if (dataReader.GetString(dataReader.GetOrdinal("Plate_#")).EqualsString(""))
                    {
                        webTableItem.SearchTags += webTableItem.Plate_ID = dataReader.GetString(dataReader.GetOrdinal("Back_Label_Plate_#"));
                        webTableItem.SearchTags += webTableItem.Art_Type = dataReader.GetString(dataReader.GetOrdinal("Art_Type_BL"));                        
                    }
                    else
                    {
                        webTableItem.SearchTags += webTableItem.Art_Type = dataReader.GetString(dataReader.GetOrdinal("Art_Type"));
                    }
                    webTableItem.SearchTags += webTableItem.Sample_ID = dataReader.GetString(dataReader.GetOrdinal("Sample_ID"));
                    webTableItem.SearchTags += webTableItem.Status = dataReader.GetString(dataReader.GetOrdinal("Status"));
                    webTableItem.SearchTags += webTableItem.Change = dataReader.GetString(dataReader.GetOrdinal("Change"));
                    webTableItem.SearchTags += webTableItem.Customer = dataReader.GetString(dataReader.GetOrdinal("Supplier_Name"));
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
                        webTableItem.Feeler = dataReader.GetString(dataReader.GetOrdinal("Feeler"));
                        webTableItem.Size_Name = dataReader.GetString(dataReader.GetOrdinal("Size_Name"));
                        webTableItem.Width = dataReader.GetString(dataReader.GetOrdinal("Width"));
                        webTableItem.Width_Measurement = dataReader.GetString(dataReader.GetOrdinal("Width_Measurement"));
                        webTableItem.Length = dataReader.GetString(dataReader.GetOrdinal("Length"));
                        webTableItem.Length_Measurement = dataReader.GetString(dataReader.GetOrdinal("Length_Measurement"));
                    }
                    if (dataReader.GetString(dataReader.GetOrdinal("Sample_Name")).EqualsString("worthwild"))
                    {
                        string thisis = dataReader.GetString(dataReader.GetOrdinal("Sample_Name"));
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
            
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    if (!dataReader.GetString(dataReader.GetOrdinal("Face_Label_Plate_#")).EqualsString(""))
                    {
                        LarModels.WebTableItem webTableItem = new();
                        webTableItem.SearchTags += webTableItem.Plate_ID = dataReader.GetString(dataReader.GetOrdinal("Face_Label_Plate_#"));
                        webTableItem.SearchTags += webTableItem.Sample_ID = dataReader.GetString(dataReader.GetOrdinal("Sample_ID"));
                        webTableItem.SearchTags += webTableItem.Status = dataReader.GetString(dataReader.GetOrdinal("Status_FL"));
                        webTableItem.SearchTags += webTableItem.Art_Type = dataReader.GetString(dataReader.GetOrdinal("Art_Type_FL"));
                        webTableItem.SearchTags += webTableItem.Change = dataReader.GetString(dataReader.GetOrdinal("Change_FL"));
                        webTableItem.SearchTags += webTableItem.Customer = dataReader.GetString(dataReader.GetOrdinal("Supplier_Name"));
                        webTableItem.SearchTags += webTableItem.Style = dataReader.GetString(dataReader.GetOrdinal("Sample_Name"));
                        webTableItem.Output = dataReader.GetInt32(dataReader.GetOrdinal("Output_FL"));
                        if (!isSoftSurface)
                        {
                            webTableItem.Size_Name = dataReader.GetString(dataReader.GetOrdinal("Size_Name"));
                            webTableItem.Width = dataReader.GetString(dataReader.GetOrdinal("Width"));
                            webTableItem.Width_Measurement = dataReader.GetString(dataReader.GetOrdinal("Width_Measurement"));
                            webTableItem.Length = dataReader.GetString(dataReader.GetOrdinal("Length"));
                            webTableItem.Length_Measurement = dataReader.GetString(dataReader.GetOrdinal("Length_Measurement"));
                            webTableItem.Feeler = dataReader.GetString(dataReader.GetOrdinal("Feeler"));
                        }

                        webTableItems.Add(webTableItem);
                        webTableItem = new();
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

            string sql = "SELECT TOP 1 Plate_#, Back_Label_Plate_#, Face_Label_Plate_#, Sample_ID, Status, Status_FL, Art_Type, Art_Type_FL, Change, Change_FL, Output, Output_FL FROM dbo.Details WHERE (Plate_#='" + plateId + "' OR Back_Label_Plate_#='" + plateId + "' OR Face_Label_Plate_#='" + plateId + "')";

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    if (dataReader.GetString(dataReader.GetOrdinal("Plate_#")).EqualsString(plateId))
                    {
                        webTableItem.Plate_ID = dataReader.GetString(dataReader.GetOrdinal("Plate_#"));
                        webTableItem.Sample_ID = dataReader.GetString(dataReader.GetOrdinal("Sample_ID"));
                        webTableItem.Status = dataReader.GetString(dataReader.GetOrdinal("Status"));
                        webTableItem.Art_Type = dataReader.GetString(dataReader.GetOrdinal("Art_Type"));
                        webTableItem.Change = dataReader.GetString(dataReader.GetOrdinal("Change"));
                        webTableItem.Output = dataReader.GetInt32(dataReader.GetOrdinal("Output"));
                    }
                    else if (dataReader.GetString(dataReader.GetOrdinal("Back_Label_Plate_#")).EqualsString(plateId))
                    {
                        webTableItem.Plate_ID = dataReader.GetString(dataReader.GetOrdinal("Back_Label_Plate_#"));
                        webTableItem.Sample_ID = dataReader.GetString(dataReader.GetOrdinal("Sample_ID"));
                        webTableItem.Status = dataReader.GetString(dataReader.GetOrdinal("Status"));
                        webTableItem.Art_Type = dataReader.GetString(dataReader.GetOrdinal("Art_Type_BL"));
                        webTableItem.Change = dataReader.GetString(dataReader.GetOrdinal("Change_BL"));
                        webTableItem.Output = dataReader.GetInt32(dataReader.GetOrdinal("Output_BL"));
                    }
                    else if (dataReader.GetString(dataReader.GetOrdinal("Face_Label_Plate_#")).EqualsString(plateId))
                    {
                        webTableItem.Plate_ID = dataReader.GetString(dataReader.GetOrdinal("Face_Label_Plate_#"));
                        webTableItem.Sample_ID = dataReader.GetString(dataReader.GetOrdinal("Sample_ID"));
                        webTableItem.Status = dataReader.GetString(dataReader.GetOrdinal("Status_FL"));
                        webTableItem.Art_Type = dataReader.GetString(dataReader.GetOrdinal("Art_Type_FL"));
                        webTableItem.Change = dataReader.GetString(dataReader.GetOrdinal("Change_FL"));
                        webTableItem.Output = dataReader.GetInt32(dataReader.GetOrdinal("Output_FL"));
                    }
                    
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
                    details.Plate_ID_BL = dataReader.GetString(dataReader.GetOrdinal("Back Label Plate #"));
                    details.Plate_ID_FL = dataReader.GetString(dataReader.GetOrdinal("Face Label Plate #"));
                    if (details.Plate_ID_BL.EqualsString(""))
                    {
                        details.Plate_ID_BL = dataReader.GetString(dataReader.GetOrdinal("Blanket Label Plate #"));
                    }
                    if (details.Plate_ID.EqualsString(""))
                    {
                        details.Plate_ID = details.Plate_ID_BL;
                    }
                    if (details.Plate_ID_FL.EqualsString(""))
                    {
                        details.Plate_ID_FL = dataReader.GetString(dataReader.GetOrdinal("Face Plate Plate #"));
                    }
                    details.ArtType = dataReader.GetString(dataReader.GetOrdinal("Art_Type"));
                    details.ArtType_BL = dataReader.GetString(dataReader.GetOrdinal("Art Type - BL"));
                    details.ArtType_FL = dataReader.GetString(dataReader.GetOrdinal("Art Type - FL"));
                    if (details.ArtType_BL.EqualsString(""))
                    {
                        details.ArtType_BL = dataReader.GetString(dataReader.GetOrdinal("Art_Type - BL"));
                    }
                    if (details.ArtType.EqualsString(""))
                    {
                        details.ArtType = details.ArtType_BL;
                    }
                    if (details.ArtType_FL.EqualsString(""))
                    {
                        details.ArtType_FL = dataReader.GetString(dataReader.GetOrdinal("Art Type - FP"));
                    }
                    details.Status = dataReader.GetString(dataReader.GetOrdinal("Status")).Trim();
                    details.Change = dataReader.GetString(dataReader.GetOrdinal("Change")).Trim();
                    details.Program = dataReader.GetString(dataReader.GetOrdinal("Program")).Trim();

                    details.Sample_ID = dataReader.GetString(dataReader.GetOrdinal("Sample_ID")).Trim();
                    details.Primary_Display = dataReader.GetString(dataReader.GetOrdinal("Primary_Display")).Trim();
                    details.Division_List = dataReader.GetString(dataReader.GetOrdinal("Division_List")).Trim();
                    details.Supplier_Name = dataReader.GetString(dataReader.GetOrdinal("Supplier_Name")).Trim();
                    details.Child_Supplier = dataReader.GetString(dataReader.GetOrdinal("Child_Supplier")).Trim();
                    details.Taxonomy = dataReader.GetString(dataReader.GetOrdinal("Taxonomy")).Trim();
                    details.Supplier_Product_Name = dataReader.GetString(dataReader.GetOrdinal("Supplier_Product_Name")).Trim();
                    details.Merchandised_Product_ID = dataReader.GetString(dataReader.GetOrdinal("Merchandised_Product_ID")).Trim();
                    details.Merch_Prod_Start_Date = dataReader.GetString(dataReader.GetOrdinal("Merch_Prod_Start_Date")).Trim();
                    details.Division_Product_Name = dataReader.GetString(dataReader.GetOrdinal("Division_Product_Name")).Trim();
                    details.Division_Collection = dataReader.GetString(dataReader.GetOrdinal("Division_Collection")).Trim();
                    details.Division_Rating = dataReader.GetString(dataReader.GetOrdinal("Division_Rating")).Trim();
                    details.Product_Type = dataReader.GetString(dataReader.GetOrdinal("Product_Type")).Trim();
                    details.Product_Class = dataReader.GetString(dataReader.GetOrdinal("Product_Class")).Trim();
                    details.Is_Web_Product = dataReader.GetString(dataReader.GetOrdinal("Is_Web_Product")).Trim();
                    details.Sample_Box_Enabled = dataReader.GetString(dataReader.GetOrdinal("Sample_Box_Enabled")).Trim();
                    details.Made_In = dataReader.GetString(dataReader.GetOrdinal("Made_In")).Trim();
                    details.Merchandise_Brand = dataReader.GetString(dataReader.GetOrdinal("Merchandise_Brand")).Trim();
                    details.Stain_Treatment = dataReader.GetString(dataReader.GetOrdinal("Stain_Treatment")).Trim();
                    details.Match = dataReader.GetString(dataReader.GetOrdinal("Match")).Trim();
                    details.Match_Length = dataReader.GetString(dataReader.GetOrdinal("Match_Length")).Trim();
                    details.Match_Width = dataReader.GetString(dataReader.GetOrdinal("Match_Width")).Trim();
                    details.Backing = dataReader.GetString(dataReader.GetOrdinal("Backing")).Trim();
                    details.Is_FHA_Certified = dataReader.GetString(dataReader.GetOrdinal("Is_FHA_Certified"));
                    details.FHA_Type = dataReader.GetString(dataReader.GetOrdinal("FHA_Type")).Trim();
                    details.FHA_Lab = dataReader.GetString(dataReader.GetOrdinal("FHA_Lab")).Trim();
                    details.Commercial_Rating = dataReader.GetString(dataReader.GetOrdinal("Commercial_Rating")).Trim();
                    details.Is_Green_Rated = dataReader.GetString(dataReader.GetOrdinal("Is_Green_Rated")).Trim();
                    details.Green_Natural_Sustained = dataReader.GetString(dataReader.GetOrdinal("Green_Natural_Sustained")).Trim();
                    details.Green_Recyclable_Content = dataReader.GetString(dataReader.GetOrdinal("Green_Recyclable_Content")).Trim();
                    details.Green_Recycled_Content = dataReader.GetString(dataReader.GetOrdinal("Green_Recycled_Content")).Trim();
                    details.Size_Name = dataReader.GetString(dataReader.GetOrdinal("Size_Name")).Trim();
                    details.Manufacturer_Product_Color_ID = dataReader.GetString(dataReader.GetOrdinal("Manufacturer_Product_Color_ID")).Trim();
                    details.Mfg_Color_Name = dataReader.GetString(dataReader.GetOrdinal("Mfg_Color_Name")).Trim();
                    details.Mfg_Color_Number = dataReader.GetString(dataReader.GetOrdinal("Mfg_Color_Number")).Trim();
                    details.Sample_Box = dataReader.GetString(dataReader.GetOrdinal("Sample_Box")).Trim();
                    details.Sample_Box_Availability = dataReader.GetString(dataReader.GetOrdinal("Sample_Box_Availability")).Trim();
                    details.Manufacturer_SKU_Number = dataReader.GetString(dataReader.GetOrdinal("Manufacturer_SKU_Number")).Trim();
                    details.Merchandised_Product_Color_ID = dataReader.GetString(dataReader.GetOrdinal("Merchandised_Product_Color_ID")).Trim();
                    details.Merch_Color_Start_Date = dataReader.GetString(dataReader.GetOrdinal("Merch_Color_Start_Date")).Trim();
                    details.Merch_Color_Name = dataReader.GetString(dataReader.GetOrdinal("Merch_Color_Name")).Trim();
                    details.Merch_Color_Number = dataReader.GetString(dataReader.GetOrdinal("Merch_Color_Number")).Trim();
                    details.Merchandised_SKU_Number = dataReader.GetString(dataReader.GetOrdinal("Merchandised_SKU_Number")).Trim();
                    details.CcaSkuId = dataReader.GetString(dataReader.GetOrdinal("CcaSkuId")).Trim();
                    details.Output = dataReader.GetInt32(dataReader.GetOrdinal("Output"));

                    if (!isSoftSurface)
                    {
                        details.Appearance = dataReader.GetString(dataReader.GetOrdinal("Appearance")).Trim();
                        details.Barcode = dataReader.GetString(dataReader.GetOrdinal("Barcode")).Trim();
                        details.Construction = dataReader.GetString(dataReader.GetOrdinal("Construction")).Trim();
                        details.Edge_Profile = dataReader.GetString(dataReader.GetOrdinal("Edge_Profile")).Trim();
                        details.End_Profile = dataReader.GetString(dataReader.GetOrdinal("End_Profile")).Trim();
                        details.Finish = dataReader.GetString(dataReader.GetOrdinal("Finish")).Trim();
                        details.Glazed_Hardness = dataReader.GetString(dataReader.GetOrdinal("Glazed_Hardness")).Trim();
                        details.Gloss_Level = dataReader.GetString(dataReader.GetOrdinal("Gloss_Level")).Trim();
                        details.Grade = dataReader.GetString(dataReader.GetOrdinal("Grade")).Trim();
                        details.Hardness_Rating = dataReader.GetString(dataReader.GetOrdinal("Hardness_Rating")).Trim();
                        details.Installation_Method = dataReader.GetString(dataReader.GetOrdinal("Installation_Method")).Trim();
                        details.Is_Recommended_Outdoors = dataReader.GetString(dataReader.GetOrdinal("Is_Recommended_Outdoors")).Trim();
                        details.Is_Wall_Tile = dataReader.GetString(dataReader.GetOrdinal("Is_Wall_Tile")).Trim();
                        details.Length = dataReader.GetString(dataReader.GetOrdinal("Length")).Trim();
                        details.Length_Measurement = dataReader.GetString(dataReader.GetOrdinal("Length_Measurement")).Trim();
                        details.Locking_Type = dataReader.GetString(dataReader.GetOrdinal("Locking_Type")).Trim();
                        details.Radiant_Heat = dataReader.GetString(dataReader.GetOrdinal("Radiant_Heat")).Trim();
                        details.Roomscene = dataReader.GetString(dataReader.GetOrdinal("Roomscene")).Trim();
                        details.Shade_Variation = dataReader.GetString(dataReader.GetOrdinal("Shade_Variation")).Trim();
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
                        details.Fiber_Company = dataReader.GetString(dataReader.GetOrdinal("Fiber_Company")).Trim();
                        details.Fiber_Brand = dataReader.GetString(dataReader.GetOrdinal("Fiber_Brand")).Trim();
                        details.Primary_Fiber = dataReader.GetString(dataReader.GetOrdinal("Primary_Fiber")).Trim();
                        details.Primary_Fiber_Percentage = dataReader.GetString(dataReader.GetOrdinal("Primary_Fiber_Percentage")).Trim();
                        details.Second_Fiber = dataReader.GetString(dataReader.GetOrdinal("Second_Fiber")).Trim();
                        details.Second_Fiber_Percentage = dataReader.GetString(dataReader.GetOrdinal("Second_Fiber_Percentage")).Trim();
                        details.Third_Fiber = dataReader.GetString(dataReader.GetOrdinal("Third_Fiber")).Trim();
                        details.Third_Fiber_Percentage = dataReader.GetString(dataReader.GetOrdinal("Third_Fiber_Percentage")).Trim();
                        details.Percent_BCF = dataReader.GetString(dataReader.GetOrdinal("Percent_BCF")).Trim();
                        details.Percent_Spun = dataReader.GetString(dataReader.GetOrdinal("Percent_Spun")).Trim();
                        details.Pile_Line = dataReader.GetString(dataReader.GetOrdinal("Pile_Line")).Trim();
                        details.Soil_Treatment = dataReader.GetString(dataReader.GetOrdinal("Soil_Treatment")).Trim();
                        details.Dye_Method = dataReader.GetString(dataReader.GetOrdinal("Dye_Method")).Trim();
                        details.Face_Weight = dataReader.GetString(dataReader.GetOrdinal("Face_Weight")).Trim();
                        details.Yarn_Twist = dataReader.GetString(dataReader.GetOrdinal("Yarn_Twist")).Trim();
                        details.Total_Weight = dataReader.GetString(dataReader.GetOrdinal("Total_Weight")).Trim();
                        details.Density = dataReader.GetString(dataReader.GetOrdinal("Density")).Trim();
                        details.Gauge = dataReader.GetString(dataReader.GetOrdinal("Gauge")).Trim();
                        details.Pile_Height = dataReader.GetString(dataReader.GetOrdinal("Pile_Height")).Trim();
                        details.Stitches = dataReader.GetString(dataReader.GetOrdinal("Stitches")).Trim();
                        details.IAQ_Number = dataReader.GetString(dataReader.GetOrdinal("IAQ_Number")).Trim();
                        details.FHA_Class = dataReader.GetString(dataReader.GetOrdinal("FHA_Class")).Trim();
                        details.Durability_Rating = dataReader.GetString(dataReader.GetOrdinal("Durability_Rating")).Trim();
                        details.Flammability = dataReader.GetString(dataReader.GetOrdinal("Flammability")).Trim();
                        details.Static_AATCC134 = dataReader.GetString(dataReader.GetOrdinal("Static_AATCC134")).Trim();
                        details.NBS_Smoke_Density_ASTME662 = dataReader.GetString(dataReader.GetOrdinal("NBS_Smoke_Density_ASTME662")).Trim();
                        details.Radiant_Panel_ASTME648 = dataReader.GetString(dataReader.GetOrdinal("Radiant_Panel_ASTME648")).Trim();
                        details.Installation_Pattern = dataReader.GetString(dataReader.GetOrdinal("Installation_Pattern")).Trim();
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
                sql = "SELECT Sample_ID, Sample_Name, Feeler, Sample_Type, Shared_Card, Multiple_Color_Lines, Split_Board FROM dbo.Sample WHERE Sample_ID = '" + sampleId + "'";
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
                        sample.Split_Board = dataReader.GetString(dataReader.GetOrdinal("Split_Board"));
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
            string sql = "";
            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
                //sql = "UPDATE dbo.Details SET Status='" + theStatus + "' WHERE \"Plate_#\"='" + plateId + "'";
                sql = "UPDATE dbo.Details SET Status = case when (Plate_# ='" + plateId + "' OR Back_Label_Plate_# ='" + plateId + "') then '" + theStatus + "' ELSE Status end, " +
                "Status_FL = case when Face_Label_Plate_# = '" + plateId + "' then '" + theStatus + "' ELSE Status_FL end " +
                "WHERE (\"Plate_#\"='" + plateId + "' OR \"Face_Label_Plate_#\"='" + plateId + "' OR \"Back_Label_Plate_#\"='" + plateId + "')";
                if (theStatus.Equals("Approved"))
                {
                    //sql = "UPDATE dbo.Details SET Status='" + theStatus + "', Change='' WHERE \"Plate_#\"='" + plateId + "'";
                    sql = "UPDATE dbo.Details SET Change = '', Status = case when (Plate_# ='" + plateId + "' OR Back_Label_Plate_# ='" + plateId + "') then '" + theStatus + "' ELSE Status end, " +
                     "Change_FL = '', Status_FL = case when Face_Label_Plate_# = '" + plateId + "' then '" + theStatus + "' ELSE Status_FL end " +
                     "WHERE (\"Plate_#\"='" + plateId + "' OR \"Face_Label_Plate_#\"='" + plateId + "' OR \"Back_Label_Plate_#\"='" + plateId + "')";
                }
            }
            else
            {
                SqlConnect("CCA");
                sql = "UPDATE dbo.Details SET Status = case when (Plate_# ='" + plateId + "' OR Back_Label_Plate_# ='" + plateId + "') then '" + theStatus + "' ELSE Status end, " +
                "Status_FL = case when Face_Label_Plate_# = '" + plateId + "' then '" + theStatus + "' ELSE Status_FL end " +
                "WHERE (\"Plate_#\"='" + plateId + "' OR \"Face_Label_Plate_#\"='" + plateId + "' OR \"Back_Label_Plate_#\"='" + plateId + "')";

                if (theStatus.Equals("Approved"))
                {
                    sql = "UPDATE dbo.Details SET Change = '', Status = case when (Plate_# ='" + plateId + "' OR Back_Label_Plate_# ='" + plateId + "') then '" + theStatus + "' ELSE Status end, " +
                     "Change_FL = '', Status_FL = case when Face_Label_Plate_# = '" + plateId + "' then '" + theStatus + "' ELSE Status_FL end " +
                     "WHERE (\"Plate_#\"='" + plateId + "' OR \"Face_Label_Plate_#\"='" + plateId + "' OR \"Back_Label_Plate_#\"='" + plateId + "')";
                }
            }
            SqlCommand command;
            SqlDataReader dataReader;

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
            string sql = "UPDATE dbo.Details SET Change = case when (Plate_# ='" + plateId + "' OR Back_Label_Plate_# ='" + plateId + "') then '" + theChange + "' ELSE Change end, " +
                "Change_FL = case when Face_Label_Plate_# = '" + plateId + "' then '" + theChange + "' ELSE Change_FL end " +
                "WHERE (\"Plate_#\"='" + plateId + "' OR \"Face_Label_Plate_#\"='" + plateId + "' OR \"Back_Label_Plate_#\"='" + plateId + "')";

            //string sql = "UPDATE dbo.Details SET Change='" + theChange + "' WHERE \"Plate_#\"='" + plateId + "'";

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

        public static void SqlWebDBUpdate(LarModels.LARXlsSheet larModels, bool isSoftSurface)
        {
            List<LarModels.MktSpreadsheetItem> mktSSI = SqlWebDBCleanup(larModels, isSoftSurface);

            string sql = "";
            if (isSoftSurface)
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
                        "CCASKUID, Art_Type, Art_Type_BL, Art_Type_FL, Plate_#, Back_Label_Plate_#, Face_Label_Plate_#, " +
                        "ADDNumber, Color_Sequence, Status, Status_FL, Change, Change_FL, Program, Output, Output_FL, Layout" +
                        ") VALUES (" +
                        "'" + d.Sample_ID + "', '" + d.Primary_Display.Replace("'", "''") + "', '" + d.Division_List.Replace("'", "''") + "', " +
                        "'" + d.Supplier_Name.Replace("'", "''") + "', '" + d.Child_Supplier.Replace("'", "''") + "', '" + d.Taxonomy.Replace("'", "''") + "', " +
                        "'" + d.Supplier_Product_Name.Replace("'", "''") + "', '" + d.Merchandised_Product_ID.Replace("'", "''") + "', '" + d.Merch_Prod_Start_Date.Replace("'", "''") + "', " +
                        "'" + d.Division_Product_Name.Replace("'", "''") + "', '" + d.Division_Collection.Replace("'", "''") + "', '" + d.Division_Rating.Replace("'", "''") + "', " +
                        "'" + d.Product_Type.Replace("'", "''") + "', '" + d.Product_Class.Replace("'", "''") + "', '" + d.Is_Web_Product.Replace("'", "''") + "', '" + d.Sample_Box_Enabled.Replace("'", "''") + "', " +
                        "'" + d.Number_of_Colors.Replace("'", "''") + "', '" + d.Made_In.Replace("'", "''") + "', '" + d.Fiber_Company.Replace("'", "''") + "', '" + d.Fiber_Brand.Replace("'", "''") + "', " +
                        "'" + d.Merchandise_Brand.Replace("'", "''") + "', '" + d.Primary_Fiber.Replace("'", "''") + "', '" + d.Primary_Fiber_Percentage.Replace("'", "''") + "', " +
                        "'" + d.Second_Fiber.Replace("'", "''") + "', '" + d.Second_Fiber_Percentage.Replace("'", "''") + "', '" + d.Third_Fiber.Replace("'", "''") + "', '" + d.Third_Fiber_Percentage.Replace("'", "''") + "', " +
                        "'" + d.Percent_BCF.Replace("'", "''") + "', '" + d.Percent_Spun.Replace("'", "''") + "', '" + d.Pile_Line.Replace("'", "''") + "', '" + d.Stain_Treatment.Replace("'", "''") + "', " +
                        "'" + d.Soil_Treatment.Replace("'", "''") + "', '" + d.Dye_Method.Replace("'", "''") + "', '" + d.Face_Weight.Replace("'", "''") + "', '" + d.Yarn_Twist.Replace("'", "''") + "', " +
                        "'" + d.Match.Replace("'", "''") + "', '" + d.Match_Length.Replace("'", "''") + "', '" + d.Match_Width.Replace("'", "''") + "', '" + d.Total_Weight.Replace("'", "''") + "', " +
                        "'" + d.Density.Replace("'", "''") + "', '" + d.Gauge.Replace("'", "''") + "', '" + d.Pile_Height.Replace("'", "''") + "', '" + d.Stitches.Replace("'", "''") + "', '" + d.Backing.Replace("'", "''") + "', " +
                        "'" + d.IAQ_Number.Replace("'", "''") + "', '" + d.Is_FHA_Certified.Replace("'", "''") + "', '" + d.FHA_Type.Replace("'", "''") + "', '" + d.FHA_Class.Replace("'", "''") + "', " +
                        "'" + d.FHA_Lab.Replace("'", "''") + "', '" + d.Durability_Rating.Replace("'", "''") + "', '" + d.Flammability.Replace("'", "''") + "', " +
                        "'" + d.Static_AATCC134.Replace("'", "''") + "', '" + d.NBS_Smoke_Density_ASTME662.Replace("'", "''") + "', '" + d.Radiant_Panel_ASTME648.Replace("'", "''") + "', " +
                        "'" + d.Installation_Pattern.Replace("'", "''") + "', '" + d.Commercial_Rating.Replace("'", "''") + "', '" + d.Is_Green_Rated.Replace("'", "''") + "', " +
                        "'" + d.Green_Natural_Sustained.Replace("'", "''") + "', '" + d.Green_Recyclable_Content.Replace("'", "''") + "', '" + d.Green_Recycled_Content.Replace("'", "''") + "', " +
                        "'" + d.Size_Name.Replace("'", "''") + "', '" + d.Manufacturer_Product_Color_ID.Replace("'", "''") + "', '" + d.Mfg_Color_Name.Replace("'", "''") + "', " +
                        "'" + d.Manufacturer_Feeler.Replace("'", "''") + "', '" + d.Mfg_Color_Number.Replace("'", "''") + "', '" + d.Sample_Box.Replace("'", "''") + "', '" + d.Sample_Box_Availability.Replace("'", "''") + "', " +
                        "'" + d.Manufacturer_SKU_Number.Replace("'", "''") + "', '" + d.Merchandised_Product_Color_ID.Replace("'", "''") + "', '" + d.Merch_Color_Start_Date.Replace("'", "''") + "', " +
                        "'" + d.Merch_Color_Name.Replace("'", "''") + "', '" + d.Merch_Color_Number.Replace("'", "''") + "', '" + d.Merchandised_SKU_Number.Replace("'", "''") + "', " +
                        "'" + d.CcaSkuId.Replace("'", "''") + "', '" + d.ArtType.Replace("'", "''") + "', '" + d.ArtType_BL.Replace("'", "''") + "', '" + d.ArtType_FL.Replace("'", "''") + "', '" + d.Plate_ID.Replace("'", "''") + "', '" + d.Plate_ID_BL.Replace("'", "''") + "', '" + d.Plate_ID_FL.Replace("'", "''") + "', " +
                        "'" + d.ADDNumber.Replace("'", "''") + "', '" + d.Color_Sequence.Replace("'", "''") + "', '" + d.Status.Replace("'", "''") + "', '" + d.Status_FL.Replace("'", "''") + "', '" + d.Change.Replace("'", "''") + "', '" + d.Change_FL.Replace("'", "''") + "', '" + d.Program.Replace("'", "''") + "', '" + d.Output + "', '" + d.Output_FL + "', '" + d.Layout.Replace("'", "''") + "')";
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
            else
            {
                foreach (LarModels.Details d in larModels.DetailsList)
                {
                    sql = "INSERT INTO dbo.Details (Sample_ID, Primary_Display, Division_List, Supplier_Name, " +
                        "Child_Supplier, Taxonomy, Supplier_Product_Name, Merchandised_Product_ID, " +
                        "Merch_Prod_Start_Date, Division_Product_Name, Web_Product_Name, Division_Collection, " +
                        "Division_Rating, Product_Type, Product_Class, Is_Web_Product, Sample_Box_Enabled, " +
                        "Number_of_Colors, Made_In, Appearance, Backing, Edge_Profile, End_Profile, FHA_Class, " +
                        "FHA_Lab, FHA_Type, Finish, Glazed_Hardness, Grade, Is_FHA_Certified, Is_Recommended_Outdoors, " +
                        "Is_Wall_Tile, Locking_Type, Radiant_Heat, Shade_Variation, Stain_Treatment, Wear_Layer, " +
                        "Wear_Layer_Type, Construction, Gloss_Level, Hardness_Rating, Installation_Method, Match, " +
                        "Match_Length, Match_Width, Species, Merchandise_Brand, Commercial_Rating, Is_Green_Rated, " +
                        "Green_Natural_Sustained, Green_Recyclable_Content, Green_Recycled_Content, Size_Name, Length, " +
                        "Length_Measurement, Width, Width_Measurement, Thickness, Thickness_Measurement, " +
                        "Thickness_Fraction, Manufacturer_Product_Color_ID, Mfg_Color_Name, Mfg_Color_Number, " +
                        "Sample_Box, Sample_Box_Availability, Manufacturer_SKU_Number, Merchandised_Product_Color_ID, " +
                        "Merch_Color_Start_Date, Merch_Color_Name, Merch_Color_Number, Merchandised_SKU_Number, Barcode, " +
                        "CCASKUID, Size_UC, Roomscene, Plate_#, Back_Label_Plate_#, Face_Label_Plate_#, Art_Type, " +
                        "Art_Type_BL, Art_Type_FL, Status, Status_FL, Change, Change_FL, Program, Output,Output_FL) " +
                        "VALUES " +
                        "('" + d.Sample_ID.Replace("'", "''") +"', '" + d.Primary_Display.Replace("'", "''") +"', '" +
                        "" + d.Division_List.Replace("'", "''") +"', '" + d.Supplier_Name.Replace("'", "''") +"', '" +
                        "" + d.Child_Supplier.Replace("'", "''") +"', '" + d.Taxonomy.Replace("'", "''") +"', '" +
                        "" + d.Supplier_Product_Name.Replace("'", "''") +"', '" +
                        "" + d.Merchandised_Product_ID.Replace("'", "''") +"', '" +
                        "" + d.Merch_Prod_Start_Date.Replace("'", "''") +"', '" +
                        "" + d.Division_Product_Name.Replace("'", "''") +"', '" +
                        "" + d.Web_Product_Name.Replace("'", "''") +"', '" + d.Division_Collection.Replace("'", "''") +"', '" +
                        "" + d.Division_Rating.Replace("'", "''") +"', '" + d.Product_Type.Replace("'", "''") +"', '" +
                        "" + d.Product_Class.Replace("'", "''") +"', '" + d.Is_Web_Product.Replace("'", "''") +"', '" +
                        "" + d.Sample_Box_Enabled.Replace("'", "''") +"', '" + d.Number_of_Colors.Replace("'", "''") +"', '" +
                        "" + d.Made_In.Replace("'", "''") +"', '" + d.Appearance.Replace("'", "''") +"', '" +
                        "" + d.Backing.Replace("'", "''") +"', '" + d.Edge_Profile.Replace("'", "''") +"', '" +
                        "" + d.End_Profile.Replace("'", "''") +"', '" + d.FHA_Class.Replace("'", "''") +"', '" +
                        "" + d.FHA_Lab.Replace("'", "''") +"', '" + d.FHA_Type.Replace("'", "''") +"', '" +
                        "" + d.Finish.Replace("'", "''") +"', '" + d.Glazed_Hardness.Replace("'", "''") +"', '" +
                        "" + d.Grade.Replace("'", "''") +"', '" + d.Is_FHA_Certified.Replace("'", "''") +"', '" +
                        "" + d.Is_Recommended_Outdoors.Replace("'", "''") +"', '" + d.Is_Wall_Tile.Replace("'", "''") +"', '" +
                        "" + d.Locking_Type.Replace("'", "''") +"', '" + d.Radiant_Heat.Replace("'", "''") +"', '" +
                        "" + d.Shade_Variation.Replace("'", "''") +"', '" + d.Stain_Treatment.Replace("'", "''") +"', '" +
                        "" + d.Wear_Layer.Replace("'", "''") +"', '" + d.Wear_Layer_Type.Replace("'", "''") +"', '" +
                        "" + d.Construction.Replace("'", "''") +"', '" + d.Gloss_Level.Replace("'", "''") +"', '" +
                        "" + d.Hardness_Rating.Replace("'", "''") +"', '" + d.Installation_Method.Replace("'", "''") +"', '" +
                        "" + d.Match.Replace("'", "''") +"', '" + d.Match_Length.Replace("'", "''") +"', '" +
                        "" + d.Match_Width.Replace("'", "''") +"', '" + d.Species.Replace("'", "''") +"', '" +
                        "" + d.Merchandise_Brand.Replace("'", "''") +"', '" + d.Commercial_Rating.Replace("'", "''") +"', '" +
                        "" + d.Is_Green_Rated.Replace("'", "''") +"', '" +
                        "" + d.Green_Natural_Sustained.Replace("'", "''") +"', '" +
                        "" + d.Green_Recyclable_Content.Replace("'", "''") +"', '" +
                        "" + d.Green_Recycled_Content.Replace("'", "''") +"', '" + d.Size_Name.Replace("'", "''") +"', '" +
                        "" + d.Length.Replace("'", "''") +"', '" + d.Length_Measurement.Replace("'", "''") +"', '" +
                        "" + d.Width.Replace("'", "''") +"', '" + d.Width_Measurement.Replace("'", "''") +"', '" +
                        "" + d.Thickness.Replace("'", "''") +"', '" + d.Thickness_Measurement.Replace("'", "''") +"', '" +
                        "" + d.Thickness_Fraction.Replace("'", "''") +"', '" +
                        "" + d.Manufacturer_Product_Color_ID.Replace("'", "''") +"', '" +
                        "" + d.Mfg_Color_Name.Replace("'", "''") +"', '" + d.Mfg_Color_Number.Replace("'", "''") +"', '" +
                        "" + d.Sample_Box.Replace("'", "''") +"', '" + d.Sample_Box_Availability.Replace("'", "''") +"', '" +
                        "" + d.Manufacturer_SKU_Number.Replace("'", "''") +"', '" +
                        "" + d.Merchandised_Product_Color_ID.Replace("'", "''") +"', '" +
                        "" + d.Merch_Color_Start_Date.Replace("'", "''") +"', '" +
                        "" + d.Merch_Color_Name.Replace("'", "''") +"', '" + d.Merch_Color_Number.Replace("'", "''") +"', '" +
                        "" + d.Merchandised_SKU_Number.Replace("'", "''") +"', '" + d.Barcode.Replace("'", "''") +"', '" +
                        "" + d.CcaSkuId.Replace("'", "''") +"', '" + d.Size_UC.Replace("'", "''") +"', '" +
                        "" + d.Roomscene.Replace("'", "''") +"', '" + d.Plate_ID.Replace("'", "''") +"', '" +
                        "" + d.Plate_ID_BL.Replace("'", "''") +"', '" + d.Plate_ID_FL.Replace("'", "''") +"', '" +
                        "" + d.ArtType.Replace("'", "''") +"', '" + d.ArtType_BL.Replace("'", "''") +"', '" +
                        "" + d.ArtType_FL.Replace("'", "''") +"', '" + d.Status.Replace("'", "''") +"', '" +
                        "" + d.Status_FL.Replace("'", "''") +"', '" + d.Change.Replace("'", "''") +"', '" +
                        "" + d.Change_FL.Replace("'", "''") +"', '" + d.Program.Replace("'", "''") +"', '" +
                        "" + d.Output +"', '" + d.Output_FL + "')";
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

                sql = "UPDATE dbo.Details SET Status='" + mkt.Status + "', Status_FL='" + mkt.Status_FL + "', Program='" + mkt.Program + "', Change='" + mkt.Change + "', Change_FL='" + mkt.Change_FL + "'  WHERE Sample_ID='" + mkt.Sample_ID + "'";

                SqlCommand commandU;
                SqlDataReader dataReaderU;


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
                            "'" + s.Sampled_Color_SKU.Replace("'", "''") + "', '" + s.Shared_Card + "', '" + s.Sampled_With_Merch_Product_ID + "', '" + s.Quick_Ship + "', '" + s.Binder + "', " +
                            "'" + s.Border + "', '" + s.Character_Rating_by_Color + "', '" + s.Feeler.Replace("'", "''") + "', '" + s.MSRP + "', '" + s.MSRP_Canada + "', " +
                            "'" + s.Our_Price + "', '" + s.Our_Price_Canada + "', '" + s.RRP_US + "', '" + s.Sampling_Color_Description + "', '" + s.Split_Board.Replace("'", "''") + "', " +
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
                            "'" + s.Our_Price + "', '" + s.Our_Price_Canada + "', '" + s.RRP_US + "', '" + s.Sampling_Color_Description + "', '" + s.Split_Board.Replace("'", "''") + "', " +
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
            string sql = "";
            if (isSoftSurface)
            {
                SqlConnect("CCA-SS");
            }
            else
            {
                SqlConnect("CCA");
            }

            sql = "SELECT DISTINCT Sample_ID, Status, Status_FL, Program, Change, Change_FL FROM dbo.Details";

            SqlCommand commandS;
            SqlDataReader dataReaderS;

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
                    mktSpreadsheetItem.Status_FL = dataReaderS.GetString(dataReaderS.GetOrdinal("Status_FL"));
                    mktSpreadsheetItem.Change = dataReaderS.GetString(dataReaderS.GetOrdinal("Change"));
                    mktSpreadsheetItem.Change_FL = dataReaderS.GetString(dataReaderS.GetOrdinal("Change_FL"));

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
            foreach (LarModels.Details d in larModels.DetailsList.Distinct())
            {
                sql = "DELETE FROM dbo.Details WHERE Sample_ID ='" + d.Sample_ID + "'";

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
            /*if (isSoftSurface)
            {
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
            }
            else
            {*/
            if (x.ToLower().Equals("waitingfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_FL from dbo.Details where (Status_FL='Waiting for Approval' and Art_Type_FL Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("waitingbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_BL from dbo.Details where (Status='Waiting for Approval' and Art_Type_BL Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("rejectedfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_FL from dbo.Details where (Status_FL='Rejected' and Art_Type_FL Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("rejectedbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_BL from dbo.Details where (Status='Rejected' and Art_Type_BL Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("approvedpendfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_FL from dbo.Details where (Status_FL='Approved Pending' and Art_Type_FL Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("approvedpendbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_BL from dbo.Details where (Status='Approved Pending' and Art_Type_BL Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("approvedfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_FL from dbo.Details where (Status_FL LIKE '%Reprint%' OR Status_FL='Approved' and Art_Type_FL Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("approvedbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_BL from dbo.Details where (Status LIKE '%Reprint%' OR Status='Approved' and Art_Type_BL Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("questionsfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_FL from dbo.Details where (Status_FL='Questions' and Art_Type_FL Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("questionsbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_BL from dbo.Details where (Status='Questions' and Art_Type_BL Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("notdonefl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_FL from dbo.Details where (Status_FL='Not Done' and Art_Type_FL Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("notdonebl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_BL from dbo.Details where (Status='Not Done' and Art_Type_BL Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("totalfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_FL from dbo.Details where (Art_Type_FL Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("totalbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_BL from dbo.Details where (Art_Type_BL Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("reprint-waitingfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_FL from dbo.Details where (Status_FL='Reprint-Waiting for Approval' and Art_Type_FL Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("reprint-waitingbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_BL from dbo.Details where (Status='Reprint-Waiting for Approval' and Art_Type_BL Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("reprint-rejectedfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_FL from dbo.Details where (Status_FL='Reprint-Rejected' and Art_Type_FL Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("reprint-rejectedbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_BL from dbo.Details where (Status='Reprint-Rejected' and Art_Type_BL Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("reprint-approvedpendfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_FL from dbo.Details where (Status_FL='Reprint-Approved Pending' and Art_Type_FL Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("reprint-approvedpendbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_BL from dbo.Details where (Status='Reprint-Approved Pending' and Art_Type_BL Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("reprint-approvedfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_FL from dbo.Details where (Status_FL='Reprint-Approved' and Art_Type_FL Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("reprint-approvedbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_BL from dbo.Details where (Status='Reprint-Approved' and Art_Type_BL Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("reprint-questionsfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_FL from dbo.Details where (Status_FL='Reprint-Questions' and Art_Type_FL Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("reprint-questionsbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_BL from dbo.Details where (Status='Reprint-Questions' and Art_Type_BL Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("reprint-notdonefl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_FL from dbo.Details where (Status_FL='Reprint-Not Done' and Art_Type_FL Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("reprint-notdonebl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_BL from dbo.Details where (Status='Reprint-Not Done' and Art_Type_BL Like '%BL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("reprint-totalfl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_FL from dbo.Details where (Status_FL Like '%Reprint%' and Art_Type_FL Like '%FL%' and Program = '" + program + "')) t";
            }
            if (x.ToLower().Equals("reprint-totalbl"))
            {
                sql = "Select COUNT(*) FROM (Select DISTINCT Sample_ID, Art_Type_BL from dbo.Details where (Status Like '%Reprint%' and Art_Type_BL Like '%BL%' and Program = '" + program + "')) t";
            }
            //}

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
