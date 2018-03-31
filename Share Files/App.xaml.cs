using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Shell;
using MyClasses;
using EnvoieDeFichiers;

namespace Share_Files
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "Share Files";

        [STAThread]
        public static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            LastUserConnection usr = new LastUserConnection();
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                if (usr.getNomUtil() == null)
                {
                    System.Windows.Forms.Application.Run(new Login.Form1());
                }
                if (usr.getNomUtil() != null)
                {
                    if (args.Length > 1)
                    {
                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Environment.CurrentDirectory);
                        Envoie_De_Fichiers f = new Envoie_De_Fichiers(di.ToString(), args[1]);
                        f.Show();
                    }
                    var application = new App();
                    application.InitializeComponent();
                    application.Run();
                    // Allow single instance code to perform cleanup operations
                    SingleInstance<App>.Cleanup();
                }
            }
            else
            {
                if (usr.getNomUtil() == null)
                {
                    System.Windows.Forms.Application.Run(new Login.Form1());
                }
                if (usr.getNomUtil() != null)
                {
                    if (args.Length > 1)
                    {
                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Environment.CurrentDirectory);
                        System.Windows.Forms.Application.Run(new Envoie_De_Fichiers(di.ToString(), args[1]));
                    }
                }
            }
        }

        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // handle command line arguments of second instance
            // …

            return true;
        }

        #endregion
    }
}