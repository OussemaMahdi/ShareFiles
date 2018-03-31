using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace MyClasses
{
    public class LastPcServerName
    {
        private string pathDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+@"\ShareFiles";
        private string pathFile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ShareFiles\LasServer.xml";        
        private XmlDocument annex = new XmlDocument();

        public LastPcServerName()
        {
            if (!Directory.Exists(pathDirectory))
            {
                DirectoryInfo di = Directory.CreateDirectory(pathDirectory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            
            if (!File.Exists(pathFile))
            {           
            annex.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><Server></Server>");
            annex.Save(pathFile);
            }
        }
        public void ajouterNamePcServer(string _nomUtil)
        {
            deconnecter();
            annex.Load(pathFile);
            XmlNode NamePcServer = annex.CreateElement("NamePcServer");
            XmlNode name = annex.CreateElement("name");
            name.InnerText = _nomUtil;
            NamePcServer.AppendChild(name);

            annex.DocumentElement.AppendChild(NamePcServer);
            annex.Save(pathFile);        
        }       
        public string getNamePcServer()
        {
            annex.Load(pathFile);
            XmlNodeList xnList = annex.SelectNodes("/Server/NamePcServer");
            foreach (XmlNode xn in xnList)
            {
                string name = xn["name"].InnerText;
                return name;
            }
            return null;
        }
        public void deconnecter()
        {
            annex.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><Server></Server>");
            annex.Save(pathFile);
        }
    }
}
