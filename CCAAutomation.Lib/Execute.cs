using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCAAutomation.Lib;

namespace CCAAutomation.Lib
{
    public class Execute
    {
        static string lastPlate = "";
        static List<LarModels.LARFinal> ssList = new();
        static List<LarModels.LARFinal> runList = new();
        string[] files = CommonMethods.ApprovedRoomscenes();

        public static List<string> Run(bool isShaw, string[] files, string plateId, string export, LarModels.LARXlsSheet LARXlsSheet)
        {
            List<string> missingImagesRun = new();
            List<string> approvedPlateIdsList = new();
            lastPlate = "";
            ssList = new();
            runList = new();
            bool forced = false;

            if (!File.Exists(Path.Combine(export, "Approved.txt")))
            {
                using StreamWriter approvedPlateIdsWriter = new(Path.Combine(export, "Approved.txt"));
            }
            using (StreamReader approvedPlateIdsReader = new(Path.Combine(export, "Approved.txt")))
            {
                while (!approvedPlateIdsReader.EndOfStream)
                {
                    approvedPlateIdsList.Add(approvedPlateIdsReader.ReadLine());
                }
            }
            if (plateId.Trim().ToLower().EndsWith("/f"))
            {
                forced = true;
                plateId = plateId.Replace("/f", "").Trim();
            }
            if (plateId.Equals(""))
            {                
                List<LarModels.LARFinal> aRFinals = new (Lar.GetLarFinal(LARXlsSheet, plateId));
                List<string> plateList = new();
                foreach (LarModels.LARFinal lf in aRFinals)
                {
                    //if (!SqlMethods.SqlApprovalCheck(lf.DetailsFinal.Plate_ID))
                    string thisPlateId = lf.DetailsFinal.Plate_ID;
                    if (isShaw && lf.DetailsFinal.Supplier_Name.ToLower().Contains("shaw"))
                    {
                        thisPlateId = thisPlateId += " Shaw";
                    }
                    if (isShaw &&!approvedPlateIdsList.Any(p => p.EqualsString(thisPlateId)) && lf.DetailsFinal.Supplier_Name.ToLower().Contains("shaw"))
                    {
                        plateList.Add(lf.DetailsFinal.Plate_ID);
                    }
                    else if (!isShaw && !approvedPlateIdsList.Any(p => p.EqualsString(thisPlateId)))
                    {
                        plateList.Add(lf.DetailsFinal.Plate_ID);
                    }
                }
                foreach (string p in plateList.Distinct())
                {
                    runList = aRFinals.Where(l => (l.DetailsFinal.Plate_ID.EqualsString(p) && ((l.DetailsFinal.Division_List.ToLower().Trim().Contains("c1")) || (l.DetailsFinal.Division_List.ToLower().Trim().Contains("fa"))))).ToList();
                    if (!runList.Count.Equals(0))
                    {
                        string key = GetKey(runList[0].DetailsFinal.ArtType);
                        files = GoSwitch(forced, isShaw, files, export, plateId, missingImagesRun, key);
                    }
                }
            }
            else
            {
                List<string> plateList = new();
                if (plateId.Trim().ToLower().EndsWith("/f"))
                {
                    if (plateId.Contains(" "))
                    {
                        string[] plateSplit = plateId.Split(' ');
                        forced = true;
                        plateId = plateSplit[0];
                    }
                }
                if (plateId.Trim().ToLower().EndsWith("/s"))
                {
                    if (plateId.Contains(" "))
                    {
                        string[] plateSplit = plateId.Split(' ');
                        isShaw = true;
                        plateId = plateSplit[0];
                    }
                }
                List<LarModels.LARFinal> aRFinals = new(Lar.GetLarFinal(LARXlsSheet, plateId));
                if (aRFinals.Count == 0)
                {
                    Console.WriteLine("---------------------------------------");
                    Console.WriteLine("Plate Id " + plateId + " does not exist.");
                    Console.WriteLine("---------------------------------------");
                }
                foreach (LarModels.LARFinal lf in aRFinals)
                {
                    /*if (lf == aRFinals.Last())
                    {
                        lastPlate = "end";
                    }
                    string key = GetKey(lf.DetailsFinal.ArtType);
                    files = GoSwitch(files, goWorkShop, export, plateId, missingImagesRun, key, lf);*/
                    plateList.Add(lf.DetailsFinal.Plate_ID);
                }
                foreach (string p in plateList.Distinct())
                {
                    runList = aRFinals.Where(l => (l.DetailsFinal.Plate_ID.EqualsString(p) && ((l.DetailsFinal.Division_List.ToLower().Trim().Contains("c1")) || (l.DetailsFinal.Division_List.ToLower().Trim().Contains("fa"))))).ToList();
                    if (!runList.Count.Equals(0))
                    {
                        string key = GetKey(runList[0].DetailsFinal.ArtType);
                        files = GoSwitch(forced, isShaw, files, export, plateId, missingImagesRun, key);
                    }
                }
            }

            return missingImagesRun;
        }

        private static string[] GoSwitch(bool forced, bool isShaw, string[] files, string export, string plateId, List<string> missingImagesRun, string key)
        {
            switch (key)
            {
                case "HS18x24BL":
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine("Plate #: " + runList[0].DetailsFinal.Plate_ID);
                    if (runList.Count.Equals(1))
                    {
                        missingImagesRun.AddRange(HSBL18x24.CreateXMLHS18x24BL(forced, files, false, runList[0], plateId, export));
                    }
                    else
                    {
                        Console.WriteLine("Duplicate Plate Numbers which is not possible for HS atm.");
                    }
                    Console.WriteLine("--------------------------------------------");
                    break;
                case "HS4.5x2.1875FL":
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine("Plate #: " + runList[0].DetailsFinal.Plate_ID);
                    if (runList.Count.Equals(1))
                    {
                        missingImagesRun.AddRange(HSFL4_5x2_1875.CreateXMLHS4_5x2_1875(runList[0], export));
                    }
                    else
                    {
                        Console.WriteLine("Duplicate Plate Numbers which is not possible for HS atm.");
                    }
                    Console.WriteLine("--------------------------------------------");
                    break;
                case "SS19.375x28.5BL":
                    if (runList[0].SampleFinal.Sample_Type.Trim().ToLower().Equals("hanger"))
                    {                        
                        Console.WriteLine("--------------------------------------------");
                        Console.WriteLine("Plate #: " + runList[0].DetailsFinal.Plate_ID);

                        var result = SS19_375x28_5BL.CreateXMLSS19_375x28_5BL(forced, isShaw, files, false, runList, export);
                        missingImagesRun.AddRange(result.missingItems);
                        files = result.updatedFiles;

                        Console.WriteLine("--------------------------------------------");
                    }
                    runList = new();
                    break;
                case "SS19.375x28.5BLConv":
                    if (runList[0].SampleFinal.Sample_Type.Trim().ToLower().Equals("hanger"))
                    {
                        Console.WriteLine("--------------------------------------------");
                        Console.WriteLine("Plate #: " + runList[0].DetailsFinal.Plate_ID);

                        var result = SS19_375x28_5BLConv.CreateXMLSS19_375x28_5BLConv(files, false, runList, plateId, export);
                        missingImagesRun.AddRange(result.missingItems);
                        files = result.updatedFiles;

                        Console.WriteLine("--------------------------------------------");
                    }
                    runList = new();
                    break;
                default:
                    Console.WriteLine("Template does not Exist.");
                    break;
            }
            return files;
        }        

        private static string GetKey(string artType)
        {
            string template = "";
            if (artType.Trim().ToLower().Equals("hs18x24bl"))
            {
                template = "HS18x24BL";
            }
            if (artType.Trim().ToLower().Equals("hs4.5x2.1875fl"))
            {
                template = "HS4.5x2.1875FL";
            }
            if (artType.Trim().ToLower().Equals("ss19.375x28.5bl") || artType.Trim().ToLower().Equals("ss19.625x28.5bl"))
            {
                template = "SS19.375x28.5BL";
            }
            if (artType.Trim().ToLower().Equals("ss19.375x28.5blconv"))
            {
                template = "SS19.375x28.5BLConv";
            }
            return template;
        }
    }
}
