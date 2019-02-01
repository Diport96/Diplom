using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetOfConstants
{
    /// <summary>
    /// Класс, предоставляющий набор констант предназначенных для отправки\получения сообщений.
    /// </summary>
    public static class MessageTypes 
    {
        /// <summary>
        ///  Тип запроса на соединение с сервером
        /// </summary>
        public const string REQUSET_TO_CONNECT = "CONNECT";

        /// <summary>
        ///  Тип запроса на закрытие соединения с сервером
        /// </summary>
        public const string REQUSET_TO_DISCONNECT = "DISCONNECT";

        /// <summary>
        ///  Тип запроса на рассылку сообщений микроконтроллерам с поледующим откликом 
        /// </summary>
        public const string BROADCAST_CONNECTION_REQUSET = "HELLO";

        /// <summary>
        /// Тип зпароса на подтверждение соединения с сервером
        /// </summary>
        public const string PERMIT_TO_CONNECT = "CONNACK";
    }      
}
