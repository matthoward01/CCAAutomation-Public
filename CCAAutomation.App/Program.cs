using System;
using System.Collections.Generic;
using static CCAAutomation.Lib.LarModels;
using static CCAAutomation.Lib.CommonMethods;
using static CCAAutomation.Lib.Lar;
using static CCAAutomation.Lib.Execute;
using System.Linq;

namespace CCAAutomation.App
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //string path = GetSyncedRoomscenes("12345");
            Console.WriteLine("Testing? (y/n)");
            bool testing = false;
            if (Console.ReadLine().Trim().ToLower().Equals("y"))
            {
                testing = true;
            }
            string fileName = "";
            bool useSql = false;
            bool go = true;
            List<string> missingImagesProc = new List<string>();

            if (!useSql)
            {
                Console.WriteLine("What XLS File?");
                fileName = Console.ReadLine();
                fileName = fileName.Replace("\"", "");
            }

            Console.WriteLine("Export XML Where?");
            string export = Console.ReadLine();
            export = export.Replace("\"", "");

            bool goWorkshop = false;
            LARXlsSheet LARXlsSheet = GetLar(fileName);
            string[] files = ApprovedRoomscenes();

            /*Console.WriteLine("Path to Plate_Id Excel Sheet? (Use if you only need a list of specific Plate_Ids.");
            string xlsProcess = Console.ReadLine();
            xlsProcess = xlsProcess.Replace("\"", "");*/

            while (go)
            {
                //if (xlsProcess.Equals(""))
                //{
                Console.WriteLine("What Plate Id?");
                string plateId = Console.ReadLine();
                if (plateId.EqualsString("rebuild"))
                {
                    ApprovedRoomscenes();
                    Console.WriteLine("Roomscene Cache Rebuilt...");
                    Console.WriteLine("What Plate Id?");
                    plateId = Console.ReadLine();
                }
                missingImagesProc.AddRange(Run(testing, files, goWorkshop, plateId, export, LARXlsSheet));
                var uniqueMissing = missingImagesProc.Distinct();
                foreach (string s in uniqueMissing)
                {
                    Console.WriteLine(s);
                }
                missingImagesProc = new();
                /*}
                else
                {
                    foreach (Settings.RunSheet r in Settings.GetRunFiles(xlsProcess))
                    {
                        Run(goWorkshop, r.Plate_ID, export, LARXlsSheet);
                    }
                    var uniqueMissing = missingImagesProc.Distinct();
                    Console.WriteLine("Missing Files:");
                    foreach (string s in uniqueMissing)
                    {
                        Console.WriteLine(s);
                    }
                    Console.WriteLine("Path to Plate_Id Excel Sheet? (Use if you only need a list of specific Plate_Ids.");  
                    xlsProcess = Console.ReadLine();
                }*/
            }

            Console.ReadLine();
        }

    }
}
