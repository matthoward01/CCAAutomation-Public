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
    public class SS19_375x28_5BLConv
    {
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

        private static string TrimStyleName(string style)
        {
            string styleName = style;
            if (style.Contains("-"))
            {
                string[] styleSplit = style.Split('-');
                styleName = styleSplit[0].Trim();
            }
            return styleName;
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
                specs.Add(width + " x Random");
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
                specs.Add("Random x " + length);
            }
            else if (details.Match.Trim().ToLower().Equals("random"))
            {
                //Spec 3
                specs.Add("Random");
            }            
            else if (!details.Match.Trim().ToLower().Equals("none") && !details.Match.Trim().ToLower().Equals("null") && !details.Match.EqualsString(""))
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

            /*foreach (string sku in ccaSkuId.Distinct())
            {
                //Spec 6 (7)
                specs.Add(sku);
            }*/

            return specs;
        }        

        public static (List<string> missingItems, string[] updatedFiles) CreateXMLSS19_375x28_5BLConv(string[] files, bool skip, List<LARFinal> lARFinal, string plateId, string export, bool isConv = false)
        {
            List<Details> details = new();            
            //goWorkShop = true;
            List<string> missingImages = new();
            List<string> colorList = new();
            List<string> ccaSkuId = new();
            List<string> widthList = new();
            List<string> styleList = new();
            List<int> featurePositionList = new();
            List<Swatches.SwatchColors> swatchColors = new();
            List<CCASkuIdModel> ccaSkuIdList = new();
            CCASkuIdModel cCASkuIdModel = new();
            bool seqError = false;
            bool secondStyleError = false;
            string twoFeature = "";
            int featurePosition = 0;
            int colorCount = 0;

            foreach (LARFinal lf in lARFinal)
            {
                details.Add(lf.DetailsFinal);
                styleList.Add(lf.DetailsFinal.Supplier_Product_Name);
                Swatches.SwatchColors colors = new();
                if (!lf.DetailsFinal.Color_Sequence.EqualsString("0") && int.TryParse(lf.DetailsFinal.Color_Sequence, out int result))
                {
                    colors.Color = lf.DetailsFinal.Mfg_Color_Name;
                    colors.ColorSequence = lf.DetailsFinal.Color_Sequence.PadLeft(2, '0');
                    swatchColors.Add(colors);
                }
                else if (isConv)
                {
                    colors.Color = lf.DetailsFinal.Mfg_Color_Name;
                    colors.ColorSequence = lf.DetailsFinal.Color_Sequence.PadLeft(2, '0');
                    swatchColors.Add(colors);
                }

                if (lf.SampleFinal.Sample_Name.Contains("/") && (lf.SampleFinal.Shared_Card.Trim().ToLower().Equals("yes") || lf.SampleFinal.Multiple_Color_Lines.EqualsString("yes")))
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
                    seqError = true;
                }
                widthList.Add(lf.DetailsFinal.Size_Name);
            }            
            widthList.Sort();
            
            foreach (LARFinal lf in lARFinal)
            {
                if (lf.SampleFinal.Sample_Name.Contains("/") && (lf.SampleFinal.Shared_Card.Trim().ToLower().Equals("yes") || lf.SampleFinal.Multiple_Color_Lines.EqualsString("yes")))
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
                else if (lf.SampleFinal.Shared_Card.Trim().ToLower().Equals("yes") || lf.SampleFinal.Multiple_Color_Lines.EqualsString("yes"))
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
            }
            string division = XmlRemapping(lARFinal[0].DetailsFinal.Division_List, "Divisions");
            TemplateModel settings = GetTemplateSettings("SS19.375x28.5BL", "Normal");
            string sharedFeature = lARFinal[0].SampleFinal.Shared_Card.ToLower().Trim();
            //string styleName = ConvertToTitleCase(lARFinal[0].SampleFinal.Sample_Name);
            string styleName = ConvertToTitleCase(TrimStyleName(lARFinal[0].DetailsFinal.Supplier_Product_Name));
            string styleName2 = ConvertToTitleCase(TrimStyleName(lARFinal[0].DetailsFinal.Supplier_Product_Name));
            /*int styleIndex = lARFinal.IndexOf(i => i.DetailsFinal.Division_Product_Name.Contains(lARFinal[0].SampleFinal.Split_Board));
            if (!styleIndex.Equals(-1))
            {
                styleName2 = ConvertToTitleCase(lARFinal[styleIndex].DetailsFinal.Supplier_Product_Name);
            }*/
            bool multiColorError = false;

            //if (sharedFeature.Equals("no") && lARFinal[0].SampleFinal.Multiple_Color_Lines.EqualsString("yes"))
            if (styleList.Distinct().Count().Equals(2) && !lARFinal[0].SampleFinal.Shared_Card.EqualsString("yes") && lARFinal[0].SampleFinal.Multiple_Color_Lines.EqualsString("yes"))
            {
                settings = GetTemplateSettings("SS19.375x28.5BL", "Combo");
                List<string> distStyleList = new(styleList.Distinct());
                //distStyleList.Sort();
                if (distStyleList.Count.Equals(2))
                {
                    styleName = TrimStyleName(distStyleList[0]) + " / " + TrimStyleName(distStyleList[1]);
                }
                multiColorError = true;
            }
            else if (sharedFeature.Equals("yes"))
            {
                settings = GetTemplateSettings("SS19.375x28.5BL", "Feature");
                twoFeature = " wFeat";
                if (lARFinal.Any(lf => lf.DetailsFinal.Face_Weight.Distinct().Count() > 1))
                {
                    var minvalue = lARFinal.Min(x => x.DetailsFinal.Face_Weight);
                    var lfMin = lARFinal.Where(x => x.DetailsFinal.Face_Weight == minvalue).ToList();
                    var maxvalue = lARFinal.Max(x => x.DetailsFinal.Face_Weight);
                    var lfMax = lARFinal.Where(x => x.DetailsFinal.Face_Weight == maxvalue).ToList();
                    styleName2 = TrimStyleName(lfMin[0].DetailsFinal.Supplier_Product_Name);
                    styleName = TrimStyleName(lfMax[0].DetailsFinal.Supplier_Product_Name) + " / " + TrimStyleName(lfMin[0].DetailsFinal.Supplier_Product_Name);
                }
                /*else if (lARFinal.Any(lf => lf.SampleFinal.Sample_Name.Contains("/")))
                {
                    string[] splitStyleName = lARFinal[0].SampleFinal.Sample_Name.Split("/");
                    styleName2 = splitStyleName.Last();
                }
                else
                {
                    secondStyleError = true;
                }*/
            }
            else if (styleList.Distinct().Count().Equals(2) && !lARFinal[0].SampleFinal.Shared_Card.EqualsString("yes") && lARFinal[0].SampleFinal.Multiple_Color_Lines.EqualsString("yes"))
            {
                List<string> distStyleList = new(styleList.Distinct());
                //distStyleList.Sort();
                if (distStyleList.Count.Equals(2))
                {
                    styleName = TrimStyleName(distStyleList[0]) + " / " + TrimStyleName(distStyleList[1]);
                }
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
            string roomScenePath = "WebShop:webshop roomscenes:";
            //string roomScenePath = mainPath + settings.RoomscenePath;

            //string warranty = "need warranty";
            //string snippetWarranties = XmlRemapping(lARFinal[0].DetailsFinal.Division_Rating.ToLower(), "Ratings") + ".idms" + "<!--Division_Rating-->";
            /*foreach (Labels l in lARFinal[0].LabelsFinal.Distinct())
            {
                if (l.Division_Label_Type.Trim().ToLower().Equals("wbug"))
                {
                    warranty = l.Division_Label_Name;                       
                }
            }*/
            //string snippetWarranties = XmlRemapping(warranty.ToLower(), "Ratings") + ".idms";

            string roomScene = "FPOwaitingonroom.tif";
            if (lARFinal[0].DetailsFinal.Supplier_Name.ToLower().Contains("beaulieu"))
            {
                roomScene = "1090312_Beaulieu_RS_C1_LoAndBehold_S0015_CMYK.tif";
            }
            else if (lARFinal[0].DetailsFinal.Supplier_Name.ToLower().Contains("dixie"))
            {
                roomScene = "1090147_dixie_Teton_D017_21832_Scenic Drive_RM.tif";
            }
            else if (lARFinal[0].DetailsFinal.Supplier_Name.ToLower().Contains("masland"))
            {
                roomScene = "1090147_dixie_Teton_D017_21832_Scenic Drive_RM.tif";
            }
            else if (lARFinal[0].DetailsFinal.Supplier_Name.ToLower().Contains("kane"))
            {
                roomScene = "1008736_Kane_Cutting Edge - Current room.tif";
            }
            else if (lARFinal[0].DetailsFinal.Supplier_Name.ToLower().Contains("milliken"))
            {
                roomScene = "1084665_CCA_ARTISTIC_BRUSH_PRAIRIE.tif";
            }
            else if (lARFinal[0].DetailsFinal.Supplier_Name.ToLower().Contains("mohawk"))
            {
                roomScene = "1090755_MHK_28687_room_00.tif";
            }
            else if (lARFinal[0].DetailsFinal.Supplier_Name.ToLower().Contains("shaw"))
            {
                roomScene = "1094359_CCA-Acrostic-5E629-00751-RM.tif";
            }
            else if (lARFinal[0].DetailsFinal.Supplier_Name.ToLower().Contains("engineered"))
            {
                roomScene = "St Lucia_6130.tif";
            }
            else if (lARFinal[0].DetailsFinal.Supplier_Name.ToLower().Contains("mannington"))
            {
                roomScene = "FP024-165-RS-Mykonos.tif";
            }
            string prevRoomscene = roomScene;
            foreach (string s in lARFinal[0].SampleFinal.Merchandised_Product_Color_ID_C1)
            {
                var result = GetSyncedRoomscenes(files, s, roomScene);
                roomScene = result.Roomscene;
                files = result.UpdatedFiles;
            }
            if (roomScene.Contains(prevRoomscene))
            {
                foreach (string s in lARFinal[0].SampleFinal.Merchandised_Product_Color_ID_FA)
                {
                    var result = GetSyncedRoomscenes(files, s, roomScene);
                    roomScene = result.Roomscene;
                    files = result.UpdatedFiles;
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
            if (roomScene.EqualsString(prevRoomscene))
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
            if (skip)
            {
                roomScene = prevRoomscene + "<!--Roomscene skipped-->";
            }
            
                

            //styleName2 = ConvertToTitleCase(styleName2);
            string feeler = ConvertToTitleCase(lARFinal[0].SampleFinal.Feeler);
            int feelerIndex = lARFinal.IndexOf(i => i.DetailsFinal.Manufacturer_Feeler.EqualsString("yes"));
            if (!feelerIndex.Equals(-1))
            {
                feeler = ConvertToTitleCase(lARFinal[feelerIndex].DetailsFinal.Mfg_Color_Name);
            }

            List<string> specList = GetSpecList(lARFinal[0].DetailsFinal, ccaSkuId, widthList);
            List<string> xmlData = new();
            xmlData.Add("<jobs>");
            xmlData.Add("	<job>");
            xmlData.Add("		<template useremail=\"" + userEmail + "\">" + CheckForMissing(jobName, template, missingImages) + "</template>");
            xmlData.Add("		<output jobname=\"" + jobName + "\">Webshop:InputPDF</output>");
            xmlData.Add("		<snippets>");
            //xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(snippetPath + snippetWarranties, missingImages) + "</snippet>");
            if (lARFinal[0].SampleFinal.Shared_Card.Trim().ToLower().Equals("yes") || lARFinal[0].SampleFinal.Multiple_Color_Lines.Trim().ToLower().Equals("yes"))
            {
                xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + "Retail 2.0 w Feat.idms", missingImages) + "</snippet>");
            }
            else
            {
                xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + "Retail 2.0.idms", missingImages) + "</snippet>");
            }
            List<string> usedLogoList = new();
            foreach (Labels b in lARFinal[0].LabelsFinal.Distinct())
            {
                if (!usedLogoList.Contains(b.Division_Label_Name))
                {
                    if (b.Division_Label_Type.ToLower().Trim().Equals("logo") && lARFinal[0].SampleFinal.Shared_Card.EqualsString("yes"))
                    {
                        xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + XmlRemapping(b.Division_Label_Name + " SS " + lARFinal[0].SampleFinal.Shared_Card, "Images", "path", true), missingImages) + "</snippet>");
                        usedLogoList.Add(b.Division_Label_Name);
                    }
                    else if (b.Division_Label_Type.ToLower().Trim().Equals("logo"))
                    {
                        xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + XmlRemapping(b.Division_Label_Name + " SS", "Images", "path", true), missingImages) + "</snippet>");
                        usedLogoList.Add(b.Division_Label_Name);
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
            xmlData.Add("			<text type=\"stylename\">" + styleName + "<!--SampleFinal.Sample_Name--></text>");
            xmlData.Add("			<text type=\"stylename1\">" + styleName + "<!--SampleFinal.Sample_Name--></text>");
            if (lARFinal[0].SampleFinal.Shared_Card.ToLower().Trim().Equals("yes"))
            {
                xmlData.Add("			<text type=\"stylename2\">" + styleName2 + "</text>");
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
                            divisionPath = "Carpet One:C1 - ";
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
                var swatchXmlResults = Swatches.SwatchXML(feeler, colorList, "1", settings.Id + twoFeature, out seqError, out colorCount);
                xmlData.AddRange(swatchXmlResults.swatchXml);
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
            //xmlData.AddRange(InsiteXMLSnippet(" - ss", "BL", lARFinal[0].DetailsFinal.Supplier_Name, lARFinal[0].SampleFinal.Sample_Name, styleName, ConvertToTitleCase(lARFinal[0].SampleFinal.Feeler), Path.GetFileNameWithoutExtension(template.Replace(":", "\\"))));
            xmlData.Add("	</job>");
            xmlData.Add("</jobs>");
            Directory.CreateDirectory(Path.Combine(export, "Reports"));
            //if ((!template.Equals("")) && (!seqError) && !lARFinal[0].DetailsFinal.ADDNumber.EqualsString(""))
            bool layoutError = false;

            if (int.TryParse(GetColorCount(lARFinal[0].DetailsFinal.Layout), out int colorCountLayout) && !colorCount.Equals(colorCountLayout))
            {
                Console.WriteLine("Color Sequence Missing, 2nd Style Problem, or Incomplete, but we have Roomscene");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Possible Layout Problem.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName + "|" + colorList.Distinct().Count() + "|" + lARFinal[0].DetailsFinal.Layout);
                }
                layoutError = true;
            }

            if ((!template.Equals("")) && (!layoutError) && (!seqError) && (!roomScene.Contains("FPOwaitingonroom")) && !lARFinal[0].DetailsFinal.ADDNumber.EqualsString("") && !lARFinal[0].DetailsFinal.ADDNumber.EqualsString("NULL"))
            {
                multiColorError = false;

                ExportXML(jobName, xmlData, export, "WorkShop XML");
                xmlData[roomsceneLineIndex] = xmlData[roomsceneLineIndex].Replace("WebShop:webshop roomscenes:", mainPath + settings.RoomscenePath);
                ExportXML(jobName, xmlData, export, "WebShop XML");

                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Roomscene Matchups.txt"), append:true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName + "|" + "Yes" + "|" + lARFinal[0].DetailsFinal.Supplier_Name + "|" + roomScene);
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
            if (secondStyleError && seqError)
            //if (secondStyleError && seqError && !roomScene.Contains("FPOwaitingonroom"))
            {
                Console.WriteLine("Color Sequence Missing, 2nd Style Problem, or Incomplete, but we have Roomscene");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Roomscenes no Sequence.txt"), append:true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName + "|" + "No" + "|" + lARFinal[0].DetailsFinal.Supplier_Name + "|" + roomScene);
                }
            }
            if (seqError)
            {
                Console.WriteLine("Color Sequence Missing.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Color Sequence Missing.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName);
                }
            }
            else
            {
                Console.WriteLine("Color Sequence is Complete.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Color Sequence Complete.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName);
                }
            }
            if (multiColorError)
            {
                Console.WriteLine("Multiple Color Lines. No Template");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Multiple Color Lines Problem.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName);
                }
            }
            if (roomScene.Contains("FPOwaitingonroom"))
            {
                Console.WriteLine("Roomscene Missing.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Roomscene Missing.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName);
                }

                //SyncRoomsceneDoubleCheck(details, export);
                details = new();
            }
            if (lARFinal[0].DetailsFinal.ADDNumber.EqualsString("") || lARFinal[0].DetailsFinal.ADDNumber.EqualsString("NULL"))
            {
                Console.WriteLine("ADD Number is Blank.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "ADD Number is Blank.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName);
                }
            }
            if (lARFinal[0].DetailsFinal.ADDNumber.EqualsString("NULL") && !seqError)
            {
                Console.WriteLine("ADD Number is NULL.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "ADD Number is Null.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName);
                }
            }
            if (colorList.Distinct().Count() > 32)
            {
                Console.WriteLine("Color Count greater than 32.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Color Count too High.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal[0].DetailsFinal.Sample_ID + "|" + styleName + "|" + colorList.Distinct().Count() + "|" + lARFinal[0].DetailsFinal.Layout);
                }
            }

            return (missingImages, files);
        }
    }
}