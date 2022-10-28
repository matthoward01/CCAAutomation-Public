using System;
using System.Collections.Generic;
using static CCAAutomation.Lib.LarModels;
using static CCAAutomation.Lib.CommonMethods;
using static CCAAutomation.Lib.Lar;
using static CCAAutomation.Lib.Execute;
using System.Linq;
using CCAAutomation.Lib;
using System.IO;

namespace CCAAutomation.App
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
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
            Console.ReadLine();
        }

        private static void WebIntegrationApp()
        {            
            try
            {                
                bool go = true;
                while (go)
                {
                    List<string> runJobListHS = new(SqlMethods.GetRunJobs(false));
                    List<string> runJobListSS = new(SqlMethods.GetRunJobs(true));
                    foreach (string j in runJobListHS)
                    {
                        //LARFinal lf = new(SqlMethods.SqlGetLarFinal(false, j));
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private static void StandAloneApp()
        {
            string fileName = "";
            bool go = true;
            List<string> missingImagesProc = new List<string>();

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
                }
                missingImagesProc.AddRange(Run(files, goWorkshop, plateId, export, LARXlsSheet));
                var uniqueMissing = missingImagesProc.Distinct();
                foreach (string s in uniqueMissing)
                {
                    Console.WriteLine(s);
                }
                missingImagesProc = new();
            }

            Console.ReadLine();
        }
    }
}
