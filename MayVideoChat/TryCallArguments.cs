using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MayVideoChat
{
    public class TryCallArguments:EventArgs
    {
        public IPAddress Adress { get; set; }
        public TryCallArguments(IPAddress a)
        {
            Adress = a;
        }
    }
}
