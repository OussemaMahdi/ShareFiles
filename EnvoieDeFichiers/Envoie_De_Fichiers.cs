using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using MyClasses;
using Dropbox.Api;


namespace EnvoieDeFichiers
{
    public partial class Envoie_De_Fichiers : Form
    {
        string cibleFichier = null;
        string nomDuFichier = null;
        public Envoie_De_Fichiers(string cibleDossier, string cibleFichier)
        {
            InitializeComponent();
            this.cibleFichier = cibleFichier;
            this.nomDuFichier = cibleFichier.Remove(0, cibleDossier.Length + 1);
        }
        private void peuplerCheckedBox(string mot)
        {
            checkedListBox1.Items.Clear();
            checkedListBox2.Items.Clear();
            foreach (Utilisateur u in Utilisateur.getAllUsers())
            {
                if (u.nom.ToLower().IndexOf(mot.ToLower()) == 0 || u.prenom.ToLower().IndexOf(mot.ToLower()) == 0 || u.nomUtil.ToLower().IndexOf(mot.ToLower()) == 0)
                    checkedListBox1.Items.Add(u);
            }
            foreach (Groupe g in Groupe.getAllGroups())
            {
                if (g.nomDuGroup.ToLower().IndexOf(mot.ToLower()) == 0)
                    checkedListBox2.Items.Add(g);
            }
        }
        private void Cancel_Click(object sender, EventArgs e)
        {            
            this.Close();
        }          
        private void Envoie_De_Fichiers_Load(object sender, EventArgs e)
        {
                try
                {
                    Tfichier.Text = cibleFichier;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
           
        }       
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                this.Height = 310;
                peuplerCheckedBox(cherche.Text);
            }
            else
                this.Height = 120;            
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                TGroupe.Enabled = true;
            else
                TGroupe.Enabled = false;
        }
        private void cherche_TextChanged(object sender, EventArgs e)
        {
            peuplerCheckedBox(cherche.Text);
        } 

        //************************** Deplacement  du form *****************************
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private void FormMain_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }
        private void FormMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }
        private void FormMain_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }      
        //*************************************************       
        
        private void Envoyer_Click(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            string format = "yyyy-MM-dd HH:mm:ss";
            LastUserConnection usr = new LastUserConnection();
            if (radioButton2.Checked == true)
            {
                List<Utilisateur> onlyMe = new List<Utilisateur>();
                onlyMe.Add(Utilisateur.getUser(new LastUserConnection().getNomUtil()));
                ConfidentialiteFichier conf = new ConfidentialiteFichier(onlyMe, new List<Groupe>()); ;
                RecivedFiles f = new RecivedFiles(nomDuFichier, cibleFichier, textBox2.Text, usr.getNomUtil(), time.ToString(format), conf);
                BiBFilesXML bib = new BiBFilesXML();
                bib.ajouter(f);
                }
            else if (radioButton1.Checked == true)
            {
                Groupe p = new Groupe("Public", Utilisateur.getAllUsers());
                List<Groupe> lg = new List<Groupe>();
                lg.Add(p);
                ConfidentialiteFichier conf = new ConfidentialiteFichier(new List<Utilisateur>(), lg);
                RecivedFiles f = new RecivedFiles(nomDuFichier, cibleFichier, textBox2.Text, usr.getNomUtil(), time.ToString(format), conf);
                BiBFilesXML bib = new BiBFilesXML();
                bib.ajouter(f);
                bib.setEtatToUpload(f);
                }
            else if (radioButton3.Checked == true)
            {
                List<Utilisateur> lu = new List<Utilisateur>();
                lu.Add(Utilisateur.getUser(new LastUserConnection().getNomUtil()));
                List<Groupe> lg = new List<Groupe>();
                if (checkBox1.Checked == false && checkedListBox1.CheckedItems.Count >= 1)
                {
                    foreach (var elem in checkedListBox1.CheckedItems)
                        if (elem is Utilisateur)
                            lu.Add((Utilisateur)elem);
                    foreach (var elem in checkedListBox2.CheckedItems)
                        if (elem is Groupe)
                            lg.Add((Groupe)elem);
                    ConfidentialiteFichier conf = new ConfidentialiteFichier(lu, lg);
                    RecivedFiles f = new RecivedFiles(nomDuFichier, cibleFichier, textBox2.Text, usr.getNomUtil(), time.ToString(format), conf);
                    BiBFilesXML bib = new BiBFilesXML();
                    bib.ajouter(f);
                    bib.setEtatToUpload(f);
                    }
                else if (checkBox1.Checked == true && TGroupe.Text != "")
                {
                    List<Utilisateur> membres = new List<Utilisateur>();
                    foreach (var u in checkedListBox1.CheckedItems)
                        if (u is Utilisateur)
                            membres.Add((Utilisateur)u);
                    foreach (var u in checkedListBox2.CheckedItems)
                        if (u is Groupe)
                            membres.Concat((((Groupe)u).getMemberOfGroup()));
                    Groupe g = new Groupe(TGroupe.Text, membres);
                    lg.Add(g);
                    g.addToXML();
                    ConfidentialiteFichier conf = new ConfidentialiteFichier(lu, lg);
                    RecivedFiles f = new RecivedFiles(nomDuFichier, cibleFichier, textBox2.Text, usr.getNomUtil(), time.ToString(format), conf);
                    BiBFilesXML bib = new BiBFilesXML();
                    bib.ajouter(f);
                    bib.setEtatToUpload(f);
                    }
            }
        }
    }
}