using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using AssignmentCore;

namespace GuiAssignment
{
    public partial class SetAssignmentDialog : Form
    {
        public SetAssignmentDialog()
        {
            InitializeComponent();
        }

        public static SetAssignmentDialog Create(ProjectNameRequestEventArgs e)
        {
            var d = new SetAssignmentDialog();
            d.ProjectNames = e.ProjectNames;
            return d;            
        }

        private string[] _projectNames;
        public string[] ProjectNames
        {
            set 
            {
                _projectNames = value;
                UpdateProjectListView();
            }
        }

        private void UpdateProjectListView()
        {
            cboProjectList.Items.Clear();
            foreach (var p in _projectNames)
            {
                cboProjectList.Items.Add(Path.GetFileNameWithoutExtension(p));
            }            
            cboProjectList.Enabled = _projectNames.Length > 1;
            cboProjectList.SelectedIndex = 0;
        }

        public string ProjectNameAs
        {
            get
            {
                return txtName.Text.Trim();
            }
        }

        public string SelectedProjectName
        {
            get
            {
                var index = cboProjectList.SelectedIndex < 0 ? 0 : cboProjectList.SelectedIndex;
                return _projectNames[index].Trim();
            }
        }

        private void SetAssignmentDialog_Load(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            UpdateView();
        }

        void UpdateView()
        {
            btnOk.Enabled = IsValidFilename(txtName.Text.Trim());
        }

        readonly Regex containsABadCharacter = new Regex("[" + Regex.Escape(new String(Path.GetInvalidFileNameChars())) + "]");
        bool IsValidFilename(string name)
        {
            bool ok = name != "" && (Char.IsLetter(name[0]) || name[0] == '_') 
                      && !containsABadCharacter.IsMatch(name);                  
            return ok;
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Space || e.KeyData == Keys.OemPeriod)
                e.SuppressKeyPress = true;
        }
    }
}
