using System;
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

        public static (List<string> swatchXml, string swatchSize) SwatchXML(string feeler, List<string> colorList, string swatchArea, string template, out bool seqError, out int colorCount)
        {
            seqError = false;
            if (!Check_Prime(colorList.Distinct().Count()).Equals(0) && !colorList.Count.Equals(3) && !colorList.Count.Equals(5))
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
            string swatchSize = "";
            SwatchesModel swatchSettings = new();
            colorCount = colorList.Distinct().Count();
            swatchSettings = Settings.GetSwatchLayout(colorCount.ToString(), template);
            List<string> swatchXMLList = new();
            if (swatchSettings.SwatchCols.EqualsString(""))
            {
                Console.WriteLine("Something wrong with color count...");
                seqError = true;
                //Console.ReadLine();
            }
            else
            {
                swatchXMLList.Add("<swatches frametype=\"" + swatchSettings.FrameType + "\" swatchweight=\"" + swatchSettings.SwatchWeight +
                    "\" swatchrows=\"" + swatchSettings.SwatchRows + "\" swatchcols=\"" + swatchSettings.SwatchCols +
                    "\" swatchsizewidth=\"" + swatchSettings.SwatchSizeWidth + "\" swatchsizeheight=\"" + swatchSettings.SwatchSizeHeight +
                    "\" swatcharea=\"" + swatchArea + "\">");
                swatchSize = swatchSettings.SwatchSizeWidth + " x " + swatchSettings.SwatchSizeHeight;
            }

            foreach (string s in colorList.Distinct())
            {
                swatchXMLList.Add(" <colorname>" + ConvertToTitleCase(s) + "</colorname>");
            }
            swatchXMLList.Add("</swatches>");
            swatchSettings = new();
            
            return (swatchXMLList, swatchSize);
        }
        
    }
}
