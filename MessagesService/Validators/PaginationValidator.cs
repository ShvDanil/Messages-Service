using System.Collections;
using MessagesService.Models;

namespace MessagesService.Validators
{
    /// <summary>
    /// Статический класс, валидирующий пагинацию.
    /// </summary>
    public static class PaginationValidator
    {
        /// <summary>
        /// Получение валидной пагинации в зависимости от поступивших параметров в query-запросе.
        /// </summary>
        /// <param name="paginationModel">Объект пагинации.</param>
        /// <param name="users">Список пользователей.</param>
        /// <returns>Значения ограничения по максимальному количеству пользователей и порядковому номеру пользователя, 
        /// начиная с которого необходимо получать информацию.</returns>
        public static (int limit, int offset) GetValidPagination(PaginationModel paginationModel, ICollection users)
        {
            int? limit, offset;
            if (paginationModel.Limit is null && paginationModel.Offset is null)
            {
                // Присваиваем дефолтные значения в случае, если параметры не были переданы (их значения null).
                (limit, offset) = (users.Count, 0);
            }
            else
            {
                if (paginationModel.Limit is null)
                    // Присваиваем дефолтное значение максимальному количеству пользователей, если параметр null.
                    (limit, offset) = (users.Count, paginationModel.Offset);
                else if (paginationModel.Offset is null)
                    // Присваиваем дефолтное значение порядковому номеру пользователя, если параметр null.
                    (limit, offset) = (paginationModel.Limit, 0);
                else
                    // Присваиваем значения, поступившие из query-запроса.
                    (limit, offset) = (paginationModel.Limit, paginationModel.Offset);
            }

            return ((int)limit, (int)offset);
        }
    }
}