using System.Collections.Generic;
using static CCAAutomation.Lib.LarModels;
using static CCAAutomation.Lib.CommonMethods;
using static CCAAutomation.Lib.XmlMethods;
using static CCAAutomation.Lib.Settings;
using System;
using System.IO;

namespace CCAAutomation.Lib
{
    public class HSBL18x24
    {
        private static List<string> GetCharacteristics(LARFinal lARFinal)
        {
            List<string> characteristicsList = new();

            //1 Width
            string width = lARFinal.DetailsFinal.Size_Name;
            if (lARFinal.DetailsFinal.Width.Trim().Equals("") && 
                (lARFinal.DetailsFinal.Width_Measurement.ToLower().Equals("random") ||
                lARFinal.DetailsFinal.Width_Measurement.ToLower().Equals("multi")))
            {
                string[] widthSplit = width.ToLower().Split('x');
                width = widthSplit[0].Replace(",\b", ", ").Replace("\"", "");
                characteristicsList.Add("characteristics:width - " + XmlRemapping(width.ToLower(), "Widths") + "<!--Size_Name-->");
            }
            else
            {
                characteristicsList.Add("characteristics:width - " + XmlRemapping(lARFinal.DetailsFinal.Width.ToLower(), "Widths") + "<!--Widths-->");
            }

            //2 Length
            if (lARFinal.DetailsFinal.Length.ToLower().Equals(""))
            {
                characteristicsList.Add("characteristics:length - " + XmlRemapping(lARFinal.DetailsFinal.Length_Measurement.ToLower(), "Lengths") + "<!--Length_Measurement-->");
            }
            else
            {
                characteristicsList.Add("characteristics:length - " + XmlRemapping(lARFinal.DetailsFinal.Length.ToLower(), "Lengths") + "<!--Length-->");
            }

            //3 Thickness
            characteristicsList.Add("characteristics:thickness - " + XmlRemapping(lARFinal.DetailsFinal.Thickness.ToLower() + " " + lARFinal.DetailsFinal.Thickness_Measurement.ToLower(), "Thicknesses") + "<!--Thickness Thickness_Measurement-->");

            //4 Species (HARDWOOD ONLY)
            if (XmlRemapping(lARFinal.DetailsFinal.Taxonomy.ToLower(), "Types").Equals("wood"))
            {
                characteristicsList.Add("characteristics:species - " + XmlRemapping(lARFinal.DetailsFinal.Species.ToLower(), "Specieses") + "<!--Species-->");
            }

            //4 Construction (Vinyl & Laminate)
            //5 Construction (Hardwood)
            characteristicsList.Add("characteristics:construction - " + XmlRemapping(lARFinal.DetailsFinal.Product_Class.ToLower(), "Constructions") + "<!--Product_Class-->");


            //lARFinal.LabelsFinal = GetImages(lARFinal.LabelsFinal);
            foreach (Labels c in GetImages(lARFinal.LabelsFinal))
            {
                if (c.Division_Label_Name.ToLower().Contains("appearance") ||
                    c.Division_Label_Name.ToLower().Contains("profile") ||
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

        private static List<string> GetSpecList(Details details)
        {
            List<string> specs = new();
            specs.Add(ConvertToTitleCase(details.Merch_Color_Name) + "<!--Merch_Color_Name-->");
            string width = details.Size_Name;
            if (details.Width.Trim().Equals("") &&
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
                //decimal widthD = decimal.Parse(details.Width);
                bool success = decimal.TryParse(details.Width, out decimal widthD);
                if (success)
                {
                    if (width.EndsWith('0'))
                    {
                        width = widthD.ToString("0.00");
                    }
                }
                specs.Add(width.ToLower().Replace(".00", "") + "\"<!--Widths-->");
            }
            /*string width = details.Size_Name;
            if (width.ToLower().Contains("x"))
            {
                string[] widthSplit = width.ToLower().Split('x');
                width = widthSplit[0].Replace(",", ", ");
                specs.Add(width + "<!--Size_Name-->");
            }
            else
            {
                specs.Add(width + "<!--Size_Name-->");
            }*/

            if (details.Length.Trim().Equals(""))
            {
                specs.Add(details.Length_Measurement + "<!--Length_Measurement-->");
            }
            else
            {
                string length = details.Length;
                //decimal lengthD = decimal.Parse(details.Length);
                bool success = decimal.TryParse(length, out decimal lengthD);
                if (success)
                {
                    if (length.EndsWith('0'))
                    {
                        length = lengthD.ToString("0.00");
                    }
                }
                specs.Add(length.ToLower().Replace(".00", "") + "\"<!--Length-->");
                /*string length = details.Size_Name;
                if (length.ToLower().Contains("x"))
                {
                    string[] lengthSplit = length.ToLower().Split('x');
                    length = lengthSplit[1];
                    specs.Add(length + "<!--Size_Name-->");
                }
                else
                {
                    specs.Add(length + "<!--Size_Name-->");
                }*/
            }

            specs.Add(details.Size_UC.Trim().Substring(0, details.Size_UC.IndexOf(" ")) + "<!--Size_UC-->");
            if (XmlRemapping(details.Taxonomy.ToLower(), "Types").Equals("wood"))
            {
                specs.Add(details.Species + "<!--Species-->");
            }
            else if (XmlRemapping(details.Taxonomy.ToLower(), "Types").Equals("vinyl"))
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
                specs.Add("" + "<!--Laminate?? No idea what should do here if anything.-->");
            }
            specs.Add(details.CcaSkuId + "<!--CCASKUID-->");

            return specs;
        }

        private static List<string> GetTrims(LARFinal lARFinal)
        {
            List<string> trimList = new();
            //lARFinal.LabelsFinal = GetImages(lARFinal.LabelsFinal);
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

        public static List<string> CreateXMLHS18x24BL(string[] files, bool skip, bool goWorkShop, LARFinal lARFinal, string plateId, string export)
        {
            //goWorkShop = true;
            List<string> missingImages = new();
            string division = XmlRemapping(lARFinal.DetailsFinal.Division_List, "Divisions");
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
            string roomScenePath = mainPath + settings.RoomscenePath;
            if (goWorkShop)
            {
                roomScenePath = "WebShop:webshop roomscenes:";
            }
            //string iconPath = mainPath + settings.IconPath;
            //characterPath = mainPath + settings.CharacteristicPath;
            //string installationPath = mainPath + settings.InstallationPath;
            //string trimsPath = mainPath + settings.TrimsPath;
            string imagesPath = mainPath + settings.ImagesPath;
            string category = XmlRemapping(lARFinal.DetailsFinal.Taxonomy.ToLower(), "Categories");
            string snippetCategory = "category:" + category + ".idms" + "<!--Taxonomy-->";
            string snippetWarranties = "warranties:" + XmlRemapping(lARFinal.DetailsFinal.Division_Rating.ToLower(), "Ratings") + " " + category + ".idms" + "<!--Division_Rating-->";
            string styleName = ConvertToTitleCase(lARFinal.SampleFinal.Sample_Name.Trim());

            string roomScene = "FPOwaitingonroom.tif";
            foreach (string s in lARFinal.SampleFinal.Merchandised_Product_Color_ID_C1)
            {
                var result = GetSyncedRoomscenes(files, s, roomScene);
                roomScene = result.Item1;
                files = result.Item2;
            }
            if (roomScene.Contains("FPOwaitingonroom"))
            {
                foreach (string s in lARFinal.SampleFinal.Merchandised_Product_Color_ID_FA)
                {
                    var result = GetSyncedRoomscenes(files, s, roomScene);
                    roomScene = result.Item1;
                    files = result.Item2;
                }
            }
            foreach (string s in files)
            {
                if (!lARFinal.DetailsFinal.Merchandised_Product_Color_ID.EqualsString(""))
                {
                    if (Path.GetFileName(s).StartsWith(lARFinal.DetailsFinal.Merchandised_Product_Color_ID))
                    {
                        roomScene = Path.GetFileName(s);
                    }
                }
            }
            if (!lARFinal.DetailsFinal.Roomscene.Trim().Equals(""))
            {
                roomScene = lARFinal.DetailsFinal.Roomscene + "<!--Roomscene-->";
            }
            if (roomScene.EqualsString("FPOwaitingonroom.tif"))
            {
                //files = ApprovedRoomscenes();
                foreach (string s in files)
                {
                    foreach (string r in lARFinal.SampleFinal.Merchandised_Product_Color_ID_C1)
                    {
                        if (!r.EqualsString(""))
                        {
                            if (Path.GetFileName(s).StartsWith(r))
                            {
                                roomScene = Path.GetFileName(s);
                            }
                        }
                    }
                    foreach (string r in lARFinal.SampleFinal.Merchandised_Product_Color_ID_FA)
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

            List<string> specList = GetSpecList(lARFinal.DetailsFinal);
            List<string> xmlData = new();

            xmlData.Add("<jobs>");
            xmlData.Add("	<job>");
            xmlData.Add("		<template useremail=\"" + userEmail + "\">" + CheckForMissing(template, missingImages) + "</template>");
            xmlData.Add("		<output jobname=\"" + jobName + "\">Webshop:InputPDF</output>");
            xmlData.Add("		<snippets>");
            xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(snippetPath + snippetCategory, missingImages) + "</snippet>");
            xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(snippetPath + snippetWarranties, missingImages) + "</snippet>");

            foreach (Labels b in lARFinal.LabelsFinal)
            {
                if (b.Division_Label_Type.ToLower().Trim().Equals("logo"))
                {
                    xmlData.Add("           <snippet page=\"1\">" + CheckForMissing(snippetPath + XmlRemapping(b.Division_Label_Name, "Images", "path", true), missingImages) + "</snippet>");
                }
            }
            xmlData.Add("		</snippets>");
            xmlData.Add("		<texts>");
            xmlData.Add("			<text type=\"spec\">" + SpecLine(specList) + "</text>");
            int count = 1;
            foreach (string s in specList)
            {
                string spec = s;
                if (settings.Id.Equals("c1") && s.ToLower().Contains("n/a"))
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
            xmlData.Add("			<images>");

            foreach (string c in GetCharacteristics(lARFinal))
            {
                xmlData.Add("				<image>" + CheckForMissing(imagesPath + c, missingImages) + "</image>");
            }
            foreach (string i in GetInstallation(lARFinal))
            {
                xmlData.Add("				<image>" + CheckForMissing(imagesPath + i, missingImages) + "</image>");
            }
            foreach (string t in GetTrims(lARFinal))
            {
                xmlData.Add("				<image>" + CheckForMissing(imagesPath + t, missingImages) + "</image>");
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
            xmlData.AddRange(InsiteXMLSnippet("", "BL", lARFinal.DetailsFinal.Supplier_Name, lARFinal.DetailsFinal.Division_List, styleName, ConvertToTitleCase(lARFinal.DetailsFinal.Merch_Color_Name), Path.GetFileNameWithoutExtension(template.Replace(":", "\\"))));
            xmlData.Add("	</job>");
            xmlData.Add("</jobs>");

            if (!template.Equals(""))
            {
                if (goWorkShop)
                {
                    ExportXML(jobName, xmlData, export, "WorkShop XML");
                    //CreateInsiteXML("BL", settings.PreJobTemplateName, settings.PreJobPath, lARFinal.DetailsFinal.Supplier_Name, lARFinal.DetailsFinal.Division_List, export, jobName, styleName, specList[0], Path.GetFileNameWithoutExtension(template.Replace(":", "\\")));
                    goWorkShop = false;
                }
                else
                {
                    ExportXML(jobName, xmlData, export, "WebShop XML");
                    CreateXMLHS18x24BL(files, skip, true, lARFinal, plateId, export);
                }

            }
            return missingImages;
        }
    }
}