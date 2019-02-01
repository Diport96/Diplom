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
        public const string CONNECTION = "Connection";        
        public const string DEVICES = "Devices";        
        public const string SWITCHES = "Devices/Switches";        
        public const string SENSORS = "Devices/Sensors";
        public const string TERMOMETERS = "Devices/Sensors/Termometers";
    }
}
