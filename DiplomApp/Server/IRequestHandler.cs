using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Server
{
    interface IRequestHandler
    {
        void Execute(Dictionary<string,string> pairs);
    }
}
