using System.ComponentModel.DataAnnotations;

namespace MessagesService.Models
{
    /// <summary>
    /// Модель информации о сообщении.
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// Беспараметрический конструктор.
        /// </summary>
        public MessageModel() { }
        
        /// <summary>
        /// Конструктор класса, необходимый для присваивания значений свойствам.
        /// </summary>
        /// <param name="subject">Тема сообщения.</param>
        /// <param name="message">Текст сообщения.</param>
        /// <param name="senderId">Id отправителя сообщения.</param>
        /// <param name="receiverId">Id получателя сообщения.</param>
        public MessageModel(string subject, string message, string senderId, string receiverId)
            => (Subject, Message, SenderId, ReceiverId) = (subject, message, senderId, receiverId);

        /// <summary>
        /// Тема сообщения.
        /// </summary>
        [Required]
        public string Subject { get; set; }
        
        /// <summary>
        /// Текст сообщения.
        /// </summary>
        [Required]
        public string Message { get; set; }
        
        /// <summary>
        /// Id отправителя сообщения.
        /// </summary>
        [Required]
        public string SenderId { get; set; }
        
        /// <summary>
        /// Id получателя сообщения.
        /// </summary>
        [Required]
        public string ReceiverId { get; set; }

        /// <summary>
        /// Переопределенный метод ToString().
        /// </summary>
        /// <returns>Строка с информацией об актуальных данных о новом сообщении.</returns>
        public override string ToString() => $"Новое сообщение с темой: {Subject}, текстом сообщения: {Message}, " +
                                             $"Id отправителя: {SenderId}, Id получателя: {ReceiverId}";
    }
}