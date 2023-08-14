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
using System.Runtime.InteropServices;

namespace CCAAutomation.App
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {            
            StartupChoices();
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
            Console.WriteLine("\"r\" to get roomscene names from xml.");
            Console.WriteLine("\"e\" email parsing for insite.");
            Console.WriteLine("\"b\" create batch file for sending to press.");
            Console.WriteLine("\"d\" delete files based on one folder vs another (DANGEROUS KEEP A BACKUP OF THE FOLDERS!!!!).");
            Console.WriteLine("\"i\" text parsing for insite.");
            Console.WriteLine("\"l\" to get a list of unpackaged.");
            Console.WriteLine("\"p\" to create PDF LAR for MAS.");
            Console.WriteLine("\"m\" to move files based on LAR.");
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
            //else if (choice.EqualsString("i"))
            //{
            //    ImportData();
            //}
            else if (choice.EqualsString("r"))
            {
                GetRoomsceneFromXml();
            }
            else if (choice.EqualsString("e"))
            {
                StartEmailParsing();
            }
            else if (choice.EqualsString("d"))
            {
                DeleteFiles();
            }
            else if (choice.EqualsString("b"))
            {
                MakeBatchSendXml();
            }
            else if (choice.EqualsString("p"))
            {
                MakeLarPdf();
            }
            else if (choice.EqualsString("i"))
            {
                GetInsiteJobs();
            }
            else if (choice.EqualsString("l"))
            {
                GetUnPackaged();
            }
            else if (choice.EqualsString("m"))
            {
                MoveFiles();
            }
        }

        private static void MoveFiles()
        {
            bool isCanada = false;
            Console.WriteLine("Location of files...");
            string fileLocation = Console.ReadLine().Replace("\"", "");
            Console.WriteLine("Lar path...");
            string larPath = Console.ReadLine().Replace("\"", "");
            Console.WriteLine("Is this for Canada? (y/n) Default (n)");
            string isCanadaString = Console.ReadLine();
            if (isCanadaString.EqualsString("y"))
            {
                isCanada = true;
            }

            LARXlsSheet lARXlsSheet = GetLar(larPath);

            string[] fileArray = Directory.GetFiles(fileLocation, "*", SearchOption.AllDirectories);
            foreach(string f in fileArray)
            {
                string fileName = Path.GetFileNameWithoutExtension(f).Replace(".p1", "");
                int larIndex = lARXlsSheet.DetailsList.FindIndex(s => (s.Plate_ID_BL.EqualsString(fileName) || s.Plate_ID_FL.EqualsString(fileName)));

                if (larIndex != -1)
                {
                    int larSampleIndex = lARXlsSheet.SampleList.FindIndex(s => (s.Sample_ID.EqualsString(lARXlsSheet.DetailsList[larIndex].Sample_ID)));
                    if (larSampleIndex != -1)
                    {
                        string division = lARXlsSheet.DetailsList[larIndex].Division_List;
                        if (isCanada)
                        {
                            if (division.ToLower().Contains("cn"))
                            {
                                Directory.CreateDirectory(Path.Combine(fileLocation, "CN"));
                                if (File.Exists(f))
                                {
                                    if (fileName.ToLower().Contains("fl"))
                                    {
                                        Directory.CreateDirectory(Path.Combine(fileLocation, "CN", "Front Labels"));
                                        File.Move(f, Path.Combine(fileLocation, "CN", "Front Labels", Path.GetFileName(f)), true);
                                        Console.WriteLine(Path.GetFileName(f) + "have been moved to CN FLs");
                                    }
                                    else
                                    {
                                        Directory.CreateDirectory(Path.Combine(fileLocation, "CN", "Back Labels"));
                                        File.Move(f, Path.Combine(fileLocation, "CN", "Back Labels", Path.GetFileName(f)), true);
                                        Console.WriteLine(Path.GetFileName(f) + "have been moved to CN BLs");
                                    }
                                }
                            }
                            if (division.ToLower().Contains("fc"))
                            {
                                Directory.CreateDirectory(Path.Combine(fileLocation, "FC"));
                                if (fileName.ToLower().Contains("fl"))
                                {
                                    Directory.CreateDirectory(Path.Combine(fileLocation, "FC", "Front Labels"));
                                    File.Move(f, Path.Combine(fileLocation, "fc", "Front Labels", Path.GetFileName(f)), true);
                                    Console.WriteLine(Path.GetFileName(f) + "have been moved to FC FLs");
                                }
                                else
                                {
                                    Directory.CreateDirectory(Path.Combine(fileLocation, "FC", "Back Labels"));
                                    File.Move(f, Path.Combine(fileLocation, "fc", "Back Labels", Path.GetFileName(f)), true);
                                    Console.WriteLine(Path.GetFileName(f) + "have been moved to FC BLs");
                                }
                            }
                        }
                        else
                        {
                            string newFileName = lARXlsSheet.SampleList[larSampleIndex].Sample_Name.Replace("/", "-") + " - " + lARXlsSheet.SampleList[larSampleIndex].Feeler.Replace("/", "-");
                            if (division.ToLower().Contains("c1"))
                            {
                                Directory.CreateDirectory(Path.Combine(fileLocation, "C1"));
                                if (File.Exists(f))
                                {
                                    if (lARXlsSheet.DetailsList[larIndex].Plate_ID_FL.EqualsString(fileName))
                                    {
                                        Directory.CreateDirectory(Path.Combine(fileLocation, "C1", "Front Labels"));
                                        File.Move(f, Path.Combine(fileLocation, "C1", "Front Labels", newFileName + " - FL" + Path.GetExtension(f)), true);
                                        Console.WriteLine(Path.GetFileName(f) + "have been moved to C1 FLs");
                                    }
                                    else
                                    {
                                        Directory.CreateDirectory(Path.Combine(fileLocation, "C1", "Back Labels"));
                                        File.Move(f, Path.Combine(fileLocation, "C1", "Back Labels", newFileName + " - BL" + Path.GetExtension(f)), true);
                                        Console.WriteLine(Path.GetFileName(f) + "have been moved to C1 BLs");
                                    }
                                }
                            }
                            if (division.ToLower().Contains("fa"))
                            {
                                Directory.CreateDirectory(Path.Combine(fileLocation, "FA"));
                                if (lARXlsSheet.DetailsList[larIndex].Plate_ID_FL.EqualsString(fileName))
                                {
                                    Directory.CreateDirectory(Path.Combine(fileLocation, "FA", "Front Labels"));
                                    File.Move(f, Path.Combine(fileLocation, "FA", "Front Labels", newFileName + " - FL" + Path.GetExtension(f)), true);
                                    Console.WriteLine(Path.GetFileName(f) + "have been moved to FA FLs");
                                }
                                else
                                {
                                    Directory.CreateDirectory(Path.Combine(fileLocation, "FA", "Back Labels"));
                                    File.Move(f, Path.Combine(fileLocation, "FA", "Back Labels", newFileName + " - BL" + Path.GetExtension(f)), true);
                                    Console.WriteLine(Path.GetFileName(f) + "have been moved to FA BLs");
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Plate number not found.");
                }
            }
            StartupChoices();
        }

        private static void BatchXml()
        {
            List<string> batchList = new();

            Console.WriteLine("Text Document of Plate #'s to send...");
            string batchTextDocument = Console.ReadLine().Replace("\"", "");

            using (StreamReader sr = new(batchTextDocument))
            {
                while (!sr.EndOfStream)
                {
                    batchList.Add(sr.ReadLine());
                }
            }
            foreach (string b in batchList)
            {
               CreateBatchXML(Path.GetDirectoryName(batchTextDocument), b);
            }
            StartupChoices();
        }

        private static void GetUnPackaged()
        {
            List<string> approvedList = new();
            List<string> packagedList = new();

            Console.WriteLine("Text Document of approved Plate #'s...");
            string approvedTextDocument = Console.ReadLine().Replace("\"", "");
            Console.WriteLine("Text Document of packaged Plate #'s...");
            string packagedTextDocument = Console.ReadLine().Replace("\"", "");

            using (StreamReader sr = new(approvedTextDocument))
            {
                while (!sr.EndOfStream)
                {
                    approvedList.Add(sr.ReadLine());
                }
            }
            using (StreamReader sr = new(packagedTextDocument))
            {
                while (!sr.EndOfStream)
                {
                    packagedList.Add(sr.ReadLine());
                }
            }
            foreach (string a in approvedList)
            {
                if (!packagedList.Contains(a))
                {
                    Console.WriteLine(a);
                }
            }
            StartupChoices();
        }

        private static void DeleteFiles()
        {
            Console.WriteLine("This program compares two directories, and it will delete " +
                "files that exist in both directories from the path of files to delete.");
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("Path to files to possibly Delete...");
            string deleteDir = Console.ReadLine().Replace("\"", "");
            //string deleteDir = @"C:\Delete\";
            Console.WriteLine("Path to files to Keep...");
            string keepDir = Console.ReadLine().Replace("\"", "");
            Console.WriteLine("Delete if doesnt Exist (y/n) Default(n)");
            string doesntExistString = Console.ReadLine();
            bool doesntExist = false;
            if (doesntExistString.ToLower().Trim().Equals("y"))
            {
                doesntExist = true;
            }
            //string keepDir = @"C:\Keep\";
            Console.WriteLine("Creating Backup of Directories.");
            //CopyDirectory(deleteDir, Path.Combine(Path.GetDirectoryName(deleteDir), Path.GetFileNameWithoutExtension(deleteDir) + " Backup"));
            //CopyDirectory(keepDir, Path.Combine(Path.GetDirectoryName(keepDir), Path.GetFileNameWithoutExtension(keepDir) + " Backup"));

            string[] checkFiles = Directory.GetFiles(deleteDir, "*", SearchOption.AllDirectories);
            string[] doneFiles = Directory.GetFiles(keepDir, "*", SearchOption.AllDirectories);
            
            if (!doesntExist)
            {
                foreach (string s in checkFiles)
                {
                    for (int i = 0; i < doneFiles.Length; i++)
                    {

                        if (Path.GetFileNameWithoutExtension(doneFiles[i]) == Path.GetFileNameWithoutExtension(s))
                        {
                            Console.WriteLine("Found - " + doneFiles[i]);
                            Console.WriteLine("Deleting - " + s);
                            File.Delete(s);
                        }
                    }
                }
            }
            else
            {
                foreach (string s in checkFiles)
                {
                    int index = doneFiles.ToList().FindIndex(c => Path.GetFileNameWithoutExtension(c).Equals(Path.GetFileNameWithoutExtension(s)));
                    if (index == -1)
                    {
                        Console.WriteLine("Not Found - " + Path.GetFileNameWithoutExtension(s));
                        Console.WriteLine("Deleting - " + s);
                        File.Delete(s);
                    }
                }
            }
            Thread.Sleep(5000);
            Console.WriteLine("Cleanup Done...");
            Console.ReadLine();
            StartupChoices();
        }

        private static void StartEmailParsing()
        {
            bool go = true;
            do
            {
                while (go)
                {
                    Console.WriteLine("Getting Emails...");
                    Email.ReadEmails();

                    //Console.WriteLine("Checking for Run Jobs...");
                    //WebIntegrationApp(false);

                    Thread.Sleep(300000);
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;

            StartupChoices();
        }

        private static void MakeLarPdf()
        {
            Console.WriteLine("LAR XLS File...");
            string larFile = Console.ReadLine().Replace("\"", "");
            bool go = true;
            while (go)
            {
                Console.WriteLine("Sample Id...");
                string sampleId = Console.ReadLine().Replace("\"", "");
                if (sampleId.EqualsString("stop"))
                {
                    go = false;
                }
                else
                {
                    LarPdf.GetInfo(larFile, sampleId);
                }
            }
            StartupChoices();
        }

        private static void GetInsiteJobs()
        {
            Console.WriteLine("Text Document of copy and paste from Insite...");
            string insiteTextDocument = Console.ReadLine().Replace("\"", "");
            using (StreamReader sr = new(insiteTextDocument))
            {
                while (!sr.EndOfStream)
                {
                    Console.WriteLine(sr.ReadLine().Split('(', ')')[1]);
                }
            }
            StartupChoices();
        }

        private static void MakeBatchSendXml()
        {
            Console.WriteLine("Text Document of Plate numbers to send to press...");
            string batchTextDocument = Console.ReadLine().Replace("\"", "");
            if (File.Exists(batchTextDocument))
            {
                using (StreamReader batchTextDocumentReader = new(Path.Combine(batchTextDocument)))
                {
                    while (!batchTextDocumentReader.EndOfStream)
                    {
                       CreateBatchXML(Path.GetDirectoryName(batchTextDocument), batchTextDocumentReader.ReadLine());
                    }
                }
                StartupChoices();
            }
            else
            {
                Console.WriteLine("File does not exist.");
                MakeBatchSendXml();
            }    
            
        }

        private static void GetRoomsceneFromXml()
        {
            Console.WriteLine("XML Folder location...");
            string xmlFolder = Console.ReadLine().Replace("\"", "");
            Console.WriteLine("LAR File location...");
            string larFile = Console.ReadLine().Replace("\"", "");
            Console.WriteLine("Program name?...");
            string program = Console.ReadLine().Replace("\"", "");
            Console.WriteLine("Softsurface? (y/n)...(Default is n)");
            string isSoftsurfaceResult = Console.ReadLine().Replace("\"", "");
            bool isSoftsurface = false; 
            if (isSoftsurfaceResult.EqualsString("y"))
            {
                isSoftsurface = true;
            }

            if (Directory.Exists(xmlFolder) && File.Exists(larFile))
            {
                string[] xmlFileArray = Directory.GetFiles(xmlFolder, "*.xml", SearchOption.AllDirectories);
                CreateListOfRoomscenes(xmlFileArray, larFile, program, isSoftsurface);
            }
            StartupChoices();
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
                    SqlMethods.SqlWebDBUpdate(lARXlsSheet, isSoftSurface, false);
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
            Console.WriteLine("Exclude Roomscene Line? (y/n)");
            string skipRoomString = Console.ReadLine();
            bool skipRoom = false;
            if (skipRoomString.EqualsString("y"))
            {
                skipRoom = true;
            }

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
                            Directory.CreateDirectory(Path.Combine(newFolderPath, "Results"));
                            while (!newFolderReader.EndOfStream || !oldFolderReader.EndOfStream)
                            {
                                string newLine = "";
                                if (!newFolderReader.EndOfStream)
                                { 
                                    newLine = newFolderReader.ReadLine().Trim().ToLower();
                                }
                                string oldLine = "";
                                if (!oldFolderReader.EndOfStream)
                                {
                                    oldLine = oldFolderReader.ReadLine().Trim().ToLower();
                                }
                                if (!newLine.Equals(oldLine))
                                {
                                    using (StreamWriter writer1 = new(Path.Combine(newFolderPath, "Results", Path.GetFileNameWithoutExtension(f) + ".txt"), append: true))
                                    {
                                        if (!newLine.Contains("<jobalias>") && !newLine.Contains("<text type=\"spec\">") && !newLine.Contains("<insitegroup>"))
                                        {
                                            if (skipRoom)
                                            {
                                                if (!newLine.Contains("<roomscene>"))
                                                {
                                                    writer1.WriteLine("--------------------------------------");
                                                    writer1.WriteLine(oldLine);
                                                    writer1.WriteLine(newLine);
                                                    writer1.WriteLine("--------------------------------------");
                                                }
                                            }
                                            else
                                            {
                                                writer1.WriteLine("--------------------------------------");
                                                writer1.WriteLine(oldLine);
                                                writer1.WriteLine(newLine);
                                                writer1.WriteLine("--------------------------------------");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    bool deleteFile = false;
                    if (File.Exists(Path.Combine(newFolderPath, "Results", Path.GetFileNameWithoutExtension(f) + ".txt")))
                    {
                        using (StreamReader reader = new StreamReader(Path.Combine(newFolderPath, "Results", Path.GetFileNameWithoutExtension(f) + ".txt")))
                        {
                            string text = reader.ReadToEnd();
                            if (text.EqualsString(""))
                            {
                                deleteFile = true;
                            }
                        }
                    }
                    if (deleteFile)
                    {
                        if (File.Exists(Path.Combine(newFolderPath, "Results", Path.GetFileNameWithoutExtension(f) + ".txt")))
                        {
                            File.Delete(Path.Combine(newFolderPath, "Results", Path.GetFileNameWithoutExtension(f) + ".txt"));
                        }
                    }
                }

            }
            StartupChoices();
        }

        private static void WebIntegrationApp(bool isShaw)
        {   
            Console.WriteLine("--------------------------------");

            try
            {
                bool go = true;
                List<string> missingImagesProc = new();

                string export = "\\\\Mac\\Home\\Desktop\\CCA Test\\SQL Testing\\";

                Directory.CreateDirectory(export);
                
                string[] files = ApprovedRoomscenes();

                while (go)
                {
                    List<string> runJobListHS = new(SqlMethods.GetRunJobs(false));
                    List<string> runJobListSS = new(SqlMethods.GetRunJobs(true));
                    foreach (string j in runJobListHS)
                    {
                        LARXlsSheet LARXlsSheet = GetLar(false, j);
                        missingImagesProc.AddRange(Run(isShaw, files, j + " /f", export, LARXlsSheet, true));
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
                        missingImagesProc.AddRange(Run(isShaw, files, j + " /f", export, LARXlsSheet, true));
                        var uniqueMissing = missingImagesProc.Distinct();
                        foreach (string s in uniqueMissing)
                        {
                            Console.WriteLine(s);
                        }
                        SqlMethods.SqlSetToRun(j, true, 0);
                        missingImagesProc = new();
                    }
                    go = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("--------------------------------");
        }

        private static void StandAloneApp(bool isShaw)
        {
            string fileName = "";
            bool go = true;
            bool checkRoomscenes = true;
            bool isCanada = false;
            List<string> missingImagesProc = new();

            Console.WriteLine("What XLS File?");
            fileName = Console.ReadLine();
            fileName = fileName.Replace("\"", "");
            Console.WriteLine("Is this for Canada? (y/n) Default (n)");
            string isCanadaString = Console.ReadLine();
            if (isCanadaString.EqualsString("y"))
            {
                isCanada = true;
            }
            Console.WriteLine("Check Roomscenes too? (y/n) Default (y)");
            string checkRoomscsnesString = Console.ReadLine();
            if (checkRoomscsnesString.EqualsString("n"))
            {
                checkRoomscenes = false;
            }

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
                missingImagesProc.AddRange(Run(isShaw, files, plateId, export, LARXlsSheet, checkRoomscenes, isCanada));
                var uniqueMissing = missingImagesProc.Distinct();
                foreach (string s in uniqueMissing)
                {
                    Console.WriteLine(s);
                }
                missingImagesProc = new();
            }

            StartupChoices();
        }
        [DllImport("user32.dll")]
        public static extern void keybd_event(uint bVk, uint bScan, uint dwFlags, uint dwExtraInfo);
        private const int VK_ESCAPE = 0x1B;
        public static void EscapeKey()
        {
            keybd_event(VK_ESCAPE, 0, 0, 0);
        }
    }
}
