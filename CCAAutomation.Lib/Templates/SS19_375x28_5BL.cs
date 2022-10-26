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
    public class SS19_375x28_5BL
    {
        private static List<Labels> GetImages(List<Labels> inLabels)
        {
            List<Labels> workingList = new List<Labels>(inLabels);
            Labels sortLabels = new Labels();
            List<Labels> newWorkingList = new List<Labels>();

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
                    sortLabels = new Labels();
               //}
            }

            workingList = new List<Labels>();
            newWorkingList.Sort((x, y) => x.Priority.CompareTo(y.Priority));

            return newWorkingList;
        }

        private static List<string> GetSpecList(Details details, List<string> ccaSkuId, List<string> widthList)
        {
            List<string> specs = new List<string>();
            //Spec 1
            specs.Add(details.Primary_Display);

            //Spec 2
            specs.Add(details.Pile_Line);

            if (details.Match.Trim().ToLower().Equals("random"))
            {
                //Spec 3
                specs.Add("Random");
            }
            else if (!details.Match.Trim().ToLower().Equals("none") && !details.Match.Trim().ToLower().Equals("null") && !details.Match.EqualsString(""))
            { 
                string width = details.Match_Width;
                //decimal widthD = decimal.Parse(details.Width);
                bool successW = decimal.TryParse(details.Width, out decimal widthD);
                if (successW)
                {
                    if (width.EndsWith('0'))
                    {
                        width = widthD.ToString("0.##");
                    }
                }
                width = width.Replace(".00", "") + "\" W<!--Match_Width-->";


                string length = details.Match_Length;
                //decimal lengthD = decimal.Parse(details.Length);
                bool successL = decimal.TryParse(length, out decimal lengthD);
                if (successL)
                {
                    if (length.EndsWith('0'))
                    {
                        length = lengthD.ToString("0.##");
                    }
                    length = length.ToLower().Replace(".00", "") + "\" L<!--Match_Length-->";
                }

                //Spec 3
                if (width.EqualsString(""))
                {
                    specs.Add(length + " " + details.Match);
                }
                else if (length.EqualsString(""))
                {
                    specs.Add(width + " " + details.Match);
                }
                else
                {
                    specs.Add(width + " x " + length + " " + details.Match);
                }
            }            
            else
            {
                //Spec 3
                specs.Add("None");
            }

            //Spec 4
            string[] widthSize = widthList.Distinct().ToArray();
            if (widthSize.Count().Equals(1))
            {
                specs.Add(widthSize[0]);
            }
            if (widthSize.Count().Equals(2))
            {
                specs.Add(widthSize[0] + " & " + widthSize[1]);
            }

            //Spec 5
            specs.Add(details.ADDNumber);

            foreach (string sku in ccaSkuId.Distinct())
            {
                //Spec 6 (7)
                specs.Add(sku);
            }

            return specs;
        }        

        public static (List<string>, string[]) CreateXMLSS19_375x28_5BL(string[] files, bool skip, bool goWorkShop, List<LARFinal> lARFinal, string plateId, string export)
        {
            //goWorkShop = true;
            List<string> missingImages = new List<string>();
            List<string> colorList = new List<string>();
            List<string> ccaSkuId = new List<string>();
            List<string> widthList = new();
            List<int> featurePositionList = new();
            List<Swatches.SwatchColors> swatchColors = new List<Swatches.SwatchColors>();
            List<CCASkuIdModel> ccaSkuIdList = new();
            CCASkuIdModel cCASkuIdModel = new();
            bool error = false;
            string twoFeature = "";

            int featurePosition = 0;

            foreach (LARFinal lf in lARFinal)
            {
                Swatches.SwatchColors colors = new Swatches.SwatchColors();
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
                            if (widthList.Distinct().Count() < (1))
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
                    if (style[1].Trim().ToLower().Equals(lf.DetailsFinal.Division_Product_Name.Trim().ToLower()))
                    {
                        if (lf.DetailsFinal.Merch_Color_Name.Trim().ToLower().Equals(lf.SampleFinal.Feeler.Trim().ToLower()))
                        {
                            if (widthList.Distinct().Count() < (1))
                            {
                                cCASkuIdModel.Position = "02";
                                cCASkuIdModel.CCASkuId = lf.DetailsFinal.CcaSkuId;
                                ccaSkuIdList.Add(cCASkuIdModel);
                            }
                            else
                            {
                                if (lf.DetailsFinal.Size_Name.Contains("12"))
                                {
                                    cCASkuIdModel.Position = "02";
                                    cCASkuIdModel.CCASkuId = lf.DetailsFinal.CcaSkuId;
                                    ccaSkuIdList.Add(cCASkuIdModel);
                                }
                            }
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
                            if (widthList.Distinct().Count() < (1))
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
                    else
                    {
                        if (lf.DetailsFinal.Merch_Color_Name.Trim().ToLower().Equals(lf.SampleFinal.Feeler.Trim().ToLower()))
                        {
                            if (widthList.Distinct().Count() < (1))
                            {
                                cCASkuIdModel.Position = "02";
                                cCASkuIdModel.CCASkuId = lf.DetailsFinal.CcaSkuId;
                                ccaSkuIdList.Add(cCASkuIdModel);
                            }
                            else
                            {
                                if (lf.DetailsFinal.Size_Name.Contains("12"))
                                {
                                    cCASkuIdModel.Position = "02";
                                    cCASkuIdModel.CCASkuId = lf.DetailsFinal.CcaSkuId;
                                    ccaSkuIdList.Add(cCASkuIdModel);
                                }
                            }
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
            TemplateModel settings = GetTemplateSettings("SS19.375x28.5BL", "Normal");
            string sharedFeature = lARFinal[0].SampleFinal.Shared_Card.ToLower().Trim();
            if (sharedFeature.Equals("yes"))
            {
                settings = GetTemplateSettings("SS19.375x28.5BL", "Feature");
                twoFeature = " wFeat";
            }
            
            string mainPath = settings.WebShopPath;
            string template = settings.WebShopPath + settings.Name;
            string jobName = lARFinal[0].DetailsFinal.Plate_ID;
            if (jobName.Equals(""))
            {
                jobName = lARFinal[0].DetailsFinal.Sample_ID;
            }
            string userEmail = settings.UserEmail;
            string snippetPath = mainPath + settings.SnippetPath;
            string imagesPath = mainPath + settings.ImagesPath;
            string roomScenePath = mainPath + settings.RoomscenePath;
            if (goWorkShop)
            {
                roomScenePath = "WebShop:webshop roomscenes:";
            }
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

            string roomScene = "FPOwaitingonroom.tif";
            foreach (string s in lARFinal[0].SampleFinal.Merchandised_Product_Color_ID_C1)
            {
                var result = GetSyncedRoomscenes(files, s, roomScene);
                roomScene = result.Item1;
                files = result.Item2;
            }
            if (roomScene.Contains("FPOwaitingonroom"))
            {
                foreach (string s in lARFinal[0].SampleFinal.Merchandised_Product_Color_ID_FA)
                {
                    var result = GetSyncedRoomscenes(files, s, roomScene);
                    roomScene = result.Item1;
                    files = result.Item2;
                }
            }
            foreach (string s in files)
            {
                foreach (int i in featurePositionList)
                {
                    if (!lARFinal[i].DetailsFinal.Merchandised_Product_Color_ID.EqualsString(""))
                    {
                        if (Path.GetFileName(s).StartsWith(lARFinal[i].DetailsFinal.Merchandised_Product_Color_ID))
                        {
                            roomScene = Path.GetFileName(s);
                        }
                    }
                }
            }
            if (!lARFinal[0].DetailsFinal.Roomscene.Trim().Equals(""))
            {
                roomScene = lARFinal[0].DetailsFinal.Roomscene + "<!--Roomscene-->";
            }
            if (roomScene.EqualsString("FPOwaitingonroom.tif"))
            {
                //files = ApprovedRoomscenes();
                foreach (string s in files)
                {
                    foreach (string r in lARFinal[0].SampleFinal.Merchandised_Product_Color_ID_C1)
                    {
                        if (!r.EqualsString(""))
                        {
                            if (Path.GetFileName(s).StartsWith(r))
                            {
                                roomScene = Path.GetFileName(s);
                            }
                        }
                    }
                    foreach (string r in lARFinal[0].SampleFinal.Merchandised_Product_Color_ID_FA)
                    {
                        if (!r.EqualsString(""))
                        {
                            if (Path.GetFileName(s).StartsWith(r))
                            {
                                roomScene = Path.GetFileName(s);
                            }
                        }
                    }
                }
            }
            //else
            //{
            //    Console.WriteLine("Error getting roomscene. Feature position exceeds color count.");
            //}
            if (skip)
            {
                roomScene = "FPOwaitingonroom.tif" + "<!--Roomscene skipped-->";
            }            

            string styleName = ConvertToTitleCase(lARFinal[0].SampleFinal.Sample_Name);
            string feeler = ConvertToTitleCase(lARFinal[0].SampleFinal.Feeler);
            
            List<string> specList = GetSpecList(lARFinal[0].DetailsFinal, ccaSkuId, widthList);
            List<string> xmlData = new List<string>();

            xmlData.Add("<jobs>");
            xmlData.Add("	<job>");
            xmlData.Add("		<template useremail=\"" + userEmail + "\">" + CheckForMissing(template, missingImages) + "</template>");
            xmlData.Add("		<output jobname=\"" + jobName + "\">Webshop:InputPDF</output>");
            xmlData.Add("		<snippets>");
            xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(snippetPath + snippetWarranties, missingImages) + "</snippet>");
            /*if (division.Trim().ToLower().Equals("fa"))
            {
                xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(snippetPath + "FlooringAmerica.idms", missingImages) + "</snippet>");
            }
            else if(division.Trim().ToLower().Equals("c1"))
            {
                xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(snippetPath + "CarpetOne.idms", missingImages) + "</snippet>");
            }*/
            if (division.Trim().ToLower().Equals("fa") && lARFinal[0].SampleFinal.Shared_Card.Trim().ToLower().Equals("yes"))
            {
                xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(snippetPath + "FlooringAmerica w Feat.idms", missingImages) + "</snippet>");
            }
            else if (division.Trim().ToLower().Equals("c1") && lARFinal[0].SampleFinal.Shared_Card.Trim().ToLower().Equals("yes"))
            {
                xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(snippetPath + "CarpetOne w Feat.idms", missingImages) + "</snippet>");
            }
            else if (division.Trim().ToLower().Equals("fa"))
            {
                xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(snippetPath + "FlooringAmerica.idms", missingImages) + "</snippet>");
            }
            else if (division.Trim().ToLower().Equals("c1"))
            {
                xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(snippetPath + "CarpetOne.idms", missingImages) + "</snippet>");
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
            xmlData.Add("			<text type=\"stylename" + count + "\">" + styleName + "<!--SampleFinal.Sample_Name--></text>");
            if (lARFinal[0].SampleFinal.Shared_Card.ToLower().Trim().Equals("yes"))
            {
                xmlData.Add("           <text type=\"topcolor1\">" + feeler + "<!--SampleFinal.Feeler--></text>");
                xmlData.Add("           <text type=\"topcolor2\">" + feeler + "<!--SampleFinal.Feeler--></text>");
            }
            else
            {
                xmlData.Add("           <text type=\"topcolor1\">" + feeler + "<!--SampleFinal.Feeler--></text>");
            }

            xmlData.Add("		</texts>");
            xmlData.Add("		<graphics>");
            xmlData.Add("			<roomscenes>");
            try
            {
                if (!skip)
                {                    
                    var result = CopyRoomscene(files, skip, roomScenePath + roomScene, settings.WebShopPath + settings.RoomscenePath);
                    skip = result.Item1;
                    files = result.Item2;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Something happened copying roomscene.");
                Console.WriteLine(e.Message);
            }

            xmlData.Add("				<roomscene>" + CheckForMissing(roomScenePath + roomScene, missingImages) + "</roomscene>");
            xmlData.Add("			</roomscenes>");
            xmlData.Add("			<images>");

            List<string> usedIconList = new List<string>();  

            foreach (Labels i in GetImages(lARFinal[0].LabelsFinal))
            {
                string divisionPath = "";
                if (!usedIconList.Contains(i.Division_Label_Name))
                {
                    if (!i.Division_Label_Type.Trim().ToLower().Equals("wbug"))
                    {
                        if (i.Division_Label_Name.Trim().ToLower().Contains("fiber") && division.ToLower().Equals("c1"))
                        {
                            divisionPath = "Carpet One:C1 - ";
                        }
                        if (i.Division_Label_Name.Trim().ToLower().Contains("fiber") && division.ToLower().Equals("fa"))
                        {
                            divisionPath = "Flooring America:FA - ";
                        }
                        if (!i.Division_Label_Name.ToLower().Contains("residential"))
                        {
                            xmlData.Add("		    	<image>" + CheckForMissing(imagesPath + divisionPath + XmlRemapping(i.Division_Label_Name, "Images"), missingImages) + "</image>");
                        }
                        usedIconList.Add(i.Division_Label_Name);
                    }
                }
            }
            xmlData.Add("			</images>");
            xmlData.Add("			<inlines>");
            xmlData.Add("			</inlines>");
            xmlData.Add("		</graphics>");

            xmlData.AddRange(Swatches.SwatchXML(feeler, colorList, "1", settings.Id+twoFeature));

            xmlData.Add("		<JobName>" + jobName + "</JobName>");
            xmlData.Add("		<JobTemplate>" + settings.PreJobTemplateName + "</JobTemplate>");
            xmlData.Add("		<JobGroup>" + settings.PreJobPath + "</JobGroup>");
            xmlData.Add("		<inputfiles>");
            xmlData.Add("			<string>\\\\MAG1PVSF7\\WebShop\\InputPDF\\" + jobName + ".pdf</string>");
            xmlData.Add("		</inputfiles>");
            xmlData.Add("		<indd>");
            xmlData.Add("			<string>\\\\MAG1PVSF7\\WebShop\\InputPDF\\" + jobName + ".indd</string>");
            xmlData.Add("		</indd>");
            xmlData.AddRange(InsiteXMLSnippet(" - ss", "BL", lARFinal[0].DetailsFinal.Supplier_Name, lARFinal[0].SampleFinal.Sample_Name, styleName, ConvertToTitleCase(lARFinal[0].SampleFinal.Feeler), Path.GetFileNameWithoutExtension(template.Replace(":", "\\"))));
            xmlData.Add("	</job>");
            xmlData.Add("</jobs>");

            if ((!template.Equals("")) && (!error) && (!roomScene.Contains("FPOwaitingonroom")) && !lARFinal[0].DetailsFinal.ADDNumber.EqualsString(""))
            {
                if (goWorkShop)
                {
                    ExportXML(jobName, xmlData, export, "WorkShop XML");
                    goWorkShop = false;
                    using (StreamWriter textFile = new StreamWriter(Path.Combine(export, "Roomscene Matchups.txt"), append:true))
                    {
                        textFile.WriteLine(jobName + "|" + styleName + "|" + "Yes" + "|" + lARFinal[0].DetailsFinal.Supplier_Name + "|" + roomScene);
                    }
                }
                else
                {
                    ExportXML(jobName, xmlData, export, "WebShop XML");
                    CreateXMLSS19_375x28_5BL(files, skip, true, lARFinal, plateId, export);
                    Console.WriteLine("--------------------------------------------");
                }
            }
            else if (error && !roomScene.Contains("FPOwaitingonroom"))
            {
                Console.WriteLine("Color Sequence Missing or Incomplete, but we have Roomscene");
                using (StreamWriter textFile = new StreamWriter(Path.Combine(export, "Roomscenes no Sequence.txt"), append:true))
                {
                    textFile.WriteLine(jobName + "|" + styleName + "|" + "No" + "|" + lARFinal[0].DetailsFinal.Supplier_Name + "|" + roomScene);
                }
            }
            else if (error)
            {
                Console.WriteLine("Color Sequence Missing or Incomplete");
            }
            else if (template.EqualsString(""))
            {
                Console.WriteLine("Template Blank.");
            }
            else if (roomScene.Contains("FPOwaitingonroom"))
            {
                Console.WriteLine("Roomscene Missing.");
            }
            else if (lARFinal[0].DetailsFinal.ADDNumber.EqualsString(""))
            {
                Console.WriteLine("ADD Number is Blank.");
            }

            return (missingImages, files);
        }
    }
}