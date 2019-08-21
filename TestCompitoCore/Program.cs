using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Reflection;

using AssignmentCore;
namespace TestCompitoCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //foreach (var p in ProcessManager.GetOpenWindows())
            //{
            //    var title = p.MainWindowTitle.Length > 20 ? p.MainWindowTitle.Substring(0, 20) : p.MainWindowTitle;
            //    Console.WriteLine("{0, -25} {1} ", p.ProcessName, title);
            //}

            ////ProcessManager.CloseExplorer();

            //return;
            if (ProcessManager.IsRunning())
                return;

            if (!Global.IsAssignmentAccount)
                return;

            VirtualPath.SetDefaultTemplates();

            Config config = new Config(@"Config.ini");
            if (config.AssignmentMode == AssignmentMode.Start)
            {
                var ast = new AssignmentStart(config);
                ast.AssignmentRequestProjectName += am_AssignmentRequestProjectName;
                ast.AssignmentStartError += am_AssignmentStartError;
                ast.AssignmentStartCompleted += am_AssignmentStartCompleted;
                ast.AssignmentStartAborted += am_AssignmentStartAborted;
                ast.Execute();
            }
            else
            {
                var ac = new AssignmentComplete(config);
                ac.AssignmentCompleteError += ac_AssignmentCompleteError;
                ac.AssignmentCompleteCompleted += ac_AssignmentCompleteCompleted;                
                ac.Execute();
            }
        }

        static void ac_AssignmentCompleteCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("Consegna del compito effettuata.");
        }

        static void ac_AssignmentCompleteError(object sender, AssignmentErrorEventArgs e)
        {
            Console.WriteLine("Errore durante la consegna del compito: {0}", e.ErrorType);
            if (e.Error != null)
                Console.WriteLine("{0}", e.Error.Message);
            Console.ReadKey();
        }
       

        static void am_AssignmentStartAborted(object sender, EventArgs e)
        {
            Console.WriteLine("Start compito annullato!");
            Console.ReadKey();
        }

        static void am_AssignmentStartCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("Start compito completato.");
        }

        static void am_AssignmentStartError(object sender, AssignmentErrorEventArgs e)
        {
            Console.WriteLine("Si è verificato un errore: {0}", e.ErrorType);
            if (e.Error != null)
                Console.WriteLine("{0}", e.Error.Message);
        }

        static void am_AssignmentRequestProjectName(object sender, ProjectNameRequestEventArgs e)
        {
            e.SelectedProjectName = e.ProjectNames[0];
            e.NameAs = "Rossi";
            return;
            Console.Write("Nome: ");
            string name = Console.ReadLine();
            if (name == "")
            {
                e.Abort = true;
                return;
            }
            e.NameAs = name;
            
        }
    }
}
