using JetBrains.Annotations;

namespace MessagesService.Models
{
    /// <summary>
    /// Модель ответа сервера на запрос клиента.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class StatusResponseModel
    {
        /// <summary>
        /// Сообщение, описывающее результат выполненного запроса.
        /// </summary>
        public string Message { get; set; }
    }
}