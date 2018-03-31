using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace MyClasses
{
    public class BiBFilesXML
    {
        private string pathDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ShareFiles";
        private string pathFile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ShareFiles\BibFiles.xml";
        private XmlDocument bib = new XmlDocument();

        public BiBFilesXML()
        {
            if (!Directory.Exists(pathDirectory))
            {
                DirectoryInfo di = Directory.CreateDirectory(pathDirectory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            if (!File.Exists(pathFile))
            {
                bib.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><fichiers></fichiers>");
                bib.Save(pathFile);
            }
        }
        public void ajouter(RecivedFiles rf)
        {
            bool exist = false;
            bib.Load(pathFile);

            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nom"].InnerText == rf.getPathOfSave() && xn["date"].InnerText == rf.getDateRecep())
                {
                    XmlAttribute inserver = bib.CreateAttribute("IsInServer");
                    inserver.Value = "false";
                    XmlAttribute indropbox = bib.CreateAttribute("IsInDropbox");
                    indropbox.Value = "false";
                    XmlAttribute etat = bib.CreateAttribute("Etat");
                    etat.Value = "Synchronized";

                    xn.Attributes.Append(etat);
                    xn.Attributes.Append(inserver);
                    xn.Attributes.Append(indropbox);

                    bib.DocumentElement.AppendChild(xn);

                    XmlNode nom = xn.SelectSingleNode("nom");
                    XmlNode path = xn.SelectSingleNode("path");
                    XmlNode comment = xn.SelectSingleNode("commentaire");
                    XmlNode createur = xn.SelectSingleNode("createur");
                    XmlNode date = xn.SelectSingleNode("date");

                    nom.InnerText = rf.getNom();
                    if (rf.getPathOfSave() != "" && rf.getPathOfSave() != null)
                    path.InnerText = rf.getPathOfSave();
                    comment.InnerText = rf.getComment();
                    createur.InnerText = rf.getNomCreateur();
                    date.InnerText = rf.getDateRecep();

                    XmlNode oldConfidentialite = xn.SelectSingleNode("Confidentialite");

                    XmlNode oldUtilisateurs = oldConfidentialite.SelectSingleNode("utilisateurs");
                    XmlNode newUtilisateurs = bib.CreateElement("utilisateurs");
                    try
                    {
                        foreach (Utilisateur u in rf.confidance.getListUtil())
                        {
                            XmlNode utilisateur = bib.CreateElement("utilisateur");

                            XmlNode nomUtil = bib.CreateElement("nomUtil");
                            nomUtil.InnerText = u.getNomUtil();
                            utilisateur.AppendChild(nomUtil);

                            XmlNode nomut = bib.CreateElement("nom");
                            nomut.InnerText = u.nom;
                            utilisateur.AppendChild(nomut);

                            XmlNode prenut = bib.CreateElement("prenom");
                            prenut.InnerText = u.prenom;
                            utilisateur.AppendChild(prenut);

                            newUtilisateurs.AppendChild(utilisateur);
                        }
                    }
                    catch { }
                    oldConfidentialite.ReplaceChild(newUtilisateurs, oldUtilisateurs);

                    XmlNode oldgroups = oldConfidentialite.SelectSingleNode("groups");
                    XmlNode newgroups = bib.CreateElement("groups");
                    try
                    {
                        foreach (Groupe g in rf.confidance.getListGroup())
                        {
                            XmlNode group = bib.CreateElement("group");

                            XmlNode nomGroup = bib.CreateElement("nomGroup");
                            nomGroup.InnerText = g.nomDuGroup;
                            group.AppendChild(nomGroup);

                            XmlNode membres = bib.CreateElement("membres");
                            try
                            {
                                foreach (Groupe gr in rf.confidance.getListGroup())
                                {
                                    foreach (Utilisateur u in gr.ListeUtilisateurs)
                                    {
                                        XmlNode membre = bib.CreateElement("membre");

                                        XmlNode nomUtil = bib.CreateElement("nomUtil");
                                        nomUtil.InnerText = u.getNomUtil();
                                        membre.AppendChild(nomUtil);

                                        XmlNode nomut = bib.CreateElement("nom");
                                        nomut.InnerText = u.nom;
                                        membre.AppendChild(nomut);

                                        XmlNode prenut = bib.CreateElement("prenom");
                                        prenut.InnerText = u.prenom;
                                        membre.AppendChild(prenut);

                                        membres.AppendChild(membre);
                                    }
                                }
                            }
                            catch { }
                            group.AppendChild(membres);
                            newgroups.AppendChild(group);
                        }
                    }
                    catch { }
                    oldConfidentialite.ReplaceChild(newgroups, oldgroups);
                    exist = true;
                }
            }
            if (!exist)
            {
                XmlNode fichierx = bib.CreateElement("fichier");

                XmlAttribute inserver = bib.CreateAttribute("IsInServer");
                inserver.Value = "false";
                XmlAttribute indropbox = bib.CreateAttribute("IsInDropbox");
                indropbox.Value = "false";
                XmlAttribute etat = bib.CreateAttribute("Etat");
                etat.Value = "Synchronized";

                fichierx.Attributes.Append(etat);
                fichierx.Attributes.Append(inserver);
                fichierx.Attributes.Append(indropbox);


                bib.DocumentElement.AppendChild(fichierx);

                XmlNode nom = bib.CreateElement("nom");
                nom.InnerText = rf.getNom();
                fichierx.AppendChild(nom);

                XmlNode path = bib.CreateElement("path");
                path.InnerText = rf.getPathOfSave();
                fichierx.AppendChild(path);

                XmlNode comment = bib.CreateElement("commentaire");
                comment.InnerText = rf.getComment();
                fichierx.AppendChild(comment);

                XmlNode createur = bib.CreateElement("createur");
                createur.InnerText = rf.getNomCreateur();
                fichierx.AppendChild(createur);

                XmlNode date = bib.CreateElement("date");
                date.InnerText = rf.getDateRecep();
                fichierx.AppendChild(date);

                XmlNode Confidentialite = bib.CreateElement("Confidentialite");

                XmlNode utilisateurs = bib.CreateElement("utilisateurs");
                try
                {
                    foreach (Utilisateur u in rf.confidance.getListUtil())
                    {
                        XmlNode utilisateur = bib.CreateElement("utilisateur");

                        XmlNode nomUtil = bib.CreateElement("nomUtil");
                        nomUtil.InnerText = u.getNomUtil();
                        utilisateur.AppendChild(nomUtil);

                        XmlNode nomut = bib.CreateElement("nom");
                        nomut.InnerText = u.nom;
                        utilisateur.AppendChild(nomut);

                        XmlNode prenut = bib.CreateElement("prenom");
                        prenut.InnerText = u.prenom;
                        utilisateur.AppendChild(prenut);

                        utilisateurs.AppendChild(utilisateur);
                    }
                }
                catch { }
                Confidentialite.AppendChild(utilisateurs);

                XmlNode groups = bib.CreateElement("groups");
                try
                {
                    foreach (Groupe g in rf.confidance.getListGroup())
                    {
                        XmlNode group = bib.CreateElement("group");

                        XmlNode nomGroup = bib.CreateElement("nomGroup");
                        nomGroup.InnerText = g.nomDuGroup;
                        group.AppendChild(nomGroup);

                        XmlNode membres = bib.CreateElement("membres");
                        try
                        {
                            foreach (Groupe gr in rf.confidance.getListGroup())
                            {
                                foreach (Utilisateur u in gr.ListeUtilisateurs)
                                {
                                    XmlNode membre = bib.CreateElement("membre");

                                    XmlNode nomUtil = bib.CreateElement("nomUtil");
                                    nomUtil.InnerText = u.getNomUtil();
                                    membre.AppendChild(nomUtil);

                                    XmlNode nomut = bib.CreateElement("nom");
                                    nomut.InnerText = u.nom;
                                    membre.AppendChild(nomut);

                                    XmlNode prenut = bib.CreateElement("prenom");
                                    prenut.InnerText = u.prenom;
                                    membre.AppendChild(prenut);

                                    membres.AppendChild(membre);
                                }
                            }
                        }
                        catch { }
                        group.AppendChild(membres);
                        groups.AppendChild(group);
                    }
                }
                catch { }
                Confidentialite.AppendChild(groups);
                fichierx.AppendChild(Confidentialite);
            }
            bib.Save(pathFile);
        }
        public List<RecivedFiles> getAllFiles()
        {
            bib.Load(pathFile);
            List<RecivedFiles> l = new List<RecivedFiles>();
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            bool isForMe = false;
            foreach (XmlNode xn in xnList)
            {
                    string nomfile = xn["nom"].InnerText;
                    string path = xn["path"].InnerText;
                    string commentaire = xn["commentaire"].InnerText;
                    string createurfile = xn["createur"].InnerText;
                    string daterecep = xn["date"].InnerText;

                    List<Utilisateur> listUtil = new List<Utilisateur>();
                    List<Groupe> listGroup = new List<Groupe>();

                    XmlNodeList Utilisateurs = xn.SelectNodes("Confidentialite/utilisateurs/utilisateur");
                    foreach (XmlNode Utilisateur in Utilisateurs)
                    {
                        string nomutil = Utilisateur["nomUtil"].InnerText;
                        string nomut = Utilisateur["nom"].InnerText;
                        string prenomut = Utilisateur["prenom"].InnerText;
                        if (nomutil == new LastUserConnection().getNomUtil())
                            isForMe = true;
                        listUtil.Add(new MyClasses.Utilisateur(nomutil, nomut, prenomut));
                    }

                    XmlNodeList groups = xn.SelectNodes("Confidentialite/groups/group");
                    foreach (XmlNode group in groups)
                    {
                        string nomg = group["nomGroup"].InnerText;

                        List<Utilisateur> listMembre = new List<Utilisateur>();
                        XmlNodeList membres = group.SelectNodes("membres/membre");
                        foreach (XmlNode membre in membres)
                        {
                            string nomutil = membre["nomUtil"].InnerText;
                            string nomut = membre["nom"].InnerText;
                            string prenomut = membre["prenom"].InnerText;
                            if (nomutil == new LastUserConnection().getNomUtil())
                                isForMe = true;
                            listMembre.Add(new Utilisateur(nomutil, nomut, prenomut));
                        }
                        Groupe g = new Groupe(nomg, listMembre);
                        listGroup.Add(g);
                    }
                    ConfidentialiteFichier conf = new ConfidentialiteFichier(listUtil, listGroup);
                    if (isForMe)    
                l.Add(new RecivedFiles(nomfile, path, commentaire, createurfile, daterecep, conf));
            }
            return l;
        }       
        public List<RecivedFiles>rechercheFichiers(string mot)
        {
            List<RecivedFiles> l = new List<RecivedFiles>();
            if (mot != "")
            {
                bib.Load(pathFile);
                XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
                foreach (XmlNode xn in xnList)
                {
                    if ((xn["nom"].InnerText.ToLower().IndexOf(mot.ToLower()) >= 0) || (xn["commentaire"].InnerText.ToLower().IndexOf(mot.ToLower()) >= 0))
                    {
                        string nomfile = xn["nom"].InnerText;
                        string path = xn["path"].InnerText;
                        string commentaire = xn["commentaire"].InnerText;
                        string createurfile = xn["createur"].InnerText;
                        string daterecep = xn["date"].InnerText;

                        List<Utilisateur> listUtil = new List<Utilisateur>();
                        List<Groupe> listGroup = new List<Groupe>();

                        XmlNodeList Utilisateurs = xn.SelectNodes("Confidentialite/utilisateurs/utilisateur");
                        foreach (XmlNode Utilisateur in Utilisateurs)
                        {
                            string nomutil = Utilisateur["nomUtil"].InnerText;
                            string nomut = Utilisateur["nom"].InnerText;
                            string prenomut = Utilisateur["prenom"].InnerText;

                            listUtil.Add(new MyClasses.Utilisateur(nomutil, nomut, prenomut));
                        }

                        XmlNodeList groups = xn.SelectNodes("Confidentialite/groups/group");
                        foreach (XmlNode group in groups)
                        {
                            string nomg = group["nomGroup"].InnerText;

                            List<Utilisateur> listMembre = new List<Utilisateur>();
                            XmlNodeList membres = group.SelectNodes("membres/membre");
                            foreach (XmlNode membre in membres)
                            {
                                string nomutil = membre["nomUtil"].InnerText;
                                string nomut = membre["nom"].InnerText;
                                string prenomut = membre["prenom"].InnerText;
                                listMembre.Add(new Utilisateur(nomutil, nomut, prenomut));
                            }
                            Groupe g = new Groupe(nomg, listMembre);
                            listGroup.Add(g);
                        }
                        ConfidentialiteFichier conf = new ConfidentialiteFichier(listUtil, listGroup);
                        l.Add(new RecivedFiles(nomfile, path, commentaire, createurfile, daterecep, conf));
                    }
                }
            }
            return l;
        }

        public List<RecivedFiles> getFilesToDownload()
        {
            bib.Load(pathFile);
            List<RecivedFiles> l = new List<RecivedFiles>();
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            bool isForMe = false;
            foreach (XmlNode xn in xnList)
            {
                if ((xn.Attributes["Etat"].Value == "To Download") && ((xn.Attributes["IsInDropbox"].Value == "true") || (xn.Attributes["IsInServer"].Value == "true")))
                {
                string nomfile = xn["nom"].InnerText;
                string path = xn["path"].InnerText;
                string commentaire = xn["commentaire"].InnerText;
                string createurfile = xn["createur"].InnerText;
                string daterecep = xn["date"].InnerText;

                List<Utilisateur> listUtil = new List<Utilisateur>();
                List<Groupe> listGroup = new List<Groupe>();

                XmlNodeList Utilisateurs = xn.SelectNodes("Confidentialite/utilisateurs/utilisateur");
                foreach (XmlNode Utilisateur in Utilisateurs)
                {
                    string nomutil = Utilisateur["nomUtil"].InnerText;
                    string nomut = Utilisateur["nom"].InnerText;
                    string prenomut = Utilisateur["prenom"].InnerText;
                    if (nomutil == new LastUserConnection().getNomUtil())
                        isForMe = true;
                    listUtil.Add(new MyClasses.Utilisateur(nomutil, nomut, prenomut));
                }

                XmlNodeList groups = xn.SelectNodes("Confidentialite/groups/group");
                foreach (XmlNode group in groups)
                {
                    string nomg = group["nomGroup"].InnerText;

                    List<Utilisateur> listMembre = new List<Utilisateur>();
                    XmlNodeList membres = group.SelectNodes("membres/membre");
                    foreach (XmlNode membre in membres)
                    {
                        string nomutil = membre["nomUtil"].InnerText;
                        string nomut = membre["nom"].InnerText;
                        string prenomut = membre["prenom"].InnerText;
                        if (nomutil == new LastUserConnection().getNomUtil())
                            isForMe = true;
                        listMembre.Add(new Utilisateur(nomutil, nomut, prenomut));
                    }
                    Groupe g = new Groupe(nomg, listMembre);
                    listGroup.Add(g);
                }
                ConfidentialiteFichier conf = new ConfidentialiteFichier(listUtil, listGroup);
                if (isForMe)
                    l.Add(new RecivedFiles(nomfile, path, commentaire, createurfile, daterecep, conf));
            }}
            return l;
        }       

        public List<RecivedFiles> getFilesToUploadToDropbox()
        {
            List<RecivedFiles> l = new List<RecivedFiles>();
            bib.Load(pathFile);
                XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
                foreach (XmlNode xn in xnList)
                {
                    if ((xn.Attributes["Etat"].Value == "To Upload") && (xn.Attributes["IsInDropbox"].Value == "false"))
                    {
                        string nomfile = xn["nom"].InnerText;
                        string path = xn["path"].InnerText;
                        string commentaire = xn["commentaire"].InnerText;
                        string createurfile = xn["createur"].InnerText;
                        string daterecep = xn["date"].InnerText;

                        List<Utilisateur> listUtil = new List<Utilisateur>();
                        List<Groupe> listGroup = new List<Groupe>();

                        XmlNodeList Utilisateurs = xn.SelectNodes("Confidentialite/utilisateurs/utilisateur");
                        foreach (XmlNode Utilisateur in Utilisateurs)
                        {
                            string nomutil = Utilisateur["nomUtil"].InnerText;
                            string nomut = Utilisateur["nom"].InnerText;
                            string prenomut = Utilisateur["prenom"].InnerText;

                            listUtil.Add(new MyClasses.Utilisateur(nomutil, nomut, prenomut));
                        }

                        XmlNodeList groups = xn.SelectNodes("Confidentialite/groups/group");
                        foreach (XmlNode group in groups)
                        {
                            string nomg = group["nomGroup"].InnerText;

                            List<Utilisateur> listMembre = new List<Utilisateur>();
                            XmlNodeList membres = group.SelectNodes("membres/membre");
                            foreach (XmlNode membre in membres)
                            {
                                string nomutil = membre["nomUtil"].InnerText;
                                string nomut = membre["nom"].InnerText;
                                string prenomut = membre["prenom"].InnerText;
                                listMembre.Add(new Utilisateur(nomutil, nomut, prenomut));
                            }
                            Groupe g = new Groupe(nomg, listMembre);
                            listGroup.Add(g);
                        }
                        ConfidentialiteFichier conf = new ConfidentialiteFichier(listUtil, listGroup);
                        l.Add(new RecivedFiles(nomfile, path, commentaire, createurfile, daterecep, conf));
                    }
                }
                return l;
            }
        public List<RecivedFiles> getFilesToUploadToServer()
        {
            List<RecivedFiles> l = new List<RecivedFiles>();
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if ((xn.Attributes["Etat"].Value == "To Upload") && (xn.Attributes["IsInServer"].Value == "false"))
                {
                    string nomfile = xn["nom"].InnerText;
                    string path = xn["path"].InnerText;
                    string commentaire = xn["commentaire"].InnerText;
                    string createurfile = xn["createur"].InnerText;
                    string daterecep = xn["date"].InnerText;

                    List<Utilisateur> listUtil = new List<Utilisateur>();
                    List<Groupe> listGroup = new List<Groupe>();

                    XmlNodeList Utilisateurs = xn.SelectNodes("Confidentialite/utilisateurs/utilisateur");
                    foreach (XmlNode Utilisateur in Utilisateurs)
                    {
                        string nomutil = Utilisateur["nomUtil"].InnerText;
                        string nomut = Utilisateur["nom"].InnerText;
                        string prenomut = Utilisateur["prenom"].InnerText;

                        listUtil.Add(new MyClasses.Utilisateur(nomutil, nomut, prenomut));
                    }

                    XmlNodeList groups = xn.SelectNodes("Confidentialite/groups/group");
                    foreach (XmlNode group in groups)
                    {
                        string nomg = group["nomGroup"].InnerText;

                        List<Utilisateur> listMembre = new List<Utilisateur>();
                        XmlNodeList membres = group.SelectNodes("membres/membre");
                        foreach (XmlNode membre in membres)
                        {
                            string nomutil = membre["nomUtil"].InnerText;
                            string nomut = membre["nom"].InnerText;
                            string prenomut = membre["prenom"].InnerText;
                            listMembre.Add(new Utilisateur(nomutil, nomut, prenomut));
                        }
                        Groupe g = new Groupe(nomg, listMembre);
                        listGroup.Add(g);
                    }
                    ConfidentialiteFichier conf = new ConfidentialiteFichier(listUtil, listGroup);
                    l.Add(new RecivedFiles(nomfile, path, commentaire, createurfile, daterecep, conf));
                }
            }
            return l;
        }
        public RecivedFiles rechercheFichier(string p)
        {
            RecivedFiles FichierChercher = null;
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (FichierChercher != null)
                {
                    if (xn["path"].InnerText == p && xn["date"].InnerText.CompareTo(FichierChercher.getDateRecep()) > 0)
                    {
                        string nomfile = xn["nom"].InnerText;
                        string path = xn["path"].InnerText;
                        string commentaire = xn["commentaire"].InnerText;
                        string createurfile = xn["createur"].InnerText;
                        string daterecep = xn["date"].InnerText;

                        List<Utilisateur> listUtil = new List<Utilisateur>();
                        List<Groupe> listGroup = new List<Groupe>();

                        XmlNodeList Utilisateurs = xn.SelectNodes("Confidentialite/utilisateurs/utilisateur");
                        foreach (XmlNode Utilisateur in Utilisateurs)
                        {
                            string nomutil = Utilisateur["nomUtil"].InnerText;
                            string nomut = Utilisateur["nom"].InnerText;
                            string prenomut = Utilisateur["prenom"].InnerText;

                            listUtil.Add(new MyClasses.Utilisateur(nomutil, nomut, prenomut));
                        }

                        XmlNodeList groups = xn.SelectNodes("Confidentialite/groups/group");
                        foreach (XmlNode group in groups)
                        {
                            string nomg = group["nomGroup"].InnerText;

                            List<Utilisateur> listMembre = new List<Utilisateur>();
                            XmlNodeList membres = group.SelectNodes("membres/membre");
                            foreach (XmlNode membre in membres)
                            {
                                string nomutil = membre["nomUtil"].InnerText;
                                string nomut = membre["nom"].InnerText;
                                string prenomut = membre["prenom"].InnerText;
                                listMembre.Add(new Utilisateur(nomutil, nomut, prenomut));
                            }
                            Groupe g = new Groupe(nomg, listMembre);
                            listGroup.Add(g);
                        }
                        ConfidentialiteFichier conf = new ConfidentialiteFichier(listUtil, listGroup);
                        FichierChercher = new RecivedFiles(nomfile, path, commentaire, createurfile, daterecep, conf);
                    }
                }
                else
                {
                    if (xn["path"].InnerText == p)
                    {
                        string nomfile = xn["nom"].InnerText;
                        string path = xn["path"].InnerText;
                        string commentaire = xn["commentaire"].InnerText;
                        string createurfile = xn["createur"].InnerText;
                        string daterecep = xn["date"].InnerText;

                        List<Utilisateur> listUtil = new List<Utilisateur>();
                        List<Groupe> listGroup = new List<Groupe>();

                        XmlNodeList Utilisateurs = xn.SelectNodes("Confidentialite/utilisateurs/utilisateur");
                        foreach (XmlNode Utilisateur in Utilisateurs)
                        {
                            string nomutil = Utilisateur["nomUtil"].InnerText;
                            string nomut = Utilisateur["nom"].InnerText;
                            string prenomut = Utilisateur["prenom"].InnerText;

                            listUtil.Add(new MyClasses.Utilisateur(nomutil, nomut, prenomut));
                        }

                        XmlNodeList groups = xn.SelectNodes("Confidentialite/groups/group");
                        foreach (XmlNode group in groups)
                        {
                            string nomg = group["nomGroup"].InnerText;

                            List<Utilisateur> listMembre = new List<Utilisateur>();
                            XmlNodeList membres = group.SelectNodes("membres/membre");
                            foreach (XmlNode membre in membres)
                            {
                                string nomutil = membre["nomUtil"].InnerText;
                                string nomut = membre["nom"].InnerText;
                                string prenomut = membre["prenom"].InnerText;
                                listMembre.Add(new Utilisateur(nomutil, nomut, prenomut));
                            }
                            Groupe g = new Groupe(nomg, listMembre);
                            listGroup.Add(g);
                        }
                        ConfidentialiteFichier conf = new ConfidentialiteFichier(listUtil, listGroup);
                        FichierChercher = new RecivedFiles(nomfile, path, commentaire, createurfile, daterecep, conf);
                    }
                }
            }
            return FichierChercher;
        }

        public bool isExistAndNotInServer(RecivedFiles rf)
        {
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nom"].InnerText == rf.getNom() && xn["date"].InnerText == rf.getDateRecep())
                { return xn.Attributes["IsInServer"].Value == "false"; }
            }
            return false;
        }
        public bool isExistAndNotInDropbox(RecivedFiles rf)
        {
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nom"].InnerText == rf.getNom() && xn["date"].InnerText == rf.getDateRecep())
                { return xn.Attributes["IsInDropbox"].Value == "false"; }
            }
            return false;
        }

        public void setPathOfSave(RecivedFiles rf)
        {
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nom"].InnerText == rf.getNom() && xn["date"].InnerText == rf.getDateRecep())
                { xn["path"].InnerText = rf.getPathOfSave(); }
            }
            bib.Save(pathFile);
        }
        public void deleteFile(RecivedFiles rf)
        {
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nom"].InnerText == rf.getNom() && xn["date"].InnerText == rf.getDateRecep())
                {
                    xn.ParentNode.RemoveChild(xn);
                }
            }
            bib.Save(pathFile);
        }

        public void setFilIsInServer(RecivedFiles rf)
        {
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nom"].InnerText == rf.getNom() && xn["date"].InnerText == rf.getDateRecep())
                { xn.Attributes["IsInServer"].Value = "true"; }
            }
            bib.Save(pathFile);
        }
        public void setFilIsInDropbox(RecivedFiles rf)
        {
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nom"].InnerText == rf.getNom() && xn["date"].InnerText == rf.getDateRecep())
                { xn.Attributes["IsInDropbox"].Value = "true"; }
            }
            bib.Save(pathFile);
        }

        public void setFilIsNotInServer(RecivedFiles rf)
        {
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nom"].InnerText == rf.getNom() && xn["date"].InnerText == rf.getDateRecep())
                { xn.Attributes["IsInServer"].Value = "false"; }
            }
            bib.Save(pathFile);
        }
        public void setFilIsNotInDropbox(RecivedFiles rf)
        {
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nom"].InnerText == rf.getNom() && xn["date"].InnerText == rf.getDateRecep())
                { xn.Attributes["IsInDropbox"].Value = "false"; }
            }
            bib.Save(pathFile);
        }

        public bool FilIsInServer(RecivedFiles rf)
        {
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nom"].InnerText == rf.getNom() && xn["date"].InnerText == rf.getDateRecep())
                { if (xn.Attributes["IsInServer"].Value == "true")return true; }
            }
            return false;
        }
        public bool FilIsInDropbox(RecivedFiles rf)
        {
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nom"].InnerText == rf.getNom() && xn["date"].InnerText == rf.getDateRecep())
                { if (xn.Attributes["IsInDropbox"].Value == "true")return true; }
            }
            return (false);
        }

        public void setEtatToDownload(RecivedFiles rf)
        {
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nom"].InnerText == rf.getNom() && xn["date"].InnerText == rf.getDateRecep())
                { xn.Attributes["Etat"].Value = "To Download"; }
            }
            bib.Save(pathFile);
        }
        public void setEtatToUpload(RecivedFiles rf)
        {
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nom"].InnerText == rf.getNom() && xn["date"].InnerText == rf.getDateRecep())
                { xn.Attributes["Etat"].Value = "To Upload"; }
            }
            bib.Save(pathFile);
        }
        public void setEtatSynchronized(RecivedFiles rf)
        {
            bib.Load(pathFile);
            XmlNodeList xnList = bib.SelectNodes("/fichiers/fichier");
            foreach (XmlNode xn in xnList)
            {
                if (xn["nom"].InnerText == rf.getNom() && xn["date"].InnerText == rf.getDateRecep())
                { xn.Attributes["Etat"].Value = "Synchronized"; }
            }
            bib.Save(pathFile);
        }
        /* -------------------- server ----------------------------------*/
        public void saveFileInBibServer(string LocalNetworkShare, RecivedFiles rf)
        {
            string pathDirectoryInServer = LocalNetworkShare;
            string pathFileInServer = LocalNetworkShare + @"\BibFiles.xml";
            if (File.Exists(pathDirectoryInServer + @"\" + rf.getNom()))
            { rf.setNom(rf.dateRecept.ToString().Replace('/','+').Replace(':','+') + rf.nom); }
            string destFile = pathDirectoryInServer + @"\" + rf.getNom();
            File.Copy(rf.getPathOfSave(), destFile, true);
            rf.setPathOfSave(destFile);
            XmlDocument bibServer = new XmlDocument();
            if (Directory.Exists(pathDirectoryInServer))
            {
                if (!File.Exists(pathFileInServer))
                {
                    bibServer.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><fichiers></fichiers>");
                    bibServer.Save(pathFileInServer);
                }                
                bool exist = false;
                bibServer.Load(pathFileInServer);

                XmlNodeList xnList = bibServer.SelectNodes("/fichiers/fichier");
                foreach (XmlNode xn in xnList)
                {
                    if (xn["path"].InnerText == rf.getPathOfSave() && xn["date"].InnerText == rf.getDateRecep())
                    {

                        bibServer.DocumentElement.AppendChild(xn);

                        XmlNode nom = xn.SelectSingleNode("nom");
                        XmlNode path = xn.SelectSingleNode("path");
                        XmlNode comment = xn.SelectSingleNode("commentaire");
                        XmlNode createur = xn.SelectSingleNode("createur");
                        XmlNode date = xn.SelectSingleNode("date");

                        nom.InnerText = rf.getNom();
                        path.InnerText = rf.getPathOfSave();
                        comment.InnerText = rf.getComment();
                        createur.InnerText = rf.getNomCreateur();
                        date.InnerText = rf.getDateRecep();

                        XmlNode oldConfidentialite = xn.SelectSingleNode("Confidentialite");

                        XmlNode oldUtilisateurs = oldConfidentialite.SelectSingleNode("utilisateurs");
                        XmlNode newUtilisateurs = bibServer.CreateElement("utilisateurs");
                        try
                        {
                            foreach (Utilisateur u in rf.confidance.getListUtil())
                            {
                                XmlNode utilisateur = bibServer.CreateElement("utilisateur");

                                XmlNode nomUtil = bibServer.CreateElement("nomUtil");
                                nomUtil.InnerText = u.getNomUtil();
                                utilisateur.AppendChild(nomUtil);

                                XmlNode nomut = bibServer.CreateElement("nom");
                                nomut.InnerText = u.nom;
                                utilisateur.AppendChild(nomut);

                                XmlNode prenut = bibServer.CreateElement("prenom");
                                prenut.InnerText = u.prenom;
                                utilisateur.AppendChild(prenut);

                                newUtilisateurs.AppendChild(utilisateur);
                            }
                        }
                        catch { }
                        oldConfidentialite.ReplaceChild(newUtilisateurs, oldUtilisateurs);

                        XmlNode oldgroups = oldConfidentialite.SelectSingleNode("groups");
                        XmlNode newgroups = bibServer.CreateElement("groups");
                        try
                        {
                            foreach (Groupe g in rf.confidance.getListGroup())
                            {
                                XmlNode group = bibServer.CreateElement("group");

                                XmlNode nomGroup = bibServer.CreateElement("nomGroup");
                                nomGroup.InnerText = g.nomDuGroup;
                                group.AppendChild(nomGroup);

                                XmlNode membres = bibServer.CreateElement("membres");
                                try
                                {
                                    foreach (Groupe gr in rf.confidance.getListGroup())
                                    {
                                        foreach (Utilisateur u in gr.ListeUtilisateurs)
                                        {
                                            XmlNode membre = bibServer.CreateElement("membre");

                                            XmlNode nomUtil = bibServer.CreateElement("nomUtil");
                                            nomUtil.InnerText = u.getNomUtil();
                                            membre.AppendChild(nomUtil);

                                            XmlNode nomut = bibServer.CreateElement("nom");
                                            nomut.InnerText = u.nom;
                                            membre.AppendChild(nomut);

                                            XmlNode prenut = bibServer.CreateElement("prenom");
                                            prenut.InnerText = u.prenom;
                                            membre.AppendChild(prenut);

                                            membres.AppendChild(membre);
                                        }
                                    }
                                }
                                catch { }
                                group.AppendChild(membres);
                                newgroups.AppendChild(group);
                            }
                        }
                        catch { }
                        oldConfidentialite.ReplaceChild(newgroups, oldgroups);
                        exist = true;
                    }
                }
                if (!exist)
                {
                    XmlNode fichierx = bibServer.CreateElement("fichier");

                    bibServer.DocumentElement.AppendChild(fichierx);

                    XmlNode nom = bibServer.CreateElement("nom");
                    nom.InnerText = rf.getNom();
                    fichierx.AppendChild(nom);

                    XmlNode path = bibServer.CreateElement("path");
                    path.InnerText = rf.getPathOfSave();
                    fichierx.AppendChild(path);

                    XmlNode comment = bibServer.CreateElement("commentaire");
                    comment.InnerText = rf.getComment();
                    fichierx.AppendChild(comment);

                    XmlNode createur = bibServer.CreateElement("createur");
                    createur.InnerText = rf.getNomCreateur();
                    fichierx.AppendChild(createur);

                    XmlNode date = bibServer.CreateElement("date");
                    date.InnerText = rf.getDateRecep();
                    fichierx.AppendChild(date);

                    XmlNode Confidentialite = bibServer.CreateElement("Confidentialite");

                    XmlNode utilisateurs = bibServer.CreateElement("utilisateurs");
                    try
                    {
                        foreach (Utilisateur u in rf.confidance.getListUtil())
                        {
                            XmlNode utilisateur = bibServer.CreateElement("utilisateur");

                            XmlNode nomUtil = bibServer.CreateElement("nomUtil");
                            nomUtil.InnerText = u.getNomUtil();
                            utilisateur.AppendChild(nomUtil);

                            XmlNode nomut = bibServer.CreateElement("nom");
                            nomut.InnerText = u.nom;
                            utilisateur.AppendChild(nomut);

                            XmlNode prenut = bibServer.CreateElement("prenom");
                            prenut.InnerText = u.prenom;
                            utilisateur.AppendChild(prenut);

                            utilisateurs.AppendChild(utilisateur);
                        }
                    }
                    catch { }
                    Confidentialite.AppendChild(utilisateurs);

                    XmlNode groups = bibServer.CreateElement("groups");
                    try
                    {
                        foreach (Groupe g in rf.confidance.getListGroup())
                        {
                            XmlNode group = bibServer.CreateElement("group");

                            XmlNode nomGroup = bibServer.CreateElement("nomGroup");
                            nomGroup.InnerText = g.nomDuGroup;
                            group.AppendChild(nomGroup);

                            XmlNode membres = bibServer.CreateElement("membres");
                            try
                            {
                                foreach (Groupe gr in rf.confidance.getListGroup())
                                {
                                    foreach (Utilisateur u in gr.ListeUtilisateurs)
                                    {
                                        XmlNode membre = bibServer.CreateElement("membre");

                                        XmlNode nomUtil = bibServer.CreateElement("nomUtil");
                                        nomUtil.InnerText = u.getNomUtil();
                                        membre.AppendChild(nomUtil);

                                        XmlNode nomut = bibServer.CreateElement("nom");
                                        nomut.InnerText = u.nom;
                                        membre.AppendChild(nomut);

                                        XmlNode prenut = bibServer.CreateElement("prenom");
                                        prenut.InnerText = u.prenom;
                                        membre.AppendChild(prenut);

                                        membres.AppendChild(membre);
                                    }
                                }
                            }
                            catch { }
                            group.AppendChild(membres);
                            groups.AppendChild(group);
                        }
                    }
                    catch { }
                    Confidentialite.AppendChild(groups);
                    fichierx.AppendChild(Confidentialite);
                }
                bibServer.Save(pathFileInServer);
            }  
        }
        public List<RecivedFiles> getMyFilesFromServer(string LocalNetworkShare)
        {
            string pathDirectoryInServer = LocalNetworkShare;
            string pathFileInServer = LocalNetworkShare+@"\BibFiles.xml";
            XmlDocument bibServer = new XmlDocument();
            if (Directory.Exists(pathDirectoryInServer))
            {
                if (!File.Exists(pathFileInServer))
                {
                    bibServer.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><fichiers></fichiers>");
                    bibServer.Save(pathFileInServer);
                }
                else
                    bibServer.Load(pathFileInServer);
                List<RecivedFiles> l = new List<RecivedFiles>();
                XmlNodeList xnList = bibServer.SelectNodes("/fichiers/fichier");
                bool isForMe = false;
                foreach (XmlNode xn in xnList)
                {
                    if (xn["createur"].InnerText != new LastUserConnection().getNomUtil())
                    {
                        string nomfile = xn["nom"].InnerText;
                        string path = xn["path"].InnerText;
                        string commentaire = xn["commentaire"].InnerText;
                        string createurfile = xn["createur"].InnerText;
                        string daterecep = xn["date"].InnerText;

                        List<Utilisateur> listUtil = new List<Utilisateur>();
                        List<Groupe> listGroup = new List<Groupe>();

                        XmlNodeList Utilisateurs = xn.SelectNodes("Confidentialite/utilisateurs/utilisateur");
                        foreach (XmlNode Utilisateur in Utilisateurs)
                        {
                            string nomutil = Utilisateur["nomUtil"].InnerText;
                            string nomut = Utilisateur["nom"].InnerText;
                            string prenomut = Utilisateur["prenom"].InnerText;
                            if (nomutil == new LastUserConnection().getNomUtil())
                                isForMe = true;
                            listUtil.Add(new MyClasses.Utilisateur(nomutil, nomut, prenomut));
                        }

                        XmlNodeList groups = xn.SelectNodes("Confidentialite/groups/group");
                        foreach (XmlNode group in groups)
                        {
                            string nomg = group["nomGroup"].InnerText;

                            List<Utilisateur> listMembre = new List<Utilisateur>();
                            XmlNodeList membres = group.SelectNodes("membres/membre");
                            foreach (XmlNode membre in membres)
                            {
                                string nomutil = membre["nomUtil"].InnerText;
                                string nomut = membre["nom"].InnerText;
                                string prenomut = membre["prenom"].InnerText;
                                if (nomutil == new LastUserConnection().getNomUtil())
                                    isForMe = true;
                                listMembre.Add(new Utilisateur(nomutil, nomut, prenomut));
                            }
                            Groupe g = new Groupe(nomg, listMembre);
                            listGroup.Add(g);
                        }
                        ConfidentialiteFichier conf = new ConfidentialiteFichier(listUtil, listGroup);
                        if (isForMe)                           
                            l.Add(new RecivedFiles(nomfile, path, commentaire, createurfile, daterecep, conf));
                    }
                }
                return l;
            }
            return null;
        }
        /* --------------------------------------------------------------*/
    }
}