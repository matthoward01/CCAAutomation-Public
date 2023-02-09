using System.Collections.Generic;
using static CCAAutomation.Lib.LarModels;
using static CCAAutomation.Lib.CommonMethods;
using static CCAAutomation.Lib.XmlMethods;
using static CCAAutomation.Lib.Settings;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CCAAutomation.Lib
{
    public class SS19_375x28_5BL
    {
        public static (List<string> missingItems, string[] updatedFiles) CreateXMLSS19_375x28_5BL(bool forced, bool isShaw, string[] files, bool skip, List<LARFinal> lARFinal, string export)
        {
            List<Details> details = new();
            //goWorkShop = true;
            List<string> missingImages = new();
            List<string> colorList = new();
            List<string> ccaSkuId = new();
            List<string> widthList = new();
            List<int> patternRepeatList = new();
            List<int> featurePositionList = new();
            List<Swatches.SwatchColors> swatchColors = new();
            List<CCASkuIdModel> ccaSkuIdList = new();
            List<int> sequenceList = new();
            CCASkuIdModel cCASkuIdModel = new();
            int colorCount = 0;
            bool seqError = false;
            bool secondStyleError = false;
            string twoFeature = "";
            int featurePosition = 0;

            foreach (LARFinal lf in lARFinal)
            {
                details.Add(lf.DetailsFinal);
                Swatches.SwatchColors colors = new();
                if (!lf.DetailsFinal.Color_Sequence.EqualsString("0") && int.TryParse(lf.DetailsFinal.Color_Sequence, out int result))
                {
                    colors.Color = lf.DetailsFinal.Merch_Color_Name;
                    colors.ColorSequence = lf.DetailsFinal.Color_Sequence.PadLeft(2, '0');
                    swatchColors.Add(colors);
                }
                /*else if (isConv)
                {
                    colors.Color = lf.DetailsFinal.Merch_Color_Name;
                    colors.ColorSequence = lf.DetailsFinal.Color_Sequence.PadLeft(2, '0');
                    swatchColors.Add(colors);
                }*/

                if (lf.SampleFinal.Sample_Name.Contains("/") && lf.SampleFinal.Shared_Card.Trim().ToLower().Equals("yes"))
                {
                    string[] style = lf.SampleFinal.Sample_Name.Split('/');
                    if (style[0].Trim().ToLower().Equals(lf.DetailsFinal.Division_Product_Name.Trim().ToLower()) && lf.DetailsFinal.Merch_Color_Name.Trim().ToLower().Equals(lf.SampleFinal.Feeler.Trim().ToLower()))
                    {
                        ccaSkuId.Add(lf.DetailsFinal.CcaSkuId);
                    }
                }
                if (lf.DetailsFinal.Merch_Color_Name.EqualsString(lf.SampleFinal.Feeler) && lf.DetailsFinal.Manufacturer_Feeler.EqualsString("yes"))
                {
                    featurePositionList.Add(featurePosition);
                }
                featurePosition++;
                if (lf.DetailsFinal.Color_Sequence.EqualsString(""))
                {
                    seqError = true;
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
                    if (style[1].Trim().ToLower().Equals(lf.DetailsFinal.Division_Product_Name.Trim().ToLower()))
                    {
                        if (lf.DetailsFinal.Merch_Color_Name.Trim().ToLower().Equals(lf.SampleFinal.Feeler.Trim().ToLower()))
                        {
                            if (!widthList.Distinct().Any())
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
                else if (lf.SampleFinal.Shared_Card.Trim().ToLower().Equals("yes") || lf.SampleFinal.Multiple_Color_Lines.Trim().ToLower().Equals("yes"))
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
                    else
                    {
                        if (lf.DetailsFinal.Merch_Color_Name.Trim().ToLower().Equals(lf.SampleFinal.Feeler.Trim().ToLower()))
                        {
                            if (!widthList.Distinct().Any())
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
                int.TryParse(c.ColorSequence, out int seq);
                sequenceList.Add(seq);
            }

            bool useSqCrop = false;
            List<string> SqCropList = new();
            
            if (!File.Exists(Path.Combine(export, "UseSqCrop.txt")))
            {
                using StreamWriter SqCropWriter = new(Path.Combine(export, "UseSqCrop.txt"));
            }
            using (StreamReader SqCropReader = new(Path.Combine(export, "UseSqCrop.txt")))
            {
                while (!SqCropReader.EndOfStream)
                {
                    SqCropList.Add(SqCropReader.ReadLine());
                }
            }

            string division = XmlRemapping(lARFinal[0].DetailsFinal.Division_List, "Divisions");
            TemplateModel settings = GetTemplateSettings("SS19.375x28.5BL", "Normal");
            //TemplateModel settingsFL = GetTemplateSettings("SS8_25x0_875FL", "Normal");
            if (!lARFinal[0].SampleFinal.Shared_Card.EqualsString("yes") && lARFinal[0].SampleFinal.Multiple_Color_Lines.EqualsString("yes"))
            {
                settings = GetTemplateSettings("SS19.375x28.5BL", "Combo");
            }
            else if (lARFinal[0].SampleFinal.Shared_Card.EqualsString("yes"))
            {
                settings = GetTemplateSettings("SS19.375x28.5BL", "Feature");
                twoFeature = " wFeat";
            }
            else if (SqCropList.Contains(lARFinal[0].DetailsFinal.Sample_ID))
            {
                settings = GetTemplateSettings("SS19.375x28.5BL", "SqCrop");
                useSqCrop = true;
            }

            string mainPath = settings.WebShopPath;
            string template = settings.WebShopPath + settings.Name;
            string jobName = lARFinal[0].DetailsFinal.Plate_ID;
            if (jobName.Equals(""))
            {
                jobName = lARFinal[0].DetailsFinal.Sample_ID;
            }
            if (isShaw)
            {
                jobName = jobName += " Shaw";
            }
            string userEmail = settings.UserEmail;
            string snippetPath = mainPath + settings.SnippetPath;
            string imagesPath = mainPath + settings.ImagesPath;
            string roomScenePath = "WebShop:webshop roomscenes:";
            //string roomScenePath = mainPath + settings.RoomscenePath;

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
            string styleName2 = ConvertToTitleCase(lARFinal[0].SampleFinal.Split_Board);

            //styleName2 = ConvertToTitleCase(styleName2);
            string feeler = ConvertToTitleCase(lARFinal[0].SampleFinal.Feeler);

            List<string> specList = GetSpecList(lARFinal[0].DetailsFinal, ccaSkuId, widthList);
            string primaryStyleName = "";
            if (styleName.Contains("/"))
            {
                string[] split = styleName.Split('/');
                primaryStyleName = split[0].Trim();
            }
            int primaryPattern = lARFinal.FindIndex(i => i.DetailsFinal.Division_Product_Name.ToLower().Contains(primaryStyleName.ToLower()));
            specList[2] = GetPatternRepeat(lARFinal[primaryPattern].DetailsFinal);

            if (lARFinal[0].SampleFinal.Multiple_Color_Lines.EqualsString("yes") && !lARFinal[0].SampleFinal.Shared_Card.EqualsString("yes"))
            {
                int index = lARFinal.FindIndex(i => i.DetailsFinal.CcaSkuId.EqualsString(specList[5]));
                int index2 = lARFinal.FindIndex(i => (i.DetailsFinal.Division_Product_Name != lARFinal[index].DetailsFinal.Division_Product_Name));

                specList.Add(lARFinal[index2].DetailsFinal.CcaSkuId);
            }

            string roomScene = GetRoomScene(jobName, styleName, export, ref files, skip, lARFinal, featurePositionList);

            List<string> xmlData = new();
            List<string> xmlDataFL = new();

            xmlData.Add("<jobs>");
            xmlData.Add("	<job>");
            xmlData.Add("		<template useremail=\"" + userEmail + "\">" + CheckForMissing(jobName, template, missingImages) + "</template>");
            xmlData.Add("		<output jobname=\"" + jobName + "\">Webshop:InputPDF</output>");
            xmlData.Add("		<snippets>");
            xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + snippetWarranties, missingImages) + "</snippet>");
            if (division.Trim().ToLower().Equals("fa") && (lARFinal[0].SampleFinal.Shared_Card.EqualsString("yes") || lARFinal[0].SampleFinal.Multiple_Color_Lines.EqualsString("yes")))
            {
                xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + "FlooringAmerica w Feat.idms", missingImages) + "</snippet>");
            }
            else if (division.Trim().ToLower().Equals("c1") && (lARFinal[0].SampleFinal.Shared_Card.EqualsString("yes") || lARFinal[0].SampleFinal.Multiple_Color_Lines.EqualsString("yes")))
            {
                xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + "CarpetOne w Feat.idms", missingImages) + "</snippet>");
            }
            else if (division.Trim().ToLower().Equals("fa"))
            {
                if (useSqCrop)
                {
                    xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + "FlooringAmerica Single wSqCrop.idms", missingImages) + "</snippet>");
                }
                else
                {
                    xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + "FlooringAmerica.idms", missingImages) + "</snippet>");
                }
            }
            else if (division.Trim().ToLower().Equals("c1"))
            {
                if (useSqCrop)
                {
                    xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + "CarpetOne Single wSqCrop.idms", missingImages) + "</snippet>");
                }
                else
                {
                    xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + "CarpetOne.idms", missingImages) + "</snippet>");
                }
            }
            foreach (Labels b in lARFinal[0].LabelsFinal.Distinct())
            {
                if (b.Division_Label_Type.ToLower().Trim().Equals("logo") && (lARFinal[0].SampleFinal.Shared_Card.EqualsString("yes") || lARFinal[0].SampleFinal.Multiple_Color_Lines.EqualsString("yes")))
                {
                    xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + XmlRemapping(b.Division_Label_Name + " SS " + lARFinal[0].SampleFinal.Shared_Card, "Images", "path", true), missingImages) + "</snippet>");
                }
                else if (b.Division_Label_Type.ToLower().Trim().Equals("logo"))
                {
                    if (useSqCrop)
                    {
                        xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + XmlRemapping(b.Division_Label_Name + " SS sqcrop", "Images", "path", true), missingImages) + "</snippet>");
                    }
                    else
                    {
                        xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + XmlRemapping(b.Division_Label_Name + " SS", "Images", "path", true), missingImages) + "</snippet>");
                    }
                }
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
            xmlData.Add("			<text type=\"stylename\">" + styleName + "</text>");
            xmlData.Add("			<text type=\"stylename1\">" + styleName + "</text>");
            if (lARFinal[0].SampleFinal.Shared_Card.ToLower().Trim().Equals("yes"))
            {
                xmlData.Add("			<text type=\"stylename2\">" + styleName2 + "</text>");
                xmlData.Add("           <text type=\"topcolor1\">" + feeler + "</text>");



                int index = lARFinal.FindIndex(i => i.DetailsFinal.Division_Product_Name.ToLower().Contains(styleName2.ToLower()));
                if (!specList.Contains(GetPatternRepeat(lARFinal[index].DetailsFinal)))
                {
                    xmlData.Add("           <text type=\"topcolor2\">" + feeler + " - " + GetPatternRepeat(lARFinal[index].DetailsFinal) + "</text>");
                }
                else
                {
                    xmlData.Add("           <text type=\"topcolor2\">" + feeler + "</text>");
                }

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
                    skip = result.Skip;
                    files = result.UpdatedFiles;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Something happened copying roomscene.");
                Console.WriteLine(e.Message);
            }

            xmlData.Add("				<roomscene>" + CheckForMissing(jobName, roomScenePath + roomScene, missingImages) + "</roomscene>");
            int roomsceneLineIndex = xmlData.IndexOf(xmlData.Last());
            MainSettings mainSettings = GetMainSettings();
            //CheckRoomSceneCrop(Path.Combine(OsXPathConversion(mainSettings.WebShopRoomScenes), roomScene));
            xmlData.Add("			</roomscenes>");
            xmlData.Add("			<images>");

            List<string> usedIconList = new();

            foreach (Labels i in GetImages(lARFinal[0].LabelsFinal))
            {
                string divisionPath = "";
                if (!usedIconList.Contains(i.Division_Label_Name))
                {
                    if (!i.Division_Label_Type.Trim().ToLower().Equals("wbug") && !i.Division_Label_Type.Trim().ToLower().Equals("logo"))
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
                            xmlData.Add("		    	<image>" + CheckForMissing(jobName, imagesPath + divisionPath + XmlRemapping(i.Division_Label_Name, "Images"), missingImages) + "</image>");
                        }
                        usedIconList.Add(i.Division_Label_Name);
                    }
                }
            }
            xmlData.Add("			</images>");
            xmlData.Add("			<inlines>");
            xmlData.Add("			</inlines>");
            xmlData.Add("		</graphics>");

            if (!seqError)
            {
                xmlData.AddRange(Swatches.SwatchXML(feeler, colorList, "1", settings.Id + twoFeature, out seqError, out colorCount));
            }

            xmlData.Add("		<JobName>" + jobName + "</JobName>");
            xmlData.Add("		<JobTemplate>" + settings.PreJobTemplateName + "</JobTemplate>");
            xmlData.Add("		<JobGroup>" + settings.PreJobPath + "</JobGroup>");
            xmlData.Add("		<inputfiles>");
            xmlData.Add("			<string>\\\\MAG1PVSF7\\WebShop\\InputPDF\\" + jobName + ".pdf</string>");
            xmlData.Add("		</inputfiles>");
            xmlData.Add("		<indd>");
            xmlData.Add("			<string>\\\\MAG1PVSF7\\WebShop\\InputPDF\\" + jobName + ".indd</string>");
            xmlData.Add("		</indd>");
            xmlData.AddRange(InsiteXMLSnippet(" - ss", "BL", lARFinal[0].DetailsFinal.Supplier_Name, lARFinal[0].DetailsFinal.Division_List, styleName, ConvertToTitleCase(lARFinal[0].SampleFinal.Feeler), Path.GetFileNameWithoutExtension(template.Replace(":", "\\"))));
            xmlData.Add("	</job>");
            xmlData.Add("</jobs>");
            Directory.CreateDirectory(Path.Combine(export, "Reports"));
            bool layoutError = false;
            List<int> sequenceListDistinct = sequenceList.Distinct().ToList();
            bool sequenceSeq = sequenceListDistinct.OrderBy(a => a).Zip(sequenceListDistinct.Skip(1), (a, b) => (a + 1) == b).All(x => x);
            if (sequenceSeq)
            {
                Console.WriteLine("Sequential");
            }
            if (!sequenceSeq)
            {
                Console.WriteLine("NOT SEQUENTIAL");
            }
            if (!forced && int.TryParse(GetColorCount(lARFinal[0].DetailsFinal.Layout), out int colorCountLayout) && !colorCount.Equals(colorCountLayout) || !colorCount.Equals(sequenceList.Distinct().Count()) || !sequenceSeq)
            {
                if (!lARFinal[0].DetailsFinal.Layout.EqualsString(""))
                {
                    Console.WriteLine("Color Sequence Missing, 2nd Style Problem, or Incomplete, but we have Roomscene");
                    using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Possible Layout Problem.txt"), append: true))
                    {
                        textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName + "|" + colorCount + "|" + lARFinal[0].DetailsFinal.Layout);
                    }
                
                    layoutError = true;
                }
            }
            if ((forced || !forced) && (!isShaw && !template.Equals("") && !layoutError && !seqError && !roomScene.Contains("FPOwaitingonroom") && !lARFinal[0].DetailsFinal.ADDNumber.EqualsString("")) ||
                isShaw && !template.EqualsString("") && !seqError && !layoutError)
            {
                ExportXML(jobName, xmlData, export, "WorkShop XML");
                xmlData[roomsceneLineIndex] = xmlData[roomsceneLineIndex].Replace("WebShop:webshop roomscenes:", mainPath + settings.RoomscenePath);
                ExportXML(jobName, xmlData, export, "WebShop XML");

                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Roomscene Matchups.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName + "|" + "Yes" + "|" + lARFinal[0].DetailsFinal.Supplier_Name + "|" + roomScene);
                }
            }
            else if (!roomScene.Contains("FPOwaitingonroom"))
            {
                Console.WriteLine("Roomscene Available");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Roomscenes Available.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName + "|" + "No" + "|" + lARFinal[0].DetailsFinal.Supplier_Name + "|" + roomScene);
                }
            }
            else if (secondStyleError)
            {
                Console.WriteLine("2nd Style Problem.");
            }
            else if (template.EqualsString(""))
            {
                Console.WriteLine("Template Blank.");
            }
            if (!forced && secondStyleError && seqError && !roomScene.Contains("FPOwaitingonroom"))
            {
                Console.WriteLine("Color Sequence Missing, 2nd Style Problem, or Incomplete, but we have Roomscene");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Roomscenes no Sequence.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName + "|" + "No" + "|" + lARFinal[0].DetailsFinal.Supplier_Name + "|" + roomScene);
                }
            }
            if (!forced && seqError)
            {
                Console.WriteLine("Color Sequence Missing.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Color Sequence Missing.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName);
                }
            }
            if (!sequenceSeq)
            {
                Console.WriteLine("Color Sequence Not Sequential.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Color Sequence Not Sequential.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName);
                }
            }
            if (!colorCount.Equals(sequenceList.Distinct().Count()))
            {
                Console.WriteLine("Color Sequence Incomplete.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Color Sequence Incomplete.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName);
                }
            }
            if (!forced && roomScene.Contains("FPOwaitingonroom"))
            {
                Console.WriteLine("Roomscene Missing.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Roomscene Missing.txt"), append: true))
                {
                    foreach (string s in lARFinal[0].SampleFinal.Merchandised_Product_Color_ID_C1)
                    {
                        textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName + "|" + s);
                    }
                    foreach (string s in lARFinal[0].SampleFinal.Merchandised_Product_Color_ID_FA)
                    {
                        textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName + "|" + s);
                    }
                }
                Console.WriteLine("Checking for possible match...");
                SyncRoomsceneDoubleCheck(details, Path.Combine(export, "Reports"));
                details = new();
            }
            if (!forced && lARFinal[0].DetailsFinal.ADDNumber.EqualsString("") || lARFinal[0].DetailsFinal.ADDNumber.EqualsString("NULL"))
            {
                Console.WriteLine("ADD Number is Blank.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "ADD Number is Blank.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName);
                }
            }
            if (!forced && lARFinal[0].DetailsFinal.ADDNumber.EqualsString("NULL"))
            {
                Console.WriteLine("ADD Number is NULL.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "ADD Number is Null.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName);
                }
            }
            if (!forced && colorList.Distinct().Count() > 32)
            {
                Console.WriteLine("Color Count greater than 32.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Color Count too High.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName + "|" + colorList.Distinct().Count() + "|" + lARFinal[0].DetailsFinal.Layout);
                }
            }
            bool mismatch = false;
            if ((lARFinal[0].SampleFinal.Merchandised_Product_Color_ID_C1.Count.Equals(0) && lARFinal[0].DetailsFinal.Division_List.ToLower().Contains("c1"))
                || lARFinal[0].SampleFinal.Merchandised_Product_Color_ID_FA.Count.Equals(0) && lARFinal[0].DetailsFinal.Division_List.ToLower().Contains("fa"))
            {
                Console.WriteLine("Possible Details Tab and Sample Tab Feeler problem.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Possible Feeler Mismatch.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName);
                }
                mismatch = true;                
            }
            if (!mismatch)
            {                
                missingImages.AddRange(SS8_25x0_875FL.CreateXMLSS8_25x0_875FL(lARFinal, export));
            }

            return (missingImages, files);
        }

        private static string GetRoomScene(string jobName, string styleName, string export, ref string[] files, bool skip, List<LARFinal> lARFinal, List<int> featurePositionList)
        {
            List<string> roomsceneList = new();
            string roomScene = "FPOwaitingonroom.tif";
            foreach (string s in lARFinal[0].SampleFinal.Merchandised_Product_Color_ID_C1)
            {
                var result = GetSyncedRoomscenes(files, s, roomScene);
                roomScene = result.Roomscene;
                if (!roomScene.Contains("FPOwaitingonroom"))
                {
                    roomsceneList.Add(result.Roomscene);
                }
                files = result.UpdatedFiles;
            }
            //if (roomScene.Contains("FPOwaitingonroom"))
            //{
            foreach (string s in lARFinal[0].SampleFinal.Merchandised_Product_Color_ID_FA)
            {
                var result = GetSyncedRoomscenes(files, s, roomScene);
                roomScene = result.Roomscene;
                if (!roomScene.Contains("FPOwaitingonroom"))
                {
                    roomsceneList.Add(result.Roomscene);
                }
                files = result.UpdatedFiles;
            }
            //}
            /*foreach (string s in files)
            {
                foreach (int i in featurePositionList)
                {
                    if (!lARFinal[i].DetailsFinal.Merchandised_Product_Color_ID.EqualsString(""))
                    {
                        if (Path.GetFileName(s).StartsWith(lARFinal[i].DetailsFinal.Merchandised_Product_Color_ID))
                        {
                            roomScene = Path.GetFileName(s);
                            if (!roomScene.Contains("FPOwaitingonroom"))
                            {
                                roomsceneList.Add(roomScene);
                            }
                        }
                    }
                }
            }*/
            if (!lARFinal[0].DetailsFinal.Roomscene.Trim().Equals(""))
            {
                roomScene = lARFinal[0].DetailsFinal.Roomscene + "<!--Roomscene-->";
                roomsceneList.Add(lARFinal[0].DetailsFinal.Roomscene);
            }
            if (roomScene.EqualsString("FPOwaitingonroom.tif"))
            {
                //files = ApprovedRoomscenes();
                foreach (string s in files)
                {
                    foreach (string r in lARFinal[0].SampleFinal.Merchandised_Product_Color_ID_C1.Distinct())
                    {
                        if (!r.EqualsString(""))
                        {
                            if (Path.GetFileName(s).StartsWith(r))
                            {
                                roomScene = Path.GetFileName(s);
                                if (!roomScene.Contains("FPOwaitingonroom"))
                                {
                                    roomsceneList.Add(roomScene);
                                }
                            }
                        }
                    }
                    foreach (string r in lARFinal[0].SampleFinal.Merchandised_Product_Color_ID_FA.Distinct())
                    {
                        if (!r.EqualsString(""))
                        {
                            if (Path.GetFileName(s).StartsWith(r))
                            {
                                roomScene = Path.GetFileName(s);
                                if (!roomScene.Contains("FPOwaitingonroom"))
                                {
                                    roomsceneList.Add(roomScene);
                                }
                            }
                        }
                    }
                }
            }
            if (skip)
            {
                roomScene = "FPOwaitingonroom.tif" + "<!--Roomscene skipped-->";
            }

            if(roomsceneList.Count > 1)
            {
                /*Console.WriteLine("Multiple Roomscene Found. Which should be used?");
                int count = 0;
                foreach (string s in roomsceneList)
                {
                    Console.WriteLine("[" + count + "] - " + s);
                }
                string result = Console.ReadLine();
                if (result.EqualsString(""))
                {*/
                Directory.CreateDirectory(Path.Combine(export, "Reports"));

                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Multiple Roomscenes.txt"), append: true))
                    {
                        foreach (string s in roomsceneList)
                        {
                            textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName + "|" + lARFinal[0].DetailsFinal.Supplier_Name + "|" + s);
                        }
                    }
                /*}
                else
                {
                    int.TryParse(result, out int choice);
                    if (choice < roomsceneList.Count)
                    {
                        roomScene = roomsceneList[choice];
                    }
                }*/
                //roomScene = "FPOwaitingonroom.tif";
            }

            return roomScene;
        }

        private static List<Labels> GetImages(List<Labels> inLabels)
        {
            List<Labels> workingList = new(inLabels);
            Labels sortLabels = new();
            List<Labels> newWorkingList = new();

            foreach (Labels l in workingList)
            {
                sortLabels.Division_Label_Name = XmlIconPath(l.Division_Label_Name, "Images") + XmlRemapping(l.Division_Label_Name, "Images");
                sortLabels.Division_Label_Type = l.Division_Label_Type;
                sortLabels.Merchandised_Product_ID = l.Merchandised_Product_ID;
                sortLabels.Priority = XmlIconOrder(l.Division_Label_Name, "Images");
                sortLabels.Sample_ID = l.Sample_ID;
                newWorkingList.Add(sortLabels);
                sortLabels = new Labels();
            }

            workingList = new List<Labels>();
            newWorkingList.Sort((x, y) => x.Priority.CompareTo(y.Priority));

            return newWorkingList;
        }

        private static string GetColorCount(string layout)
        {
            string colorCount = "";
            if (layout.ToLower().Contains("x"))
            {
                string[] splitLayout = layout.ToLower().Split('x');
                splitLayout[0] = Regex.Match(splitLayout[0].Trim(), @"\d+").Value;
                splitLayout[1] = Regex.Match(splitLayout[1].Trim(), @"\d+").Value;
                _ = int.TryParse(splitLayout[0], out int across);
                _ = int.TryParse(splitLayout[1], out int down);
                colorCount = (across * down).ToString();
            }

            return colorCount;
        }

        private static List<string> GetSpecList(Details details, List<string> ccaSkuId, List<string> widthList)
        {
            List<string> specs = new();
            //Spec 1
            specs.Add(details.Primary_Display);

            //Spec 2
            specs.Add(details.Pile_Line);

            //Spec 3
            specs.Add(GetPatternRepeat(details));

            //Spec 4
            string[] widthSize = widthList.Distinct().ToArray();
            if (widthSize.Length.Equals(1))
            {
                specs.Add(widthSize[0]);
            }
            if (widthSize.Length.Equals(2))
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

        private static string GetPatternRepeat(Details details)
        {
            string patternRepeat = "";
            if (details.Match.Trim().ToLower().Equals("random") && !details.Match_Width.EqualsString(""))
            {
                string width = details.Match_Width;
                bool successW = decimal.TryParse(details.Match_Width, out decimal widthD);
                if (successW)
                {
                    if (width.EndsWith('0'))
                    {
                        width = widthD.ToString("0.00");
                    }
                }
                width = width.Replace(".00", "") + "\" W<!--Match_Width-->";
                patternRepeat = width + " x Random";
            }
            else if (details.Match.Trim().ToLower().Equals("random") && !details.Match_Length.EqualsString(""))
            {
                string length = details.Match_Length;
                bool successL = decimal.TryParse(length, out decimal lengthD);
                if (successL)
                {
                    if (length.EndsWith('0'))
                    {
                        length = lengthD.ToString("0.00");
                    }
                    length = length.ToLower().Replace(".00", "") + "\" L<!--Match_Length-->";
                }
                patternRepeat = "Random x " + length;
            }
            else if (details.Match.Trim().ToLower().Equals("random"))
            {
                //Spec 3
                patternRepeat = "Random";
            }
            else if ((!details.Match.Trim().ToLower().Equals("none") && !details.Match.Trim().ToLower().Equals("null") && !details.Match.EqualsString("")) ||
                 (!details.Match_Length.EqualsString("") || !details.Match_Width.EqualsString("")))
            {
                string width = details.Match_Width;
                bool successW = decimal.TryParse(details.Match_Width, out decimal widthD);
                if (successW)
                {
                    if (width.EndsWith('0'))
                    {
                        width = widthD.ToString("0.00");
                    }
                    width = width.Replace(".00", "") + "\" W<!--Match_Width-->";
                }


                string length = details.Match_Length;
                bool successL = decimal.TryParse(length, out decimal lengthD);
                if (successL)
                {
                    if (length.EndsWith('0'))
                    {
                        length = lengthD.ToString("0.00");
                    }
                    length = length.ToLower().Replace(".00", "") + "\" L<!--Match_Length-->";
                }

                //Spec 3
                if (width.EqualsString(""))
                {
                    patternRepeat = length + " " + details.Match;
                }
                else if (length.EqualsString(""))
                {
                    patternRepeat = width + " " + details.Match;
                }
                else if (details.Match.EqualsString("none"))
                {
                    patternRepeat = width + " x " + length;
                }
                else
                {
                    patternRepeat = width + " x " + length + " " + details.Match;
                }
            }
            else
            {
                //Spec 3
                patternRepeat = "None";
            }

            return patternRepeat;
        }        
    }
}