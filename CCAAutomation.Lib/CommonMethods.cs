using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace CCAAutomation.Lib
{
    public class CommonMethods
    {
        /// <summary>
        /// Convert a fraction to a decimal
        /// </summary>
        /// <param name="stringIn">The string to be converted to a decimal</param>
        /// <returns>Returns a string that is a decimal value of a fraction.</returns>
        public static string ConvertToDecimal(string stringIn)
        {
            string stringOut = "";
            stringIn = stringIn.Replace("\"", "");
            if (!stringIn.Contains("."))
            {
                string[] stringSplit = stringIn.Split('-');
                try
                {
                    string[] stringFraction = stringSplit[1].Split('/');
                    double value = double.Parse(stringFraction[0]) / double.Parse(stringFraction[1]);
                    double stringDecimal = value + int.Parse(stringSplit[0]);
                    stringOut = stringDecimal.ToString();
                }
                catch (Exception)
                {
                    Console.WriteLine("Problem With Fraction Conversion.");
                }
            }
            return stringOut + "\"";
        }

        /// <summary>
        /// Convert a string to Title Case
        /// </summary>
        /// <param name="passedString">String to be converted to title case.</param>
        /// <returns>Returns the passed string as Title Case.</returns>
        public static string ConvertToTitleCase(string passedString)
        {
            TextInfo textInfo = new CultureInfo("en-us", false).TextInfo;
            return textInfo.ToTitleCase(passedString.ToLower());
        }

        /// <summary>
        ///     Checks a string for special characters like &
        /// </summary>
        /// <param name="passedString">The string to be checked.</param>
        /// <returns>Returns the processed string.</returns>
        public static string SpecialCharacters(string passedString)
        {
            string s = passedString;
            s = s.Replace("&", "&amp;");
            s = s.Replace("é", "&#233;");
            s = s.Replace("É", "&#201;");
            //s = s.Replace("<", "&#60;");
            //s = s.Replace(">", "&#62;");
            //s = s.Replace("½", "1/2");

            return s;
        }

        /// <summary>
        /// Gets a list of column names
        /// </summary>
        /// <param name="sheet">Spreadsheet to process</param>
        /// <returns>Returns a list of column names.</returns>
        public static List<string> GetHeaderColumns(ISheet sheet)
        {
            int count = 0;
            List<string> headers = new();
            while (!IsCellBlank(sheet, 0, count))
            {
                headers.Add(GetCell(sheet, 0, count));
                count++;
            }
            return headers;
        }

        /// <summary>
        /// Gets the number of rows in the spreadsheet.
        /// </summary>
        /// <param name="sheet">The sheet to check</param>
        /// <returns>Returns the number of rows.</returns>
        public static int GetRowCount(ISheet sheet)
        {
            int count = 1;
            while (!IsCellBlank(sheet, count, 0))
            {
                count++;
            }
            return count;
        }

        /// <summary>
        ///     Gets the info from a excel cell.
        /// </summary>
        /// <param name="sheet">The sheet to check in the excel file.</param>
        /// <param name="r">The row.</param>
        /// <param name="c">The Column.</param>
        /// <returns>Returns the Cells Value.</returns>
        public static string GetCell(ISheet sheet, int r, int c)
        {
            string value = "";
            try
            {
                IRow row = sheet.GetRow(r);
                if (row != null)
                {
                    ICell cell = CellUtil.GetCell(row, c);
                    cell.SetCellType(CellType.String);
                    value = cell.StringCellValue;
                }
            }
            catch (Exception) { }

            return value.Trim();
        }

        /// <summary>
        ///     Checks to see if a cell is blank.
        /// </summary>
        /// <param name="sheet">The sheet to check in the excel files.</param>
        /// <param name="r">The row.</param>
        /// <param name="c">The cell.</param>
        /// <returns>Returns a bool of whether the cell is empty or not.</returns>
        private static bool IsCellBlank(ISheet sheet, int r, int c)
        {
            bool isEmpty = false;
            try
            {
                string value = GetCell(sheet, r, c);
                if (value.Trim().Equals(""))
                {
                    isEmpty = true;
                }
            }
            catch (Exception) { isEmpty = true; }

            return isEmpty;
        }
        
        /// <summary>
        /// Keep Roman Numerals UpperCase
        /// </summary>
        /// <param name="passedString">The string to check for Roman Numerals.</param>
        /// <returns></returns>
        public static string RomanNumeralHandling(string passedString)
        {
            string pattern = @"\bIi\b";
            passedString = Regex.Replace(passedString, pattern, "II");
            pattern = @"\bIii\b";
            passedString = Regex.Replace(passedString, pattern, "III");
            pattern = @"\bIv\b";
            passedString = Regex.Replace(passedString, pattern, "IV");
            pattern = @"\bVi\b";
            passedString = Regex.Replace(passedString, pattern, "VI");
            pattern = @"\bVii\b";
            passedString = Regex.Replace(passedString, pattern, "VII");
            pattern = @"\bViii\b";
            passedString = Regex.Replace(passedString, pattern, "VIII");
            pattern = @"\bIx\b";
            passedString = Regex.Replace(passedString, pattern, "IX");

            return passedString;
        }
        
        /// <summary>
        ///     Removes the XML Comments for Export
        /// </summary>
        /// <param name="passedString">XML string to have comments removed from</param>
        /// <returns></returns>
        public static string RemoveComment(string passedString)
        {
            passedString = Regex.Replace(passedString, "<!--.*?-->", String.Empty, RegexOptions.Singleline);
            return passedString;
        }

        /// <summary>
        /// Make a Spec Line for WebShop XML from a spec list
        /// </summary>
        /// <param name="specList">The spec list to be made into a spec line.</param>
        /// <returns></returns>
        public static string SpecLine(List<string> specList)
        {
            string newSpecLine = "";
            int count = 0;
            foreach (string s in specList)
            {
                if (count == 0)
                {
                    newSpecLine = string.Concat(newSpecLine, s);
                    count++;
                }
                else
                {
                    newSpecLine = string.Concat(newSpecLine, "^p" + s);
                }
            }

            return newSpecLine;
        }

        public static void DownloadRoomScene(string webAddress, string fileName)
        {
            string httpType = "https";
            WebClient webClient;

            Uri URL = webAddress.StartsWith(httpType, StringComparison.OrdinalIgnoreCase) ? new Uri(webAddress) : new Uri(httpType + webAddress);
            try
            {                
                HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(URL);
                request.Method = "HEAD";
                request.KeepAlive = false;
                request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.64 Safari/537.36";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                //HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    string disposition = response.Headers["Content-Disposition"];
                    fileName = disposition.Substring(disposition.IndexOf("filename=") + 10).Replace("\"", "");
                    //response.Close();
                }
                using (webClient = new WebClient())
                {
                    //webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.64 Safari/537.36");
                    //string header_contentDisposition = webClient.ResponseHeaders["Content-Disposition"];
                    //fileName = new ContentDisposition(header_contentDisposition).FileName;
                    webClient.DownloadFile(URL, @"\\Mac\Home\Desktop\CCA Test\" + fileName);
                }
            }
            catch (WebException)
            { }
            
        }

        private static (bool, string[]) FindRoomscene(string[] approvedRoomscenes, bool skip, string roomsceneName, string searchPath, string copyPath, string alias, bool doAliasOnly, string filePath, string originalAlias)
        {
            //string[] files = Directory.GetFiles(searchPath, roomsceneName, SearchOption.AllDirectories);
            string[] files = Array.FindAll(approvedRoomscenes, x => x.Contains(roomsceneName, StringComparison.OrdinalIgnoreCase));

            if (files.Length.Equals(0))
            {
                if (!skip)
                {
                    Console.WriteLine(roomsceneName + " does not exist. Press any button to continue.");
                    Console.WriteLine("Try again? (y/n)");
                    string goAgain = Console.ReadLine();
                    if (goAgain.ToLower().Trim().Equals("y"))
                    {
                        approvedRoomscenes = ApprovedRoomscenes();
                        var result = CopyRoomscene(approvedRoomscenes, skip, filePath, alias);
                        skip = result.Skip;
                        approvedRoomscenes = result.UpdatedFiles;
                    }
                    else
                    {
                        skip = true;
                    }
                }
            }
            else if (!files.Length.Equals(1))
            {
                Console.WriteLine("Multiple files exsist with the name " + roomsceneName + " choose one via the number:");
                int count = 0;
                foreach (string f in files)
                {
                    Console.WriteLine(count + ": " + f);
                    count++;
                }
                string responseString = Console.ReadLine();
                int.TryParse(responseString, out int response);
                if (!responseString.Trim().Equals(""))
                {
                    if (response < count)
                    {
                        FileInfo appovedRoomInfo = new(files[response]);
                        FileInfo webShopRoomInfo = new(Path.Combine(copyPath, Path.GetFileName(files[response])));
                        if (!webShopRoomInfo.Length.Equals(appovedRoomInfo.Length))
                        {
                            File.Copy(files[response], Path.Combine(copyPath, Path.GetFileName(files[response])), true);
                            using (StreamWriter sw = new(Path.Combine(alias, Path.GetFileName(files[response]))))
                            {
                                sw.WriteLine("Automated Alias");
                                Console.WriteLine("Alias Created");
                            }
                            Console.WriteLine(Path.GetFileName(files[0]) + " has been copied.");
                        }
                        else
                        {
                            Console.WriteLine("Roomscene is Up to date.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Skipping...");
                }
            }
            else
            {
                FileInfo appovedRoomInfo = new(files[0]);
                FileInfo webShopRoomInfo = new(Path.Combine(copyPath, Path.GetFileName(files[0])));
                if (webShopRoomInfo.Exists)
                {
                    if (!webShopRoomInfo.Length.Equals(appovedRoomInfo.Length))
                    {
                        File.Copy(files[0], Path.Combine(copyPath, Path.GetFileName(files[0])), true);
                        using (StreamWriter sw = new(Path.Combine(alias, Path.GetFileName(files[0]))))
                        {
                            sw.WriteLine("Automated Alias");
                            Console.WriteLine("Alias Created");
                        }
                        Console.WriteLine(Path.GetFileName(files[0]) + " has been copied.");
                    }
                    else
                    {
                        Console.WriteLine("Roomscene is Up to date.");
                    }
                }
                else
                {
                    File.Copy(files[0], Path.Combine(copyPath, Path.GetFileName(files[0])), true);
                    using (StreamWriter sw = new(Path.Combine(alias, Path.GetFileName(files[0]))))
                    {
                        sw.WriteLine("Automated Alias");
                        Console.WriteLine("Alias Created");
                    }
                    Console.WriteLine(Path.GetFileName(files[0]) + " has been copied.");
                }
            }

            if (doAliasOnly)
            {
                using (StreamWriter sw = new(Path.Combine(alias, roomsceneName)))
                {
                    sw.WriteLine("Automated Alias");
                }
                Console.WriteLine("Alias created for " + roomsceneName);
            }

            return (skip, approvedRoomscenes);
        }
        
        public static (bool Skip, string[] UpdatedFiles) CopyRoomscene(string[] files, bool skip, string filePath, string alias)
        {
            bool doAliasOnly = false;
            string fileToCheck = "";
            string aliasPath = "";
            if (!filePath.StartsWith("\\\\MAG1PVSF7\\") && !filePath.StartsWith("/Volumes/"))
            {
                fileToCheck = "\\\\MAG1PVSF7\\" + RemoveComment(filePath.Replace(":", "\\"));
            }
            if (!alias.StartsWith("\\\\MAG1PVSF7\\") && !alias.StartsWith("/Volumes/"))
            {
                aliasPath = "\\\\MAG1PVSF7\\" + alias.Replace(":", "\\");
            }
            string searchPath = "\\\\MAG1PVSF4\\" + "Resources\\Approved Roomscenes\\";
            string copyPath = "\\\\MAG1PVSF7\\" + "WebShop\\Webshop Roomscenes\\";
            
                

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (!filePath.StartsWith("/Volumes/"))
                {
                    fileToCheck = "/Volumes/" + RemoveComment(filePath.Replace(":", "/"));
                }
                searchPath = "/Volumes/" + "Resources/Approved Roomscenes/";
                copyPath = "/Volumes/" + "WebShop/Webshop Roomscenes/";
                if (!alias.StartsWith("/Volumes/"))
                {
                    aliasPath = "/Volumes/" + alias.Replace(":", "/");
                }
            }            

            if (fileToCheck.ToLower().Contains("roomscene"))
            {
                Console.WriteLine("Attempting to find " + Path.GetFileName(fileToCheck) + " under Approved Roomscenes...");
                var result = FindRoomscene(files, skip, Path.GetFileName(fileToCheck), searchPath, copyPath, aliasPath, doAliasOnly, filePath, alias);
                skip = result.Item1;
                files = result.Item2;
                //skip = FindRoomscene(files, skip, Path.GetFileName(fileToCheck), searchPath, copyPath, aliasPath, doAliasOnly, filePath, alias);
            }

            if (!File.Exists(Path.Combine(aliasPath, Path.GetFileName(fileToCheck))))
            {
                doAliasOnly = true;
                var result = FindRoomscene(files, skip, Path.GetFileName(fileToCheck), searchPath, copyPath, aliasPath, doAliasOnly, filePath, alias);
                skip = result.Item1;
                files = result.Item2;
            }

            return (skip, files);
        }

        public static string[] ApprovedRoomscenes()
        {
            string searchPath = "\\\\MAG1PVSF4\\" + "Resources\\Approved Roomscenes\\";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                searchPath = "/Volumes/" + "Resources/Approved Roomscenes/";
            }

            string[] files = Directory.GetFiles(searchPath, "*",SearchOption.AllDirectories);

            return files;
        }

        public static void SyncRoomsceneDoubleCheck(List<LarModels.Details> detailsList, string outputPath)
        {
            Settings.MainSettings mainSettings = Settings.GetMainSettings();

            foreach (LarModels.Details d in detailsList)
            {
                string[] files = Directory.GetFiles(OsXPathConversion(mainSettings.SyncFolder), d.Merchandised_Product_Color_ID + "*", SearchOption.AllDirectories);
                if (files.Any())
                {
                    using (StreamWriter streamWriter = new(Path.Combine(outputPath, "Possible Roomscene Matches.txt"), append: true))
                    {
                        int lineId = detailsList.IndexOf(x => x.Manufacturer_Feeler.EqualsString("yes"));
                        foreach (string f in files)
                        {
                            streamWriter.WriteLine(detailsList[lineId].Plate_ID + "|" + detailsList[lineId].Sample_ID + "|" + detailsList[lineId].Merchandised_Product_Color_ID + "|" + Path.GetFileName(f));
                        }
                    }
                }
            }            
        }

        /// <summary>
        /// Copy and Move Roomscene from Syncplicity to approved roomscenes.
        /// </summary>
        /// <param name="merchProdColorId">The Merchandised_Product_Color_ID to search for on Syncplicity.</param>
        /// <returns></returns>
        public static (string Roomscene, string[] UpdatedFiles) GetSyncedRoomscenes(string[] approvedRoomscenesFiles, string merchProdColorId, string roomsceneName)
        {
            Settings.MainSettings mainSettings = Settings.GetMainSettings();

            try
            {
                if (!merchProdColorId.EqualsString(""))
                {
                    string[] files = Directory.GetFiles(OsXPathConversion(mainSettings.SyncFolder), merchProdColorId + "*", SearchOption.AllDirectories);

                    if (!files.Length.Equals(0))
                    {
                        foreach (string f in files)
                        {
                            FileInfo file = new(f);
                            if (file.Exists)
                            {
                                string filename = file.Name;
                                Console.WriteLine("Copying New Roomscene from Syncplicity");
                                Directory.CreateDirectory(mainSettings.ApprovedRoomScenes);
                                file.CopyTo(Path.Combine(OsXPathConversion(mainSettings.ApprovedRoomScenes), file.Name), true);

                                Console.WriteLine("Moving New Roomscene to Processed folder on Syncplicity");
                                //Directory.CreateDirectory(Path.Combine(osXPathConversion(mainSettings.SyncFolderProcessed)));
                                file.MoveTo(Path.Combine(OsXPathConversion(mainSettings.SyncFolderProcessed), file.Name), true);

                                roomsceneName = Path.GetFileName(file.FullName);
                                approvedRoomscenesFiles = ApprovedRoomscenes();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Error trying to move or copy the file " + roomsceneName + " on Syncplicity.");
                Console.WriteLine(e.Message);
                Console.WriteLine("-----------------------------------");
            }

            return (roomsceneName, approvedRoomscenesFiles);
        }

        /// <summary>
        ///     Checks for Missing Images and reports it the Console.
        /// </summary>
        /// <param name="filePath">The file path to be checked.</param>
        /// <returns></returns>
        public static string CheckForMissing(string filePath, List<string> missingImages)
        {
            string fileToCheck = "\\\\MAG1PVSF7\\" + RemoveComment(filePath.Replace(":", "\\"));


            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                fileToCheck = "/Volumes/" + RemoveComment(filePath.Replace(":", "/"));
            }

            if (!File.Exists(fileToCheck))
            {
                //Console.WriteLine("The following file is missing: ");
                //Console.WriteLine(fileToCheck);
                missingImages.Add(fileToCheck);
            }

            return filePath;
        }

        /// <summary>
        ///     XML snippet to update the WorkShop job with Insite information.
        /// </summary>
        /// <param name="surface">Soft Surface (" - ss") or Hard Surface ("")</param>
        /// <param name="type">Front Label or Back Label</param>
        /// <param name="supplier">The customer for Insite</param>
        /// <param name="division">The division of the product. Ex. FA or C1</param>
        /// <param name="styleName">The style name to be used in the job alias.</param>
        /// <param name="colorName">The color name to be used in the job alias.</param>
        /// <param name="template">The template name to be used in the job alias.</param>
        public static List<string> InsiteXMLSnippet(string surface, string type, string supplier, string division, string styleName, string colorName, string template)
        {
            List<string> xmlData = new();
            if (type.ToLower().Trim().Equals("fl"))
            {
                xmlData.Add("       <InsiteGroup>" + XmlMethods.XmlRemapping(supplier.ToLower() + surface, "InsiteCustomers") + "_FL" + "</InsiteGroup>");
            }
            else
            {
                xmlData.Add("       <InsiteGroup>" + XmlMethods.XmlRemapping(supplier.ToLower() + surface, "InsiteCustomers") + "</InsiteGroup>");
            }
            xmlData.Add("       <alias>");
            xmlData.Add("           <jobaliass>");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                xmlData.Add("               <jobalias>" + styleName + " - " + RemoveComment(colorName) + " - " + template.Substring(template.LastIndexOf("\\"), (template.Length - template.LastIndexOf("\\"))).Replace("\\", "") + " - " + type + "</jobalias>");
            }
            else
            {
                xmlData.Add("               <jobalias>" + styleName + " - " + RemoveComment(colorName) + " - " + Path.GetFileNameWithoutExtension(template) + "</jobalias>");
            }
            xmlData.Add("           </jobaliass>");
            xmlData.Add("       </alias>");

            return xmlData;
        }

        /// <summary>
        ///     Created the XML to update the WorkShop job with Insite information.
        /// </summary>
        /// <param name="preJobTemplateName">WorkShop Pre Job Name</param>
        /// <param name="preJobPath">WorkShop Pre Job Path</param>
        /// <param name="supplier">The customer for Insite</param>
        /// <param name="division">The division of the product. Ex. FA or C1</param>
        /// <param name="export">The export Path</param>
        /// <param name="jobName">The job name of the WorkShop job.</param>
        /// <param name="styleName">The style name to be used in the job alias.</param>
        /// <param name="colorName">The color name to be used in the job alias.</param>
        /// <param name="template">The template name to be used in the job alias.</param>
        public static void CreateInsiteXML(string type, string preJobTemplateName, string preJobPath, string supplier, string division, string export, string jobName, string styleName, string colorName, string template)
        {
            List<string> xmlData = new();

            xmlData.Add("<jobs>");
            xmlData.Add("	<job>");
            xmlData.Add("       <JobName>" + jobName + "</JobName>");
            xmlData.Add("		<JobTemplate>" + preJobTemplateName + "</JobTemplate>");
            xmlData.Add("		<JobGroup>" + preJobPath + "</JobGroup>");
            xmlData.Add("       <InsiteGroup>" + XmlMethods.XmlRemapping(supplier.ToLower() + " - " + division.ToLower(), "InsiteCustomers") + "</InsiteGroup>");
            xmlData.Add("       <alias>");
            xmlData.Add("           <jobaliass>");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                xmlData.Add("               <jobalias>" + styleName + " - " + RemoveComment(colorName) + " - " + template.Substring(template.LastIndexOf("\\"), (template.Length - template.LastIndexOf("\\"))).Replace("\\", "") + " - " + type + "</jobalias>");
            }
            else
            {
                xmlData.Add("               <jobalias>" + styleName + " - " + RemoveComment(colorName) + " - " + Path.GetFileNameWithoutExtension(template) + "</jobalias>");
            }
            xmlData.Add("           </jobaliass>");
            xmlData.Add("       </alias>");
            xmlData.Add("   </job>");
            xmlData.Add("</jobs>");
            ExportXML(jobName, xmlData, export, "Insite XML");
        }

        /// <summary>
        ///     Export the XML file.
        /// </summary>
        /// <param name="jobName">The name of the xml file.</param>
        /// <param name="xmlData">The xml data that will be exported.</param>
        /// <param name="export">Export Path.</param>
        /// <param name="type">The use of the XML.</param>
        public static void ExportXML(string jobName, List<string> xmlData, string export, string type)
        {
            try
            {
                export = Path.Combine(export, type);
                Directory.CreateDirectory(export);
                using (StreamWriter xmlFile = new(Path.Combine(export, jobName + ".xml")))
                {
                    foreach (string s in xmlData)
                    {
                        xmlFile.WriteLine(SpecialCharacters(RomanNumeralHandling(RemoveComment(s))));
                    }
                }

                Console.WriteLine(type + " file " + jobName + ".xml has been created.");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static int Check_Prime(int number)
        {
            int i;
            for (i = 2; i <= number - 1; i++)
            {
                if (number % i == 0)
                {
                    return 0;
                }
            }
            if (i == number)
            {
                return 1;
            }
            return 0;
        }

        public static string OsXPathConversion(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                path = "/Volumes" + RemoveComment(path).Replace(path.Substring(0, path.IndexOfNth("\\", 2)), "").Replace(":", "/").Replace("\\", "/");
            }

            return path;
        }

        public static LarModels.LARFinal GetMerchandisedProductColorIds(LarModels.LARFinal lf)
        {
            string merchandisedProductColorID = "";

            if (lf.DetailsFinal.Sample_ID.EqualsString(lf.SampleFinal.Sample_ID))
            {
                if (lf.SampleFinal.Feeler.EqualsString(lf.DetailsFinal.Merch_Color_Name) && lf.DetailsFinal.Sample_ID.EqualsString(lf.SampleFinal.Sample_ID))
                {
                    merchandisedProductColorID = lf.DetailsFinal.Merchandised_Product_Color_ID;
                }
                if (lf.DetailsFinal.Merchandised_Product_Color_ID.EqualsString(merchandisedProductColorID) && lf.DetailsFinal.Sample_ID.EqualsString(lf.SampleFinal.Sample_ID))
                {
                    if (lf.DetailsFinal.Division_List.Trim().ToLower().Contains("fa") && !lf.DetailsFinal.Division_List.EqualsString("fc"))
                    {
                        lf.SampleFinal.Merchandised_Product_Color_ID_FA.Add(lf.DetailsFinal.Merchandised_Product_Color_ID);
                    }
                    if (lf.DetailsFinal.Division_List.Trim().ToLower().Contains("c1") && !lf.DetailsFinal.Division_List.EqualsString("cn"))
                    {
                        lf.SampleFinal.Merchandised_Product_Color_ID_C1.Add(lf.DetailsFinal.Merchandised_Product_Color_ID);
                    }
                }
            }

            return lf;
        }

        public static LarModels.LARFinal GetMerchandisedProductColorIds(LarModels.LARFinal lf, LarModels.LARXlsSheet ls)
        {
            string manufacturerProductColorID = "";

            /*foreach (LarModels.Details d in ls.DetailsList)
            {
                foreach (LarModels.Sample s in ls.SampleList)
                {
                    if (d.Sample_ID.Equals(lf.DetailsFinal.Sample_ID))
                    {
                        if (s.Feeler.Trim().ToLower().Equals(lf.DetailsFinal.Merch_Color_Name.Trim().ToLower()) && lf.DetailsFinal.Sample_ID.EqualsString(s.Sample_ID))
                        {
                            manufacturerProductColorID = lf.DetailsFinal.Manufacturer_Product_Color_ID;
                        }
                    }
                }
            }*/
            if (ls.DetailsList.Any(d => d.Sample_ID.Equals(lf.DetailsFinal.Sample_ID)))
            {
                if (ls.SampleList.Any(s => (s.Feeler.EqualsString(lf.DetailsFinal.Merch_Color_Name) && s.Sample_ID.EqualsString(lf.DetailsFinal.Sample_ID))))
                {
                    manufacturerProductColorID = lf.DetailsFinal.Manufacturer_Product_Color_ID;
                }
            }

            foreach (LarModels.Details d in ls.DetailsList)
            {
                if (d.Manufacturer_Product_Color_ID.Equals(manufacturerProductColorID))
                {
                    if (d.Division_List.Trim().ToLower().Contains("fa") && !d.Division_List.Trim().ToLower().Equals("fc"))
                    {
                        lf.SampleFinal.Merchandised_Product_Color_ID_FA.Add(d.Merchandised_Product_Color_ID);
                    }
                    if (d.Division_List.Trim().ToLower().Contains("c1") && !d.Division_List.Trim().ToLower().Equals("cn"))
                    {
                        lf.SampleFinal.Merchandised_Product_Color_ID_C1.Add(d.Merchandised_Product_Color_ID);
                    }
                }
            }

            /*int index = ls.DetailsList.FindIndex(d => (d.Manufacturer_Product_Color_ID.EqualsString(manufacturerProductColorID) &&
            d.Division_List.Trim().ToLower().Contains("fa") && !d.Division_List.Trim().ToLower().Equals("fc")));
            if (index != -1)
            {
                lf.SampleFinal.Merchandised_Product_Color_ID_FA.Add(ls.DetailsList[index].Merchandised_Product_Color_ID);
            }

            index = ls.DetailsList.FindIndex(d => (d.Manufacturer_Product_Color_ID.EqualsString(manufacturerProductColorID) &&
            d.Division_List.Trim().ToLower().Contains("c1") && !d.Division_List.Trim().ToLower().Equals("cn")));
            if (index != -1)
            {
                lf.SampleFinal.Merchandised_Product_Color_ID_C1.Add(ls.DetailsList[index].Merchandised_Product_Color_ID);
            }*/

            return lf;
        }
    }
}
public static class Extensions
{
    public static int IndexOfNth(this string str, string value, int nth)
    {
        if (nth < 0)
            throw new ArgumentException("Can not find a negative index of substring in string. Must start with 0");

        int offset = str.IndexOf(value);
        for (int i = 0; i < nth; i++)
        {
            if (offset == -1) return -1;
            offset = str.IndexOf(value, offset + 1);
        }

        return offset;
    }

    public static bool EqualsString(this string str, string compare)
    {
        bool value = false;
        if (str == null)
        {
            str = "";
        }
        if (str.Trim().ToLower() == compare.Trim().ToLower())
        {
            value = true;
        }
        return value;
    }
    public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {

        var index = 0;
        foreach (var item in source)
        {
            if (predicate.Invoke(item))
            {
                return index;
            }
            index++;
        }

        return -1;
    }    
}
static class EnumerableExtensions
{
    public static T MaxObject<T, U>(this IEnumerable<T> source, Func<T, U> selector)
      where U : IComparable<U>
    {
        if (source == null) throw new ArgumentNullException("source");
        bool first = true;
        T maxObj = default(T);
        U maxKey = default(U);
        foreach (var item in source)
        {
            if (first)
            {
                maxObj = item;
                maxKey = selector(maxObj);
                first = false;
            }
            else
            {
                U currentKey = selector(item);
                if (currentKey.CompareTo(maxKey) > 0)
                {
                    maxKey = currentKey;
                    maxObj = item;
                }
            }
        }
        if (first) throw new InvalidOperationException("Sequence is empty.");
        return maxObj;
    }
}