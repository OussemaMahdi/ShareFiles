using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace MyClasses
{
    public class LastUserConnection
    {
        private string pathDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+@"\ShareFiles";
        private string pathFile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ShareFiles\LasUsr.xml";        
        private XmlDocument annex = new XmlDocument();
        
        public LastUserConnection()
        {
            if (!Directory.Exists(pathDirectory))
            {
                DirectoryInfo di = Directory.CreateDirectory(pathDirectory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            
            if (!File.Exists(pathFile))
            {           
            annex.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><utilisateurs></utilisateurs>");
            annex.Save(pathFile);
            }
        }      
        public void ajouter(string _nomUtil)
        {
            deconnecter();
            annex.Load(pathFile);
            XmlNode Utilisateur = annex.CreateElement("utilisateur");
            XmlNode nomUtil = annex.CreateElement("nomUtil");
            nomUtil.InnerText = _nomUtil;
            Utilisateur.AppendChild(nomUtil);

            annex.DocumentElement.AppendChild(Utilisateur);
            annex.Save(pathFile);        
        }
        public bool isconnected()
        {
            annex.Load(pathFile);
            XmlNodeList xnList = annex.SelectNodes("/utilisateurs/utilisateur");
            return xnList.Count >= 1;
        }       
        public string getNomUtil()
        {
            annex.Load(pathFile);
            XmlNodeList xnList = annex.SelectNodes("/utilisateurs/utilisateur");
            foreach (XmlNode xn in xnList)
            {
                string nomUtil = xn["nomUtil"].InnerText;
                return nomUtil;
            }
            return null;

        }
        public void deconnecter()
        {
            annex.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><utilisateurs></utilisateurs>");
            annex.Save(pathFile);
        }
    }
}
