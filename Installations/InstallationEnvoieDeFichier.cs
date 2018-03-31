using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace Installations
{
    [RunInstaller(true)]
    public partial class InstallationEnvoieDeFichier : System.Configuration.Install.Installer
    {
        public InstallationEnvoieDeFichier()
        {
            InitializeComponent();
        }
        public override void Install(IDictionary stateSaver)
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"*\shell", true);
                key = key.CreateSubKey("MyDocs");
                key.SetValue("Icon", @"C:\Program Files (x86)\FSS Projects\MyDocs\icon.ico");
                key.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\r\nUnable to create the example key: {0}", ex);
            }

            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"*\shell\MyDocs\", true);
                key = key.CreateSubKey("Command");
                key.SetValue("", @"C:\Program Files (x86)\FSS Projects\MyDocs\Share files.exe %1");

                //key.SetValue("",System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+@"\sign.exe");
                key.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\r\nUnable to create the example key: {0}", ex);
            }
        }
        public override void Uninstall(IDictionary savedState)
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"*\shell\MyDocs", true);
                key.DeleteSubKey("Command");
                key.Close();
            }
            catch (ArgumentException)
            {
                // ArgumentException is thrown if the key does not exist. In 
                // this case, there is no reason to display a message.
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to delete the example key: {0}", ex);
                return;
            }
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"*\shell", true);
                key.DeleteSubKey("MyDocs");
                key.Close();
            }
            catch (ArgumentException)
            {
                // ArgumentException is thrown if the key does not exist. In 
                // this case, there is no reason to display a message.
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to delete the example key: {0}", ex);
                return;
            }
        }
    }
}
