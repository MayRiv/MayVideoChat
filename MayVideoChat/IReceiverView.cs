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
        TextBox Log { get; }
        PictureBox Picture { get; }
        void Call(TryCallArguments a);
        event EventHandler<EventArgs> TryClose;
        void ShowError(Exception e);
    }
}
