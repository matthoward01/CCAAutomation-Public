using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Linq;

namespace CCAAutomation.Lib
{
    public class LarPdf
    {
        public static void GetInfo(string fileName, string sampleId)
        {
            LarModels.LARXlsSheet lARXlsSheet = Lar.GetLar(fileName);

            List<LarModels.LARFinal> laRFinal = new(Lar.GetLarFinal(lARXlsSheet, sampleId));
            List<string> sampleIdList = new();
            List<LarModels.LARFinal> runList = new();
            foreach (LarModels.LARFinal lf in laRFinal)
            {
                sampleIdList.Add(lf.DetailsFinal.Sample_ID);                
            }
            foreach (string s in sampleIdList.Distinct())
            {
                //runList = laRFinal.Where(l => (l.DetailsFinal.Sample_ID.EqualsString(s) && ((l.DetailsFinal.Division_List.ToLower().Trim().Contains("c1")) || (l.DetailsFinal.Division_List.ToLower().Trim().Contains("fa"))))).DistinctBy(d => d.DetailsFinal.Mfg_Color_Name).ToList();
                runList = laRFinal.Where(l => (l.DetailsFinal.Sample_ID.EqualsString(s) && ((l.DetailsFinal.Division_List.ToLower().Trim().Contains("c1")) || (l.DetailsFinal.Division_List.ToLower().Trim().Contains("fa"))))).ToList();
                if (!runList.Count.Equals(0))
                {
                    CreatePdf(runList, Path.GetDirectoryName(fileName));
                }
            }
        }

        private static void CreatePdf(List<LarModels.LARFinal> larFinal, string export)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            //string gillRFont = "GIL_____.TTF";
            //string gillBFont = "GILB____.TTF";
            string gillRFont = Path.Combine(appDir, "Fonts", @"GIL_____.ttf");
            string gillBFont = Path.Combine(appDir, "Fonts", @"GILB____.ttf");
            //string name = larFinal[0].DetailsFinal.Division_Product_Name;
            string name = larFinal[0].SampleFinal.Sample_Name.Replace("/", "_") + " - " + larFinal[0].SampleFinal.Sample_ID;

            Directory.CreateDirectory(Path.Combine(export, "Lar Pdfs"));
            FileStream fs = new FileStream(Path.Combine(export, "Lar Pdfs", name + ".pdf"), FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            writer.PdfVersion = PdfWriter.VERSION_1_3;
            doc.SetPageSize(new iTextSharp.text.Rectangle(792, 612));
            doc.SetMargins(0, 0, 0, 0);
            doc.Open();
            PdfContentByte cb = writer.DirectContent;
            BaseFont GillSansR = BaseFont.CreateFont(gillRFont, BaseFont.CP1252, BaseFont.EMBEDDED);
            BaseFont GillSansB = BaseFont.CreateFont(gillBFont, BaseFont.CP1252, BaseFont.EMBEDDED);
            Font GillSansRegular = new(GillSansR, 8, Font.NORMAL);
            Font GillSansBold = new(GillSansB, 8, Font.BOLD);
            //doc.NewPage();
            //cb.BeginText();
            //cb.SetFontAndSize(GillSansB, 18);
            //cb.SetTextMatrix(18, 560);
            //cb.ShowText(name);
            //cb.EndText();
            //cb.SetLineWidth(2f);
            //cb.SetGrayStroke(0f);
            //cb.MoveTo(60, 798);
            //cb.LineTo(576, 798);
            //cb.Stroke();
            Paragraph paragraphTable = new Paragraph();
            //paragraphTable.SpacingAfter = 20f;
            PdfPTable table = new PdfPTable(11);
            //table.HeaderRows = 1;
            table.WidthPercentage = 98f;
            float[] widths = new float[] { 1f, 2f, 2f, 2f, 0.75f, 2f, 0.75f, 2f, 0.75f, 1f, 1f };
            table.SetWidths(widths);
            PdfPCell divisionList = new PdfPCell(new Phrase("Division List", GillSansBold));
            table.AddCell(divisionList);

            PdfPCell supplierName = new PdfPCell(new Phrase("Supplier Name", GillSansBold));
            table.AddCell(supplierName);

            PdfPCell supplierProductName = new PdfPCell(new Phrase("Supplier Product Name", GillSansBold));
            table.AddCell(supplierProductName);

            PdfPCell divisionProductName = new PdfPCell(new Phrase("Division Product Name", GillSansBold));
            table.AddCell(divisionProductName);

            PdfPCell numberOfColors = new PdfPCell(new Phrase("Number of Colors", GillSansBold));
            table.AddCell(numberOfColors);

            PdfPCell mfgColorName = new PdfPCell(new Phrase("Mfg Color Name", GillSansBold));
            table.AddCell(mfgColorName);

            PdfPCell mfgColorNumber = new PdfPCell(new Phrase("Mfg Color Number", GillSansBold));
            table.AddCell(mfgColorNumber);

            PdfPCell merchColorName = new PdfPCell(new Phrase("Merch Color Name", GillSansBold));
            table.AddCell(merchColorName);

            PdfPCell merchColorNumber = new PdfPCell(new Phrase("Merch Color Number", GillSansBold));
            table.AddCell(merchColorNumber);

            PdfPCell styleColorCombo = new PdfPCell(new Phrase("Style Color Combo", GillSansBold));
            table.AddCell(styleColorCombo);

            PdfPCell addNumber = new PdfPCell(new Phrase("ADD Number", GillSansBold));
            table.AddCell(addNumber);

            for (int i = 0; i < larFinal.Count; i++)
            {
                string divisionName = larFinal[i].DetailsFinal.Division_List;
                if (larFinal[i].DetailsFinal.Division_List.ToLower().Contains("c1"))
                {
                    divisionName = "Carpet One";
                }
                if (larFinal[i].DetailsFinal.Division_List.ToLower().Contains("fa"))
                {
                    divisionName = "Flooring America";
                }
                PdfPCell divisionListText = new PdfPCell(new Phrase(divisionName, GillSansRegular));
                table.AddCell(divisionListText);

                PdfPCell supplierNameText = new PdfPCell(new Phrase(larFinal[i].DetailsFinal.Supplier_Name, GillSansRegular));
                table.AddCell(supplierNameText);

                PdfPCell supplierProductNameText = new PdfPCell(new Phrase(larFinal[i].DetailsFinal.Supplier_Product_Name, GillSansRegular));
                table.AddCell(supplierProductNameText);

                PdfPCell divisionProductNameText = new PdfPCell(new Phrase(larFinal[i].DetailsFinal.Division_Product_Name, GillSansRegular));
                table.AddCell(divisionProductNameText);

                PdfPCell numberOfColorsText = new PdfPCell(new Phrase(larFinal[i].DetailsFinal.Number_of_Colors, GillSansRegular));
                table.AddCell(numberOfColorsText);

                PdfPCell mfgColorNameText = new PdfPCell(new Phrase(larFinal[i].DetailsFinal.Mfg_Color_Name, GillSansRegular));
                table.AddCell(mfgColorNameText);

                PdfPCell mfgColorNumberText = new PdfPCell(new Phrase(larFinal[i].DetailsFinal.Mfg_Color_Number, GillSansRegular));
                table.AddCell(mfgColorNumberText);

                PdfPCell merchColorNameText = new PdfPCell(new Phrase(larFinal[i].DetailsFinal.Merch_Color_Name, GillSansRegular));
                table.AddCell(merchColorNameText);

                PdfPCell merchColorNumberText = new PdfPCell(new Phrase(larFinal[i].DetailsFinal.Merch_Color_Number, GillSansRegular));
                table.AddCell(merchColorNumberText);

                PdfPCell styleColorComboText = new PdfPCell(new Phrase(larFinal[i].DetailsFinal.Style_Color_Combo, GillSansRegular));
                table.AddCell(styleColorComboText);

                PdfPCell addNumberText = new PdfPCell(new Phrase(larFinal[i].DetailsFinal.ADDNumber, GillSansRegular));
                table.AddCell(addNumberText);
                
            }
            table.SpacingBefore = 8f;
            table.SpacingAfter = 8f;
            paragraphTable.Add("    " + DateTime.Now.ToString());

            doc.Add(paragraphTable);
            doc.Add(table);

            doc.Close();
        }
    }  
}


