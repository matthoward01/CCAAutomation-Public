using System;
using System.Collections.Generic;
using static CCAAutomation.Lib.LarModels;
using static CCAAutomation.Lib.CommonMethods;
using static CCAAutomation.Lib.Lar;
using static CCAAutomation.Lib.Execute;
using System.Linq;
using CCAAutomation.Lib;
using System.IO;
using System.Threading;

namespace CCAAutomation.App
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {            
            StartupChoices(args);
        }

        private static void StartupChoices(string[] args = null)
        {
            bool webIntegration = false;
            Console.WriteLine("---------------------------------");
            Console.WriteLine("/////////////////////////////////");
            Console.WriteLine("Must be connected to Webshop, Resources, and CCA_Approved_Roomscenes or you will get errors.");
            Console.WriteLine("/////////////////////////////////");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Function?");
            Console.WriteLine("\"c\" to compare xml files in two different directories.");
            Console.WriteLine("\"s\" to make shaw versions for approval.");
            Console.WriteLine("\"i\" to update the roomscene array.");
            Console.WriteLine("\"r\" to get roomscene names from xml.");
            Console.WriteLine("Anything else or nothing to continue to Program");
            string choice = Console.ReadLine().Trim().ToLower();
            if (choice.EqualsString(""))
            {
                if (!webIntegration)
                {
                    StandAloneApp(false);
                }
                else
                {
                    WebIntegrationApp(false);
                }
            }
            else if (choice.EqualsString("s"))
            {
                if (!webIntegration)
                {
                    StandAloneApp(true);
                }
                else
                {
                    WebIntegrationApp(true);
                }
            }
            else if (choice.EqualsString("c"))
            {
                CompareXMLFiles();
            }
            else if (choice.EqualsString("i"))
            {
                ImportData();
            }
            else if (choice.EqualsString("r"))
            {
                GetRoomsceneFromXml();
            }
        }

        private static void GetRoomsceneFromXml()
        {
            Console.WriteLine("XML Folder location...");
            string xmlFolder = Console.ReadLine().Replace("\"", "");
            Console.WriteLine("LAR File location...");
            string larFile = Console.ReadLine().Replace("\"", "");
            if (Directory.Exists(xmlFolder) && File.Exists(larFile))
            {
                string[] xmlFileArray = Directory.GetFiles(xmlFolder, "*.xml", SearchOption.AllDirectories);
                CreateListOfRoomscenes(xmlFileArray, larFile);
            }
        }

        private static void ImportData()
        {
            bool isSoftSurface = false;
            Console.WriteLine("For Soft Surface type \"s\" | For Hard Surface type \"h\"");
            string type = Console.ReadLine();
            if (type.EqualsString("s"))
            {
                isSoftSurface = true;
            }
            else if (type.EqualsString("h"))
            {
                isSoftSurface = false;
            }
            else
            {
                Console.WriteLine("Please choose \"s\" or \"h\"");
                ImportData();
            }
            Console.WriteLine("New File Name?");
            string newFileName = Console.ReadLine().Replace("\"", "");
            if (File.Exists(newFileName))
            {
                LARXlsSheet lARXlsSheet = GetLar(newFileName);
                Console.WriteLine("Proceed with the database update? Please type \"yes\"");
                if (Console.ReadLine().EqualsString("yes"))
                {
                    SqlMethods.SqlWebDBUpdate(lARXlsSheet, isSoftSurface);
                }
                else
                {
                    StartupChoices();
                }
            }
        }

        private static void CompareXMLFiles()
        {            
            Console.WriteLine("New Folder Path?");
            string newFolderPath = Console.ReadLine().Replace("\"", "");
            Console.WriteLine("Old Folder Path?");
            string oldFolderPath = Console.ReadLine().Replace("\"", "");

            if (Directory.Exists(Path.Combine(newFolderPath, "Results")))
            {
                Directory.Delete(Path.Combine(newFolderPath, "Results"), true);
            }

            string[] newFolderPathFiles = Directory.GetFiles(newFolderPath, "*.xml", SearchOption.AllDirectories);
            foreach (string f in newFolderPathFiles)
            {
                using (StreamReader newFolderReader = new(f))
                {
                    if (File.Exists(Path.Combine(oldFolderPath, Path.GetFileName(f))))
                    {
                        using (StreamReader oldFolderReader = new(Path.Combine(oldFolderPath, Path.GetFileName(f))))
                        {
                            while (!newFolderReader.EndOfStream)
                            {
                                string newLine = newFolderReader.ReadLine();
                                string oldLine = oldFolderReader.ReadLine();
                                if (!newLine.Equals(oldLine))
                                {
                                    Directory.CreateDirectory(Path.Combine(newFolderPath, "Results"));
                                    using (StreamWriter writer1 = new(Path.Combine(newFolderPath, "Results", Path.GetFileNameWithoutExtension(f) + ".txt"), append: true))
                                    {
                                        if (!newLine.Contains("<jobalias>") && !newLine.Contains("<text type=\"spec\">"))
                                        {
                                            writer1.WriteLine(newLine);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            StartupChoices();
        }

        private static void WebIntegrationApp(bool isShaw)
        {            
            try
            {
                bool go = true;
                List<string> missingImagesProc = new();

                string export = "\\\\Mac\\Home\\Desktop\\CCA Test\\SQL Testing\\";
                                
                string[] files = ApprovedRoomscenes();

                while (go)
                {
                    List<string> runJobListHS = new(SqlMethods.GetRunJobs(false));
                    List<string> runJobListSS = new(SqlMethods.GetRunJobs(true));
                    foreach (string j in runJobListHS)
                    {
                        LARXlsSheet LARXlsSheet = GetLar(false, j);
                        missingImagesProc.AddRange(Run(isShaw, files, j, export, LARXlsSheet));
                        var uniqueMissing = missingImagesProc.Distinct();
                        foreach (string s in uniqueMissing)
                        {
                            Console.WriteLine(s);
                        }
                        SqlMethods.SqlSetToRun(j, false, 0);
                        missingImagesProc = new();
                    }
                    foreach (string j in runJobListSS)
                    {
                        LARXlsSheet LARXlsSheet = GetLar(true, j);
                        missingImagesProc.AddRange(Run(isShaw, files, j, export, LARXlsSheet));
                        var uniqueMissing = missingImagesProc.Distinct();
                        foreach (string s in uniqueMissing)
                        {
                            Console.WriteLine(s);
                        }
                        SqlMethods.SqlSetToRun(j, true, 0);
                        missingImagesProc = new();
                    }
                    Thread.Sleep(10000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void StandAloneApp(bool isShaw)
        {
            string fileName = "";
            bool go = true;
            List<string> missingImagesProc = new();

            Console.WriteLine("What XLS File?");
            fileName = Console.ReadLine();
            fileName = fileName.Replace("\"", "");

            /*Console.WriteLine("Export XML Where?");
            string export = Console.ReadLine();
            export = export.Replace("\"", "");
            if (export.EqualsString(""))
            {*/
                string export = Path.GetDirectoryName(fileName);
            //}
            //Console.WriteLine(export);

            LARXlsSheet LARXlsSheet = GetLar(fileName);
            string[] files = ApprovedRoomscenes();

            while (go)
            {
                Console.WriteLine("What Plate Id?");
                Console.WriteLine("To force a plate through use /f");
                Console.WriteLine("To run a shaw plate use /s");
                string plateId = Console.ReadLine();
                if (plateId.EqualsString("rebuild"))
                {
                    files = ApprovedRoomscenes();
                    Console.WriteLine("Roomscene Cache Rebuilt...");
                    Console.WriteLine("What Plate Id?");
                    plateId = Console.ReadLine();
                    if (plateId.EqualsString("restart"))
                    {
                        StartupChoices();
                    }
                }
                else if (plateId.EqualsString("restart"))
                {
                    StartupChoices();
                }
                missingImagesProc.AddRange(Run(isShaw, files, plateId, export, LARXlsSheet));
                var uniqueMissing = missingImagesProc.Distinct();
                foreach (string s in uniqueMissing)
                {
                    Console.WriteLine(s);
                }
                missingImagesProc = new();
            }

            StartupChoices();
        }
    }
}
