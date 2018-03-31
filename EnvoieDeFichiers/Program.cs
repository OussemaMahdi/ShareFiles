using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MyClasses;

namespace EnvoieDeFichiers
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LastUserConnection usr = new LastUserConnection();
            if(usr.getNomUtil()==null)
            {
                Application.Run(new Login.Form1());
            }
            if (usr.getNomUtil() != null)
            {
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                {
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Environment.CurrentDirectory);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Envoie_De_Fichiers(di.ToString(), args[1]));
                }
            }
        }
    }
}
