using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using AssignmentCore;

namespace GuiAssignment
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private LogLevel logLevel = LogLevel.Verbose;

        private void Form1_Load(object sender, EventArgs e)
        {
            if (ProcessManager.IsRunning() || !Global.IsAssignmentAccount)
            {
                Close();
                return;
            }

            ParseCommandArguments();
            bool deletePre = Global.IsDevelopAccount ? true : false;
            Log.Open(new DefaultTextLog(logLevel, deletePre:deletePre));

            VirtualPath.SetDefaultTemplates();
            Config config = null;
            try
            {
                config = new Config(@"Config.ini");
                //config = new Config(@"ConfigComplete.ini");
            }
            catch(Exception ex)
            {
                MessageDialog.Show("Errore nella produre di inizio!\n[" + ex.Message + "]\nChiama il professore!", MessageInfoType.Error);
                Log.Write(LogLevel.Error, "Lettura file di configurazione:\n({0})", ex.Message);
                Close();
                return;
            }

            if (config.AssignmentMode == AssignmentMode.Start)
            {
                var ast = new AssignmentStart(config);
                ast.AssignmentCheckDoubleStart += ast_AssignmentCheckDoubleStart;
                ast.AssignmentRequestProjectName += am_AssignmentRequestProjectName;
                ast.AssignmentStartError += am_AssignmentStartError;
                ast.AssignmentStartCompleted += am_AssignmentStartCompleted;
                ast.AssignmentStartAborted += am_AssignmentStartAborted;
                ast.Execute();
            }
            else
            {
                var ac = new AssignmentComplete(config);
                ac.AssignmentCompleteStart += ac_AssignmentCompleteStart;
                ac.AssignmentCompleteAnticipateStart += ac_AssignmentCompleteAnticipateStart;
                ac.AssignmentRequestCloseTarget += ac_AssignmentRequestCloseTarget;
                ac.AssignmentCompleteAborted += ac_AssignmentCompleteAborted;
                ac.AssignmentCompleteError += ac_AssignmentCompleteError;
                ac.AssignmentCompleteCompleted += ac_AssignmentCompleteCompleted;
                ac.Execute();
            }
            Close();
        }

        private void ParseCommandArguments()
        {
            string[] args = Environment.GetCommandLineArgs();
            foreach (var arg in args)
            {
                if (arg.Eq("-lognormal"))
                    logLevel = LogLevel.Normal;

                if (arg.Eq("-logverbose"))
                    logLevel = LogLevel.Verbose;
            }

        }

        void ac_AssignmentRequestCloseTarget(object sender, CompleteRequestCloseTargetEventArgs e)
        {
            Log.Write(LogLevel.Normal, "COMPLETE: consegna compito: richiesta chiusura applicazione");

            var msg = string.Format("[{0}] è ancora aperto.\nChiudilo prima di procedere!" +
                      "\nClicca Annulla per annullare operazione", e.TargetInfo.Description);
            var cmd = MessageDialog.Show(msg, MessageInfoType.Warning, true);
            if (cmd == DialogResult.Cancel)
                e.Abort = true;
        }

        void ast_AssignmentCheckDoubleStart(object sender, CheckDoubleStartEventArgs e)
        {
            Log.Write(LogLevel.Normal, "START: doppio avvio inizio compito");

            var msg = "Il compito è già cominciato\nSei sicuro di voler ricominciare?" +
                      "\n(Quanto già fatto sarà eliminato!)" +
                      "\nClicca Annulla per annullare operazione";
            var cmd = MessageDialog.Show(msg, MessageInfoType.Warning, true);
            if (cmd == DialogResult.Cancel)
                e.Abort = true;
        }

        void ac_AssignmentCompleteAborted(object sender, EventArgs e)
        {
            Log.Write(LogLevel.Normal, "COMPLETE: annullata consegna compito");
            MessageDialog.Show("Consegna compito interrotta!", MessageInfoType.Warning);
        }

        void ac_AssignmentCompleteStart(object sender, CompleteStartEventArgs e)
        {
            Log.Write(LogLevel.Normal, "COMPLETE: inizio consegna compito");
            string msg = string.Format("Consegna del compito:\n{0}", e.ProjectName);
            var cmd = MessageDialog.Show(msg, MessageInfoType.Warning, true);
            if (cmd != DialogResult.OK)
                e.Abort = true;
        }
        void ac_AssignmentCompleteAnticipateStart(object sender, CompleteAnticipateStartEventArgs e)
        {
            Log.Write(LogLevel.Normal, "COMPLETE: consegna anticipata");

            var msg = "Hai già finito?\nSe consegni adesso non potrai modificare il compito!" +
                      "\nClicca Annulla per annullare operazione";
            var cmd = MessageDialog.Show(msg, MessageInfoType.Warning, true);
            if (cmd == DialogResult.Cancel)
                e.Abort = true;
        }

        string CreateErrorText(AssignmentErrorEventArgs e)
        {
            string errorMsg = e.Error != null ? e.Error.Message : "";
            string msg = string.Format("[{0}]\n{1}", e.ErrorType.Message(), errorMsg);
            return msg + "\n\nChiama il professore!";
        }

        string CreateErrorLogText(AssignmentErrorEventArgs e)
        {
            string errorMsg = e.Error != null ? e.Error.Message : "";
            return string.Format("[{0}]\n{1}", e.ErrorType.Message(), errorMsg);            
        }

        void ac_AssignmentCompleteCompleted(object sender, EventArgs e)
        {
            Log.Write(LogLevel.Normal, "COMPLETE: consegna completata");
            MessageDialog.Show("Compito consegnato", MessageInfoType.Info);
        }

        void ac_AssignmentCompleteError(object sender, AssignmentErrorEventArgs e)
        {
            var msg = e.Error != null ? e.Error.Message : "";
            Log.Write(LogLevel.Error, "COMPLETE: {0}", CreateErrorLogText(e));
            MessageDialog.Show(CreateErrorText(e), MessageInfoType.Error);
        }


        void am_AssignmentStartAborted(object sender, EventArgs e)
        {
            Log.Write(LogLevel.Normal, "START: inizio compito interrotto");
            MessageDialog.Show("Inizio compito interrotto!", MessageInfoType.Warning);
        }

        void am_AssignmentStartCompleted(object sender, EventArgs e)
        {
            Log.Write(LogLevel.Normal, "START: inizio compito completato");
        }

        void am_AssignmentStartError(object sender, AssignmentErrorEventArgs e)
        {
            var msg = e.Error != null ? e.Error.Message : "";
            Log.Write(LogLevel.Error, "START: {0}", CreateErrorLogText(e));
            MessageDialog.Show(CreateErrorText(e), MessageInfoType.Error);
        }

        void am_AssignmentRequestProjectName(object sender, ProjectNameRequestEventArgs e)
        {
            Log.Write(LogLevel.Normal, "START: richiesto nome progetto");
            if (Global.IsDevelopAccount)
            {
                e.SelectedProjectName = e.ProjectNames[0];
                e.NameAs = "Rossi";
                return;
            }
            var d = SetAssignmentDialog.Create(e);
            var cmd = d.ShowDialog();
            if (cmd == DialogResult.OK)
            {
                e.SelectedProjectName = d.SelectedProjectName;
                e.NameAs = d.ProjectNameAs;
            }
            else
            {
                e.Abort = true;
            }
            d.Dispose();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Close();
        }
    }
}
