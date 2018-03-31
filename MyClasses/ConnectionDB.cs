using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace MyClasses
{
    public class ConnectionDB
    {   //private static string ConnStr = @"Data Source=OUSSEMA-PC\SQLEXPRESS;Initial Catalog=ShareFilesDB;Integrated Security=True";
        private static string ConnStr = @"Data Source=h2158087.stratoserver.net\SQLEXPRESS;Initial Catalog=master;User ID=youyou;Password=youyou";
        
        private SqlConnection conn = sqlConnection();
        private static ConnectionDB INSTANCE = new ConnectionDB();
        private static ConnectionDB InstanceForDownloader = new ConnectionDB();
        private static ConnectionDB InstanceForSync = new ConnectionDB();

        private static SqlConnection sqlConnection ()
        { try { return (new SqlConnection(ConnStr)); } catch { MessageBox.Show("Cannot open Data Base", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); return null; } }      
        private ConnectionDB()
        {}

        public static ConnectionDB getInstanceForDownloader()
        {
            try
            { return InstanceForDownloader; }
            catch { return null; }
        }
        public static ConnectionDB getInstanceForSync()
        {
            try
            {
                return InstanceForSync;
            }
            catch { return null; }
        }
        public static ConnectionDB getInstance()
        {
            try
            {
                return INSTANCE;
            }
            catch { return null; }
        }

        private void ouvrirConnection()
        {
            try
            {
                if (conn.State==ConnectionState.Closed)
                conn.Open();
            }
            catch
            {}
        }

        private void fermerConnection()
        {
            try
            {
            if (conn.State == ConnectionState.Open)
            conn.Close();
            }
            catch
            { }
        
        }

        public void executer(string req)
        {
            try
            {
                this.ouvrirConnection();
                SqlCommand cmd = new SqlCommand(req, conn);
                cmd.ExecuteNonQuery();
                this.fermerConnection();
            }
            catch
            {
                this.fermerConnection();
            }
       
        }

        public DataSet selectionner(string req)
        {
            try
            {
                this.ouvrirConnection();
                SqlCommand commande = new SqlCommand(req, conn);
                SqlDataAdapter da = new SqlDataAdapter(req, ConnStr);
                this.fermerConnection();
                DataSet ds = new DataSet();
                da.Fill(ds);
                return (ds);
            }catch
            {
                this.fermerConnection();
                return new DataSet(); }
        }

        public int nombreSelectionner(string req)
        {
            try
            {
                return (selectionner(req).Tables[0].Rows.Count);
            }
            catch
            { return 0; }
        }

        public int getMax(string attribut, string table)
        {
            try
            {
                this.ouvrirConnection();
                string req = "SELECT Max(" + attribut + ") FROM " + table;
                SqlCommand commande = new SqlCommand(req, conn);
                Int32 max = (Int32)commande.ExecuteScalar();
                this.fermerConnection();
                return max;                
            }
            catch
            {
                this.fermerConnection();
                return 0;
            }
        }
      
        public bool isAut(string pathFile)
        {
            try
            {
                LastUserConnection usr = new LastUserConnection();
                string req1 = "select * from Conf1 where nomutilisateur='" + usr.getNomUtil() + "' and path=" + pathFile;
                string req2 = "select * from Conf2 where idGroup IN (select idGroup from Appartient where nomutilisateur='" + usr.getNomUtil() + "' and path=" + pathFile + ")";
                return nombreSelectionner(req1) > 0 || nombreSelectionner(req2) > 0;
            }
            catch { return false; }
        }

        public List<RecivedFiles> getReciveFileFromDB()
        {
            try
            {
                LastUserConnection usr = new LastUserConnection();
                string listFiles = "select nomfile from Conf1 where nomutilisateur='" + usr.getNomUtil() + "'";
                string listGroups = "select nomfile from Conf2 where nomGroup IN (select nomGroup from Appartient where nomutilisateur='" + usr.getNomUtil() + "')";
                               
                List<RecivedFiles> lrf = new List<RecivedFiles>();              
                foreach (DataRow drf in selectionner(listFiles).Tables[0].Rows)
                {
                    List<Utilisateur> luf = new List<Utilisateur>();
                    string listUtilisateursOfFile = "select Distinct(nomutilisateur) from Conf1 where nomfile='" + drf[0].ToString() + "'";
                    foreach (DataRow dru in selectionner(listUtilisateursOfFile).Tables[0].Rows)
                    {
                        string req = "select * from Utilisateur where nomutilisateur='" + dru[0].ToString() + "'";
                        luf.Add(new Utilisateur(selectionner(req).Tables[0].Rows[0][0].ToString(), selectionner(req).Tables[0].Rows[0][1].ToString(), selectionner(req).Tables[0].Rows[0][2].ToString()));
                    }

                    List<Groupe> lgf = new List<Groupe>();
                    string listGroupsOfFile = "select Distinct(nomGroup) from Conf2 where nomfile='" + drf[0].ToString() + "'";
                    foreach (DataRow drg in selectionner(listGroupsOfFile).Tables[0].Rows)
                    {
                        string req = "select * from Groupe where nomGroup='" + drg[0].ToString() + "'";
                        string listMembres = "select nomutilisateur from Appartient where nomGroup='" + drg[0].ToString() + "'";               
                        List<Utilisateur> membres = new List<Utilisateur>();
                        foreach (DataRow drmg in selectionner(listMembres).Tables[0].Rows)
                        {
                            string req1 = "select * from Utilisateur where nomutilisateur='" + drmg[0] + "'";
                            membres.Add(new Utilisateur(selectionner(req1).Tables[0].Rows[0][0].ToString(),selectionner(req1).Tables[0].Rows[0][1].ToString(),selectionner(req1).Tables[0].Rows[0][2].ToString()));          
                        }
                        lgf.Add(new Groupe(drg[0].ToString(), membres));
                    }
                    ConfidentialiteFichier conf = new ConfidentialiteFichier(luf, lgf);
                    string files="select * from fichier where nomFile='"+drf[0].ToString()+"'";
                    lrf.Add(new RecivedFiles(selectionner(files).Tables[0].Rows[0][0].ToString(), selectionner(files).Tables[0].Rows[0][1].ToString(), selectionner(files).Tables[0].Rows[0][2].ToString(), selectionner(files).Tables[0].Rows[0][3].ToString(),conf));
                }

                foreach (DataRow drf in selectionner(listGroups).Tables[0].Rows)
                {
                    List<Utilisateur> luf = new List<Utilisateur>();
                    string listUtilisateursOfFile = "select Distinct(nomutilisateur) from Conf1 where nomfile='" + drf[0].ToString() + "'";
                    foreach (DataRow dru in selectionner(listUtilisateursOfFile).Tables[0].Rows)
                    {
                        string req = "select * from Utilisateur where nomutilisateur='" + dru[0].ToString() + "'";
                        luf.Add(new Utilisateur(selectionner(req).Tables[0].Rows[0][0].ToString(), selectionner(req).Tables[0].Rows[0][1].ToString(), selectionner(req).Tables[0].Rows[0][2].ToString()));
                    }

                    List<Groupe> lgf = new List<Groupe>();
                    string listGroupsOfFile = "select Distinct(nomGroup) from Conf2 where nomfile='" + drf[0].ToString() + "'";
                    foreach (DataRow drg in selectionner(listGroupsOfFile).Tables[0].Rows)
                    {
                        string req = "select * from Groupe where nomGroup='" + drg[0].ToString() + "'";
                        string listMembres = "select nomutilisateur from Appartient where nomGroup='" + drg[0].ToString() + "'";               
                        List<Utilisateur> membres = new List<Utilisateur>();
                        foreach (DataRow drmg in selectionner(listMembres).Tables[0].Rows)
                        {
                            string req1 = "select * from Utilisateur where nomutilisateur='" + drmg[0] + "'";
                            membres.Add(new Utilisateur(selectionner(req1).Tables[0].Rows[0][0].ToString(),selectionner(req1).Tables[0].Rows[0][1].ToString(),selectionner(req1).Tables[0].Rows[0][2].ToString()));          
                        }
                        lgf.Add(new Groupe(drg[0].ToString(), membres));
                    }
                    ConfidentialiteFichier conf = new ConfidentialiteFichier(luf, lgf);
                    string files="select * from fichier where nomFile='"+drf[0].ToString()+"'";
                    lrf.Add(new RecivedFiles(selectionner(files).Tables[0].Rows[0][0].ToString(), selectionner(files).Tables[0].Rows[0][1].ToString(), selectionner(files).Tables[0].Rows[0][2].ToString(), selectionner(files).Tables[0].Rows[0][3].ToString(),conf));
                }
                return lrf;
            }
            catch { return new List<RecivedFiles>(); }
        }

        public List<Utilisateur> getUserAutorizedToFile(string nomFichier)
        {
            List<Utilisateur> lu = new List<Utilisateur>();
            string req = "select * from utilisateur where nomutilisateur IN(select nomutilisateur from conf1 where nomfile='" + nomFichier + "')|| nomutilisateur IN(select nomutilisateur from Appartient where nomGroup IN (select nomGroup from Conf2 where nomfile='" + nomFichier + "'))";
            try
            {
                DataSet dsu = getInstanceForDownloader().selectionner(req);
                foreach (DataRow dr in dsu.Tables[0].Rows)
                {
                    lu.Add(new Utilisateur(dr[0].ToString(), dr[1].ToString(), dr[2].ToString()));
                }
                return (lu);
            }
            catch { return null; }
        }


    }
}
