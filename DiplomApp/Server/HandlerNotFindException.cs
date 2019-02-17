using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Server
{
    [Serializable]
    public class HandlerNotFindException : Exception
    {
        public HandlerNotFindException() { }
        public HandlerNotFindException(string message) : base(message) { }
        public HandlerNotFindException(string message, Exception inner) : base(message, inner) {  }
        protected HandlerNotFindException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
