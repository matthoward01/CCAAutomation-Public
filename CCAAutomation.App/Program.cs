using System;
using System.Collections.Generic;
using static CCAAutomation.Lib.LarModels;
using static CCAAutomation.Lib.CommonMethods;
using static CCAAutomation.Lib.Lar;
using static CCAAutomation.Lib.Execute;
using System.Linq;
using CCAAutomation.Lib;

namespace CCAAutomation.App
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {            
            bool webIntegration = false;     
            
            if (!webIntegration)
            {
                StandAloneApp();
            }
            else
            {
                WebIntegrationApp();
            }
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
