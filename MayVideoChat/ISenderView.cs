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
        event EventHandler<EventArgs> TryCall;
        void ShowError(Exception e);
    }
}
