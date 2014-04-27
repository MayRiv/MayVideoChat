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

namespace MayVideoChat
{
    public partial class Form1 : Form, ISenderView, IReceiverView
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string IP
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }
        public PictureBox Picture
        {
            get { return pictureBox1; }
        }
        private void callButton_Click(object sender, EventArgs e)
        {
            if (callButton.Text == "Call") 
            {
                IPAddress addr = IPAddress.Parse(IP);
                TryCall.Invoke(sender, new TryCallArguments(addr));
                callButton.Text = "End Call";
            }
        }

        public void ShowError(Exception e)
        {
            MessageBox.Show(e.Message);
        }

        public event EventHandler<TryCallArguments> TryCall;
        public event EventHandler<EventArgs> TryClose;

        private void receivingButton_Click(object sender, EventArgs e)
        {
            //StartReceive.Invoke(sender, e);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            TryClose.Invoke(sender, e);
        }
    }
}
