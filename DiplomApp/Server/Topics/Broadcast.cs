using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Server.Requests
{
    static class Broadcast
    {
        private const string VALUE = "HELLO";
        public static string Type { get; } = "Broadcast";
        
        public static void Execute() { }
    }
}
