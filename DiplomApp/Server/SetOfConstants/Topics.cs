using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetOfConstants
{
    /// <summary>
    /// Предоставляет набор констант с названиями топиков
    /// для взаимодействия сервера с микроконтроллерами по протоколу MQTT
    /// </summary>
    public static class Topics
    {   
        /// <summary>
        /// Топик, определяющий доменную область для обмена сообщениями 
        /// телекоммуникационного характера между сервером и микроконтроллерами
        /// </summary>
        public const string CONNECTION = "Connection";  
        
        /// <summary>
        /// Топик, определяющий доменную область для обмена сообщениями общего характера 
        /// между сервером и микроконтроллерами. Также определяет 
        /// доменную область для устройств, тип которых не удалость определить
        /// </summary>
        public const string DEVICES = "Devices";

        /// <summary>
        /// Топик, определяющий доменную область для обмена данными 
        /// между сервером и переключателями
        /// </summary>
        public const string SWITCHES = "Devices/Switches";

        /// <summary>
        /// Топик, определяющий доменную область для обмена данными 
        /// между сервером и датчиками
        /// </summary>
        public const string SENSORS = "Devices/Sensors";

        /// <summary>
        /// Топик, определяющий доменную область для обмена данными 
        /// между сервером и датчиками-термометрами
        /// </summary>
        public const string TERMOMETERS = "Devices/Sensors/Termometers";
    }
}
