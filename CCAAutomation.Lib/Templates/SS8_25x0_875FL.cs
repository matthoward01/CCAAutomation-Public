using System.Collections.Generic;
using static CCAAutomation.Lib.LarModels;
using static CCAAutomation.Lib.CommonMethods;
using static CCAAutomation.Lib.XmlMethods;
using static CCAAutomation.Lib.Settings;
using System;
using System.IO;
using System.Linq;

namespace CCAAutomation.Lib
{
    public class SS8_25x0_875FL
    {
        private static List<Labels> GetImages(List<Labels> inLabels)
        {
            List<Labels> workingList = new(inLabels);
            Labels sortLabels = new();
            List<Labels> newWorkingList = new();

            foreach (Labels l in workingList)
            {
                //if (l.Division_Label_Name.Contains(":"))
                //{
                    sortLabels.Division_Label_Name = XmlIconPath(l.Division_Label_Name, "Images") + XmlRemapping(l.Division_Label_Name, "Images");
                    sortLabels.Division_Label_Type = l.Division_Label_Type;
                    sortLabels.Merchandised_Product_ID = l.Merchandised_Product_ID;
                    sortLabels.Priority = XmlIconOrder(l.Division_Label_Name, "Images");
                    sortLabels.Sample_ID = l.Sample_ID;
                    newWorkingList.Add(sortLabels);
                    sortLabels = new();
               //}
            }

            workingList = new();
            newWorkingList.Sort((x, y) => x.Priority.CompareTo(y.Priority));

            return newWorkingList;
        }

        private static List<string> GetSpecList(Details details, List<string> ccaSkuId, List<string> widthList)
        {
            List<string> specs = new();
            //Spec 1
            foreach (string sku in ccaSkuId.Distinct())
            {
                //Spec 6 (7)
                specs.Add(sku);
            }

            return specs;
        }        

        public static List<string> CreateXMLSS8_25x0_875FL(List<LARFinal> lARFinal, string export, bool forced)
        {
            List<string> missingImages = new();
            List<string> colorList = new();
            List<string> ccaSkuId = new();
            List<string> widthList = new();
            List<int> featurePositionList = new();
            List<Swatches.SwatchColors> swatchColors = new();
            List<CCASkuIdModel> ccaSkuIdList = new();
            CCASkuIdModel cCASkuIdModel = new();
            bool error = false;

            int featurePosition = 0;

            foreach (LARFinal lf in lARFinal)
            {
                Swatches.SwatchColors colors = new();
                colors.Color = lf.DetailsFinal.Merch_Color_Name;                
                colors.ColorSequence = lf.DetailsFinal.Color_Sequence.PadLeft(2,'0');
                swatchColors.Add(colors);

                if (lf.SampleFinal.Sample_Name.Contains("/") && lf.SampleFinal.Shared_Card.Trim().ToLower().Equals("yes"))
                {
                    string[] style = lf.SampleFinal.Sample_Name.Split('/');
                    if (style[0].Trim().ToLower().Equals(lf.DetailsFinal.Division_Product_Name.Trim().ToLower()) && lf.DetailsFinal.Merch_Color_Name.Trim().ToLower().Equals(lf.SampleFinal.Feeler.Trim().ToLower()))
                    {
                        ccaSkuId.Add(lf.DetailsFinal.CcaSkuId);
                    }
                }
                if(lf.DetailsFinal.Merch_Color_Name.EqualsString(lf.SampleFinal.Feeler))
                {
                    featurePositionList.Add(featurePosition); 
                }
                featurePosition++;
                if (lf.DetailsFinal.Color_Sequence.EqualsString(""))
                {
                    error = true;
                }
                widthList.Add(lf.DetailsFinal.Size_Name);
            }
            widthList.Sort();
            foreach (LARFinal lf in lARFinal)
            {
                if (lf.SampleFinal.Sample_Name.Contains("/") && lf.SampleFinal.Shared_Card.Trim().ToLower().Equals("yes"))
                {
                    string[] style = lf.SampleFinal.Sample_Name.Split('/');
                    if (style[0].Trim().ToLower().Equals(lf.DetailsFinal.Division_Product_Name.Trim().ToLower()))
                    {
                        if (lf.DetailsFinal.Merch_Color_Name.Trim().ToLower().Equals(lf.SampleFinal.Feeler.Trim().ToLower()))
                        {
                            if (!widthList.Distinct().Any())
                            {
                                cCASkuIdModel.Position = "01";
                                cCASkuIdModel.CCASkuId = lf.DetailsFinal.CcaSkuId;
                                ccaSkuIdList.Add(cCASkuIdModel);
                            }
                            else
                            {
                                if (lf.DetailsFinal.Size_Name.Contains("12"))
                                {
                                    cCASkuIdModel.Position = "01";
                                    cCASkuIdModel.CCASkuId = lf.DetailsFinal.CcaSkuId;
                                    ccaSkuIdList.Add(cCASkuIdModel);
                                }
                            }
                            //ccaSkuId.Add(lf.DetailsFinal.CcaSkuId);
                        }
                    }
                    cCASkuIdModel = new();
                }
                else if (lf.SampleFinal.Shared_Card.Trim().ToLower().Equals("yes"))
                {
                    if (lf.DetailsFinal.Division_Product_Name.Trim().ToLower().Contains(lf.SampleFinal.Sample_Name.Trim().ToLower()))
                    {
                        if (lf.DetailsFinal.Merch_Color_Name.Trim().ToLower().Equals(lf.SampleFinal.Feeler.Trim().ToLower()))
                        {
                            if (!widthList.Distinct().Any())
                            {
                                cCASkuIdModel.Position = "01";
                                cCASkuIdModel.CCASkuId = lf.DetailsFinal.CcaSkuId;
                                ccaSkuIdList.Add(cCASkuIdModel);
                            }
                            else
                            {
                                if (lf.DetailsFinal.Size_Name.Contains("12"))
                                {
                                    cCASkuIdModel.Position = "01";
                                    cCASkuIdModel.CCASkuId = lf.DetailsFinal.CcaSkuId;
                                    ccaSkuIdList.Add(cCASkuIdModel);
                                }
                            }
                            //ccaSkuId.Add(lf.DetailsFinal.CcaSkuId);
                        }
                    }
                    cCASkuIdModel = new();
                }
                else
                {
                    if (lf.DetailsFinal.Merch_Color_Name.Trim().ToLower().Equals(lf.SampleFinal.Feeler.Trim().ToLower()))
                    {
                        ccaSkuId.Add(lf.DetailsFinal.CcaSkuId);
                    }
                }
            }
            ccaSkuIdList.Sort((x, y) => x.Position.CompareTo(y.Position));
            foreach (CCASkuIdModel sku in ccaSkuIdList)
            {
                ccaSkuId.Add(sku.CCASkuId);
            }
            swatchColors.Sort((x, y) => x.ColorSequence.CompareTo(y.ColorSequence)); 
            foreach (Swatches.SwatchColors c in swatchColors)
            {
                colorList.Add(ConvertToTitleCase(c.Color));
            }
            string division = XmlRemapping(lARFinal[0].DetailsFinal.Division_List, "Divisions");
            TemplateModel settings = GetTemplateSettings("SS8.25x0.875FL", "Normal");            
            
            string mainPath = settings.WebShopPath;
            string template = settings.WebShopPath + settings.Name;
            string jobName = lARFinal[0].DetailsFinal.Plate_ID_FL;
            if (jobName.Equals(""))
            {
                jobName = lARFinal[0].DetailsFinal.Sample_ID;
            }
            string userEmail = settings.UserEmail;
            string snippetPath = mainPath + settings.SnippetPath;
            string imagesPath = mainPath + settings.ImagesPath;
            string warranty = "need warranty";
            //string snippetWarranties = XmlRemapping(lARFinal[0].DetailsFinal.Division_Rating.ToLower(), "Ratings") + ".idms" + "<!--Division_Rating-->";
            foreach (Labels l in lARFinal[0].LabelsFinal.Distinct())
            {
                if (l.Division_Label_Type.Trim().ToLower().Equals("wbug"))
                {
                    warranty = l.Division_Label_Name;                       
                }
            }
            string snippetWarranties = XmlRemapping(warranty.ToLower(), "Ratings") + ".idms";

            string styleName = ConvertToTitleCase(lARFinal[0].SampleFinal.Sample_Name);
            string feeler = ConvertToTitleCase(lARFinal[0].SampleFinal.Feeler);
            
            List<string> specList = GetSpecList(lARFinal[0].DetailsFinal, ccaSkuId, widthList);
            List<string> xmlData = new();

            xmlData.Add("<jobs>");
            xmlData.Add("	<job>");
            xmlData.Add("		<template useremail=\"" + userEmail + "\">" + CheckForMissing(jobName, template, missingImages) + "</template>");
            xmlData.Add("		<output jobname=\"" + jobName + "\">Webshop:InputPDF</output>");
            xmlData.Add("		<snippets>");
            xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + snippetWarranties, missingImages) + "</snippet>");
            if (division.Trim().ToLower().Equals("fa"))
            {
                xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + "FlooringAmerica.idms", missingImages) + "</snippet>");
            }
            else if (division.Trim().ToLower().Equals("c1"))
            {
                xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + "CarpetOne.idms", missingImages) + "</snippet>");
            }
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
            xmlData.Add("			<text type=\"stylename\">" + styleName + "<!--SampleFinal.Sample_Name--></text>");
            xmlData.Add("			<text type=\"stylename1\">" + styleName + "<!--SampleFinal.Sample_Name--></text>");
            xmlData.Add("           <text type=\"topcolor1\">" + feeler + "<!--SampleFinal.Feeler--></text>");
            xmlData.Add("		</texts>");
            xmlData.Add("		<graphics>");
            xmlData.Add("			<inlines>");
            List<string> usedIconList = new();

            foreach (Labels i in GetImages(lARFinal[0].LabelsFinal))
            {
                string divisionPath = "";
                if (!usedIconList.Contains(i.Division_Label_Name) && !i.Division_Label_Type.Trim().ToLower().Equals("logo"))
                {
                    if (!i.Division_Label_Type.Trim().ToLower().Equals("wbug"))
                    {
                        if (!i.Division_Label_Name.ToLower().Contains("residential") && (!i.Division_Label_Name.ToLower().Contains("fiber")))
                        {
                            xmlData.Add("		    	<inline>" + CheckForMissing(jobName, imagesPath + divisionPath + XmlRemapping(i.Division_Label_Name, "Images"), missingImages) + "</inline>");
                        }
                        usedIconList.Add(i.Division_Label_Name);
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
            xmlData.AddRange(InsiteXMLSnippet(" - ss", "FL", lARFinal[0].DetailsFinal.Supplier_Name, lARFinal[0].DetailsFinal.Division_List, styleName, ConvertToTitleCase(lARFinal[0].SampleFinal.Feeler), Path.GetFileNameWithoutExtension(template.Replace(":", "\\"))));
            xmlData.Add("	</job>");
            xmlData.Add("</jobs>");

            List<string> approvedPlateIdsList = new();
            if (!File.Exists(Path.Combine(export, "Approved.txt")))
            {
                using StreamWriter approvedPlateIdsWriter = new(Path.Combine(export, "Approved.txt"));
            }
            using (StreamReader approvedPlateIdsReader = new(Path.Combine(export, "Approved.txt")))
            {
                while (!approvedPlateIdsReader.EndOfStream)
                {
                    approvedPlateIdsList.Add(approvedPlateIdsReader.ReadLine());
                }
            }
            if (forced || ((!template.Equals("")) && (!error) && !approvedPlateIdsList.Any(p => p.EqualsString(jobName))))
            {
                    ExportXML(jobName, xmlData, export, "WorkShop XML");
                    ExportXML(jobName, xmlData, export, "WebShop XML");
            }
            else if (template.EqualsString(""))
            {
                Console.WriteLine("Template Blank.");
            }

            return (missingImages);
        }
    }
}