﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CCAAutomation.Lib.CommonMethods;

namespace CCAAutomation.Lib
{
    class Swatches
    {
        public class SwatchesModel
        {
            public string Total { get; set; }
            public string Template { get; set; }
            public string FrameType { get; set; }
            public string SwatchWeight { get; set; }
            public string SwatchRows { get; set; }
            public string SwatchCols { get; set; }
            public string SwatchSizeWidth { get; set; }
            public string SwatchSizeHeight { get; set; }
        }

        public class SwatchColors
        {
            public string Color { get; set; }
            public string ColorSequence { get; set; }
        }

        public static List<string> SwatchXML(string feeler, List<string> colorList, string swatchArea, string template)
        {
            if (!Check_Prime(colorList.Count()).Equals(0))
            {
                if (colorList.Contains(feeler))
                {
                    colorList.Remove(feeler);
                }
                else
                {
                    Console.WriteLine("Color list does not contain the feeler color.  " +
                        "There is some sort of mismatch. Check the spreadsheet to be sure the feeler " +
                        "on the sample tab matches the feeler name on the details tab.");
                    Console.ReadLine();
                }
            }
            SwatchesModel swatchSettings = new SwatchesModel();
            swatchSettings = Settings.GetSwatchLayout(colorList.Distinct().Count().ToString(), template);
            List<string> swatchXMLList = new List<string>();

            swatchXMLList.Add("<swatches frametype=\"" + swatchSettings.FrameType + "\" swatchweight=\"" + swatchSettings.SwatchWeight +
                "\" swatchrows=\"" + swatchSettings.SwatchRows + "\" swatchcols=\"" + swatchSettings.SwatchCols +
                "\" swatchsizewidth=\"" + swatchSettings.SwatchSizeWidth + "\" swatchsizeheight=\"" + swatchSettings.SwatchSizeHeight +
                "\" swatcharea=\"" + swatchArea + "\">");

            foreach (string s in colorList.Distinct())
            {
                swatchXMLList.Add(" <colorname>" + ConvertToTitleCase(s) + "</colorname>");
            }
            swatchXMLList.Add("</swatches>");

            swatchSettings = new SwatchesModel();
            return swatchXMLList;
        }
        
    }
}