using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MayVideoChat
{
    interface ISenderView
    {
        string IP { get; }
        string Message { get; }

        string Nickname { get; }

        event EventHandler<TryCallArguments> TryCall;
        event EventHandler<EventArgs> TryClose;
        event EventHandler<EventArgs> TrySendMessage;
        void ShowError(Exception e);
    }
}
