using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using System.Runtime.InteropServices;
using Etiquette;
using System.Windows.Forms;
using System.Drawing;
using MyClasses;
using System.Data;
using System.Windows.Interop;
using System.IO;
using Microsoft.Shell;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace Share_Files 
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<RecivedFiles> MyFiles = new List<RecivedFiles>();
        private static string nomUtilisateur = new LastUserConnection().getNomUtil();
        private static string LocalNetworkShare = new LastPcServerName().getNamePcServer();

        private NotifyIcon notifyIcon = new NotifyIcon();
        private System.Windows.Threading.DispatcherTimer DownloaderTimer = new System.Windows.Threading.DispatcherTimer();

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                if (Buff.ToString() != "Program Manager" && Buff.ToString() != "MyLabel" && Buff.ToString() != "MainWindow")
                    return Buff.ToString();
            }
            return null;
        }        

        private bool testExt(string ext)
        {
            ext = ext.ToLower();
            return (ext == ".txt" || ext == ".pdf" || ext == ".doc" || ext == ".docx");
        }
        public string getPathFromProcess(MyProcess myProcess)
        {
            var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine, Name FROM Win32_Process where ProcessId='" + myProcess.getProcess().Id + "'";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                var query = from p in Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int)(uint)mo["ProcessId"]
                            select new
                            {
                                CommandLine = (string)mo["CommandLine"],
                            };
                string path = null;
                try
                {
                    string words = query.ElementAt(0).ToString();
                    string[] split = words.Split(new Char[] { '"', ',', '{', '}', '\n' });
                    foreach (string itm in split)
                    {
                        try
                        {
                            string ext = "";
                            path = itm;
                            ext = System.IO.Path.GetExtension(path);

                            while (path[0] == ' ')
                            {
                                path = itm.Remove(0, 1);
                                ext = System.IO.Path.GetExtension(path);
                            }
                            while (path[path.Length - 1] == ' ')
                            {
                                path = path.Remove(path.Length - 1, 1);
                                ext = System.IO.Path.GetExtension(path);
                            }
                            if (File.Exists(path) && testExt(ext))
                                return path;
                        }
                        catch { }
                    }
                }
                catch
                {
                    path = null;
                }
            }
            return null;
        }

        /* --------- Connection Verif ----------------- */
        private WebConnectionState webConnectionState = WebConnectionState.None;

        private bool IsOnline()
        {
            try{
                IPHostEntry dummy = Dns.GetHostEntry("www.google.com"); //using System.Net;
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static bool PingHost()
        {
            bool pingable = false;
            
            try
            {
                
                pingable = Directory.Exists(LocalNetworkShare);
            }
            catch
            {
                // Discard PingExceptions and return false;
            }
            return pingable;
        }
       /*-------------------------------------------------*/

        /* --------------------------- Synchronisation ---------------------------- */
        private void SynchronizeAll()
        {
            if (webConnectionState == WebConnectionState.InternetAndLocal)
            {
                SynchronizeDBuser();
                DownloadNewfilesInfoFromDropBoxAndServer();
                UploadfilesToDropBoxAndServer();
            }
            else if (webConnectionState == WebConnectionState.InternetOnly)
            {
                SynchronizeDBuser();
                DownloadNewfilesInfoFromDropBox();
                UploadfilesToDropBox();
            }
            else if (webConnectionState == WebConnectionState.LocalNetworkOnly)
            {
                DownloadNewfilesInfoFromServer();
                UploadfilesToServer();
            }
            refreshListeOfFiles();
        }

        private void SynchronizeDBuser()
        {
            try
            {
                ConnectionDB conn = ConnectionDB.getInstanceForSync();
                string req = "select users from Synchronization WHERE nomutilisateur='" + new LastUserConnection().getNomUtil() + "'";
                if (conn.selectionner(req).Tables[0].Rows[0][0].ToString() == "True")
                {
                    req = "select * from Utilisateur";
                    Utilisateur.creeBibUsers();
                    foreach (DataRow dr in conn.selectionner(req).Tables[0].Rows)
                    {
                        new Utilisateur(dr[0].ToString(), dr[1].ToString(), dr[2].ToString()).addToXML();
                    }
                    req = "UPDATE Synchronization SET users = 'False' WHERE nomutilisateur='" + new LastUserConnection().getNomUtil() + "'";
                    conn.executer(req);
                }
            }
            catch { }
        }
        private void GetLocalNetworkShare()
        {
            try
            {
                ConnectionDB conn = ConnectionDB.getInstanceForSync();
                string req = "select setting from Synchronization WHERE nomutilisateur='" + new LastUserConnection().getNomUtil() + "'";
                string s = conn.selectionner(req).Tables[0].Rows[0][0].ToString();
                if (conn.selectionner(req).Tables[0].Rows[0][0].ToString() == "True")
                {
                    LastPcServerName lpsn = new LastPcServerName();
                    ConnectionDB cnn = ConnectionDB.getInstanceForSync();
                    req = "select * from Serveur";
                    DataSet ds = cnn.selectionner(req);
                    lpsn.ajouterNamePcServer(ds.Tables[0].Rows[0][0].ToString());
                    LocalNetworkShare = lpsn.getNamePcServer();
                }
                req = "UPDATE Synchronization SET setting = 'False' WHERE nomutilisateur='" + new LastUserConnection().getNomUtil() + "'";
                conn.executer(req);
            }
            catch { }
        }

        private void DownloadNewfilesInfoFromServer()
        {
            try
            {
                bool haveNews = false;
                BiBFilesXML bib = new BiBFilesXML();
                List<RecivedFiles> lrf = bib.getMyFilesFromServer(LocalNetworkShare);
                List<RecivedFiles> MyFiles = bib.getAllFiles();
                foreach (RecivedFiles rf in lrf)
                {
                    string d = rf.dateRecept.ToString().Replace(':', '+').Replace('/', '+');
                    if (rf.nom.IndexOf(d) == 0)
                        rf.setNom(rf.nom.Remove(0, rf.dateRecept.ToString().Length));
                    if (bib.isExistAndNotInServer(rf))
                    {
                        bib.setFilIsInServer(rf);
                        bib.setPathOfSave(rf);
                        haveNews = true;
                    }
                    else if (!MyFiles.Contains(rf))
                    {
                        bib.ajouter(rf);
                        bib.setFilIsInServer(rf);
                        bib.setEtatToDownload(rf);
                        haveNews = true;
                    }
                }
                if (haveNews)
                {
                    notifyIcon.ShowBalloonTip(5000, "MyDocs Notification", "You Have new files in share folder", ToolTipIcon.Info);
                }
            }
            catch { }
        }
        private void DownloadNewfilesInfoFromDropBox()
        {
            try
            {
                bool haveNews = false;
                ConnectionDB conn = ConnectionDB.getInstanceForSync();
                string req = "select files from Synchronization WHERE nomutilisateur='" + new LastUserConnection().getNomUtil() + "'";
                if (conn.selectionner(req).Tables[0].Rows[0][0].ToString() == "True")
                {

                    List<RecivedFiles> lrf = SynchronisationAvecBD.getReciveFileFromDB();
                    BiBFilesXML bib = new BiBFilesXML();
                    List<RecivedFiles> MyFiles = bib.getAllFiles();
                    foreach (RecivedFiles rf in lrf)
                    {
                        
                        string d = rf.dateRecept.ToString().Replace(':', '+').Replace('/', '+');
                        if (rf.nom.IndexOf(d) == 0)
                            rf.setNom(rf.nom.Remove(0, rf.dateRecept.ToString().Length));
                        if (bib.isExistAndNotInDropbox(rf))
                        {
                            bib.setFilIsInDropbox(rf);
                            haveNews = true;
                        }

                        else if (!MyFiles.Contains(rf))
                        {
                            bib.ajouter(rf);
                            bib.setFilIsInDropbox(rf);
                            bib.setEtatToDownload(rf);
                            haveNews = true;
                        }
                        
                    }
                    if (haveNews)
                    {
                        notifyIcon.ShowBalloonTip(5000, "MyDocs Notification", "You Have new files in DropBox", ToolTipIcon.Info);
                        req = "UPDATE Synchronization SET files = 'False' WHERE nomutilisateur='" + new LastUserConnection().getNomUtil() + "'";
                        conn.executer(req);
                    }                   
                }              
            }
            catch { }
        }
        private void DownloadNewfilesInfoFromDropBoxAndServer()
        {
            DownloadNewfilesInfoFromDropBox();
            DownloadNewfilesInfoFromServer();
        }

        private void UploadfilesToServer()
        {
            BiBFilesXML bib = new BiBFilesXML();
            List<RecivedFiles> lf = bib.getFilesToUploadToServer();
            foreach (RecivedFiles rf in lf)
            {
                string originalName = rf.getNom();
                try
                {
                    bib.setFilIsInServer(rf);
                    bib.saveFileInBibServer(LocalNetworkShare, rf);
                }
                catch
                { bib.setFilIsNotInServer(rf); }

                if (bib.FilIsInServer(rf) && bib.FilIsInDropbox(rf))
                { bib.setEtatSynchronized(rf); }
            }
        }
        private void UploadfilesToDropBox()
        {
            BiBFilesXML bib = new BiBFilesXML();
            List<RecivedFiles> lf = bib.getFilesToUploadToDropbox();
            foreach (RecivedFiles rf in lf)
            {
                string originalName = rf.getNom();
                try
                {
                    rf.addToDB();
                    try
                    {
                        ConnectionDropbox.DeposeFile(rf);
                        rf.setNom(originalName);
                        bib.setFilIsInDropbox(rf);
                    }
                    catch { rf.setNom(originalName); rf.supprimerFromDB(); bib.setFilIsNotInDropbox(rf); }
                }
                catch { bib.setFilIsNotInDropbox(rf); }
                if (bib.FilIsInServer(rf) && bib.FilIsInDropbox(rf))
                { bib.setEtatSynchronized(rf); }
            }

        }
        private void UploadfilesToDropBoxAndServer()
        {
            UploadfilesToServer();
            UploadfilesToDropBox();
        }

        public void refreshEtatNetwork()
        {
            webConnectionState = WebConnectionState.None;

            bool internetisok = false;

            if (NetworkInterface.GetIsNetworkAvailable() && IsOnline())
            {
                internetisok = true;
                GetLocalNetworkShare();
            }

            bool localisok = false;

            if (NetworkInterface.GetIsNetworkAvailable() && PingHost())
            {
                localisok = true;
            }
            webConnectionState = internetisok ? (localisok ? WebConnectionState.InternetAndLocal :
                WebConnectionState.InternetOnly) :
                 (localisok ? WebConnectionState.LocalNetworkOnly :
                WebConnectionState.None);
        }

        void OnNetworkChange(object sender, EventArgs e)
        {
            refreshEtatNetwork();
        }

        private void Downloader_Tick(object sender, EventArgs e)
        {
            refreshEtatNetwork();
            try
            {
                switch (webConnectionState)
                {
                    case WebConnectionState.None:
                        imgintV.Visibility = Visibility.Hidden;
                        imglocV.Visibility = Visibility.Hidden;
                        imgintR.Visibility = Visibility.Visible;
                        imglocR.Visibility = Visibility.Visible;
                        break;
                    case WebConnectionState.InternetAndLocal:
                        imgintV.Visibility = Visibility.Visible;
                        imglocV.Visibility = Visibility.Visible;
                        imgintR.Visibility = Visibility.Hidden;
                        imglocR.Visibility = Visibility.Hidden;
                        break;
                    case WebConnectionState.InternetOnly:
                        imgintV.Visibility = Visibility.Visible;
                        imglocV.Visibility = Visibility.Hidden;
                        imgintR.Visibility = Visibility.Hidden;
                        imglocR.Visibility = Visibility.Visible;
                        break;
                    case WebConnectionState.LocalNetworkOnly:
                        imgintV.Visibility = Visibility.Hidden;
                        imglocV.Visibility = Visibility.Visible;
                        imgintR.Visibility = Visibility.Visible;
                        imglocR.Visibility = Visibility.Hidden;
                        break;
                }
            }
            catch { }
            SynchronizeAll();
        }
        /* ------------------------------------------------------------------------- */

        public void refreshListeOfFiles()
        {
            listedefichiers.Items.Clear();
            MyFiles = new BiBFilesXML().getFilesToDownload();
            MyFiles.Sort();
            foreach (RecivedFiles rf in MyFiles)
            { listedefichiers.Items.Add(rf); }
        }
        public MainWindow()
        {
            InitializeComponent();
            

            NetworkChange.NetworkAddressChanged += OnNetworkChange;
            OnNetworkChange(null, null);
            refreshListeOfFiles();
        }


        /* ----------------------- raccourci clavier ------------------------------- */
        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private HwndSource _source;
        private const int HOTKEY_ID = 9000;
        private const int HOTKEY_IDForSerch = 9001;      
        
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
            RegisterHotKeyForSarch();
        }
        protected override void OnClosed(EventArgs e)
        {
            try
            {
                _source.RemoveHook(HwndHook);
                _source = null;
                UnregisterHotKey();
                UnregisterHotKeyForSarch();
                base.OnClosed(e);
            }
            catch{ base.OnClosed(e);}
        }
        
        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            const uint VK_N = 0x4e;
            const uint MOD_CTRL = 0x0002;
            const uint MOD_SHIFT = 0x0004;
            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL + MOD_SHIFT, VK_N))
            {
                // handle error
            }
        }
        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        private void RegisterHotKeyForSarch()
        {
            var helper = new WindowInteropHelper(this);
            const uint VK_R = 0x52;
            const uint MOD_CTRL = 0x0002;
            const uint MOD_SHIFT = 0x0004;
            if (!RegisterHotKey(helper.Handle, HOTKEY_IDForSerch, MOD_CTRL + MOD_SHIFT, VK_R))
            {
                // handle error
            }
        }
        private void UnregisterHotKeyForSarch()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_IDForSerch);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            OnHotKeyPressed();
                            handled = true;
                            break;
                        case HOTKEY_IDForSerch:
                            OnHotKeyPressedForSarch();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressed()
        {
            string curentWindow=GetActiveWindowTitle();
            MyProcess p = new MyProcess(curentWindow);
            if (curentWindow != null && Etiquette.BibProcess.getProcessByWindow(curentWindow) == null&&p.existProcess()&& getPathFromProcess(p)!=null)
            {
            MyLabel l = new MyLabel(curentWindow, getPathFromProcess(p));
            l.Show();
            }
        }
        private void OnHotKeyPressedForSarch()
        {
            Recherche.MainWindow w=new Recherche.MainWindow();
            w.Topmost = true;
            w.Show();
        }

        /* --------------------------------------------------------------------------------- */

        public System.Windows.Forms.ContextMenu contextMenu1 = new System.Windows.Forms.ContextMenu();
        public void creeContextMenu()
        {
            System.Windows.Forms.MenuItem mnuItemClose = new System.Windows.Forms.MenuItem();       
            mnuItemClose.Text = "&Close Application";
            mnuItemClose.Click += new System.EventHandler(this.mnuItemClose_Click);
            contextMenu1.MenuItems.Add(mnuItemClose);
        }
        private void mnuItemClose_Click(object sender, System.EventArgs e)
        {
            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Want to Close Application?", "Close Application", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                this.Close();
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;           

            DownloaderTimer.Tick += new EventHandler(Downloader_Tick);
            DownloaderTimer.Interval = new TimeSpan(0,0,3,0);
            DownloaderTimer.Start();

            Icon myIcon = Share_Files.Properties.Resources.iconpro;
            notifyIcon.Click += new EventHandler(notifyIcon_Click);
            notifyIcon.Visible = true;
            notifyIcon.Icon = myIcon;
            creeContextMenu();
            notifyIcon.ContextMenu = contextMenu1;

            notifyIcon.BalloonTipClicked += new EventHandler(notifyIcon_BalloonTipClicked);
            notifyIcon.ShowBalloonTip(5000, "MyDocs", "The application start", ToolTipIcon.Info);

            LastUserConnection usr = new LastUserConnection();
            UtilConnec.Content = usr.getNomUtil();
    
        }
        private void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            this.Show();
            this.Topmost = true;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
            this.Hide();
        }
        private void notifyIcon_Click(object sender, EventArgs e)
        {
            this.Show();
            this.Topmost = true;
        }        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
                Login.Form1 f = new Login.Form1();
                f.Show();
        }
               
        private void listedefichiers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listedefichiers.SelectedItem != null)
            {
                RecivedFiles item = (RecivedFiles)listedefichiers.SelectedItem;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save an File";
                string extension = System.IO.Path.GetExtension(item.getNom());
                saveFileDialog.DefaultExt = extension;
                saveFileDialog.FileName = item.getNom();
                saveFileDialog.Filter = "(*" + extension + ")|*" + extension;
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string destFile = saveFileDialog.FileName;
                    string pathOfFile = item.getPathOfSave();
                    BiBFilesXML bib = new BiBFilesXML();
                    // Save here
                    switch (webConnectionState)
                    {
                        case WebConnectionState.None:
                            System.Windows.MessageBox.Show("Please connect you");
                            break;
                        case WebConnectionState.InternetAndLocal:
                            try
                            {
                                if (bib.FilIsInServer(item))
                                {
                                    //if (File.Exists(pathOfFile))
                                    //{
                                        File.Copy(pathOfFile, destFile, true);
                                        item.setPathOfSave(destFile);
                                        bib.setPathOfSave(item);
                                        bib.setEtatSynchronized(item);
                                    //}
                                    //else
                                    //{
                                    //    System.Windows.MessageBox.Show("this file is deleted or renamed", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                                    //    bib.deleteFile(item);
                                    //}
                                }
                                else if (bib.FilIsInDropbox(item))
                                {
                                    ConnectionDropbox.DownloadFile(item,destFile);
                                    item.setPathOfSave(destFile);
                                    bib.setPathOfSave(item);
                                    bib.setEtatSynchronized(item);
                                }
                            }
                            catch
                            {
                                item.setPathOfSave(pathOfFile);
                                bib.setEtatToDownload(item);
                            }
                            break;
                        case WebConnectionState.LocalNetworkOnly:
                            try
                            {
                                if (bib.FilIsInServer(item))
                                {
                                    //if (File.Exists(pathOfFile))
                                    //{
                                        File.Copy(pathOfFile, destFile, true);
                                        item.setPathOfSave(destFile);
                                        bib.setPathOfSave(item);
                                        bib.setEtatSynchronized(item);
                                    //}
                                    //else
                                    //{
                                    //    System.Windows.MessageBox.Show("this file is deleted or renamed", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                                    //    bib.deleteFile(item);
                                    //}
                                }
                                else if (bib.FilIsInDropbox(item))
                                {
                                    System.Windows.MessageBox.Show("Connect to Internet please","Alert",MessageBoxButton.OK,MessageBoxImage.Information);
                                }
                            }
                            catch
                            {
                                item.setPathOfSave(pathOfFile);
                                bib.setEtatToDownload(item);
                            }
                            break;
                        case WebConnectionState.InternetOnly:
                            try
                            {
                                if (bib.FilIsInDropbox(item))
                                {
                                    ConnectionDropbox.DownloadFile(item,destFile);
                                    item.setPathOfSave(destFile);
                                    bib.setPathOfSave(item);
                                    bib.setEtatSynchronized(item);
                                }
                                else if (bib.FilIsInServer(item))
                                {
                                    System.Windows.MessageBox.Show("Connect to Local Network please", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                            catch
                            {
                                item.setPathOfSave(pathOfFile);
                                bib.setEtatToDownload(item);
                            }
                            break;
                    }
                }
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Want to Disconnect?", "Disconnect", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                LastUserConnection usr = new LastUserConnection();
                usr.deconnecter();
                this.Close();
            }
        }
    }

    enum WebConnectionState
    {
        None,
        InternetOnly,
        LocalNetworkOnly,
        InternetAndLocal
        
    }
}
