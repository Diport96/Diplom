using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Controllers
{

    [Serializable]
    public class InvalidControllerTypeException : Exception
    {
        public InvalidControllerTypeException() { }
        public InvalidControllerTypeException(string message) : base(message) { }
        public InvalidControllerTypeException(string message, Exception inner) : base(message, inner) { }
        protected InvalidControllerTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
