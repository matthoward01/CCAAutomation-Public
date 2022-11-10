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
            
            StartupChoices();
        }

        private static void StartupChoices()
        {
            bool webIntegration = false;
            Console.WriteLine("Function?");
            Console.WriteLine("\"c\" to compare xml files in two different directories.");
            Console.WriteLine("Anything else or nothing to continue to Program");
            string choice = Console.ReadLine();
            if (choice.EqualsString(""))
            {
                if (!webIntegration)
                {
                    StandAloneApp();
                }
                else
                {
                    WebIntegrationApp();
                }
            }
            else if (choice.EqualsString("c"))
            {
                CompareXMLFiles();
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
                                        writer1.WriteLine(newLine);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            StartupChoices();
        }

        private static void WebIntegrationApp()
        {            
            try
            {
                bool go = true;
                List<string> missingImagesProc = new();

                string export = "\\\\Mac\\Home\\Desktop\\CCA Test\\SQL Testing\\";

                bool goWorkshop = false;
                
                string[] files = ApprovedRoomscenes();

                while (go)
                {
                    List<string> runJobListHS = new(SqlMethods.GetRunJobs(false));
                    List<string> runJobListSS = new(SqlMethods.GetRunJobs(true));
                    foreach (string j in runJobListHS)
                    {
                        LARXlsSheet LARXlsSheet = GetLar(false, j);
                        missingImagesProc.AddRange(Run(files, goWorkshop, j, export, LARXlsSheet));
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
                        missingImagesProc.AddRange(Run(files, goWorkshop, j, export, LARXlsSheet));
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

        private static void StandAloneApp()
        {
            string fileName = "";
            bool go = true;
            List<string> missingImagesProc = new();

            Console.WriteLine("What XLS File?");
            fileName = Console.ReadLine();
            fileName = fileName.Replace("\"", "");

            Console.WriteLine("Export XML Where?");
            string export = Console.ReadLine();
            export = export.Replace("\"", "");

            bool goWorkshop = false;
            LARXlsSheet LARXlsSheet = GetLar(fileName);
            string[] files = ApprovedRoomscenes();

            while (go)
            {
                Console.WriteLine("What Plate Id?");
                string plateId = Console.ReadLine();
                if (plateId.EqualsString("rebuild"))
                {
                    ApprovedRoomscenes();
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
                missingImagesProc.AddRange(Run(files, goWorkshop, plateId, export, LARXlsSheet));
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
