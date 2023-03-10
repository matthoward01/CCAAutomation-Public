using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace CCAAutomation.Lib
{
    class XmlMethods
    {
        public static string XmlRemapping(string passedString, string nodeName)
        {
            string newString = passedString.Trim();
            XmlDocument doc = new XmlDocument();
            string xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Config.xml");
            doc.Load(xmlPath);
            XmlNode searchNode = doc.DocumentElement.SelectSingleNode("//*[name() = '" + nodeName + "']");

            foreach (XmlNode node in searchNode.ChildNodes)
            {
                foreach(XmlNode cNode in node.ChildNodes)
                {
                    if (cNode.InnerText.Trim().ToLower().Equals(passedString.Trim().ToLower()) && !passedString.Trim().Equals(""))
                    {
                          newString = cNode.ParentNode.Attributes["name"].Value;
                    }
                }
            }          

            return newString.ToLower();
        }

        public static (string plateId, string roomsceneName) GetRoomSceneFromXml(string xmlFile)
        {
            XmlDocument doc = new XmlDocument();
            string xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), xmlFile);
            doc.Load(xmlPath);
            XmlNode jobNameNode = doc.DocumentElement.SelectSingleNode("//*[name() = '" + "output" + "']");
            XmlNode roomsceneNode = doc.DocumentElement.SelectSingleNode("//*[name() = '" + "roomscene" + "']");

            string plateId = jobNameNode.Attributes["jobname"].Value;
            string roomsceneName = roomsceneNode.InnerText; 

            return (plateId, roomsceneName);

        }

        public static string XmlRemapping(string passedString, string nodeName, string attribute, bool addPath)
        {
            string newString = passedString.Trim();
            XmlDocument doc = new XmlDocument();
            string xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Config.xml");
            doc.Load(xmlPath);
            XmlNode searchNode = doc.DocumentElement.SelectSingleNode("//*[name() = '" + nodeName + "']");

            foreach (XmlNode node in searchNode.ChildNodes)
            {
                foreach (XmlNode cNode in node.ChildNodes)
                {
                    if (cNode.InnerText.Trim().ToLower().Equals(passedString.Trim().ToLower()) && !passedString.Trim().Equals(""))
                    {
                        string path = cNode.ParentNode.Attributes[attribute].Value;
                        string name = cNode.ParentNode.Attributes["name"].Value;
                        newString = path + name;
                    }
                }
            }

            return newString.ToLower();
        }
        public static string XmlRemapping(string passedString, string nodeName, string attribute)
        {
            string newString = passedString.Trim();
            XmlDocument doc = new XmlDocument();
            string xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Config.xml");
            doc.Load(xmlPath);
            XmlNode searchNode = doc.DocumentElement.SelectSingleNode("//*[name() = '" + nodeName + "']");

            foreach (XmlNode node in searchNode.ChildNodes)
            {
                foreach (XmlNode cNode in node.ChildNodes)
                {
                    if (cNode.InnerText.Trim().ToLower().Equals(passedString.Trim().ToLower()) && !passedString.Trim().Equals(""))
                    {
                        newString = cNode.ParentNode.Attributes[attribute].Value;
                    }
                }
            }

            return newString.ToLower();
        }
        public static string XmlRemapping(string passedString, string nodeName, bool contains)
        {
            string newString = passedString.Trim();
            XmlDocument doc = new XmlDocument();
            string xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Config.xml");
            doc.Load(xmlPath);
            XmlNode searchNode = doc.DocumentElement.SelectSingleNode("//*[name() = '" + nodeName + "']");

            foreach (XmlNode node in searchNode.ChildNodes)
            {
                foreach (XmlNode cNode in node.ChildNodes)
                {
                    if (passedString.Trim().ToLower().Contains(cNode.InnerText.Trim().ToLower()) && !passedString.Trim().Equals("") && contains)
                    {
                        newString = cNode.ParentNode.Attributes["name"].Value;
                    }
                }
            }

            return newString.ToLower();
        }
        public static string XmlIconOrder(string passedString, string nodeName)
        {
            string newString = passedString.Trim();
            XmlDocument doc = new XmlDocument();
            string xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Config.xml");
            doc.Load(xmlPath);
            XmlNode searchNode = doc.DocumentElement.SelectSingleNode("//*[name() = '" + nodeName + "']");

            foreach (XmlNode node in searchNode.ChildNodes)
            {
                foreach (XmlNode cNode in node.ChildNodes)
                {
                    if (cNode.InnerText.Trim().ToLower().Equals(passedString.Trim().ToLower()) && !passedString.Trim().Equals(""))
                    {
                        newString = cNode.ParentNode.Attributes["priority"].Value;
                    }
                }
            }

            return newString.ToLower();
        }
        public static string XmlIconPath(string passedString, string nodeName)
        {
            string newString = passedString.Trim();
            XmlDocument doc = new XmlDocument();
            string xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Config.xml");
            doc.Load(xmlPath);
            XmlNode searchNode = doc.DocumentElement.SelectSingleNode("//*[name() = '" + nodeName + "']");

            foreach (XmlNode node in searchNode.ChildNodes)
            {
                foreach (XmlNode cNode in node.ChildNodes)
                {
                    if (cNode.InnerText.Trim().ToLower().Equals(passedString.Trim().ToLower()) && !passedString.Trim().Equals(""))
                    {
                        newString = cNode.ParentNode.Attributes["path"].Value;
                    }
                }
            }

            return newString.ToLower();
        }        
    }
}
