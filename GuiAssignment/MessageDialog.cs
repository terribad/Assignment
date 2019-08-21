using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuiAssignment
{
    public partial class MessageDialog : Form
    {
        public MessageDialog()
        {
            InitializeComponent();
            CancelButtonVisible = false;
        }

        public static DialogResult Show(string text, MessageInfoType messageType, bool cancelVisible = false)
        {
            var d = new MessageDialog();
            d.MessageType = messageType;
            d.MessageText = text;
            d.CancelButtonVisible = cancelVisible;
            var cmd = d.ShowDialog();
            d.Dispose();
            return cmd;
            
        }

        public bool CancelButtonVisible
        {
            get { return btnCancel.Visible; }
            set { btnCancel.Visible = value; }
        }

        private MessageInfoType _messageType;
        public MessageInfoType MessageType
        {
            get { return _messageType; }
            set 
            {
                _messageType = value;
                UpdateIcon();
            }
        }

        private string _messageText;

        public string MessageText
        {
            get { return _messageText; }
            set 
            { 
                _messageText = value;
                lblMessageText.Text = value;
            }
        }

        void UpdateIcon()
        {
            switch (MessageType)
            {
                case MessageInfoType.Info:
                    picIcon.Image = Properties.Resources.Info;
                    Text = "INFORMAZIONE";                    
                    break;
                case MessageInfoType.Warning:
                    picIcon.Image = Properties.Resources.Alert;
                    Text = "ATTENZIONE";
                    break;
                case MessageInfoType.Error:
                    picIcon.Image = Properties.Resources.Error;
                    Text = "ERRORE!";
                    break;
                default: picIcon.Image = Properties.Resources.Info;
                    Text = "INFORMAZIONE";                    
                    break;
            }
        }

        private void MessageDialog_Load(object sender, EventArgs e)
        {
            
        }
        
    }
}
