using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace DiplomApp.Controllers
{
    /// <summary>
    /// Абстрактный класс, определяющий первичную модель данных для всех устройств
    /// </summary>
    public abstract class Controller : INotifyPropertyChanged
    {
        private string name;
                
        /// <summary>
        /// Определяет уникальный идентификатор устройства
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }

        /// <summary>
        /// Определяет название устройства
        /// </summary>
        [Required]
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public Controller() { }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="id">Идентификатор устройства</param>
        /// <param name="name">Название устройства</param>
        public Controller(string id, string name)
        {
            ID = id;            
            Name = name;
        }

        /// <summary>
        /// Возникает при изменении свойства компонента
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Вызывается при изменении свойства компонента, привязанного к пользовательскому интерфейсу
        /// </summary>
        /// <param name="propertyName">Название свойства</param>
        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }        
    }
}
