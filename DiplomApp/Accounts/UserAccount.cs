using System.ComponentModel.DataAnnotations;

namespace DiplomApp.Accounts
{
    /// <summary>
    /// Класс, проедставляющий аккаунт пользователя
    /// </summary>
    public class UserAccount : IAccount
    {
        /// <summary>
        /// Уникальный идентификатор аккаунта
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Логин от аккаунта
        /// </summary>
        [Required]
        public string Login { get; set; }

        /// <summary>
        /// Хешированный набор символов, который является частью ключа и определяет его уникальность.
        /// Используется в криптографическом алгоритме «Password-Based Key Derivation Function v2.0» (PBKDF2)
        /// </summary>
        [Required]
        public byte[] Salt { get; set; }

        /// <summary>
        /// Хэш пароля
        /// </summary>
        [Required]
        public byte[] Key { get; set; }
    }
}
