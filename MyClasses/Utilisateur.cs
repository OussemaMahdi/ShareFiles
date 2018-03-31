using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace MyClasses
{
    public class Utilisateur
    {
        public string nomUtil { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }

        public Utilisateur(string nomUtil, string nom, string prenom)
        {
            this.nomUtil = nomUtil;
            this.nom = nom;
            this.prenom = prenom;
        }
        public void ajouter()
        {
            string req = "INSERT INTO utilisateur VALUES ('" + nomUtil + "','" + nom + "','" + prenom + "')";
            ConnectionDB Conn = ConnectionDB.getInstance();
            if (Conn != null)
            Conn.executer(req);
        }
        public string getNomUtil()
        { return nomUtil; }
        public override string ToString()
        {return(nom+" "+prenom);}


        private static string pathDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ShareFiles";
        private static string pathFileAllUsers = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ShareFiles\BibUsers.xml";
        private static XmlDocument users = new XmlDocument();       
        
        public static void creeBibUsers()
        {
            if (!Directory.Exists(pathDirectory))
            {
                DirectoryInfo di = Directory.CreateDirectory(pathDirectory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
                users.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><utilisateurs></utilisateurs>");
                users.Save(pathFileAllUsers);
            
        }
        public void addToXML()
        {
            if (!Directory.Exists(pathDirectory))
            {
                DirectoryInfo di = Directory.CreateDirectory(pathDirectory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            if (!File.Exists(pathFileAllUsers))
            {
                users.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><utilisateurs></utilisateurs>");
                users.Save(pathFileAllUsers);
            }
            bool exist = false;
            users.Load(pathFileAllUsers);

            XmlNodeList xnList = users.SelectNodes("/utilisateurs/utilisateur");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nomUtil"].InnerText == nomUtil)
                {
                    XmlNode nomutil = xn.SelectSingleNode("nomUtil");
                    XmlNode nom = xn.SelectSingleNode("nom");
                    XmlNode prenom = xn.SelectSingleNode("prenom");

                    nomutil.InnerText = nomUtil;
                    nom.InnerText = this.nom;
                    prenom.InnerText = this.prenom;                   

                    exist = true;
                }
            }
            if (!exist)
            {
                XmlNode fichierx = users.CreateElement("utilisateur");
                users.DocumentElement.AppendChild(fichierx);

                XmlNode nomutil = users.CreateElement("nomUtil");
                nomutil.InnerText = this.nomUtil;
                fichierx.AppendChild(nomutil);

                XmlNode nom = users.CreateElement("nom");
                nom.InnerText = this.nom;
                fichierx.AppendChild(nom);

                XmlNode prenom = users.CreateElement("prenom");
                prenom.InnerText = this.prenom;
                fichierx.AppendChild(prenom);
            }

            users.Save(pathFileAllUsers);
        
        }

        public static Utilisateur getUser(string nomUtil)
    {
        if (!Directory.Exists(pathDirectory))
        {
            DirectoryInfo di = Directory.CreateDirectory(pathDirectory);
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
        }
        if (!File.Exists(pathFileAllUsers))
        {
            users.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><utilisateurs></utilisateurs>");
            users.Save(pathFileAllUsers);
        }
        users.Load(pathFileAllUsers);

        List<Utilisateur> listUtil = new List<Utilisateur>();
        users.Load(pathFileAllUsers);
        XmlNodeList Utilisateurs = users.SelectNodes("/utilisateurs/utilisateur");
        foreach (XmlNode Utilisateur in Utilisateurs)
        {
            if (Utilisateur["nomUtil"].InnerText == nomUtil)
            {
                string nomutil = Utilisateur["nomUtil"].InnerText;
                string nomut = Utilisateur["nom"].InnerText;
                string prenomut = Utilisateur["prenom"].InnerText;
                return (new Utilisateur(nomutil, nomut, prenomut));
            }
        }
        return null;
    }

        public static List<Utilisateur> getAllUsers()
        {
            if (!Directory.Exists(pathDirectory))
            {
                DirectoryInfo di = Directory.CreateDirectory(pathDirectory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            if (!File.Exists(pathFileAllUsers))
            {
                users.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><utilisateurs></utilisateurs>");
                users.Save(pathFileAllUsers);
            }
            users.Load(pathFileAllUsers);

            List<Utilisateur> listUtil = new List<Utilisateur>();
            users.Load(pathFileAllUsers);
            XmlNodeList Utilisateurs = users.SelectNodes("/utilisateurs/utilisateur");
            foreach (XmlNode Utilisateur in Utilisateurs)
            {
                if (Utilisateur["nomUtil"].InnerText != new LastUserConnection().getNomUtil())
                {
                    string nomutil = Utilisateur["nomUtil"].InnerText;
                    string nomut = Utilisateur["nom"].InnerText;
                    string prenomut = Utilisateur["prenom"].InnerText;
                    listUtil.Add(new Utilisateur(nomutil, nomut, prenomut));
                }
            }
            return listUtil;
        }
    }
}
