using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MayVideoChat
{
    interface IReceiverView
    {
        PictureBox Picture { get; }
        event EventHandler<EventArgs> TryCall;
        event EventHandler<EventArgs> StartReceive;
        void ShowError(Exception e);
    }
}
