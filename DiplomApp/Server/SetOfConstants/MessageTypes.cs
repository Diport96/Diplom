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
        ///  Тип запроса на рассылку сообщений микроконтроллерам с последующим откликом 
        /// </summary>
        public const string BROADCAST_CONNECTION = "HELLO";

        /// <summary>
        /// Тип запроса на подтверждение соединения с сервером
        /// </summary>
        public const string PERMIT_TO_CONNECT = "CONNACK";

        /// <summary>
        /// Тип запроса на рассылку значений микроконтроллеров серверу
        /// </summary>
        public const string DISTRIBUTION_OF_VALUES = "DISTRIBUTE";

        /// <summary>
        /// Тип запроса на изменение состояния переключателя
        /// </summary>
        public const string CHANGE_SWITCH_STATE = "STATE";
    }      
}
