using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Server
{
    /// <summary>
    /// Исключение, возникающее когда не найден обработчик запроса
    /// для типа полученного по MQTT сообщения
    /// </summary>
    [Serializable]
    public class HandlerNotFindException : Exception
    {
        /// <summary>
        /// Конструктор исключения
        /// </summary>
        public HandlerNotFindException() { }

        /// <summary>
        /// Конструктор исключения
        /// </summary>
        /// <param name="message">Передаваемое вместе с исключением сообщение</param>
        public HandlerNotFindException(string message) : base(message) { }

        /// <summary>
        /// Конструктор исключения
        /// </summary>
        /// <param name="message">Передаваемое вместе с исключением сообщение</param>
        /// <param name="inner">Ссылка на внутреннее исключение</param>
        public HandlerNotFindException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Конструктор исключения
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected HandlerNotFindException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
