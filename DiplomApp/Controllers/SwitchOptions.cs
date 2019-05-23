using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Controllers
{
    /// <summary>
    /// Класс, являющийся моделю данных, определяющий опции устройства переключателя
    /// </summary>
    public class SwitchOptions
    {
        /// <summary>
        /// Представляет уникальный идентификатор экземпляра класса
        /// </summary>
        [Key]
        [ForeignKey("DeviceInfo")]
        public string ID { get; set; }

        /// <summary>
        /// Определяет количество возможных способов управления переключателем
        /// </summary>
        [Flags]
        public enum SwitchControl
        {
            /// <summary>
            /// Отсутствует какой-либо алгоритм управления переключателем,
            /// управление осуществляется через пользовательский интерфейс
            /// </summary>
            No,

            /// <summary>
            /// Управление переключателем осуществляется по 
            /// циклическому алгоритму задержки времени
            /// </summary>
            SwitchToDelay,

            /// <summary>
            /// Управление переключателем осуществляется по сигналу датчика
            /// </summary>
            SwitchToSignal
        };

        /// <summary>
        /// Определяет, какой алгоритм управления переключателем применяется на данный момент
        /// </summary>
        public SwitchControl Control { get; set; }

        /// <summary>
        /// Определяет идентификатор датчика, через который осуществляется управление переключателем
        /// </summary>
        public string SensorId { get; set; }

        /// <summary>
        /// Определяет задержку в миллисекундах, перед изменением состояния переключателя
        /// </summary>
        public int? DelayToSwitch { get; set; }

        /// <summary>
        /// Определяет, на какое состояние выполнять переключение
        /// </summary>
        public bool? ValueTo { get; set; }

        /// <summary>
        /// Определяет сущность данных об зарегистрированном устройстве, 
        /// с которым связаны данные опции
        /// </summary>
        public RegisteredDeviceInfo DeviceInfo { get; set; }

        /// <summary>
        /// Конструктор класса. Инициализирует экземпляр класса со
        /// стандартным алгоритмом управления переключателем
        /// </summary>
        public SwitchOptions()
        {            
            Control = SwitchControl.No;
            SensorId = null;
            DelayToSwitch = null;
            ValueTo = null;
        }

        /// <summary>
        /// Конструктор класса. Инициализирует экземпляр класса
        /// с алгоритмом управления переключателем по задержке времени
        /// </summary>
        /// <param name="delay">Задержка времени в миллсекундах</param>
        /// <param name="valueTo">Значение, на которое должно осуществляться переключение</param>
        public SwitchOptions(int delay, bool valueTo)
        {
            Control = SwitchControl.SwitchToDelay;
            SensorId = null;
            DelayToSwitch = delay;
            ValueTo = valueTo;
        }

        /// <summary>
        /// Конструктор класса. Инициализирует экземпляр класса
        /// с алгоритмом управления переключателем по сигналу датчика
        /// </summary>
        /// <param name="sensorId">Идентификатор датчика</param>
        /// <param name="valueTo">Значение, на которое должно осуществляться переключение</param>
        public SwitchOptions(string sensorId, bool valueTo)
        {
            Control = SwitchControl.SwitchToSignal;
            SensorId = sensorId;
            DelayToSwitch = null;
            ValueTo = valueTo;
        }
    }
}
