using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace CCAAutomation.Lib
{
    class Settings
    {
        public class TemplateModel
        {
            public string Id { get; set; }
            public string TypeId { get; set; }
            public string Name { get; set; }
			public string WebShopPath { get; set; }
			public string PreJobPath { get; set; }
            public string PreJobTemplateName { get; set; }
            public string UserEmail { get; set; }
			public string SnippetPath { get; set; }
			public string RoomscenePath { get; set; }
			public string IconPath { get; set; }
			public string CharacteristicPath { get; set; }
			public string InstallationPath { get; set; }
			public string TrimsPath { get; set; }
            public string ImagesPath { get; set; }
        }

        public class MainSettings
        {
            public string SyncFolder { get; set; }
            public string SyncFolderProcessed { get; set; }
            public string ApprovedRoomScenes { get; set; }
            public string WebShopRoomScenes { get; set; }
            public bool UseSql { get; set; }
        }   

		public class RunSheet
        {
            public string Plate_ID { get; set; }
        }

        public static MainSettings GetMainSettings()
        {
            MainSettings mainSettings = new();
            XmlDocument doc = new();
            string xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Config.xml");
            doc.Load(xmlPath);
            XmlNode mainSettingNode = doc.DocumentElement.SelectSingleNode("MainSettings");

            //foreach (XmlNode node in mainSettingNode.ChildNodes)
            //{
                mainSettings.ApprovedRoomScenes = mainSettingNode.SelectSingleNode("ApprovedRoomScenes").InnerText;
                mainSettings.SyncFolder = mainSettingNode.SelectSingleNode("SyncFolder").InnerText;
                mainSettings.SyncFolderProcessed = mainSettingNode.SelectSingleNode("SyncFolderProcessed").InnerText;
                mainSettings.WebShopRoomScenes = mainSettingNode.SelectSingleNode("WebShopRoomScenes").InnerText;
            //}

            return mainSettings;
        }

        public static TemplateModel GetTemplateSettings(string passedString, string type)
        {
            TemplateModel template = new();
            XmlDocument doc = new();
            string xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Config.xml");
            doc.Load(xmlPath);
            XmlNode templatesNode = doc.DocumentElement.SelectSingleNode("Templates");

            foreach (XmlNode node in templatesNode.ChildNodes)
            {
                XmlNode idNode = node.SelectSingleNode("Id");

                if (idNode.InnerText.Equals(passedString) && node.SelectSingleNode("TypeId").InnerText.Equals(type))
                {
                    template.CharacteristicPath = node.SelectSingleNode("CharacteristicPath").InnerText;
                    template.IconPath = node.SelectSingleNode("IconPath").InnerText;
                    template.Id = node.SelectSingleNode("Id").InnerText;
                    template.ImagesPath = node.SelectSingleNode("ImagesPath").InnerText;
                    template.InstallationPath = node.SelectSingleNode("InstallationPath").InnerText;
                    template.Name = node.SelectSingleNode("Name").InnerText;
                    template.PreJobTemplateName = node.SelectSingleNode("PreJobTemplateName").InnerText;
                    template.PreJobPath = node.SelectSingleNode("PreJobPath").InnerText;
                    template.RoomscenePath = node.SelectSingleNode("RoomscenePath").InnerText;
                    template.SnippetPath = node.SelectSingleNode("SnippetPath").InnerText;
                    template.TrimsPath = node.SelectSingleNode("TrimsPath").InnerText;
                    template.TypeId = node.SelectSingleNode("TypeId").InnerText;
                    template.UserEmail = node.SelectSingleNode("UserEmail").InnerText;
                    template.WebShopPath = node.SelectSingleNode("WebShopPath").InnerText;
                }
            }

            return template;
        }

        public static Swatches.SwatchesModel GetSwatchLayout(string passedString, string template)
        {
            Swatches.SwatchesModel swatches = new();
            XmlDocument doc = new();
            string xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Config.xml");
            doc.Load(xmlPath);
            XmlNode swatchesNode = doc.DocumentElement.SelectSingleNode("Swatches");

            foreach (XmlNode node in swatchesNode.ChildNodes)
            {
                XmlNode idNode = node.SelectSingleNode("Total");
                XmlNode templateNode = node.SelectSingleNode("Template");

                if (idNode.InnerText.Equals(passedString) && templateNode.InnerText.Equals(template))
                {
                    swatches.FrameType = node.SelectSingleNode("FrameType").InnerText;
                    swatches.SwatchCols = node.SelectSingleNode("SwatchCols").InnerText;
                    swatches.SwatchRows = node.SelectSingleNode("SwatchRows").InnerText;
                    swatches.SwatchSizeHeight = node.SelectSingleNode("SwatchSizeHeight").InnerText;
                    swatches.SwatchSizeWidth = node.SelectSingleNode("SwatchSizeWidth").InnerText;
                    swatches.SwatchWeight = node.SelectSingleNode("SwatchWeight").InnerText;
                    swatches.Template = node.SelectSingleNode("Template").InnerText;
                    swatches.Total = node.SelectSingleNode("Total").InnerText;
                }
            }

            return swatches;
        }

        public static List<RunSheet> GetRunFiles(string fileName)
        {
            List<RunSheet> runSheetList = new();

            IWorkbook wb = new XSSFWorkbook(fileName);
            ISheet sheetDetails = wb.GetSheetAt(0);

            List<string> RunSheetHeaderList = new(CommonMethods.GetHeaderColumns(sheetDetails));
            for (int i = 1; i < CommonMethods.GetRowCount(sheetDetails); i++)
            {
                runSheetList.Add(GetRunFile(sheetDetails, RunSheetHeaderList, i));
            }

            return runSheetList;
        }

        public static RunSheet GetRunFile(ISheet sheet, List<string> runSheetHeaderList, int i)
        {
            RunSheet runSheet = new();
            runSheet.Plate_ID = CommonMethods.GetCell(sheet, i, runSheetHeaderList.IndexOf("Plate_ID"));
            return runSheet;
        }
    }
}
