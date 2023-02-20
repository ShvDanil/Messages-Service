using System.Net.Mail;

namespace MessagesService.Validators
{
    /// <summary>
    /// Статический класс, валидирующий электронный адрес пользователя.
    /// </summary>
    public static class EmailValidator
    {
        /// <summary>
        /// Проверка корректности (валидности) электронного адреса пользователя.
        /// </summary>
        /// <param name="emailAddress">Электронный адрес пользователя.</param>
        /// <returns>true в случае прохождения проверки и false в противном случае.</returns>
        public static bool IsValidEmailAddress(string emailAddress) => MailAddress.TryCreate(emailAddress, out _);
    }
}