using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Controllers
{
    /// <summary>
    /// Исключение, указывающее на отсутствие сопоставимого с устройством типа данных
    /// </summary>
    [Serializable]
    public class InvalidControllerTypeException : Exception
    {
        /// <summary>
        /// Конструктор исключения
        /// </summary>
        public InvalidControllerTypeException() { }

        /// <summary>
        /// Конструктор исключения
        /// </summary>
        /// <param name="message">Передаваемое вместе с исключением сообщение</param>
        public InvalidControllerTypeException(string message) : base(message) { }

        /// <summary>
        /// Конструктор исключения
        /// </summary>
        /// <param name="message">Передаваемое вместе с исключением сообщение</param>
        /// <param name="inner">Ссылка на внутреннее исключение</param>
        public InvalidControllerTypeException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Конструктор исключения
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InvalidControllerTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
