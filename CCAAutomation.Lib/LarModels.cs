using System;
using System.Collections.Generic;
using System.Text;

namespace CCAAutomation.Lib
{
    public class LarModels
    {
        public class Details
        {
            public string Status { get; set; }
            public string Change { get; set; }
            public string Plate_ID { get; set; }
            public string ArtType { get; set; }
            public string ADDNumber { get; set; }
            public string Sample_ID { get; set; }
            public string Primary_Display { get; set; }
            public string Division_List { get; set; }
            public string Supplier_Name { get; set; }
            public string Child_Supplier { get; set; }
            public string Taxonomy { get; set; }
            public string Supplier_Product_Name { get; set; }
            public string Merchandised_Product_ID { get; set; }
            public string Merch_Prod_Start_Date { get; set; }
            public string Division_Product_Name { get; set; }
            public string Web_Product_Name { get; set; }
            public string Division_Collection { get; set; }
            public string Division_Rating { get; set; }
            public string Product_Type { get; set; }
            public string Product_Class { get; set; }
            public string Is_Web_Product { get; set; }
            public string Sample_Box_Enabled { get; set; }
            public string Number_of_Colors { get; set; }
            public string Made_In { get; set; }
            public string Fiber_Company { get; set; }
            public string Fiber_Brand { get; set; }
            public string Primary_Fiber { get; set; }
            public string Primary_Fiber_Percentage { get; set; }
            public string Second_Fiber { get; set; }
            public string Second_Fiber_Percentage { get; set; }
            public string Third_Fiber { get; set; }
            public string Third_Fiber_Percentage { get; set; }
            public string Percent_BCF { get; set; }
            public string Percent_Spun { get; set; }
            public string Pile_Line { get; set; }
            public string Stain_Treatment { get; set; }
            public string Soil_Treatment { get; set; }
            public string Dye_Method { get; set; }
            public string Face_Weight { get; set; }
            public string Yarn_Twist { get; set; }
            public string Density { get; set; }
            public string Gauge { get; set; }
            public string Pile_Height { get; set; }
            public string Stitches { get; set; }
            public string IAQ_Number { get; set; }
            public string Durability_Rating { get; set; }
            public string Flammability { get; set; }
            public string Static_AATCC134 { get; set; }
            public string NBS_Smoke_Density_ASTME662 { get; set; }
            public string Radiant_Panel_ASTME648 { get; set; }
            public string Installation_Pattern { get; set; }
            public string Hardness_Rating { get; set; }
            public string Species { get; set; }
            public string Appearance { get; set; }
            public string Radiant_Heat { get; set; }
            public string Construction { get; set; }
            public string Edge_Profile { get; set; }
            public string End_Profile { get; set; }
            public string FHA_Class { get; set; }
            public string FHA_Lab { get; set; }
            public string FHA_Type { get; set; }
            public string Finish { get; set; }
            public string Glazed_Hardness { get; set; }
            public string Gloss_Level { get; set; }
            public string Installation_Method { get; set; }
            public string Locking_Type { get; set; }
            public string Shade_Variation { get; set; }
            public string Grade { get; set; }
            public string Is_FHA_Certified { get; set; }
            public string Is_Recommended_Outdoors { get; set; }
            public string Is_Wall_Tile { get; set; }
            public string Wear_Layer { get; set; }
            public string Wear_Layer_Type { get; set; }
            public string Match { get; set; }
            public string Match_Length { get; set; }
            public string Match_Width { get; set; }
            public string Total_Weight { get; set; }
            public string Backing { get; set; }
            public string Merchandise_Brand { get; set; }
            public string Commercial_Rating { get; set; }
            public string Is_Green_Rated { get; set; }
            public string Green_Natural_Sustained { get; set; }
            public string Green_Recyclable_Content { get; set; }
            public string Green_Recycled_Content { get; set; }
            public string Size_Name { get; set; }
            public string Length { get; set; }
            public string Length_Measurement { get; set; }
            public string Width { get; set; }
            public string Width_Measurement { get; set; }
            public string Thickness { get; set; }
            public string Thickness_Measurement { get; set; }
            public string Thickness_Fraction { get; set; }
            public string Manufacturer_Product_Color_ID { get; set; }
            public string Mfg_Color_Name { get; set; }
            public string Mfg_Color_Number { get; set; }
            public string Sample_Box { get; set; }
            public string Sample_Box_Availability { get; set; }
            public string Manufacturer_SKU_Number { get; set; }
            public string Merchandised_Product_Color_ID { get; set; }
            public string Merch_Color_Start_Date { get; set; }
            public string Merch_Color_Name { get; set; }
            public string Manufacturer_Feeler { get; set; }
            public string Merch_Color_Number { get; set; }
            public string Merchandised_SKU_Number { get; set; }
            public string Barcode { get; set; }
            public string CcaSkuId { get; set; }
            public string Size_UC { get; set; }
            public string Supplementary_SKUs { get; set; }
            public string Roomscene { get; set; }
            public string Color_Sequence { get; set; }
            public bool ShowRow { get; set; } = false;
            public bool EditChange { get; set; } = false;
            public List<string> Console { get; set; } = new List<string>();
            public string Program { get; set; }
        }
        public class Sample
        {
            public List<string> Merchandised_Product_Color_ID_FA { get; set; } = new();
            public List<string> Merchandised_Product_Color_ID_C1 { get; set; } = new();
            public string Sample_ID { get; set; }
            public string Sample_Name { get; set; }
            public string Sample_Size { get; set; }
            public string Sample_Type { get; set; }
            public string Sampled_Color_SKU { get; set; }
            public string Shared_Card { get; set; }
            public string Multiple_Color_Lines { get; set; }
            public string Sampled_With_Merch_Product_ID { get; set; }
            public string Quick_Ship { get; set; }
            public string Binder { get; set; }
            public string Border { get; set; }
            public string Character_Rating_by_Color { get; set; }
            public string Feeler { get; set; }
            public string MSRP { get; set; }
            public string MSRP_Canada { get; set; }
            public string Our_Price { get; set; }
            public string Our_Price_Canada { get; set; }
            public string RRP_US { get; set; }
            public string Sampling_Color_Description { get; set; }
            public string Split_Board { get; set; }
            public string Trade_Up { get; set; }
            public string Wood_Imaging { get; set; }
            public string Sample_Note { get; set; }
        }
        public class Labels
        {
            public string Merchandised_Product_ID { get; set; }
            public string Sample_ID { get; set; }
            public string Division_Label_Type { get; set; }
            public string Division_Label_Name { get; set; }
            public string Priority { get; set; }
        }
        public class Warranties
        {
            public string Merchandised_Product_ID { get; set; }
            public string Sample_ID { get; set; }
            public string Provider { get; set; }
            public string Duration { get; set; }
            public string Warranty_Period { get; set; }
            public string Product_Warranty_Type_Code { get; set; }
        }
        public class LARXlsSheet
        {
            public List<Details> DetailsList { get; set; } = new List<Details>();
            public List<Sample> SampleList { get; set; } = new List<Sample>();
            public List<Labels> LabelList { get; set; } = new List<Labels>();
            public List<Warranties> WarrantiesList { get; set; } = new List<Warranties>();
        }
        public class LARFinal
        {
            public Details DetailsFinal { get; set; } = new Details();
            public Sample SampleFinal { get; set; } = new Sample();
            public List<Labels> LabelsFinal { get; set; } = new List<Labels>();
            public List<Warranties> WarrantiesFinal { get; set; } = new List<Warranties>();
        }

        public class CCASkuIdModel
        {
            public string Position { get; set; }
            public string CCASkuId { get; set; }
        }

        public class WebTableItem
        {
            public string Plate_ID { get; set; }
            public string Sample_ID { get; set; }
            public string Style { get; set; }
            public string Status { get; set; }
            public string Art_Type { get; set; }
            public string Change { get; set; }
            public string Merchandised_Product_Color_ID { get; set; }
            public string Shared_Card { get; set; }
            public string Multiple_Color_Lines { get; set; }
            public string SearchTags { get; set; }
            public bool ShowRow { get; set; } = false;
            public bool EditChange { get; set; } = false;
            public List<string> Console { get; set; } = new List<string>();
        }

    }
}
