using System.ComponentModel.DataAnnotations;

namespace MessagesService.Models
{
    /// <summary>
    /// Модель информации о пользователе.
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Беспараметрический конструктор.
        /// </summary>
        public UserModel() { }
        
        /// <summary>
        /// Конструктор класса, необходимый для присваивания значений свойствам.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="email">Электронная почта пользователя.</param>
        public UserModel(string userName, string email) => (UserName, Email) = (userName, email);

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        [Required]
        public string UserName { get; set; }
        
        /// <summary>
        /// Электронная почта пользователя.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Переопределенный метод ToString().
        /// </summary>
        /// <returns>Строка с информацией об актуальных данных о новом пользователе.</returns>
        public override string ToString() => $"Новый пользователь с именем: {UserName}, email: {Email}";
    }
}