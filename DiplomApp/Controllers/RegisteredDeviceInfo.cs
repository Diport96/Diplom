using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Controllers
{
    /// <summary>
    /// Модель данных для устройсв, зарегистрированных в базе данных
    /// </summary>
    public class RegisteredDeviceInfo : Controller
    {
        /// <summary>
        /// Тип устройства
        /// </summary>
        [Required]
        public string DeviceType { get; set; }

        /// <summary>
        /// Дата, когда была произведена регистрация данного устройства
        /// </summary>
        [Required]
        public DateTime RegisteredDate { get; set; }

        /// <summary>
        /// Разрешено ли данному устройству выполнять подключение к серверу
        /// </summary>
        public bool PermitToConnection { get; set; }

        /// <summary>
        /// Опции данного устройства
        /// </summary>
        public SwitchOptions Options { get; set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public RegisteredDeviceInfo() { }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="id">Идентификатор устройства</param>
        /// <param name="name">Название устройства</param>
        /// <param name="type">Тип устройства</param>
        /// <param name="registeredDate">Дата регистрации устройства в базе данных</param>
        public RegisteredDeviceInfo(string id, string name, string type, DateTime registeredDate) : base(id, name)
        {
            DeviceType = type;
            RegisteredDate = registeredDate;
            PermitToConnection = true;
            Options = new SwitchOptions();
        }
    }
}
