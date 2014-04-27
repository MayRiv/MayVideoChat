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
        string IP { set; }
        PictureBox Picture { get; }
        event EventHandler<TryCallArguments> TryCall;
        event EventHandler<EventArgs> TryClose;
        void ShowError(Exception e);
    }
}
