using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyClasses
{
    public class RecivedFiles : IComparable<RecivedFiles>
    {
        public string nom { get; private set; }
        public string comment { get; private set; }
        public string nomCreateur { get; private set; }
        public string PathOfSave { get; private set; }
        public DateTime dateRecept { get; private set; }
        public ConfidentialiteFichier confidance{ get; private set; }
        public string Image { get; private set; }



        public RecivedFiles(string nomfile, string pathofsave, string comment, string nomCreateur, string daterecep, ConfidentialiteFichier conf)
        {
            this.nom = nomfile;
            this.comment = comment;
            this.nomCreateur = nomCreateur;
            this.PathOfSave = pathofsave;
            dateRecept = DateTime.ParseExact(daterecep, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            this.confidance = conf;  
        }
        public RecivedFiles(string nomfile, string comment, string nomCreateur, string daterecep, ConfidentialiteFichier conf)
        {
            this.nom = nomfile;
            this.comment = comment;
            this.nomCreateur = nomCreateur;
            dateRecept = DateTime.ParseExact(daterecep, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            this.confidance = conf;
            this.Image = "Resources/pdf_256.png";
        }

        public string getNom()
        { return nom; }
        public void setNom(string Nom)
        { this.nom = Nom; }

        public string getComment()
        { return comment; }
        public string getNomCreateur()
        { return nomCreateur; }

        public string getPathOfSave()
        { return PathOfSave; }
        public void setPathOfSave(string PathOfSave)
        { this.PathOfSave = PathOfSave; }

        public string getImage()
        { return Image; }
        public void setImage(string Image)
        { this.Image = Image; }

        public string getDateRecep()
        { return dateRecept.ToString("yyyy-MM-dd HH:mm:ss");}       

        public override bool Equals(object obj)
        {
            bool res = false;
            if (obj.GetType() == typeof(RecivedFiles))
            {
                RecivedFiles objCasted = (RecivedFiles)obj;
                res = objCasted.nom ==nom && objCasted.dateRecept == dateRecept;
            }
            return res;
        }

        public override string ToString()
        { return (nom); }

        public int CompareTo(RecivedFiles other)
        {
            if (this.dateRecept == other.dateRecept)
            {
                return this.nom.CompareTo(other.nom);
            }
            return other.dateRecept.CompareTo(this.dateRecept);
        }

        /* ---------- Data Base -------- */
        private bool FileajoutableInDB()
        {
            ConnectionDB conn = ConnectionDB.getInstance();
            string req = "select * from Fichier where nomfile='" + nom + "'";
            return conn.nombreSelectionner(req) == 0;
        }
        public void addToDB()
        {           
            if(!this.FileajoutableInDB())
            {
                string d = this.dateRecept.ToString().Replace(':', '+').Replace('/', '+');
                this.nom=this.nom.Insert(0,d);
            }
            string req1 = "INSERT INTO fichier VALUES ('" + nom + "','" + comment + "','" + nomCreateur + "','" + this.getDateRecep() + "')";
            ConnectionDB Conn = ConnectionDB.getInstance();
            Conn.executer(req1);
            this.SetConfidanceInDB();
            
        }      
        public void supprimerFromDB()
        {
            string req1 = "delete from conf1 where nomfile='" + nom + "'";
            string req2 = "delete from conf2 where nomfile='" + nom + "'";
            string req3 = "delete from fichier where nomfile='" + nom + "'";
            ConnectionDB Conn = ConnectionDB.getInstance();
            Conn.executer(req1);
            Conn.executer(req2);
            Conn.executer(req3);
        }
        private void SetConfidanceInDB()
        {
            foreach (Utilisateur u in confidance.getListUtil())
            {
                string req = "INSERT INTO conf1 VALUES ('" + u.getNomUtil() + "','" + nom + "')";
                ConnectionDB Conn = ConnectionDB.getInstance();
                Conn.executer(req);
                req = "UPDATE Synchronization SET files = 'True' WHERE nomutilisateur='"+u.getNomUtil()+"'";
                Conn.executer(req);
            }
            foreach (Groupe g in confidance.getListGroup())
            {
                if (g.GroupajoutableInDB())
                    g.addGroupInDB();
                g.giveAutToGroupInDB(this);
                    foreach (Utilisateur membre in g.ListeUtilisateurs)
                    {
                        g.SetAppartenanceInGroup(membre);
                        string req = "UPDATE Synchronization SET files = 'True' WHERE nomutilisateur='" + membre.getNomUtil() + "'";
                        ConnectionDB.getInstance().executer(req);
                    }
            }
        }            
        


    }
}
