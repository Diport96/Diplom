using DiplomApp.Controllers;
using NLog;
using SetOfConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Server.Requests
{
    static class ServerConnection
    {
        private const string VALUE = "CONNECT";
        public static string Type { get; } = "Connection";        
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();        

        public static void Execute( string jsonMessage)
        {
            
            
        }

        
    }
}
