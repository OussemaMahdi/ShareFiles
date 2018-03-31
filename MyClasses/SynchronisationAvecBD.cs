using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MyClasses
{
    public static class SynchronisationAvecBD
    {

        public static void saveAllUsers()
        {
            try
            {
                string req = "select * from Utilisateur";
                ConnectionDB cnx = ConnectionDB.getInstanceForSync();
                DataSet ds = cnx.selectionner(req);
                foreach (DataRow util in ds.Tables[0].Rows)
                {
                    Utilisateur u = new Utilisateur(util[0].ToString(), util[1].ToString(), util[2].ToString());
                    u.addToXML();
                }
            }
            catch { }
        }       
        public static List<RecivedFiles> getReciveFileFromDB()
        {
            ConnectionDB conn = ConnectionDB.getInstanceForSync();
            try
            {
                LastUserConnection usr = new LastUserConnection();
                string listFiles = "select nomfile from Conf1 where nomutilisateur='" + usr.getNomUtil() + "' and nomfile NOT IN(select nomfile from Fichier where nomcreator='" + usr.getNomUtil() + "')";
                string listGroups = "select nomfile from Conf2 where nomGroup IN (select nomGroup from Appartient where nomutilisateur='" + usr.getNomUtil() + "') and nomfile NOT IN(select nomfile from Fichier where nomcreator='" + usr.getNomUtil() + "')";

                List<RecivedFiles> lrf = new List<RecivedFiles>();
                foreach (DataRow drf in conn.selectionner(listFiles).Tables[0].Rows)
                {
                    List<Utilisateur> luf = new List<Utilisateur>();
                    string listUtilisateursOfFile = "select Distinct(nomutilisateur) from Conf1 where nomfile='" + drf[0].ToString() + "'";
                    foreach (DataRow dru in conn.selectionner(listUtilisateursOfFile).Tables[0].Rows)
                    {
                        string req = "select * from Utilisateur where nomutilisateur='" + dru[0].ToString() + "'";
                        luf.Add(new Utilisateur(conn.selectionner(req).Tables[0].Rows[0][0].ToString(), conn.selectionner(req).Tables[0].Rows[0][1].ToString(), conn.selectionner(req).Tables[0].Rows[0][2].ToString()));
                    }

                    List<Groupe> lgf = new List<Groupe>();
                    string listGroupsOfFile = "select Distinct(nomGroup) from Conf2 where nomfile='" + drf[0].ToString() + "'";
                    foreach (DataRow drg in conn.selectionner(listGroupsOfFile).Tables[0].Rows)
                    {
                        string req = "select * from Groupe where nomGroup='" + drg[0].ToString() + "'";
                        string listMembres = "select nomutilisateur from Appartient where nomGroup='" + drg[0].ToString() + "'";
                        List<Utilisateur> membres = new List<Utilisateur>();
                        foreach (DataRow drmg in conn.selectionner(listMembres).Tables[0].Rows)
                        {
                            string req1 = "select * from Utilisateur where nomutilisateur='" + drmg[0] + "'";
                            membres.Add(new Utilisateur(conn.selectionner(req1).Tables[0].Rows[0][0].ToString(), conn.selectionner(req1).Tables[0].Rows[0][1].ToString(), conn.selectionner(req1).Tables[0].Rows[0][2].ToString()));
                        }
                        lgf.Add(new Groupe(drg[0].ToString(), membres));
                    }
                    ConfidentialiteFichier conf = new ConfidentialiteFichier(luf, lgf);
                    string files = "select * from fichier where nomFile='" + drf[0].ToString() + "'";
                    lrf.Add(new RecivedFiles(conn.selectionner(files).Tables[0].Rows[0][0].ToString(), conn.selectionner(files).Tables[0].Rows[0][1].ToString(), conn.selectionner(files).Tables[0].Rows[0][2].ToString(), conn.selectionner(files).Tables[0].Rows[0][3].ToString(), conf));
                }

                foreach (DataRow drf in conn.selectionner(listGroups).Tables[0].Rows)
                {
                    List<Utilisateur> luf = new List<Utilisateur>();
                    string listUtilisateursOfFile = "select Distinct(nomutilisateur) from Conf1 where nomfile='" + drf[0].ToString() + "'";
                    foreach (DataRow dru in conn.selectionner(listUtilisateursOfFile).Tables[0].Rows)
                    {
                        string req = "select * from Utilisateur where nomutilisateur='" + dru[0].ToString() + "'";
                        luf.Add(new Utilisateur(conn.selectionner(req).Tables[0].Rows[0][0].ToString(), conn.selectionner(req).Tables[0].Rows[0][1].ToString(), conn.selectionner(req).Tables[0].Rows[0][2].ToString()));
                    }

                    List<Groupe> lgf = new List<Groupe>();
                    string listGroupsOfFile = "select Distinct(nomGroup) from Conf2 where nomfile='" + drf[0].ToString() + "'";
                    foreach (DataRow drg in conn.selectionner(listGroupsOfFile).Tables[0].Rows)
                    {
                        string req = "select * from Groupe where nomGroup='" + drg[0].ToString() + "'";
                        string listMembres = "select nomutilisateur from Appartient where nomGroup='" + drg[0].ToString() + "'";
                        List<Utilisateur> membres = new List<Utilisateur>();
                        foreach (DataRow drmg in conn.selectionner(listMembres).Tables[0].Rows)
                        {
                            string req1 = "select * from Utilisateur where nomutilisateur='" + drmg[0] + "'";
                            membres.Add(new Utilisateur(conn.selectionner(req1).Tables[0].Rows[0][0].ToString(), conn.selectionner(req1).Tables[0].Rows[0][1].ToString(), conn.selectionner(req1).Tables[0].Rows[0][2].ToString()));
                        }
                        lgf.Add(new Groupe(drg[0].ToString(), membres));
                    }
                    ConfidentialiteFichier conf = new ConfidentialiteFichier(luf, lgf);
                    string files = "select * from fichier where nomFile='" + drf[0].ToString() + "'";
                    lrf.Add(new RecivedFiles(conn.selectionner(files).Tables[0].Rows[0][0].ToString(), conn.selectionner(files).Tables[0].Rows[0][1].ToString(), conn.selectionner(files).Tables[0].Rows[0][2].ToString(), conn.selectionner(files).Tables[0].Rows[0][3].ToString(), conf));
                }
                return lrf;
            }
            catch { return new List<RecivedFiles>(); }
        }
    

    }
}
