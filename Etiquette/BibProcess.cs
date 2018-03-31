using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Etiquette
{
    public static class BibProcess
    {
        private static ArrayList bibProcess= new ArrayList();

        public static void ajouter(MyProcess p)
        {
            bibProcess.Add(p);
        }

        public static void suprimer(MyProcess p)
        {
            bibProcess.Remove(p);
        }

        public static MyProcess getProcessByWindow(string window_name)
        {
        foreach(MyProcess proc in bibProcess)
        {
            if (proc.getProcess().MainWindowTitle == window_name)
                return proc;            
        }
        return null;
        }
    }
}
