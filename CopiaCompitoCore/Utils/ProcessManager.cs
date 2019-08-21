using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;


namespace AssignmentCore
{
    public class ProcessManager
    {
        const string EXPLORER = "explorer";
        const string VISUALSTUDIO= "devenv";
        public static Process Start(string path, string args = "")
        {
            return Process.Start(path, args);
        }

        [DllImport("user32.dll")]
        static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        const int WM_SYSCOMMAND = 0x0112;
        const int SC_CLOSE = 0xF060;

        
        public static void CloseWindow(string winName)
        {
            // retrieve the handler of the window  
            int iHandle = FindWindow(null, winName);
            if (iHandle > 0)
            {
                // close the window using API        
                SendMessage(iHandle, WM_SYSCOMMAND, SC_CLOSE, 0);
            }
        }	

        public static string ExecutableName
        {
            get {return Assembly.GetEntryAssembly().GetName().Name + ".exe";}
        }

        public static bool IsExecuting(string name)
        {
            return Process.GetProcessesByName(name).Length > 0;
        }

        //NOTA: hasTitle attualmente non viene utilizzato
        public static IEnumerable<Process> GetOpenWindows(bool hasTitle = false)
        {
            var list = Process.GetProcesses().Where(p => p.MainWindowHandle != IntPtr.Zero);
            if (hasTitle)
                return list.Where(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle));
            return list;
        }

        public static void CloseOpenWindows(IReadOnlyList<string> skipProcessList = null, bool forceClose = false)
        {
            if (skipProcessList == null)
                skipProcessList = new string[0];

            var list = GetOpenWindows();
            list = list.Where(p => !skipProcessList.Contains(p.ProcessName.ToLower()));

            CloseWindows(list, forceClose);
        }

        public static void CloseWindows(IEnumerable<Process> processToClose, bool forceClose = false)
        {            
            int curId = Process.GetCurrentProcess().Id;
            foreach (var p in processToClose)
            {
                if (p.Id != curId)
                    CloseProcess(p, forceClose);                
            }
        }

        private static void CloseProcess(Process p, bool forceClose = false)
        {
            if (Global.IsDevelopAccount)
            {
                if (p.ProcessName.Eq(EXPLORER) || p.ProcessName.Eq(VISUALSTUDIO))
                {
                    Debug.WriteLine("Chiuso {0}", p.ProcessName);
                    return;
                }
                if (Global.IsLocalMachine)
                    return;
            }
            if (forceClose || p.MainWindowHandle == IntPtr.Zero)
                p.Kill();
            else
                p.CloseMainWindow();
        }

       
        
        public static bool IsRunning()
        {
            Process current = Process.GetCurrentProcess();
            var exeName = Assembly.GetEntryAssembly().GetName().Name;
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            
            foreach (Process process in processes)
            {               
                if (process.Id != current.Id)
                {
                    var procName = Path.GetFileNameWithoutExtension(current.ProcessName);
                    if (exeName == procName)
                        return true;
                }
            }
            return false;
        }

        
    }
}
