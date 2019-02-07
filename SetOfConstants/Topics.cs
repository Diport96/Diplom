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
        /// Топик, определяющий доменную обдасть для обмена сообщениями общего характера 
        /// между сервером и микроконтроллерами. Также определяет 
        /// доменную область для устройств, тип которых не удалость определить
        /// </summary>
        public const string DEVICES = "Devices";
        
        public const string SWITCHES = "Devices/Switches";        
        public const string SENSORS = "Devices/Sensors";
        public const string TERMOMETERS = "Devices/Sensors/Termometers";
    }
}
