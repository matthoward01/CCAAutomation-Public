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

        public static List<string> Run(string[] files, bool goWorkShop, string plateId, string export, LarModels.LARXlsSheet LARXlsSheet)
        {
            List<string> missingImagesRun = new();
            List<string> approvedPlateIdsList = new();
            lastPlate = "";
            ssList = new();
            runList = new();

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

            if (plateId.Equals(""))
            {
                List<LarModels.LARFinal> aRFinals = new (Lar.GetLarFinal(LARXlsSheet, plateId));
                List<string> plateList = new();
                foreach (LarModels.LARFinal lf in aRFinals)
                {
                    //if (!SqlMethods.SqlApprovalCheck(lf.DetailsFinal.Plate_ID))
                    if (!approvedPlateIdsList.Any(p => p.EqualsString(lf.DetailsFinal.Plate_ID)))
                    {
                        /*string key = GetKey(lf.DetailsFinal.ArtType);
                        if (lf == aRFinals.Last())
                        {
                            lastPlate = "end";
                        }
                        files = GoSwitch(files, goWorkShop, export, plateId, missingImagesRun, lf, key);*/
                        plateList.Add(lf.DetailsFinal.Plate_ID);
                    }
                }
                foreach (string p in plateList.Distinct())
                {
                    runList = aRFinals.Where(l => (l.DetailsFinal.Plate_ID.EqualsString(p) && ((l.DetailsFinal.Division_List.ToLower().Trim().Contains("c1")) || (l.DetailsFinal.Division_List.ToLower().Trim().Contains("fa"))))).ToList();
                    if (!runList.Count.Equals(0))
                    {
                        string key = GetKey(runList[0].DetailsFinal.ArtType);
                        files = GoSwitch(files, goWorkShop, export, plateId, missingImagesRun, key);
                    }
                }
            }
            else
            {
                List<string> plateList = new();
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
                        files = GoSwitch(files, goWorkShop, export, plateId, missingImagesRun, key);
                    }
                }
            }

            return missingImagesRun;
        }

        private static string[] GoSwitch(string[] files, bool goWorkShop, string export, string plateId, List<string> missingImagesRun, string key)
        {
            switch (key)
            {
                case "HS18x24BL":
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine("Plate #: " + runList[0].DetailsFinal.Plate_ID);
                    if (runList.Count.Equals(1))
                    {
                        missingImagesRun.AddRange(HSBL18x24.CreateXMLHS18x24BL(files, false, goWorkShop, runList[0], plateId, export));
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
                        missingImagesRun.AddRange(HSFL4_5x2_1875.CreateXMLHS4_5x2_1875(goWorkShop, runList[0], plateId, export));
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

                        var result = SS19_375x28_5BL.CreateXMLSS19_375x28_5BL(files, false, goWorkShop, runList, plateId, export);
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

        private static string[] GoSwitchOld(string[] files, bool goWorkShop, string export, string plateId, List<string> missingImagesRun, string key, LarModels.LARFinal lf = null)
        {            
            switch (key)
            {
                case "HS18x24BL":
                    Console.WriteLine("--------------------------------------------");
                    missingImagesRun.AddRange(HSBL18x24.CreateXMLHS18x24BL(files, false, goWorkShop, lf, plateId, export));
                    Console.WriteLine("--------------------------------------------");
                    break;
                case "HS4.5x2.1875FL":
                    Console.WriteLine("--------------------------------------------");
                    missingImagesRun.AddRange(HSFL4_5x2_1875.CreateXMLHS4_5x2_1875(goWorkShop, lf, plateId, export));
                    Console.WriteLine("--------------------------------------------");
                    break;
                case "SS19.375x28.5BL":
                    if (lf.SampleFinal.Sample_Type.Trim().ToLower().Equals("hanger"))
                    {
                        if (lastPlate.Trim().Equals("") || lastPlate.Equals(lf.DetailsFinal.Plate_ID))
                        {
                            if (!lf.DetailsFinal.Division_List.Trim().ToLower().Equals("cn") && !lf.DetailsFinal.Division_List.Trim().ToLower().Equals("fc"))
                            {
                                ssList.Add(lf);
                            }
                            lastPlate = lf.DetailsFinal.Plate_ID;
                        }
                        else
                        {
                            if (lastPlate.Equals("end") && !lf.DetailsFinal.Division_List.Trim().ToLower().Equals("cn") && !lf.DetailsFinal.Division_List.Trim().ToLower().Equals("fc"))
                            {
                                ssList.Add(lf);
                            }
                            lastPlate = lf.DetailsFinal.Plate_ID;

                            Console.WriteLine("--------------------------------------------");
                            Console.WriteLine("Plate #: " + lf.DetailsFinal.Plate_ID);
                            if (ssList.Count != 0)
                            { 
                                var result = SS19_375x28_5BL.CreateXMLSS19_375x28_5BL(files, false, goWorkShop, ssList, plateId, export);
                                missingImagesRun.AddRange(result.missingItems);
                                files = result.updatedFiles;
                            }
                            else
                            {
                                Console.WriteLine("List Empty for Plate #: " + lf.DetailsFinal.Plate_ID);
                                Console.WriteLine("Could there be something wrong with the divisions? As in Only CN or FC.");
                            }
                            Console.WriteLine("--------------------------------------------");

                            ssList = new List<LarModels.LARFinal>();
                            if (!lf.DetailsFinal.Division_List.Trim().ToLower().Equals("cn") && !lf.DetailsFinal.Division_List.Trim().ToLower().Equals("fc"))
                            {
                                ssList.Add(lf);
                            }
                        }
                    }
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
            return template;
        }
    }
}
