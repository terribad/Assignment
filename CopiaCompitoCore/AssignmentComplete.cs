using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AssignmentCore
{
    public class AssignmentComplete
    {
        public event EventHandler<CompleteStartEventArgs> AssignmentCompleteStart;
        public event EventHandler<CompleteAnticipateStartEventArgs> AssignmentCompleteAnticipateStart;
        public event EventHandler<CompleteRequestCloseTargetEventArgs> AssignmentRequestCloseTarget;
        public event EventHandler AssignmentCompleteCompleted;
        public event EventHandler<AssignmentErrorEventArgs> AssignmentCompleteError;
        public event EventHandler AssignmentCompleteAborted;

        readonly FileTaskManager fileManager;
        readonly Config config;
        public AssignmentComplete(Config config)
        {
            this.fileManager = new FileTaskManager(config);
            this.config = config;
        }

        public void Execute()
        {
            try
            {
                #if DEBUG  //evita warning su consegna anticipata
                    Global.Time = new FakeTimeProvider(Global.RecentSpanTimeForComplete + 5);
                #endif

                if (!fileManager.TargetExists(config.ProjectFullName))
                {
                    AssignmentCompleteError.Fire(this, AssignmentErrorEventArgs.New(AssignmentErrorType.ProjectTargetNoFound));
                    return;
                }

                if (IsAnticipateComplete())
                {
                    var e = new CompleteAnticipateStartEventArgs();
                    AssignmentCompleteAnticipateStart.Fire(this, e);
                    if (e.Abort)
                        return;
                }
                
                if (ProcessManager.IsExecuting(config.ProjectTarget.Target))
                {
                    var e = new CompleteRequestCloseTargetEventArgs() { TargetInfo = config.ProjectTarget };
                    AssignmentRequestCloseTarget.Fire(this, e);
                    if (e.Abort)
                        return;
                }
                else
                {
                    var e = new CompleteStartEventArgs { ProjectName = config.ProjectName };
                    AssignmentCompleteStart.Fire(this, e);
                    if (e.Abort)
                    {
                        AssignmentCompleteAborted.Fire(this);
                        return;
                    }
                }
                
                CloseOpenWindows();

                _Execute();
                AssignmentCompleteCompleted.Fire(this);
            }
            catch (Exception e)
            {
                AssignmentCompleteError.Fire(this, AssignmentErrorEventArgs.New(AssignmentErrorType.UnknowError, e));
            }
            finally
            {
                Global.Time = new DefaultTimeProvider();
            }
        }

        private bool IsAnticipateComplete()
        {
            return fileManager.IsRecent(Global.RecentSpanTimeForComplete, config.ProjectFullName);
        }

        private void _Execute()
        {
            CopyOnShared();
            //TODO: cancellazione del progetto (utile se si presuppone che progetto esista fuori da cartelle clear)            
            Trash.Clean();

            //!NOTE: solleva eccezione con Vs2017 peché non riesce a cancellare cartella .vs
            //TODO: bypassare l'eliminazione della cartella ".VS" 
            // (lasciando che sia la sessione start dell'utente successivo a eliminarla)
            fileManager.ClearFolders(config.ClearFolders);
            

            //!NOTE: soluzione al problema della cartella ".vs": silenziare l'eccezione!
            try
            {
                fileManager.ClearFolders(config.ClearFolders);
            }
            catch
            {

            }

        }

        public void CopyOnShared()
        {
            CopyProject(config.ProjectFullName, Global.TempFolder, config.ProjectName);
            CopyProject(config.ProjectFullName, config.DestPath, config.ProjectName);
        }

        private void CopyProject(string source, string dest, string nameAs)
        {
            if (config.ProjectTarget.TargetType == TargetType.Folder)
                fileManager.CopyFolderTo(source, dest, GetFinalFolderName(nameAs), true);
            else
            {
                var destPath = Path.Combine(dest, GetFinalFileName(nameAs));
                fileManager.CopyFile(source, destPath);
            }
        }

        public string GetFinalFolderName(string name)
        {
            return string.Format("{0}-{1:HH.mm} [{2}]", name, Global.Time.Now(), Environment.MachineName); 
        }

        public string GetFinalFileName(string name)
        {
            return string.Format("{0}-{1:HH.mm} [{2}]{3}", Path.GetFileNameWithoutExtension(name), Global.Time.Now(), Environment.MachineName, Path.GetExtension(name));
        }

        private void CloseOpenWindows()
        {
            ProcessManager.CloseOpenWindows(AssignmentManager.SkipProcessToClose, true);
        }
    }

    public class CompleteStartEventArgs : EventArgs
    {
        public string ProjectName;
        public bool Abort = false;
    }

    public class CompleteAnticipateStartEventArgs : EventArgs
    {
        public bool Abort = false;
    }

    public class CompleteRequestCloseTargetEventArgs : EventArgs
    {
        public ProjectTargetInfo TargetInfo;
        public bool Abort = false;
    }
}
