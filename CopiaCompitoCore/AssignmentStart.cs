using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace AssignmentCore
{
    public class AssignmentStart : IAssignmentStart
    {
        public event EventHandler<ProjectNameRequestEventArgs> AssignmentRequestProjectName;
        public event EventHandler<CheckDoubleStartEventArgs> AssignmentCheckDoubleStart;
        public event EventHandler AssignmentStartCompleted;
        public event EventHandler<AssignmentErrorEventArgs> AssignmentStartError;
        public event EventHandler AssignmentStartAborted;

        readonly FileTaskManager fileManager;
        readonly Config config;
        public AssignmentStart(Config config)
        {
            this.fileManager = new FileTaskManager(config);
            this.config = config;
        }
        
        public void Execute()
        {
            try
            {
                #if DEBUG  //evita warning su consegna anticipata
                    Global.Time = new FakeTimeProvider(Global.RecentSpanTimeForStart + 5);
                #endif
                var projectNames = fileManager.GetProjectNames();
                if (projectNames.Length == 0)
                {
                    AssignmentStartError.Fire(this, AssignmentErrorEventArgs.New(AssignmentErrorType.ProjectTargetNoFound));
                    return;
                }

                if (IsDoubleStart())
                {
                    var e = new CheckDoubleStartEventArgs();
                    AssignmentCheckDoubleStart.Fire(this, e);
                    if (e.Abort)
                        return;
                }

                var ea = new ProjectNameRequestEventArgs() { ProjectNames = projectNames, SelectedProjectName = projectNames[0] };
                AssignmentRequestProjectName.Fire(this, ea);
                if (ea.Abort)
                {
                    AssignmentStartAborted.Fire(this);
                    return;
                }
                
                ProcessManager.CloseOpenWindows(AssignmentManager.SkipProcessToClose, forceClose:true);
                Log.Write(LogLevel.Verbose, "START: chiuse finestre");

                _Execute(ea.SelectedProjectName, ea.NameAs);

                AssignmentStartCompleted.Fire(this);
            }
            catch(Exception e)
            {
                AssignmentStartError.Fire(this, AssignmentErrorEventArgs.New(AssignmentErrorType.UnknowError, e));
            }
            finally
            {
                Global.Time = new DefaultTimeProvider();
                Log.Close();
            }
        }


        private bool IsDoubleStart()
        {
            if (Global.IsDevelopAccount && config.CheckDoubleAssignmentStart == false)
                return false;
            bool doubleStart = fileManager.IsRecent(Global.RecentSpanTimeForStart, config.TempExecutablePath);            
            return doubleStart;

        }
       
        private void _Execute(string projectName, string nameAs)
        {
            Trash.Clean();
            Log.Write(LogLevel.Verbose, "START: svuotato cestino");

            
            if (!Directory.Exists(config.DestPath))
                Directory.CreateDirectory(config.DestPath);

            fileManager.ClearFolders(config.ClearFolders);
            Log.Write(LogLevel.Verbose, "START: pulite cartelle");
            
            //!ridondante
            //ProcessManager.CloseWindow(nameAs);



            CopyProject(projectName, nameAs);
            Log.Write(LogLevel.Verbose, "START: copiato progetto");
            
            var pathExe = CopyExecutable();
            fileManager.CreateLink(Global.LinkToExecutable, pathExe, string.Join(" ", Environment.GetCommandLineArgs()));
            Log.Write(LogLevel.Verbose, "START: copiato eseguibile e creato link");

            config.SaveForCompleteAssignment(nameAs, Path.GetDirectoryName(pathExe));
            Log.Write(LogLevel.Verbose, "START: scritto file di configurazione");
            
            ExecuteOpen();
            Global.Sleep();

            if (config.ProjectTarget.TargetType == TargetType.Folder)
            {
                string path = Path.Combine(config.DestPath, nameAs);
                ProcessManager.Start(path);
            }
            else
                ProcessManager.Start(config.DestPath);
            Log.Write(LogLevel.Verbose, "START: aperta cartella progetto");
        }
        private void ExecuteOpen()
        {
            foreach (var path in config.OpenList)
            {
                Global.Sleep();
                ProcessManager.Start(path.Path);
                Log.Write(LogLevel.Verbose, "START: aperto {0}", path.Path);
            }
        }

        public void CopyProject(string sourceName, string nameAs)
        {
            //!è ridondante (poiché la cartella $projects viene pulita) ma è utile durante sviluppo!
            fileManager.DeleteFolder(Path.Combine(config.DestPath, nameAs));

            var projectPath = Path.Combine(config.SourcePath, sourceName);
            if (config.ProjectTarget.TargetType == TargetType.Folder)
                fileManager.CopyFolderTo(projectPath, config.DestPath, nameAs, true);
            else
            {
                var destPath = Path.Combine(config.DestPath, nameAs) + config.ProjectTarget.Ext;                
                fileManager.CopyFile(projectPath, destPath, true);
            }
        }

        private string CopyExecutable()
        {
            var pathExeDest = config.TempExecutablePath;
            var pathDLLDest = Path.Combine(Global.TempFolder, Global.SourceDLL);
            fileManager.CopyFile(ProcessManager.ExecutableName, pathExeDest);
            fileManager.CopyFile(Global.SourceDLL, pathDLLDest);
            fileManager.SetWriteTime(pathExeDest);
            return pathExeDest;
        }
    }
    public class ProjectNameRequestEventArgs:EventArgs
    {
        public string NameAs;
        public string[] ProjectNames;
        public string SelectedProjectName;
        public bool Abort = false;
    }

    public class CheckDoubleStartEventArgs : EventArgs
    {
        public bool Abort = false;
    }

   

   
}
