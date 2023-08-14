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
    public class HSBL18x24
    {
        public static List<string> CreateXMLHS18x24BL(bool forced, string[] files, bool skip, LARFinal lARFinal, string plateId, string export, bool checkRoomscenes, bool isCanada)
        {
            //goWorkShop = true;
            List<string> missingImages = new();
            string division = XmlRemapping(lARFinal.DetailsFinal.Division_List, "Divisions");
            string canadaIndicator = "";
            if (isCanada)
            {
                division = XmlRemapping(lARFinal.DetailsFinal.Division_List + " canada", "Divisions");
                canadaIndicator = " canada";
            }
            TemplateModel settings = GetTemplateSettings(division, "BL");
            string mainPath = settings.WebShopPath;
            string template = settings.WebShopPath + settings.Name;
            string jobName = lARFinal.DetailsFinal.Plate_ID;
            if (jobName.Equals(""))
            {
                jobName = lARFinal.DetailsFinal.Sample_ID;
            }
            string userEmail = settings.UserEmail;
            string snippetPath = mainPath + settings.SnippetPath;
            string roomScenePath = "WebShop:webshop roomscenes:";
            //string roomScenePath = mainPath + settings.RoomscenePath;

            //string iconPath = mainPath + settings.IconPath;
            //characterPath = mainPath + settings.CharacteristicPath;
            //string installationPath = mainPath + settings.InstallationPath;
            //string trimsPath = mainPath + settings.TrimsPath;
            string imagesPath = mainPath + settings.ImagesPath;
            string category = XmlRemapping(lARFinal.DetailsFinal.Taxonomy.ToLower(), "Categories");
            string snippetCategory = "category:" + category + canadaIndicator + ".idms" + "<!--Taxonomy-->";
            if (lARFinal.SampleFinal.Sample_Note.ToLower().Contains("available"))
            {
                snippetCategory = "category:" + XmlRemapping(lARFinal.DetailsFinal.Taxonomy.ToLower() + " - multi" + canadaIndicator, "Categories") + ".idms";
            }            
            
            string snippetWarranties = "warranties:" + XmlRemapping(lARFinal.DetailsFinal.Division_Rating.ToLower(), "Ratings") + " " + category + ".idms" + "<!--Division_Rating-->";
            string styleName = ConvertToTitleCase(lARFinal.SampleFinal.Sample_Name.Trim());

            string roomScene = GetRoomScene(jobName, styleName, export, ref files, skip, lARFinal);

            List<string> specList = GetSpecList(lARFinal.DetailsFinal, lARFinal.SampleFinal);
            List<string> xmlData = new();

            xmlData.Add("<jobs>");
            xmlData.Add("	<job>");
            xmlData.Add("		<template useremail=\"" + userEmail + "\">" + CheckForMissing(jobName, template, missingImages) + "</template>");
            xmlData.Add("		<output jobname=\"" + jobName + "\">Webshop:InputPDF</output>");
            xmlData.Add("		<snippets>");
            xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + snippetCategory, missingImages) + "</snippet>");
            xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + snippetWarranties, missingImages) + "</snippet>");

            foreach (Labels b in lARFinal.LabelsFinal)
            {
                if (b.Division_Label_Type.ToLower().Trim().Equals("logo"))
                {
                    xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(jobName, snippetPath + XmlRemapping(b.Division_Label_Name, "Images", "path", true), missingImages) + "</snippet>");
                }
            }
            xmlData.Add("		</snippets>");
            xmlData.Add("		<texts>");
            xmlData.Add("			<text type=\"spec\">" + SpecLine(specList) + "</text>");
            int count = 1;
            foreach (string s in specList)
            {
                string spec = s;
                if ((settings.Id.Equals("c1") || settings.Id.Equals("cn")) && s.ToLower().Contains("n/a"))
                {
                    spec = s.ToLower();
                }
                xmlData.Add("           <text type=\"spec" + count + "\">" + spec + "</text>");
                count++;
            }

            count = 1;
            xmlData.Add("			<text type=\"stylename\">" + styleName + "<!--Division_Product_Name--></text>");
            xmlData.Add("			<text type=\"stylename" + count + "\">" + styleName + "<!--Division_Product_Name--></text>");
            xmlData.Add("		</texts>");
            xmlData.Add("		<graphics>");
            xmlData.Add("			<roomscenes>");
            try
            {
                if (!skip && checkRoomscenes)
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
            xmlData.Add("			<inlines>");
            if (!lARFinal.LabelsFinal.Count.Equals(0))
            {
                foreach (Labels l in GetInlines(lARFinal.LabelsFinal))
                {
                    if (l.Division_Label_Type.ToLower().Trim().Equals("icon"))
                    {
                        xmlData.Add("				<inline>" + CheckForMissing(jobName, imagesPath + l.Division_Label_Name, missingImages) + "</inline>" + "<!--Division_Label_Name of Labels Sheet-->");
                    }
                }
            }
            xmlData.Add("			</inlines>");
            xmlData.Add("			<images>");

            foreach (string c in GetCharacteristics(lARFinal))
            {
                xmlData.Add("				<image>" + CheckForMissing(jobName, imagesPath + c, missingImages) + "</image>");
            }
            foreach (string i in GetInstallation(lARFinal))
            {
                xmlData.Add("				<image>" + CheckForMissing(jobName, imagesPath + i, missingImages) + "</image>");
            }
            foreach (string t in GetTrims(lARFinal))
            {
                xmlData.Add("				<image>" + CheckForMissing(jobName, imagesPath + t, missingImages) + "</image>");
            }
            xmlData.Add("			</images>");
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
            xmlData.AddRange(InsiteXMLSnippet("", "BL", lARFinal.DetailsFinal.Supplier_Name, division, styleName, ConvertToTitleCase(lARFinal.DetailsFinal.Merch_Color_Name), Path.GetFileNameWithoutExtension(template.Replace(":", "\\"))));
            xmlData.Add("	</job>");
            xmlData.Add("</jobs>");

            Directory.CreateDirectory(Path.Combine(export, "Reports"));
            if (forced || (!template.Equals("") && !roomScene.Contains("FPOwaitingonroom")))
            //if (forced || (!template.Equals("")))
            {
                ExportXML(jobName, xmlData, export, "WorkShop XML");
                xmlData[roomsceneLineIndex] = xmlData[roomsceneLineIndex].Replace("WebShop:webshop roomscenes:", mainPath + settings.RoomscenePath);
                ExportXML(jobName, xmlData, export, "WebShop XML");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Roomscene Matchups.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal.DetailsFinal.Sample_ID + "|" + styleName + "|" + lARFinal.DetailsFinal.Supplier_Name + "|" + roomScene);
                }
                //CreateDeleteXML(export, jobName);
                HSFL4_5x2_1875.CreateXMLHS4_5x2_1875(lARFinal, export, isCanada);
            }
            if (roomScene.Contains("FPOwaitingonroom"))
            {
                Console.WriteLine("Roomscene Missing.");
                using (StreamWriter textFile = new(Path.Combine(export, "Reports", "Roomscene Missing.txt"), append: true))
                {
                    textFile.WriteLine(jobName + "|" + lARFinal.DetailsFinal.Sample_ID + "|" + lARFinal.DetailsFinal.Supplier_Name + "|" + styleName + "|" + lARFinal.DetailsFinal.Merch_Color_Name + "|" + lARFinal.SampleFinal.Merchandised_Product_Color_ID_C1[0]);
                }
            }
            return missingImages;
        }

        private static string GetRoomScene(string jobName, string styleName, string export, ref string[] files, bool skip, LARFinal lARFinal)
        {
            List<string> roomsceneList = new();
            string roomScene = "FPOwaitingonroom.tif";
            foreach (string s in lARFinal.SampleFinal.Merchandised_Product_Color_ID_C1)
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
            foreach (string s in lARFinal.SampleFinal.Merchandised_Product_Color_ID_FA)
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
            if (!lARFinal.DetailsFinal.Roomscene.Trim().Equals(""))
            {
                roomScene = lARFinal.DetailsFinal.Roomscene + "<!--Roomscene-->";
                roomsceneList.Add(lARFinal.DetailsFinal.Roomscene);
            }
            if (roomScene.EqualsString("FPOwaitingonroom.tif"))
            {
                //files = ApprovedRoomscenes();
                foreach (string s in files)
                {
                    foreach (string r in lARFinal.SampleFinal.Merchandised_Product_Color_ID_C1.Distinct())
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
                    foreach (string r in lARFinal.SampleFinal.Merchandised_Product_Color_ID_FA.Distinct())
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

            if (roomsceneList.Count > 1)
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
                        textFile.WriteLine(jobName + "|" + lARFinal.DetailsFinal.Sample_ID + "|" + styleName + "|" + lARFinal.DetailsFinal.Supplier_Name + "|" + s);
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

        private static List<string> GetCharacteristics(LARFinal lARFinal)
        {
            List<string> characteristicsList = new();

            //1 Width
            string width = lARFinal.DetailsFinal.Size_Name;
            if ((lARFinal.DetailsFinal.Width.Trim().Equals("") ||
                lARFinal.DetailsFinal.Width.Trim().Equals("0")) && 
                (lARFinal.DetailsFinal.Width_Measurement.ToLower().Equals("random") ||
                lARFinal.DetailsFinal.Width_Measurement.ToLower().Equals("multi")))
            {
                string[] widthSplit = width.ToLower().Split('x');
                width = widthSplit[0].Replace(",\b", ", ").Replace("\"", "");
                characteristicsList.Add("characteristics:width - " + XmlRemapping(width.ToLower(), "Widths") + "<!--Size_Name-->");
            }
            else
            {
                width = lARFinal.DetailsFinal.Width;
                bool success = decimal.TryParse(width, out decimal widthD);
                if (success)
                {
                    width = widthD.ToString("0.00");
                    if (width.EndsWith(".25") || width.EndsWith(".50") || width.EndsWith(".75"))
                    {
                        width = ConvertDecimalToFraction(widthD).Replace(" ", "-");
                    }
                }
                if (width.EndsWith(".00"))
                {
                    width = width.ToLower().Replace(".00", "");
                }
                characteristicsList.Add("characteristics:width - " + XmlRemapping(width.ToLower(), "Widths") + "<!--Widths-->");
            }

            //2 Length
            if (lARFinal.DetailsFinal.Length.EqualsString("") || lARFinal.DetailsFinal.Length.EqualsString("0") || lARFinal.DetailsFinal.Length_Measurement.EqualsString("random"))
            {
                characteristicsList.Add("characteristics:length - " + XmlRemapping(lARFinal.DetailsFinal.Length_Measurement.ToLower(), "Lengths") + "<!--Length_Measurement-->");
            }
            else
            {
                string length = lARFinal.DetailsFinal.Length;
                bool success = decimal.TryParse(length, out decimal lengthD);
                if (success)
                {
                    length = lengthD.ToString("0.00");
                    if (length.EndsWith(".25") || length.EndsWith(".50") || length.EndsWith(".75"))
                    {
                        length = ConvertDecimalToFraction(lengthD).Replace(" ", "-");
                    }
                }
                characteristicsList.Add("characteristics:length - " + XmlRemapping(lARFinal.DetailsFinal.Length.ToLower(), "Lengths") + "<!--Length-->");
            }

            //3 Thickness
            if (lARFinal.DetailsFinal.Taxonomy.EqualsString("wood"))
            {
                    characteristicsList.Add("characteristics:thickness - " + XmlRemapping(lARFinal.DetailsFinal.Thickness_Fraction.ToLower().Replace("\"", "").Replace("/", "-") + " " + lARFinal.DetailsFinal.Thickness_Measurement.ToLower(), "Thicknesses"));
            }
            else
            {
                characteristicsList.Add("characteristics:thickness - " + XmlRemapping(lARFinal.DetailsFinal.Thickness.ToLower() + " " + lARFinal.DetailsFinal.Thickness_Measurement.ToLower(), "Thicknesses") + "<!--Thickness Thickness_Measurement-->");
            }

            //4 Species (HARDWOOD ONLY)
            if (XmlRemapping(lARFinal.DetailsFinal.Taxonomy.ToLower(), "Types").Equals("wood"))
            {
                characteristicsList.Add("characteristics:species - " + XmlRemapping(lARFinal.DetailsFinal.Species.ToLower(), "Specieses") + "<!--Species-->");
            }

            //4 Construction (Vinyl & Laminate)
            //5 Construction (Hardwood)
            characteristicsList.Add("characteristics:construction - " + XmlRemapping(lARFinal.DetailsFinal.Product_Class.ToLower(), "Constructions") + "<!--Product_Class-->");

            foreach (Labels c in GetImages(lARFinal.LabelsFinal))
            {
                if (c.Division_Label_Name.ToLower().Contains("appearance") ||
                    (c.Division_Label_Name.ToLower().Contains("profile") && !c.Division_Label_Name.ToLower().Contains("trim")) ||
                    c.Division_Label_Name.ToLower().Contains("construction"))
                {
                    characteristicsList.Add(c.Division_Label_Name + "<!--Labels Sheet Icons-->");
                }
            }
            while (!characteristicsList.Count.Equals(15))
            {
                characteristicsList.Add("!blank.eps");
            }
            lARFinal = new();
            return characteristicsList;
        }

        private static List<Labels> GetImages(List<Labels> inLabels)
        {
            List<Labels> workingList = new(inLabels);
            Labels sortLabels = new();
            List<Labels> newWorkingList = new();

            foreach (Labels l in workingList)
            {
                if (l.Division_Label_Name.Contains(":"))
                {
                    sortLabels.Division_Label_Name = XmlIconPath(l.Division_Label_Name, "Images") + XmlRemapping(l.Division_Label_Name, "Images");
                    sortLabels.Division_Label_Type = l.Division_Label_Type;
                    sortLabels.Merchandised_Product_ID = l.Merchandised_Product_ID;
                    sortLabels.Priority = XmlIconOrder(l.Division_Label_Name, "Images");
                    sortLabels.Sample_ID = l.Sample_ID;
                    newWorkingList.Add(sortLabels);
                    sortLabels = new Labels();
                }
            }

            workingList = new List<Labels>();
            newWorkingList.Sort((x, y) => x.Priority.CompareTo(y.Priority));

            return newWorkingList;
        }

        /// <summary>
        /// Sort the Icons
        /// </summary>
        /// <param name="inLabels">List of label models</param>
        /// <returns>Returns the sorted label model.</returns>
        private static List<Labels> GetInlines(List<Labels> inLabels)
        {
            List<Labels> workingList = new(inLabels);
            Labels sortLabels = new();
            List<Labels> newWorkingList = new();

            foreach (Labels l in workingList)
            {
                if (!l.Division_Label_Name.Contains(":"))
                {
                    sortLabels.Division_Label_Name = XmlIconPath(l.Division_Label_Name, "Inlines") + XmlRemapping(l.Division_Label_Name, "Inlines");
                    sortLabels.Division_Label_Type = l.Division_Label_Type;
                    sortLabels.Merchandised_Product_ID = l.Merchandised_Product_ID;
                    sortLabels.Priority = XmlIconOrder(l.Division_Label_Name, "Inlines");
                    sortLabels.Sample_ID = l.Sample_ID;
                    newWorkingList.Add(sortLabels);
                    sortLabels = new Labels();
                }
            }
            workingList = new List<Labels>();
            newWorkingList.Sort((x, y) => x.Priority.CompareTo(y.Priority));

            return newWorkingList;
        }

        private static List<string> GetInstallation(LARFinal lARFinal)
        {
            List<string> installationList = new();
            //lARFinal.LabelsFinal = GetImages(lARFinal.LabelsFinal);
            foreach (Labels i in GetImages(lARFinal.LabelsFinal))
            {
                if (i.Division_Label_Name.ToLower().Contains("installation"))
                {
                    installationList.Add(i.Division_Label_Name + "<!--Labels Sheet Icons-->");
                }
            }

            while (!installationList.Count.Equals(10))
            {
                installationList.Add("!blank.eps");
            }
            lARFinal = new();
            return installationList;
        }

        private static List<string> GetSpecList(Details details, Sample sample)
        {
            List<string> specs = new();
            specs.Add(ConvertToTitleCase(details.Merch_Color_Name) + "<!--Merch_Color_Name-->");
            string width = details.Size_Name;
            if ((details.Width.Trim().Equals("") ||
                details.Width.Trim().Equals("0.00") ||
                details.Width.Trim().Equals("0")) &&
                (details.Width_Measurement.ToLower().Equals("random") ||
                details.Width_Measurement.ToLower().Equals("multi")))
            {
                string[] widthSplit = width.ToLower().Split('x');
                width = widthSplit[0].Trim().Replace(",\b", ", ").Replace("  ", " ");
                specs.Add(ConvertToTitleCase(width) + "<!--Size_Name-->");
            }
            else
            {
                width = details.Width;
                bool success = decimal.TryParse(details.Width, out decimal widthD);
                if (success)
                {
                        width = widthD.ToString("0.00");
                        if (width.EndsWith(".25") || width.EndsWith(".50") || width.EndsWith(".75"))
                        {
                            width = ConvertDecimalToFraction(widthD).Replace(" ", "-");
                        }
                }
                if (width.EndsWith(".00"))
                {
                    width = width.ToLower().Replace(".00", "");
                }
                specs.Add(width + "\"");

            }

            if ((details.Length.Trim().Equals("") || details.Length.Trim().Equals("0.00") || details.Length.Trim().Equals("0")) && 
                (details.Length_Measurement.Trim().EqualsString("random") || details.Length_Measurement.Trim().EqualsString("multi")))
            {
                specs.Add(details.Length_Measurement + "<!--Length_Measurement-->");
            }
            else
            {
                string length = details.Length;
                bool success = decimal.TryParse(length, out decimal lengthD);
                if (success)
                {
                        length = lengthD.ToString("0.00");
                        if (length.EndsWith(".25") || length.EndsWith(".50") || length.EndsWith(".75"))
                        {
                            length = ConvertDecimalToFraction(lengthD).Replace(" ", "-");
                        }
                }
                specs.Add(length.ToLower().Replace(".00", "") + "\"<!--Length-->");
            }
            try
            {
                specs.Add(details.Size_UC.Trim().Substring(0, details.Size_UC.IndexOf(" ")) + "<!--Size_UC-->");
            }
            catch (Exception)
            {
                specs.Add(details.Size_UC.Trim());
            }

            if (XmlRemapping(details.Taxonomy.ToLower(), "Types").Equals("wood"))
            {
                if (details.Species.Contains(", "))
                {
                    specs.Add(details.Species + "<!--Species-->");
                }
                else if (details.Species.Contains(","))
                {
                    specs.Add(details.Species.Replace(",", ", ") + "<!--Species-->");
                }
                else
                {
                    specs.Add(details.Species + "<!--Species-->");
                }

            }
            else if (XmlRemapping(details.Taxonomy.ToLower(), "Types").Equals("vinyl") || XmlRemapping(details.Taxonomy.ToLower(), "Types").Equals("laminate"))
            {
                if (!details.Wear_Layer.Equals("") && int.TryParse(details.Wear_Layer, out int result))
                {
                    specs.Add(details.Wear_Layer + " mil" + "<!--Wear_Layer-->");
                }
                else if (!details.Wear_Layer.Equals(""))
                {
                    specs.Add(details.Wear_Layer + "<!--Wear_Layer-->");
                }
                else
                {
                    specs.Add("N/A" + "<!--NOTE: Wear_Layer Seems To Be Blank is that correct?-->");
                }
            }
            else
            {
                specs.Add("");
            }
            specs.Add(details.CcaSkuId + "<!--CCASKUID-->");

            if (sample.Sample_Note.ToLower().Contains("available"))
            {
                if (sample.Sample_Note.Contains("."))
                { 
                    specs.Add("Also " + sample.Sample_Note.Substring(0, sample.Sample_Note.IndexOf(".")+1));
                }
                else
                {
                    specs.Add("Also " + sample.Sample_Note);
                    Console.WriteLine("Period is missing in Sample Notes for Avilable in other widths.");
                }
            }

            return specs;
        }

        private static List<string> GetTrims(LARFinal lARFinal)
        {
            List<string> trimList = new();
            foreach (Labels t in GetImages(lARFinal.LabelsFinal))
            {
                if (t.Division_Label_Name.ToLower().Contains("trim"))
                {
                    trimList.Add(t.Division_Label_Name + "<!--Labels Sheet Icons-->");
                }
            }

            while (!trimList.Count.Equals(10))
            {
                trimList.Add("!blank.eps");
            }
            lARFinal = new();
            return trimList;
        }        
    }
}