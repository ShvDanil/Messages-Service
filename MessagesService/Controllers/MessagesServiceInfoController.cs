using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessagesService.Models;
using MessagesService.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MessagesService.Controllers
{
    /// <summary>
    /// Основной контроллер веб-приложения, реализующий работу различных запросов сервиса сообщений.
    /// </summary>
    [ApiController]
    [Route("messages-service")]
    [SwaggerTag("Сервис сообщений")]
    public class MessagesServiceInfoController : Controller
    {
        // Приватные поля, содержащие путь к исходным файлам, хранящим данные о пользователях и сообщениях.
        private static readonly string s_usersJsonFilePath = $"Data{Path.DirectorySeparatorChar}usersData.json";
        private static readonly string s_messagesJsonFilePath = $"Data{Path.DirectorySeparatorChar}messagesData.json";

        // Настройки сериализации / десериализации Json-файлов.
        private static readonly JsonSerializerSettings s_jsonSerializerSettings = new()
        {
            Formatting = Formatting.Indented
        };
        
        /// <summary>
        /// Свойство, хранящее информацию о пользователях.
        /// </summary>
        private static List<UserModel> Users
        {
            get => System.IO.File.Exists(s_usersJsonFilePath)
                ?  JsonConvert.DeserializeObject<List<UserModel>>(
                    System.IO.File.ReadAllText(s_usersJsonFilePath), s_jsonSerializerSettings
                    )
                : new List<UserModel>();
            set => System.IO.File.WriteAllText(
                s_usersJsonFilePath, JsonConvert.SerializeObject(value, s_jsonSerializerSettings)
                );
        }
        
        /// <summary>
        /// Свойство, хранящее информацию о сообщениях.
        /// </summary>
        private static List<MessageModel> Messages
        {
            get => System.IO.File.Exists(s_messagesJsonFilePath) 
                ?  JsonConvert.DeserializeObject<List<MessageModel>>(
                    System.IO.File.ReadAllText(s_messagesJsonFilePath), s_jsonSerializerSettings
                    ) 
                : new List<MessageModel>();
            set => System.IO.File.WriteAllText(
                s_messagesJsonFilePath, JsonConvert.SerializeObject(value, s_jsonSerializerSettings)
                );
        }
        
        /// <summary>
        /// Инициализирует пользователей и сообщения случайным образом.
        /// </summary>
        /// <response code="200">Пользователи и сообщения были успешно инициализированы.</response>
        /// <response code="400">Пользователи или сообщения уже существуют. Невозможно выполнить операцию.</response>
        /// <returns>Ошибку в случае некорректного действия или успех выполненной операции.</returns>
        [HttpPost("initial-post")]
        [ProducesResponseType(typeof(StatusResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusResponseModel), StatusCodes.Status400BadRequest)]
        public IActionResult GenerateAndPostInitialUsersAndMessages()
        {
            // Проверка данных файлов на соответствие json формату.
            if (!InputFilesDataValidator.IsValid(s_usersJsonFilePath, s_messagesJsonFilePath))
            {
                return BadRequest(
                        new {Message = "Невозможно получить данные из файла(-ов)! Удалите их и попробуйте снова или " +
                                            "проверьте соответствие входных данных json формату!"}
                    );
            }
            
            // Проверка на отсутствие пользователей или сообщений.
            if (Users.Any() || Messages.Any())
            {
                return BadRequest(
                    new {Message = "Невозможно выполнить инициализацию! Пользователи или сообщения уже существуют!"}
                );
            }
            
            // Генерация и инициализация списков пользователей и сообщений.
            Users = Generator.GenerateUsers();
            Messages = Generator.GenerateMessages();
            
            return Ok(
                new {Message = "Пользователи и сообщения были успешно сгенерированы и проинициализированы!"}
            );
        }

        /// <summary>
        /// Получает информацию о пользователе по его идентификатору email.
        /// </summary>
        /// <remarks>Возвращает единственного пользователя.</remarks>
        /// <param name="email">Электронный адрес пользователя.</param>
        /// <response code="200">Пользователь был успешно найден.</response>
        /// <response code="400">Был введен некорректный email.</response>
        /// <response code="404">Пользователь с введенным email не найден.</response>
        /// <returns>Ошибку в случае некорректного запроса или успех о выполненной операции.</returns>
        [HttpGet("by-email-{email}")]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(StatusResponseModel), StatusCodes.Status404NotFound)]
        public IActionResult GetUserInfoByEmail(string email)
        {
            // Проверка данных файлов на соответствие json формату.
            if (!InputFilesDataValidator.IsValid(s_usersJsonFilePath, s_messagesJsonFilePath))
            {
                return BadRequest(
                    new {Message = "Невозможно получить данные из файла(-ов)! Удалите их и попробуйте снова или " +
                                   "проверьте соответствие входных данных json формату!"}
                );
            }
            
            // Проверка валидности электронного адреса.
            if (!EmailValidator.IsValidEmailAddress(email))
            {
                return BadRequest(
                    new {Message = "Некорректный email адрес! Попробуйте снова!"}
                );
            }

            // Поиск электронного адреса в уже имеющемся списке пользователей.
            var user = Users.SingleOrDefault(u => string.Equals(u.Email, email));
            
            if (user is null)
            {
                return NotFound(
                    new {Message = $"Пользователь с email адресом {email} не найден! Попробуйте снова!"}
                );
            }

            return Ok(user);
        }

        /// <summary>
        /// Получает всех пользователей, поддерживает функционал для постраничной выборки.
        /// </summary>
        /// <remarks>Для валидности значений параметров пагинации используйте: integer limit со значением &#x2265; 
        /// 1 и integer offset со значением &#x2265; 0.</remarks>
        /// <param name="paginationModel">Объект пагинации, поступающий в виде query-запроса.</param>
        /// <response code="200">Пользователи были успешно найдены.</response>
        /// <response code="400">Некорректные значения параметров пагинации (limit / offset).</response>
        /// <response code="404">Пользователи не найдены.</response>
        /// <returns>Ошибку в случае некорректного запроса или успех о выполненной операции.</returns>
        [HttpGet("all-users")]
        [ProducesResponseType(typeof(List<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(StatusResponseModel), StatusCodes.Status404NotFound)]
        public IActionResult GetAllUsersInfo([FromQuery] PaginationModel paginationModel)
        {
            // Проверка данных файлов на соответствие json формату.
            if (!InputFilesDataValidator.IsValid(s_usersJsonFilePath, s_messagesJsonFilePath))
            {
                return BadRequest(
                    new {Message = "Невозможно получить данные из файла(-ов)! Удалите их и попробуйте снова или " +
                                   "проверьте соответствие входных данных json формату!"}
                );
            }
            
            // Проверка наличия пользователей в списке.
            if (!Users.Any())
            {
                return NotFound(
                    new {Message = "Пользователи не были найдены! Добавьте новых пользователей и попробуйте снова!"}
                );
            }
            
            // Валидация свойств поступившего объекта пагинации для десериализации.
            var (limit, offset) = PaginationValidator.GetValidPagination(paginationModel, Users);

            // Проверка на корректность условия подходящих значений.
            if (limit <= 0 || offset < 0)
                return BadRequest(
                    new {Message = "Некорректные значения параметров limit или offset! Попробуйте снова!"}
                    );
            
            // Создание и заполнение списка с учетом условий после валидации пагинации объекта.
            var users = Users.Skip(offset).Take(limit).ToList();
            
            return !users.Any()
                ? NotFound(
                    new {Message = "Пользователи не были найдены! Измените параметры или добавьте новых пользователей!"}
                )
                : Ok(users);
        }
        
        /// <summary>
        /// Получает сообщения по идентификатору отправителя и получателя.
        /// </summary>
        /// <param name="senderId">Идентификатор отправителя.</param>
        /// <param name="receiverId">Идентификатор получателя.</param>
        /// <response code="200">Сообщения были успешно найдены.</response>
        /// <response code="404">Сообщения или отправитель / получатель не найдены.</response>
        /// <returns>Ошибку в случае некорректного запроса или успех о выполненной операции.</returns>
        [HttpGet("by-ids-{senderId}/{receiverId}")]
        [ProducesResponseType(typeof(List<MessageModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusResponseModel), StatusCodes.Status404NotFound)]
        public IActionResult GetMessagesBySenderAndReceiverId(string senderId, string receiverId)
        {
            // Проверка данных файлов на соответствие json формату.
            if (!InputFilesDataValidator.IsValid(s_usersJsonFilePath, s_messagesJsonFilePath))
            {
                return BadRequest(
                    new {Message = "Невозможно получить данные из файла(-ов)! Удалите их и попробуйте снова или " +
                                   "проверьте соответствие входных данных json формату!"}
                );
            }
            // Проверка на наличие отправителя / получателя.
            if (Users.FirstOrDefault(user => string.Equals(user.Email, senderId)) is null)
            {
                return NotFound(
                    new {Message = "Отправитель не найден! Попробуйте снова!"}
                );
            }
            if (Users.FirstOrDefault(user => string.Equals(user.Email, receiverId)) is null)
            {
                return NotFound(
                    new {Message = "Получатель не найден! Попробуйте снова!"}
                );
            }
            
            // Получение необходимых сообщений по id отправителя и получателя.
            var allMessages = Messages
                .Where(message => (string.Equals(message.SenderId, senderId) && 
                                  string.Equals(message.ReceiverId, receiverId)) ||
                                  (string.Equals(message.SenderId, receiverId) && 
                                   string.Equals(message.ReceiverId, senderId)))
                .Select(message => message.Message).ToList();

            return !allMessages.Any()
                ? NotFound(
                    new {Message = "Сообщения не найдены! Попробуйте снова!"}
                )
                : Ok(allMessages);
        }

        /// <summary>
        /// Получает сообщения по идентификатору отправителя.
        /// </summary>
        /// <param name="senderId">Идентификатор отправителя.</param>
        /// <response code="200">Сообщения были успешно найдены.</response>
        /// <response code="404">Сообщения или отправитель не найдены.</response>
        /// <returns>Ошибку в случае некорректного запроса или успех о выполненной операции.</returns>
        [HttpGet("by-sender-id-{senderId}")]
        [ProducesResponseType(typeof(List<MessageModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusResponseModel), StatusCodes.Status404NotFound)]
        public IActionResult GetMessagesBySenderId(string senderId)
        {
            // Проверка данных файлов на соответствие json формату.
            if (!InputFilesDataValidator.IsValid(s_usersJsonFilePath, s_messagesJsonFilePath))
            {
                return BadRequest(
                    new {Message = "Невозможно получить данные из файла(-ов)! Удалите их и попробуйте снова или " +
                                   "проверьте соответствие входных данных json формату!"}
                );
            }
            
            // Проверка на наличие отправителя.
            if (Users.FirstOrDefault(user => string.Equals(user.Email, senderId)) is null)
            {
                return NotFound(
                        new {Message = "Отправитель не найден! Попробуйте снова!"}
                    );
            }
            
            // Получение необходимых сообщений по id отправителя.
            var allSenderMessages = Messages
                .Where(message => string.Equals(message.SenderId, senderId))
                .Select(message => message.Message).ToList();
            
            return !allSenderMessages.Any()
                ? NotFound(
                        new {Message = "Сообщения от пользователя не найдены! Попробуйте снова!"}
                    ) 
                : Ok(allSenderMessages);
        }

        /// <summary>
        /// Получает сообщения по идентификатору получателя.
        /// </summary>
        /// <param name="receiverId">Идентификатор получателя.</param>
        /// <response code="200">Сообщения были успешно найдены.</response>
        /// <response code="404">Сообщения или получатель не найдены.</response>
        /// <returns>Ошибку в случае некорректного запроса или успех о выполненной операции.</returns>
        [HttpGet("by-receiver-id-{receiverId}")]
        [ProducesResponseType(typeof(List<MessageModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusResponseModel), StatusCodes.Status404NotFound)]
        public IActionResult GetMessagesByReceiverId(string receiverId)
        {
            // Проверка данных файлов на соответствие json формату.
            if (!InputFilesDataValidator.IsValid(s_usersJsonFilePath, s_messagesJsonFilePath))
            {
                return BadRequest(
                    new {Message = "Невозможно получить данные из файла(-ов)! Удалите их и попробуйте снова или " +
                                   "проверьте соответствие входных данных json формату!"}
                );
            }
            
            // Проверка на наличие получателя.
            if (Users.FirstOrDefault(user => string.Equals(user.Email, receiverId)) is null)
            {
                return NotFound(
                        new {Message = "Получатель не найден! Попробуйте снова!"}
                    );
            }
            
            // Получение необходимых сообщений по id получателя.
            var allReceiverMessages = Messages
                .Where(message => string.Equals(message.ReceiverId, receiverId))
                .Select(message => message.Message).ToList();

            return !allReceiverMessages.Any()
                ? NotFound(
                        new {Message = "Сообщения у пользователя не найдены! Попробуйте снова!"}
                    )
                : Ok(allReceiverMessages);
        }

        /// <summary>
        /// Добавляет информацию о новом пользователе.
        /// </summary>
        /// <param name="newUserModel">Объект, содержащий информацию о новом пользователе.</param>
        /// <response code="200">Информация о новом пользователе была успешно добавлена.</response>
        /// <response code="400">Был введен некорректный email, или пользователь с таким email уже существует.</response>
        /// <returns>Ошибку в случае некорректного запроса или успех о выполненной операции.</returns>
        [HttpPost("add-user")]
        [ProducesResponseType(typeof(StatusResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusResponseModel), StatusCodes.Status400BadRequest)]
        public IActionResult PostNewUser([FromQuery] UserModel newUserModel)
        {
            // Проверка данных файлов на соответствие json формату.
            if (!InputFilesDataValidator.IsValid(s_usersJsonFilePath, s_messagesJsonFilePath))
            {
                return BadRequest(
                    new {Message = "Невозможно получить данные из файла(-ов)! Удалите их и попробуйте снова или " +
                                   "проверьте соответствие входных данных json формату!"}
                );
            }
            
            // Валидация электронного адреса нового пользователя.
            if (!EmailValidator.IsValidEmailAddress(newUserModel.Email))
            {
                return BadRequest(
                    new {Message = "Некорректный email адрес! Попробуйте снова!"}
                );
            }
            
            // Проверка на уникальность электронной почты, используемой в качестве идентификатора.
            if (Users.SingleOrDefault(user => string.Equals(user.Email, newUserModel.Email)) is not null)
            {
                return BadRequest(
                    new {Message = "Такой пользователь уже существует! Попробуйте снова!"}
                );
            }
            
            // Добавление нового пользователя.
            var tmpUsersList = Users;
            tmpUsersList.Add(newUserModel);
            
            // Сортировка пользователей в лексикографическом порядке.
            tmpUsersList.Sort((user1, user2) => user1.Email.CompareTo(user2.Email));
            Users = tmpUsersList;
            
            return Ok(
                    new {Message = $"{newUserModel} был успешно добавлен!"}
                );
        }

        /// <summary>
        /// Добавляет информацию о новом сообщении.
        /// </summary>
        /// <param name="newMessageModel">Объект, содержащий информацию о новом сообщении.</param>
        /// <response code="200">Информация о новом сообщении была успешно добавлена.</response>
        /// <response code="400">Отправитель или получатель сообщения не найдены.</response>
        /// <returns>Ошибку в случае некорректного запроса или успех о выполненной операции.</returns>
        [HttpPost("add-message")]
        [ProducesResponseType(typeof(StatusResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusResponseModel), StatusCodes.Status400BadRequest)]
        public IActionResult SendNewMessage([FromQuery] MessageModel newMessageModel)
        {
            // Проверка данных файлов на соответствие json формату.
            if (!InputFilesDataValidator.IsValid(s_usersJsonFilePath, s_messagesJsonFilePath))
            {
                return BadRequest(
                    new {Message = "Невозможно получить данные из файла(-ов)! Удалите их и попробуйте снова или " +
                                   "проверьте соответствие входных данных json формату!"}
                );
            }
            
            var (senderId, receiverId) = (newMessageModel.SenderId, newMessageModel.ReceiverId);
            
            // Проверка на наличие id отправителя среди списка id всех сообщений.
            if (Users.FirstOrDefault(user => string.Equals(user.Email, senderId)) is null)
            {
                return BadRequest(
                    new {Message = "Отправителя с таким Id не существует!"}
                );
            }
            
            // Проверка на наличие id получателя среди списка id всех сообщений.
            if (Users.FirstOrDefault(user => string.Equals(user.Email, receiverId)) is null)
            {
                return BadRequest(
                    new {Message = "Получателя с таким Id не существует!"}
                );
            }
            
            // Добавление нового сообщения.
            var tmpMessagesList = Messages;
            tmpMessagesList.Add(newMessageModel);
            Messages = tmpMessagesList;
            
            return Ok(
                    new {Message = $"{newMessageModel} было успешно добавлено!"}
                );
        }
    }
}