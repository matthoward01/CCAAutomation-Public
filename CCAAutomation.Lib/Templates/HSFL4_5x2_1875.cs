using System.Collections.Generic;
using static CCAAutomation.Lib.LarModels;
using static CCAAutomation.Lib.CommonMethods;
using static CCAAutomation.Lib.XmlMethods;
using static CCAAutomation.Lib.Settings;
using System;
using System.IO;

namespace CCAAutomation.Lib
{
    public class HSFL4_5x2_1875
    {

        /// <summary>
        /// Sort the Icons
        /// </summary>
        /// <param name="inLabels">List of label models</param>
        /// <returns>Returns the sorted label model.</returns>
        private static List<Labels> GetInlines(List<Labels> inLabels)
        {
            List<Labels> workingList = new List<Labels>(inLabels);
            Labels sortLabels = new Labels();
            List<Labels> newWorkingList = new List<Labels>();

            foreach (Labels l in workingList)
            {
                if (!l.Division_Label_Name.Contains(":"))
                {
                    sortLabels.Division_Label_Name = XmlMethods.XmlIconPath(l.Division_Label_Name, "Inlines") + XmlMethods.XmlRemapping(l.Division_Label_Name, "Inlines");
                    sortLabels.Division_Label_Type = l.Division_Label_Type;
                    sortLabels.Merchandised_Product_ID = l.Merchandised_Product_ID;
                    sortLabels.Priority = XmlMethods.XmlIconOrder(l.Division_Label_Name, "Inlines");
                    sortLabels.Sample_ID = l.Sample_ID;
                    if (!sortLabels.Division_Label_Name.ToLower().Contains("h2o"))
                    {
                        newWorkingList.Add(sortLabels);
                    }
                    sortLabels = new Labels();
                }
            }
            workingList = new List<Labels>();
            newWorkingList.Sort((x, y) => x.Priority.CompareTo(y.Priority));

            return newWorkingList;
        }

        private static List<string> GetSpecList(Details details)
        {
            List<string> specs = new List<string>();
            
            /*if (details.Width.Trim().Equals("") && details.Width_Measurement.ToLower().Equals("random"))
            {
                string[] widthSplit = width.ToLower().Split('x');
                width = widthSplit[0].Replace(",", ", ");
                width = CommonMethods.ConvertToTitleCase(width) + "<!--Size_Name-->";
            }
            else
            {
                width = details.Width;
                decimal widthD = decimal.Parse(details.Width);
                if (width.EndsWith('0'))
                {
                    width = widthD.ToString("0.##");
                }
                width = width.Replace(".00", "") + "\"<!--Widths-->";
            }

            string length = details.Length;
            if (details.Length.Equals(""))
            {
                length = details.Length_Measurement + "<!--Length_Measurement-->";
            }
            else
            {                
                decimal lengthD = decimal.Parse(details.Length);
                if (length.EndsWith('0'))
                {
                    length = lengthD.ToString("0.##");
                }
                length = length.ToLower().Replace(".00", "") + "\"<!--Length-->";
            }
            specs.Add(width + " x " + length);*/
            if (details.Width_Measurement.ToLower().Trim().Equals("multi"))
            {
                if (details.Size_Name.ToLower().Contains("x"))
                {
                    string[] splitSizeName = details.Size_Name.Split('x');

                    details.Size_Name = "Multi" + " x " + splitSizeName[1].Trim();
                }
            }

            //string width = details.Size_Name;
            specs.Add(details.Size_Name.Replace("x", " x ").Replace("X", "x").Replace(",", ", ").Replace("  ", " ") + "<!--Size_Name-->");
                     
            specs.Add(details.CcaSkuId + "<!--CCASKUID-->");

            return specs;
        }

        public static List<string> CreateXMLHS4_5x2_1875(bool goWorkShop, LARFinal lARFinal, string plateId, string export)
        {
            //goWorkShop = true;
            List<string> missingImages = new List<string>();
            TemplateModel settings = GetTemplateSettings(XmlRemapping(lARFinal.DetailsFinal.Division_List, "Divisions"), "FL");
            string mainPath = settings.WebShopPath;
            string template = settings.WebShopPath + settings.Name;
            string jobName = lARFinal.DetailsFinal.Plate_ID;
            if (jobName.Equals(""))
            {
                jobName = lARFinal.DetailsFinal.Sample_ID;
            }
            string userEmail = settings.UserEmail;
            string snippetPath = mainPath + settings.SnippetPath;

            string imagesPath = mainPath + settings.ImagesPath;
            string category = XmlRemapping(lARFinal.DetailsFinal.Taxonomy.ToLower(), "Categories");
            string snippetCategory = "category:" + category + ".idms" + "<!--Taxonomy-->";

            string snippetWarranties = "rating:" + XmlRemapping(lARFinal.DetailsFinal.Division_Rating.ToLower(), "Ratings") + " " + category + ".idms" + "<!--Division_Rating-->";
            string styleName = ConvertToTitleCase(lARFinal.SampleFinal.Sample_Name.Trim());
            List<string> specList = GetSpecList(lARFinal.DetailsFinal);
            List<string> xmlData = new List<string>();

            xmlData.Add("<jobs>");
            xmlData.Add("	<job>");
            xmlData.Add("		<template useremail=\"" + userEmail + "\">" + CheckForMissing(template, missingImages) + "</template>");
            xmlData.Add("		<output jobname=\"" + jobName + "\">Webshop:InputPDF</output>");
            xmlData.Add("		<snippets>");
            xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(snippetPath + snippetCategory, missingImages) + "</snippet>");
            xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(snippetPath + snippetWarranties, missingImages) + "</snippet>");
            xmlData.Add("		</snippets>");
            xmlData.Add("		<texts>");
            xmlData.Add("			<text type=\"spec\">" + SpecLine(specList) + "</text>");
            int count = 1;
            foreach (string s in specList)
            {
                xmlData.Add("           <text type=\"spec" + count + "\">" + s + "</text>");
                count++;
            }
            count = 1;
            xmlData.Add("			<text type=\"stylename\">" + styleName + "<!--Division_Product_Name--></text>");
            xmlData.Add("			<text type=\"stylename" + count + "\">" + styleName + "<!--Division_Product_Name--></text>");
            xmlData.Add("           <text type=\"topcolor" + count + "\">" + ConvertToTitleCase(lARFinal.DetailsFinal.Merch_Color_Name) + "<!--Merch_Color_Name--></text>");
            xmlData.Add("		</texts>");
            xmlData.Add("		<graphics>");
            xmlData.Add("			<inlines>");
            if (!lARFinal.LabelsFinal.Count.Equals(0))
            {
                foreach (Labels l in GetInlines(lARFinal.LabelsFinal))
                {
                    if (l.Division_Label_Type.ToLower().Trim().Equals("icon"))
                    {
                        xmlData.Add("				<inline>" + CheckForMissing(imagesPath + l.Division_Label_Name, missingImages) + "</inline>" + "<!--Division_Label_Name of Labels Sheet-->");
                    }
                }
            }
            xmlData.Add("			</inlines>");
            xmlData.Add("		</graphics>");
            xmlData.Add("		<JobName>" + jobName + "</JobName>");
            xmlData.Add("		<JobTemplate>" + settings.PreJobTemplateName + "</JobTemplate>");
            xmlData.Add("		<JobGroup>" + settings.PreJobPath + "</JobGroup>");
            xmlData.Add("		<inputfiles>");
            xmlData.Add("			<string>\\\\MAG1PVSF7\\WebShop\\InputPDF\\" + jobName + ".pdf</string>");
            xmlData.Add("		</inputfiles>");
            xmlData.Add("		<indd>");
            xmlData.Add("			<string>\\\\MAG1PVSF7\\WebShop\\InputPDF\\" + jobName + ".indd</string>");
            xmlData.Add("		</indd>");
            xmlData.AddRange(InsiteXMLSnippet("", "FL", lARFinal.DetailsFinal.Supplier_Name, lARFinal.DetailsFinal.Division_List, styleName, ConvertToTitleCase(lARFinal.DetailsFinal.Merch_Color_Name), Path.GetFileNameWithoutExtension(template.Replace(":", "\\"))));
            xmlData.Add("	</job>");
            xmlData.Add("</jobs>");

            if (!template.Equals(""))
            {
                if (goWorkShop)
                {
                    ExportXML(jobName, xmlData, export, "WorkShop XML");
                    //CreateInsiteXML("FL", settings.PreJobTemplateName, settings.PreJobPath, lARFinal.DetailsFinal.Supplier_Name, lARFinal.DetailsFinal.Division_List, export, jobName, styleName, ConvertToTitleCase(lARFinal.DetailsFinal.Merch_Color_Name), Path.GetFileNameWithoutExtension(template.Replace(":", "\\")));
                    goWorkShop = false;
                    //Console.WriteLine("--------------------------------------------");
                }
                else
                {
                    //Console.WriteLine("--------------------------------------------");
                    ExportXML(jobName, xmlData, export, "WebShop XML");
                    CreateXMLHS4_5x2_1875(true, lARFinal, plateId, export);
                }

            }

            return missingImages;
        }
    }
}