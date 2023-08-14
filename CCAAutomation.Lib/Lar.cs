using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Text;
using static CCAAutomation.Lib.LarModels;
using static CCAAutomation.Lib.CommonMethods;
using System.Diagnostics;

namespace CCAAutomation.Lib
{
    public class Lar
    {
        /// <summary>
        /// Get the lar info from a hardsurface spreadsheet
        /// </summary>
        /// <param name="fileName">The file and path to the xls file</param>
        /// <returns>Returns a LAR Sheet for processing</returns>
        public static LARXlsSheet GetLar(string fileName)
        {
            LARXlsSheet larXlsSheet = new();            

            IWorkbook wb = new XSSFWorkbook(fileName);

            ISheet sheetDetails = wb.GetSheet("Details");
            ISheet sheetSample = wb.GetSheet("Sample");
            ISheet sheetLabels = wb.GetSheet("Labels");            

            //ISheet sheetDetails = wb.GetSheetAt(0);
            //ISheet sheetSample = wb.GetSheetAt(1);
            //ISheet sheetLabels = wb.GetSheetAt(2);
            //ISheet sheetWarranties = wb.GetSheetAt(3);

            List<string> detailsHeaderList = new(GetHeaderColumns(sheetDetails));
            List<string> sampleHeaderList = new(GetHeaderColumns(sheetSample));
            List<string> labelsHeaderList = new(GetHeaderColumns(sheetLabels));
            //List<string> warrantiesHeaderList = new(GetHeaderColumns(sheetWarranties));
            int countDetails = GetRowCount(sheetDetails);
            int countSample = GetRowCount(sheetSample);
            int countLabels = GetRowCount(sheetLabels);
            Console.WriteLine("-------------------------------------------");
            var timer = new Stopwatch();
            for (int i = 1; i < countDetails; i++)
            {
                timer.Start();
                larXlsSheet.DetailsList.Add(GetDetails(sheetDetails, detailsHeaderList, i));
                decimal progress = (i / (decimal)countDetails) * 100;
                timer.Stop();
                TimeSpan ts = timer.Elapsed;
                double timeLeft = (ts.TotalSeconds * countDetails) - (ts.TotalSeconds * i);
                string value = "s";
                if (timeLeft > 60)
                {
                    timeLeft = timeLeft / 60;
                    value = "m";
                }
                Console.Write("\rReading Details Sheet {0}% | {1:0.000}{2}", (int)Math.Round(progress), timeLeft, value);
                timer = new();
            }
            Console.Write("\rReading Details Sheet 100% | 0.000s\n");
            Console.WriteLine("-------------------------------------------");
            for (int i = 1; i < countSample; i++)
            {
                timer.Start();
                larXlsSheet.SampleList.Add(GetSample(sheetSample, sampleHeaderList, i));
                decimal progress = (i / (decimal)countSample) * 100;
                timer.Stop();
                TimeSpan ts = timer.Elapsed;
                double timeLeft = (ts.TotalSeconds * countDetails) - (ts.TotalSeconds * i);
                string value = "s";
                if (timeLeft > 60)
                {
                    timeLeft = timeLeft / 60;
                    value = "m";
                }
                Console.Write("\rReading Sample Sheet {0}% | {1:0.000}{2}", (int)Math.Round(progress), timeLeft, value);
                timer = new();
            }
            Console.Write("\rReading Sample Sheet 100% | 0.000s\n");
            Console.WriteLine("-------------------------------------------");
            for (int i = 1; i < countLabels; i++)
            {
                timer.Start();
                larXlsSheet.LabelList.Add(GetLabels(sheetLabels, labelsHeaderList, i));
                decimal progress = (i / (decimal)countLabels) * 100;
                timer.Stop();
                TimeSpan ts = timer.Elapsed;
                double timeLeft = (ts.TotalSeconds * countDetails) - (ts.TotalSeconds * i);
                string value = "s";
                if (timeLeft > 60)
                {
                    timeLeft = timeLeft / 60;
                    value = "m";
                }
                Console.Write("\rReading Labels Sheet {0}% | {1:0.000}{2}", (int)Math.Round(progress), timeLeft, value);
                timer = new();
            }
            Console.Write("\rReading Labels Sheet 100% | 0.000s\n");
            Console.WriteLine("-------------------------------------------");
            /*for (int i = 1; i < GetRowCount(sheetWarranties); i++)
            {
                larXlsSheet.WarrantiesList.Add(GetWarranties(sheetWarranties, warrantiesHeaderList, i));
            }*/

            return larXlsSheet;
        }

        /// <summary>
        /// Get the lar info from a hardsurface sql table
        /// </summary>
        /// <param name="isSoftSurface">Bool: Is the info for HS or SS</param>
        /// <param name="plateId">The platd id of the set to pull.</param>
        /// <returns>Returns a LAR Sheet for processing.</returns>
        public static LARXlsSheet GetLar(bool isSoftSurface, string plateId)
        {
            LARXlsSheet larXlsSheet = new();

            larXlsSheet.DetailsList = SqlMethods.SqlSelectDetails(plateId, isSoftSurface);
            larXlsSheet.LabelList = SqlMethods.SqlSelectLabels(larXlsSheet.DetailsList[0].Sample_ID, isSoftSurface);
            larXlsSheet.SampleList = SqlMethods.SqlSelectSample(larXlsSheet.DetailsList[0].Sample_ID, isSoftSurface);

            return larXlsSheet;
        }

        /// <summary>
        /// Get the details tab of the LAR Spreadsheet.
        /// </summary>
        /// <param name="sheet">The sheet of the spreadsheet to look at.</param>
        /// <param name="detailHeaderList">List of headers for the details tab.</param>
        /// <param name="i">The number of rows in the Tab</param>
        /// <returns>Returns the details of the details tab of a LAR spreadsheet.</returns>
        public static Details GetDetails(ISheet sheet, List<string> detailHeaderList, int i)
        {
            Details details = new();
            details.Plate_ID = GetCell(sheet, i, detailHeaderList.IndexOf("Plate"));
            details.Plate_ID_BL = GetCell(sheet, i, detailHeaderList.IndexOf("Back Label Plate #"));
            details.Plate_ID_FL = GetCell(sheet, i, detailHeaderList.IndexOf("Face Label Plate #"));
            if (details.Plate_ID_BL.EqualsString(""))
            {
                details.Plate_ID_BL = GetCell(sheet, i, detailHeaderList.IndexOf("Blanket Label Plate #"));
            }
            if (details.Plate_ID.EqualsString(""))
            {
                details.Plate_ID = details.Plate_ID_BL;
            }
            if (details.Plate_ID_FL.EqualsString(""))
            {
                details.Plate_ID_FL = GetCell(sheet, i, detailHeaderList.IndexOf("Face Plate Plate #"));
            }
            details.ArtType = GetCell(sheet, i, detailHeaderList.IndexOf("Art_Type"));
            details.ArtType_BL = GetCell(sheet, i, detailHeaderList.IndexOf("Art Type - BL"));
            details.ArtType_FL = GetCell(sheet, i, detailHeaderList.IndexOf("Art Type - FL"));
            if (details.ArtType_BL.EqualsString(""))
            {
                details.ArtType_BL = GetCell(sheet, i, detailHeaderList.IndexOf("Art_Type - BL"));
            }
            if (details.ArtType.EqualsString(""))
            {
                details.ArtType = details.ArtType_BL;
            }
            if (details.ArtType_FL.EqualsString(""))
            {
                details.ArtType_FL = GetCell(sheet, i, detailHeaderList.IndexOf("Art Type - FP"));
            }
            details.Job_Number_BL = GetCell(sheet, i, detailHeaderList.IndexOf("Job Number - BL"));
            details.Job_Number_FL = GetCell(sheet, i, detailHeaderList.IndexOf("Job Number - FL"));
            details.ADDNumber = GetCell(sheet, i, detailHeaderList.IndexOf("ADDNumber"));
            details.Layout = GetCell(sheet, i, detailHeaderList.IndexOf("Layout"));
            details.Appearance = GetCell(sheet, i, detailHeaderList.IndexOf("Appearance"));
            details.Barcode = GetCell(sheet, i, detailHeaderList.IndexOf("Barcode"));
            details.Program = GetCell(sheet, i, detailHeaderList.IndexOf("Program"));
            details.Change = GetCell(sheet, i, detailHeaderList.IndexOf("Change"));
            details.CcaSkuId = GetCell(sheet, i, detailHeaderList.IndexOf("CCASKUID"));
            details.Division_List = GetCell(sheet, i, detailHeaderList.IndexOf("Division_List"));
            details.Division_Product_Name = GetCell(sheet, i, detailHeaderList.IndexOf("Division_Product_Name"));
            details.Division_Rating = GetCell(sheet, i, detailHeaderList.IndexOf("Division_Rating"));
            details.Length = GetCell(sheet, i, detailHeaderList.IndexOf("Length"));
            details.Length_Measurement = GetCell(sheet, i, detailHeaderList.IndexOf("Length_Measurement"));
            details.Manufacturer_Product_Color_ID = GetCell(sheet, i, detailHeaderList.IndexOf("Manufacturer_Product_Color_ID"));
            details.Match = GetCell(sheet, i, detailHeaderList.IndexOf("Match"));
            details.Match_Length = GetCell(sheet, i, detailHeaderList.IndexOf("Match_Length"));
            details.Match_Width = GetCell(sheet, i, detailHeaderList.IndexOf("Match_Width"));
            details.Merchandised_Product_Color_ID = GetCell(sheet, i, detailHeaderList.IndexOf("Merchandised_Product_Color_ID"));
            details.Merch_Color_Name = GetCell(sheet, i, detailHeaderList.IndexOf("Merch_Color_Name"));
            details.Manufacturer_Feeler = GetCell(sheet, i, detailHeaderList.IndexOf("Manufacturer_Feeler"));
            details.Merch_Color_Number = GetCell(sheet, i, detailHeaderList.IndexOf("Merch_Color_Number"));
            details.Mfg_Color_Name = GetCell(sheet, i, detailHeaderList.IndexOf("Mfg_Color_Name"));
            details.Mfg_Color_Number = GetCell(sheet, i, detailHeaderList.IndexOf("Mfg_Color_Number"));
            details.Number_of_Colors = GetCell(sheet, i, detailHeaderList.IndexOf("Number_of_Colors"));
            details.Pile_Line = GetCell(sheet, i, detailHeaderList.IndexOf("Pile_Line"));
            details.Primary_Display = GetCell(sheet, i, detailHeaderList.IndexOf("Primary_Display"));
            details.Product_Class = GetCell(sheet, i, detailHeaderList.IndexOf("Product_Class"));
            details.Product_Type = GetCell(sheet, i, detailHeaderList.IndexOf("Product_Type"));
            details.Roomscene = GetCell(sheet, i, detailHeaderList.IndexOf("Roomscene"));
            details.Sample_ID = GetCell(sheet, i, detailHeaderList.IndexOf("Sample_ID"));
            details.Size_Name = GetCell(sheet, i, detailHeaderList.IndexOf("Size_Name"));
            details.Size_UC = GetCell(sheet, i, detailHeaderList.IndexOf("Size_UC"));
            details.Species = GetCell(sheet, i, detailHeaderList.IndexOf("Species"));
            details.Supplier_Name = GetCell(sheet, i, detailHeaderList.IndexOf("Supplier_Name"));
            details.Taxonomy = GetCell(sheet, i, detailHeaderList.IndexOf("Taxonomy"));
            details.Thickness_Fraction = GetCell(sheet, i, detailHeaderList.IndexOf("Thickness_Fraction"));
            details.Thickness = GetCell(sheet, i, detailHeaderList.IndexOf("Thickness"));
            details.Thickness_Measurement = GetCell(sheet, i, detailHeaderList.IndexOf("Thickness_Measurement"));
            details.Wear_Layer = GetCell(sheet, i, detailHeaderList.IndexOf("Wear_Layer"));
            details.Wear_Layer_Type = GetCell(sheet, i, detailHeaderList.IndexOf("Wear_Layer_Type"));
            details.Width = GetCell(sheet, i, detailHeaderList.IndexOf("Width"));
            details.Width_Measurement = GetCell(sheet, i, detailHeaderList.IndexOf("Width_Measurement"));
            details.Color_Sequence = GetCell(sheet, i, detailHeaderList.IndexOf("Color_Sequence"));
            details.Backing = GetCell(sheet, i, detailHeaderList.IndexOf("Backing"));
            details.Child_Supplier = GetCell(sheet, i, detailHeaderList.IndexOf("Child_Supplier"));
            details.Commercial_Rating = GetCell(sheet, i, detailHeaderList.IndexOf("Commercial_Rating"));
            details.Construction = GetCell(sheet, i, detailHeaderList.IndexOf("Construction"));
            details.Density = GetCell(sheet, i, detailHeaderList.IndexOf("Density"));
            details.Division_Collection = GetCell(sheet, i, detailHeaderList.IndexOf("Division_Collection"));
            details.Durability_Rating = GetCell(sheet, i, detailHeaderList.IndexOf("Durability_Rating"));
            details.Dye_Method = GetCell(sheet, i, detailHeaderList.IndexOf("Dye_Method"));
            details.Edge_Profile = GetCell(sheet, i, detailHeaderList.IndexOf("Edge_Profile"));
            details.End_Profile = GetCell(sheet, i, detailHeaderList.IndexOf("End_Profile"));
            details.Face_Weight = GetCell(sheet, i, detailHeaderList.IndexOf("Face_Weight"));
            details.FHA_Class = GetCell(sheet, i, detailHeaderList.IndexOf("FHA_Class"));
            details.FHA_Lab = GetCell(sheet, i, detailHeaderList.IndexOf("FHA_Lab"));
            details.FHA_Type = GetCell(sheet, i, detailHeaderList.IndexOf("FHA_Type"));
            details.Fiber_Brand = GetCell(sheet, i, detailHeaderList.IndexOf("Fiber_Brand"));
            details.Fiber_Company = GetCell(sheet, i, detailHeaderList.IndexOf("Fiber_Company"));
            details.Finish = GetCell(sheet, i, detailHeaderList.IndexOf("Finish"));
            details.Flammability = GetCell(sheet, i, detailHeaderList.IndexOf("Flammability"));
            details.Gauge = GetCell(sheet, i, detailHeaderList.IndexOf("Gauge"));
            details.Glazed_Hardness = GetCell(sheet, i, detailHeaderList.IndexOf("Glazed_Hardness"));
            details.Gloss_Level = GetCell(sheet, i, detailHeaderList.IndexOf("Gloss_Level"));
            details.Grade = GetCell(sheet, i, detailHeaderList.IndexOf("Grade"));
            details.Green_Natural_Sustained = GetCell(sheet, i, detailHeaderList.IndexOf("Green_Natural_Sustained"));
            details.Green_Recyclable_Content = GetCell(sheet, i, detailHeaderList.IndexOf("Green_Recyclable_Content"));
            details.Green_Recycled_Content = GetCell(sheet, i, detailHeaderList.IndexOf("Green_Recycled_Content"));
            details.Hardness_Rating = GetCell(sheet, i, detailHeaderList.IndexOf("Hardness_Rating"));
            details.IAQ_Number = GetCell(sheet, i, detailHeaderList.IndexOf("IAQ_Number"));
            details.Installation_Method = GetCell(sheet, i, detailHeaderList.IndexOf("Installation_Method"));
            details.Installation_Pattern = GetCell(sheet, i, detailHeaderList.IndexOf("Installation_Pattern"));
            details.Is_FHA_Certified = GetCell(sheet, i, detailHeaderList.IndexOf("Is_FHA_Certified"));
            details.Is_Green_Rated = GetCell(sheet, i, detailHeaderList.IndexOf("Is_Green_Rated"));
            details.Is_Recommended_Outdoors = GetCell(sheet, i, detailHeaderList.IndexOf("Is_Recommended_Outdoors"));
            details.Is_Wall_Tile = GetCell(sheet, i, detailHeaderList.IndexOf("Is_Wall_Tile"));
            details.Is_Web_Product = GetCell(sheet, i, detailHeaderList.IndexOf("Is_Web_Product"));
            details.Locking_Type = GetCell(sheet, i, detailHeaderList.IndexOf("Locking_Type"));
            details.Made_In = GetCell(sheet, i, detailHeaderList.IndexOf("Made_In"));
            details.Manufacturer_SKU_Number = GetCell(sheet, i, detailHeaderList.IndexOf("Manufacturer_SKU_Number"));
            details.Merchandised_Product_ID = GetCell(sheet, i, detailHeaderList.IndexOf("Merchandised_Product_ID"));
            details.Merchandised_SKU_Number = GetCell(sheet, i, detailHeaderList.IndexOf("Merchandised_SKU_Number"));
            details.Merchandise_Brand = GetCell(sheet, i, detailHeaderList.IndexOf("Merchandise_Brand"));
            details.Merch_Color_Start_Date = GetCell(sheet, i, detailHeaderList.IndexOf("Merch_Color_Start_Date"));
            details.Merch_Prod_Start_Date = GetCell(sheet, i, detailHeaderList.IndexOf("Merch_Prod_Start_Date"));
            details.NBS_Smoke_Density_ASTME662 = GetCell(sheet, i, detailHeaderList.IndexOf("NBS_Smoke_Density_ASTME662"));
            details.Percent_BCF = GetCell(sheet, i, detailHeaderList.IndexOf("Percent_BCF"));
            details.Percent_Spun = GetCell(sheet, i, detailHeaderList.IndexOf("Percent_Spun"));
            details.Pile_Height = GetCell(sheet, i, detailHeaderList.IndexOf("Pile_Height"));
            details.Primary_Fiber = GetCell(sheet, i, detailHeaderList.IndexOf("Primary_Fiber"));
            details.Primary_Fiber_Percentage = GetCell(sheet, i, detailHeaderList.IndexOf("Primary_Fiber_Percentage"));
            details.Radiant_Heat = GetCell(sheet, i, detailHeaderList.IndexOf("Radiant_Heat"));
            details.Radiant_Panel_ASTME648 = GetCell(sheet, i, detailHeaderList.IndexOf("Radiant_Panel_ASTME648"));
            details.Sample_Box = GetCell(sheet, i, detailHeaderList.IndexOf("Sample_Box"));
            details.Sample_Box_Availability = GetCell(sheet, i, detailHeaderList.IndexOf("Sample_Box_Availability"));
            details.Sample_Box_Enabled = GetCell(sheet, i, detailHeaderList.IndexOf("Sample_Box_Enabled"));
            details.Second_Fiber = GetCell(sheet, i, detailHeaderList.IndexOf("Second_Fiber"));
            details.Second_Fiber_Percentage = GetCell(sheet, i, detailHeaderList.IndexOf("Second_Fiber_Percentage"));
            details.Shade_Variation = GetCell(sheet, i, detailHeaderList.IndexOf("Shade_Variation"));
            details.Soil_Treatment = GetCell(sheet, i, detailHeaderList.IndexOf("Soil_Treatment"));
            details.Stain_Treatment = GetCell(sheet, i, detailHeaderList.IndexOf("Stain_Treatment"));
            details.Static_AATCC134 = GetCell(sheet, i, detailHeaderList.IndexOf("Static_AATCC134"));
            details.Stitches = GetCell(sheet, i, detailHeaderList.IndexOf("Stitches"));
            details.Style_Color_Combo = GetCell(sheet, i, detailHeaderList.IndexOf("Style Color Combo"));
            details.Supplementary_SKUs = GetCell(sheet, i, detailHeaderList.IndexOf("Supplementary_SKUs"));
            details.Supplier_Product_Name = GetCell(sheet, i, detailHeaderList.IndexOf("Supplier_Product_Name"));
            details.Total_Weight = GetCell(sheet, i, detailHeaderList.IndexOf("Total_Weight"));
            details.Third_Fiber = GetCell(sheet, i, detailHeaderList.IndexOf("Third_Fiber"));
            details.Third_Fiber_Percentage = GetCell(sheet, i, detailHeaderList.IndexOf("Third_Fiber_Percentage"));
            details.Web_Product_Name = GetCell(sheet, i, detailHeaderList.IndexOf("Web_Product_Name"));
            details.Yarn_Twist = GetCell(sheet, i, detailHeaderList.IndexOf("Yarn_Twist"));

            return details;
        }

        /// <summary>
        /// Get the sample tab of the LAR Spreadsheet.
        /// </summary>
        /// <param name="sheet">The sheet of the spreadsheet to look at.</param>
        /// <param name="sampleHeaderList">List of headers for the sample tab.</param>
        /// <param name="i">The number of rows in the Tab</param>
        /// <returns>Returns the sample list of the sample tab of a LAR spreadsheet.</returns>
        public static Sample GetSample(ISheet sheet, List<string> sampleHeaderList, int i)
        {
            Sample sample = new();
            sample.Feeler = GetCell(sheet, i, sampleHeaderList.IndexOf("Feeler"));
            sample.Multiple_Color_Lines = GetCell(sheet, i, sampleHeaderList.IndexOf("Multiple_Color_Lines"));
            sample.Sample_ID = GetCell(sheet, i, sampleHeaderList.IndexOf("Sample_ID"));
            sample.Sample_Name = GetCell(sheet, i, sampleHeaderList.IndexOf("Sample_Name"));
            sample.Sample_Type = GetCell(sheet, i, sampleHeaderList.IndexOf("Sample_Type"));
            sample.Shared_Card = GetCell(sheet, i, sampleHeaderList.IndexOf("Shared_Card"));

            sample.Binder = GetCell(sheet, i, sampleHeaderList.IndexOf("Binder"));
            sample.Border = GetCell(sheet, i, sampleHeaderList.IndexOf("Border"));
            sample.Character_Rating_by_Color = GetCell(sheet, i, sampleHeaderList.IndexOf("Character_Rating_by_Color"));
            sample.MSRP = GetCell(sheet, i, sampleHeaderList.IndexOf("MSRP"));
            sample.MSRP_Canada = GetCell(sheet, i, sampleHeaderList.IndexOf("MSRP_Canada"));
            sample.Our_Price = GetCell(sheet, i, sampleHeaderList.IndexOf("Our_Price"));
            sample.Our_Price_Canada = GetCell(sheet, i, sampleHeaderList.IndexOf("Our_Price_Canada"));
            sample.Quick_Ship = GetCell(sheet, i, sampleHeaderList.IndexOf("Quick_Ship"));
            sample.RRP_US = GetCell(sheet, i, sampleHeaderList.IndexOf("RRP_US"));
            sample.Sampled_Color_SKU = GetCell(sheet, i, sampleHeaderList.IndexOf("Sampled_Color_SKU"));
            sample.Sampled_With_Merch_Product_ID = GetCell(sheet, i, sampleHeaderList.IndexOf("Sampled_With_Merch_Product_ID"));
            sample.Sample_Note = GetCell(sheet, i, sampleHeaderList.IndexOf("Sample_Note"));
            sample.Sample_Size = GetCell(sheet, i, sampleHeaderList.IndexOf("Sample_Size"));
            sample.Sampling_Color_Description = GetCell(sheet, i, sampleHeaderList.IndexOf("Sampling_Color_Description"));
            sample.Split_Board = GetCell(sheet, i, sampleHeaderList.IndexOf("Split_Board"));
            sample.Trade_Up = GetCell(sheet, i, sampleHeaderList.IndexOf("Trade_Up"));
            sample.Wood_Imaging = GetCell(sheet, i, sampleHeaderList.IndexOf("Wood_Imaging"));

            return sample;
        }

        /// <summary>
        /// Get the Labels tab of the LAR Spreadsheet.
        /// </summary>
        /// <param name="sheet">The sheet of the spreadsheet to look at.</param>
        /// <param name="labelsHeaderList">List of headers for the labels tab.</param>
        /// <param name="i">The number of rows in the Tab</param>
        /// <returns>Returns the labels list of the labels tab of a LAR spreadsheet.</returns>
        public static Labels GetLabels(ISheet sheet, List<string> labelsHeaderList, int i)
        {
            Labels labels = new();
            labels.Division_Label_Name = GetCell(sheet, i, labelsHeaderList.IndexOf("Division_Label_Name"));
            labels.Division_Label_Type = GetCell(sheet, i, labelsHeaderList.IndexOf("Division_Label_Type"));
            labels.Merchandised_Product_ID = GetCell(sheet, i, labelsHeaderList.IndexOf("Merchandised_Product_ID"));
            labels.Sample_ID = GetCell(sheet, i, labelsHeaderList.IndexOf("Sample_ID"));

            return labels;
        }

        /// <summary>
        /// Get the Warranties tab of the LAR Spreadsheet.
        /// </summary>
        /// <param name="sheet">The sheet of the spreadsheet to look at.</param>
        /// <param name="warrantiesHeaderList">List of headers for the warranties tab.</param>
        /// <param name="i">The number of rows in the Tab</param>
        /// <returns>Returns the warranties list of the warranties tab of a LAR spreadsheet.</returns>
        public static Warranties GetWarranties(ISheet sheet, List<string> warrantiesHeaderList, int i)
        {
            Warranties warranties = new();
            warranties.Duration = GetCell(sheet, i, warrantiesHeaderList.IndexOf("Duration"));
            warranties.Merchandised_Product_ID = GetCell(sheet, i, warrantiesHeaderList.IndexOf("Merchandised_Product_ID"));
            warranties.Product_Warranty_Type_Code = GetCell(sheet, i, warrantiesHeaderList.IndexOf("Product_Warranty_Type_Code"));
            warranties.Provider = GetCell(sheet, i, warrantiesHeaderList.IndexOf("Provider"));
            warranties.Sample_ID = GetCell(sheet, i, warrantiesHeaderList.IndexOf("Sample_ID"));
            warranties.Warranty_Period = GetCell(sheet, i, warrantiesHeaderList.IndexOf("Warranty_Period"));

            return warranties;
        }

        /// <summary>
        /// Gets the LAR final for a specific sample based on the plateid.
        /// </summary>
        /// <param name="larXlsSheet">The full lar information.</param>
        /// <param name="plateId">The plate needed to be pulled</param>
        /// <returns>Returns the final lar to be used in building the label.</returns>
        public static List<LARFinal> GetLarFinal(LARXlsSheet larXlsSheet, string plateId, bool isCanada = false)
        {
            LARFinal lARFinal = new();
            List<LARFinal> lARFinalList = new();
            foreach (Details d in larXlsSheet.DetailsList)
            {
                if (d.Plate_ID.Equals(plateId) || d.Plate_ID_FL.EqualsString(plateId))
                {
                    lARFinal.DetailsFinal = d;

                    foreach (Sample s in larXlsSheet.SampleList)
                    {
                        if (d.Sample_ID.Equals(s.Sample_ID))
                        {
                            lARFinal.SampleFinal = s;
                        }
                    }
                    foreach (Labels l in larXlsSheet.LabelList)
                    {
                        if (d.Sample_ID.Equals(l.Sample_ID))
                        {
                            if (l.Division_Label_Type.Equals("ICON") || l.Division_Label_Type.Equals("LOGO") || l.Division_Label_Type.Equals("WBUG"))
                            {
                                lARFinal.LabelsFinal.Add(l);
                            }
                        }
                    }
                    //lARFinal.LabelsFinal = sortIcons(lARFinal.LabelsFinal);
                    /*foreach (Warranties w in larXlsSheet.WarrantiesList)
                    {
                        if (d.Sample_ID.Equals(w.Sample_ID))
                        {
                            lARFinal.WarrantiesFinal.Add(w);
                        }
                    }*/
                    lARFinal = GetMerchandisedProductColorIds(lARFinal, larXlsSheet, isCanada);
                    lARFinalList.Add(lARFinal);
                    lARFinal = new LARFinal();
                }
                else if(plateId.Equals(""))
                {
                    lARFinal.DetailsFinal = d;

                    foreach (Sample s in larXlsSheet.SampleList)
                    {
                        if (d.Sample_ID.Equals(s.Sample_ID))
                        {
                            lARFinal.SampleFinal = s;
                        }
                    }
                    foreach (Labels l in larXlsSheet.LabelList)
                    {
                        if (d.Sample_ID.Equals(l.Sample_ID))
                        {
                            if (l.Division_Label_Type.Equals("ICON") || l.Division_Label_Type.Equals("LOGO") || l.Division_Label_Type.Equals("WBUG"))
                            {
                                lARFinal.LabelsFinal.Add(l);
                            }
                        }
                    }
                    //lARFinal.LabelsFinal = sortIcons(lARFinal.LabelsFinal);
                    /*foreach (Warranties w in larXlsSheet.WarrantiesList)
                    {
                        if (d.Sample_ID.Equals(w.Sample_ID))
                        {
                            lARFinal.WarrantiesFinal.Add(w);
                        }
                    }*/
                    lARFinal = GetMerchandisedProductColorIds(lARFinal, larXlsSheet, isCanada);
                    lARFinalList.Add(lARFinal);
                    lARFinal = new LARFinal();
                }
                
            }

            return lARFinalList;
        }
    }
}
