using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MyClasses;
using System.Management;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;

namespace Etiquette
{
    public partial class MyLabel : Form
    {
        
        private MyProcess myProcess;
        private string myWindowName;
        private string pathMyProcess;
        
        public MyLabel()
        {
            InitializeComponent();
        }
        public MyLabel(string Name_Window, string path)
        {
            InitializeComponent();
            myWindowName = Name_Window;
            pathMyProcess = path;
            myProcess = new MyProcess(myWindowName);
            BibProcess.ajouter(myProcess);
            BiBFilesXML bib = new BiBFilesXML();
            if (bib.rechercheFichier(pathMyProcess)!=null)
            textBox1.Text = bib.rechercheFichier(pathMyProcess).getComment();        
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
        private void cherche_TextChanged(object sender, EventArgs e)
        {
            peuplerCheckedBox(cherche.Text);
        } 
        private void MyLabel_FormClosed(object sender, FormClosedEventArgs e)
        {
            BibProcess.suprimer(myProcess);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                Process proc = myProcess.getProcess();
                if (proc == null) this.Close();
                var placement = GetPlacement(proc.MainWindowHandle);
                var size = GetControlSize(proc.MainWindowHandle);
                if (placement.showCmd.ToString() == "Normal")
                {
                    if (this.Visible == false)
                    {
                        this.Visible = true;
                    }
                    this.Location = new Point(placement.rcNormalPosition.X+size.Width-this.Width, placement.rcNormalPosition.Y+25);
                }
                else if (placement.showCmd.ToString() == "Minimized")
                {
                    if (this.Visible == true)
                    {
                        this.Visible = false;
                    }
                }
                else if (placement.showCmd.ToString() == "Maximized")
                {
                    if (this.Visible == false)
                    {
                        this.Visible = true;
                    }
                    this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Size.Width - 120, 0);
                }
                else
                {
                    BibProcess.suprimer(myProcess);
                    this.Close(); 
                }
                if (ApplicationIsActivated(proc, myWindowName))
                {
                    if (this.TopMost != true)
                        this.TopMost = true;
                }
                else
                {
                    if (this.TopMost == true)
                        this.TopMost = false;
                }
            }
            catch
            {
                BibProcess.suprimer(myProcess);
                this.Close();
            }
        }
        private void label2_Click(object sender, EventArgs e)
        {           
            BibProcess.suprimer(myProcess);
            this.Close();
        }
        
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {

                this.Height = 350;
                button1.Top = 301;

                peuplerCheckedBox(cherche.Text);
            }
            else
            {
                this.Height = 177;
                button1.Top = 151;
            }
        }     
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                TGroupe.Enabled = true;
            else
                TGroupe.Enabled = false;
        }
        
         
        /* -------------------------------size window------------------------------ */
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        public static Size GetControlSize(IntPtr hWnd)
        {
            RECT pRect;
            Size cSize = new Size();
            // get coordinates relative to window
            GetWindowRect(hWnd, out pRect);

            cSize.Width = pRect.Right - pRect.Left;
            cSize.Height = pRect.Bottom - pRect.Top;

            return cSize;
        }       
        /* --------------------------------------------------------------------------*/
        /* Placement du Processus */
        private static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
        {
                WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                placement.length = Marshal.SizeOf(placement);
                GetWindowPlacement(hwnd, ref placement);
                return placement;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(
            IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }
        internal enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }
        /* -------------------------------------------------------------*/

        /*verification processus si est actif */

        //public bool ApplicationIsActivated(Process proc, string window_name)
        //{
        //    int processID;
        //    GetWindowThreadProcessId(GetForegroundWindow(), out processID);
        //    Process processToCheck = Process.GetProcessById(processID);
        //    if (proc.Id == processToCheck.Id && processToCheck.MainWindowTitle == window_name)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        public static bool ApplicationIsActivated(Process proc,string window_name)
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = proc.Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);
            Process processToCheck = Process.GetProcessById(activeProcId);
            return (activeProcId == procId && processToCheck.MainWindowTitle == window_name);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MyLabel_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            string format = "yyyy-MM-dd HH:mm:ss";
            LastUserConnection usr = new LastUserConnection();
            if (radioButton2.Checked == true)
            {
                List<Utilisateur> onlyMe = new List<Utilisateur>();
                onlyMe.Add(Utilisateur.getUser(new LastUserConnection().getNomUtil()));
                ConfidentialiteFichier conf = new ConfidentialiteFichier(onlyMe, new List<Groupe>()); ;
                RecivedFiles f = new RecivedFiles(Path.GetFileName(pathMyProcess), pathMyProcess, textBox1.Text, usr.getNomUtil(), time.ToString(format), conf);
                BiBFilesXML bib = new BiBFilesXML();
                bib.ajouter(f);
                }
            else if (radioButton1.Checked == true)
            {
                Groupe p = new Groupe("Public", Utilisateur.getAllUsers());
                List<Groupe> lg = new List<Groupe>();
                lg.Add(p);
                ConfidentialiteFichier conf = new ConfidentialiteFichier(new List<Utilisateur>(), lg);
                RecivedFiles f = new RecivedFiles(Path.GetFileName(pathMyProcess), pathMyProcess, textBox1.Text, usr.getNomUtil(), time.ToString(format), conf);
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
                    RecivedFiles f = new RecivedFiles(Path.GetFileName(pathMyProcess), pathMyProcess, textBox1.Text, usr.getNomUtil(), time.ToString(format), conf);
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
                    RecivedFiles f = new RecivedFiles(Path.GetFileName(pathMyProcess), pathMyProcess, textBox1.Text, usr.getNomUtil(), time.ToString(format), conf);
                    BiBFilesXML bib = new BiBFilesXML();
                    bib.ajouter(f);
                    bib.setEtatToUpload(f);
                    }
            }
        }      
        /* -------------------------------------------------------------*/ 
    }
}
