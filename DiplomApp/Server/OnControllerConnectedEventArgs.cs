using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp
{
    public class OnControllerConnectedEventArgs : EventArgs
    {
        public string ID;
        public string Name;
        public CType Type;
        public string Value;

        public OnControllerConnectedEventArgs(string id, string name, CType type, string value)
        {
            ID = id;
            Name = name;
            Type = type;
            Value = value;
        }
    }
}
