using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Etiquette
{
    public class MyProcess
    {
        private Process proc;
        private string windowName;

        public MyProcess (string window_name)
        {
            windowName = window_name;
            proc = GetMyProcess(windowName);
        }

        public Process getProcess()
        { return (proc); }

        private Process GetMyProcess(string Name_Window)
        {
            try
            {
                Process[] processlist = Process.GetProcesses();
                foreach (Process process in processlist)
                {
                    if (!String.IsNullOrEmpty(process.MainWindowTitle) && process.MainWindowTitle == Name_Window)
                    {
                        return (process);
                    }
                }
                return null;
            }
            catch
            { return null; }
        }

        public bool existProcess()
        { return proc != null; }
    }
}
