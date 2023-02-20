namespace MessagesService.Models
{
    /// <summary>
    /// Модель постраничной выборки.
    /// </summary>
    public class PaginationModel
    {
        /// <summary>
        /// Максимальное количество пользователей, которое необходимо вернуть.
        /// </summary>
        public int? Limit { get; set; }
        
        /// <summary>
        /// Порядковый номер пользователя, начиная с которого необходимо получать информацию.
        /// </summary>
        public int? Offset { get; set; }
    }
}