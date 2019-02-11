using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Server
{
    class RequestTypeAttribute : Attribute
    {
        public string MessageType { get; set; }

        public RequestTypeAttribute(string messageType)
        {
            MessageType = messageType;
        }
    }
}
