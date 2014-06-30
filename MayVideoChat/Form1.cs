using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
namespace MayVideoChat
{
    public partial class Form1 : Form, ISenderView, IReceiverView
    {
        public Form1()
        {
            InitializeComponent();

        }

        
        public TextBox Log
        {
            get
            {
                return logTextBox;
            }
        }
        public string IP
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }
        public string Message
        {
            get { return messageTextBox.Text; }
            set { messageTextBox.Text = value; }
        }
        public string Nickname
        {
            get { return nicknameTextBox.Text; }
        }
        public PictureBox Picture
        {
            get { return pictureBox1; }
        }

        
        public void Call(TryCallArguments a)
        {
            TryCall.Invoke(this, a);
        }
        private void callButton_Click(object sender, EventArgs e)
        {
            if (callButton.Text == "Call")
            {
                IPAddress addr = IPAddress.Parse(IP);
                TryCall.Invoke(sender, new TryCallArguments(addr));
                callButton.Text = "End Call";
            }
            else
            {
                TryClose.Invoke(sender, e);
                callButton.Text = "Call";
            }
        }

        public void ShowError(Exception e)
        {
            MessageBox.Show(e.Message);
        }

        public event EventHandler<TryCallArguments> TryCall;
        public event EventHandler<EventArgs> TryClose;
        public event EventHandler<EventArgs> TrySendMessage;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(3);
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            TrySendMessage.Invoke(sender, e);
            messageTextBox.Clear();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Enter))
            {
                sendButton_Click(this, new EventArgs());
                
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

     

        

      

   
    }
}
