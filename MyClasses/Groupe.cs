using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;

namespace MyClasses
{
    public class Groupe
    {
        public string nomDuGroup { get; set; }
        public List<Utilisateur> ListeUtilisateurs;

        public Groupe(string nom,List<Utilisateur> utilisateurs)
        {
           nomDuGroup=nom;
           this.ListeUtilisateurs = utilisateurs;
        }
        public override string ToString()
        { return (nomDuGroup); }
        
        public DataSet getUtilisateursOfGroup()
        {
            string req = "select utilisateur.nomutilisateur,utilisateur.nom,utilisateur.prenom from utilisateur, appartient where appartient.nomGroup='" + nomDuGroup + "' and utilisateur.nomutilisateur=appartient.nomutilisateur";
            ConnectionDB conn= ConnectionDB.getInstance();
            if (conn != null)
            {
                DataSet ds = new DataSet();
                ds = conn.selectionner(req);
                return ds;
            }
            else return null;
        }



        private static string pathDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ShareFiles";
        private static string pathFileAllgroups = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ShareFiles\bibgroups.xml";
        private static XmlDocument groups = new XmlDocument(); 

        public void addToXML()
        {
            if (!Directory.Exists(pathDirectory))
            {
                DirectoryInfo di = Directory.CreateDirectory(pathDirectory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            if (!File.Exists(pathFileAllgroups))
            {
                groups.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><groups></groups>");
                groups.Save(pathFileAllgroups);
            }


            bool exist = false;
            groups.Load(pathFileAllgroups);

            XmlNodeList xnList = groups.SelectNodes("/groups/group");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nomGroup"].InnerText == nomDuGroup)
                {
                    XmlNode oldUtilisateurs = xn.SelectSingleNode("utilisateurs");
                    XmlNode newUtilisateurs = groups.CreateElement("utilisateurs");
                    foreach (Utilisateur u in ListeUtilisateurs)
                    {
                        XmlNode utilisateur = groups.CreateElement("utilisateur");

                        XmlNode nomUtil = groups.CreateElement("nomUtil");
                        nomUtil.InnerText = u.getNomUtil();
                        utilisateur.AppendChild(nomUtil);

                        XmlNode nomut = groups.CreateElement("nom");
                        nomut.InnerText = u.nom;
                        utilisateur.AppendChild(nomut);

                        XmlNode prenut = groups.CreateElement("prenom");
                        prenut.InnerText = u.prenom;
                        utilisateur.AppendChild(prenut);

                        newUtilisateurs.AppendChild(utilisateur);
                    }
                    xn.ReplaceChild(newUtilisateurs, oldUtilisateurs);
                    exist = true;
                }
            }
            if (!exist)
            {
                XmlNode groupx = groups.CreateElement("group");
                groups.DocumentElement.AppendChild(groupx);

                XmlNode nom = groups.CreateElement("nomGroup");
                nom.InnerText = nomDuGroup;
                groupx.AppendChild(nom);              

                XmlNode utilisateurs = groups.CreateElement("utilisateurs");
                foreach (Utilisateur u in ListeUtilisateurs)
                {
                    XmlNode utilisateur = groups.CreateElement("utilisateur");

                    XmlNode nomUtil = groups.CreateElement("nomUtil");
                    nomUtil.InnerText = u.getNomUtil();
                    utilisateur.AppendChild(nomUtil);

                    XmlNode nomut = groups.CreateElement("nom");
                    nomut.InnerText = u.nom;
                    utilisateur.AppendChild(nomut);

                    XmlNode prenut = groups.CreateElement("prenom");
                    prenut.InnerText = u.prenom;
                    utilisateur.AppendChild(prenut);

                    utilisateurs.AppendChild(utilisateur);
                }
                groupx.AppendChild(utilisateurs);
            }
            groups.Save(pathFileAllgroups);

        }
        public static List<Groupe> getAllGroups()
        {
            if (!Directory.Exists(pathDirectory))
            {
                DirectoryInfo di = Directory.CreateDirectory(pathDirectory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            if (!File.Exists(pathFileAllgroups))
            {
                groups.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><groups></groups>");
                groups.Save(pathFileAllgroups);
            }

            groups.Load(pathFileAllgroups);
            List<Groupe> listGrp = new List<Groupe>();
            XmlNodeList allgroup = groups.SelectNodes("/groups/group");
            foreach (XmlNode group in allgroup)
            {
                XmlNodeList utilisateurs = group.SelectNodes("utilisateurs/utilisateur");
                List<Utilisateur> lu=new List<Utilisateur>();
                foreach (XmlNode utilisateur in utilisateurs)
                {
                    lu.Add(new Utilisateur(utilisateur["nomUtil"].InnerText, utilisateur["nom"].InnerText, utilisateur["prenom"].InnerText));
                }
                listGrp.Add(new Groupe(group["nomGroup"].InnerText,lu));
            }
            return listGrp;
        }
        public List<Utilisateur> getMemberOfGroup()
        {
            if (!Directory.Exists(pathDirectory))
            {
                DirectoryInfo di = Directory.CreateDirectory(pathDirectory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            if (!File.Exists(pathFileAllgroups))
            {
                groups.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><groups></groups>");
                groups.Save(pathFileAllgroups);
            }

            List<Utilisateur> lu = new List<Utilisateur>();
            groups.Load(pathFileAllgroups);
            XmlNodeList allgroup = groups.SelectNodes("/groups/group");
            foreach (XmlNode group in allgroup)
            {
                if (group["nomGroup"].InnerText == nomDuGroup)
                {
                    XmlNodeList utilisateurs = group.SelectNodes("utilisateurs/utilisateur");
                    foreach (XmlNode utilisateur in utilisateurs)
                    {
                        lu.Add(new Utilisateur(utilisateur["nomUtil"].InnerText, utilisateur["nom"].InnerText, utilisateur["prenom"].InnerText));
                    }
                }
            }
            return lu;

        }

        /* -------------------- Data Base --------------------*/
        public bool GroupajoutableInDB()
        {
            try
            {
                ConnectionDB conn = ConnectionDB.getInstance();
                string req = "select * from Groupe where nomGroup='" + nomDuGroup + "'";
                return conn.nombreSelectionner(req) == 0;
            }
            catch { return false; }
        }
        public void addGroupInDB()
        {
            if(nomDuGroup!="Public")
            {           
            string req = "INSERT INTO Groupe VALUES ('" + nomDuGroup + "')";
            ConnectionDB Conn = ConnectionDB.getInstance();
            Conn.executer(req);
            }
        }
        public void giveAutToGroupInDB(RecivedFiles rf)
        {
            string req = "INSERT INTO conf2 VALUES ('" + nomDuGroup + "','" + rf.nom + "')";
            ConnectionDB Conn = ConnectionDB.getInstance();
            Conn.executer(req);
        }
        public void SetAppartenanceInGroup(Utilisateur u)
        {
            try
            {
                string req2 = "INSERT INTO appartient VALUES ('" + nomDuGroup + "','" + u.getNomUtil() + "')";
                ConnectionDB Conn = ConnectionDB.getInstance();
                Conn.executer(req2);
            }
            catch { }
        }
    }
}
