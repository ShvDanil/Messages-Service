using System;
using System.Collections.Generic;
using MessagesService.Models;

namespace MessagesService
{
    /// <summary>
    /// Класс, описывающий генератор, который инициализирует пользователей и сообщения.
    /// </summary>
    public static class Generator
    {
        // Приватные поля, необходимые для работы методов.
        private static readonly Random s_random = new();
        private static readonly int s_collectionLength = s_random.Next(2, 15);
        private static int s_userCount;
        private const string Domain = "example.com";

        /// <summary>
        /// Метод, отвечающий за генерацию пользователей.
        /// </summary>
        /// <returns>Список сгенерированных пользователей.</returns>
        public static List<UserModel> GenerateUsers()
        {
            s_userCount = 0;
            var users = new List<UserModel>();
            for (var i = 0; i < s_collectionLength; i++)
            {
                // Создание имени и электронного адреса пользователя.
                var userName = CreateUserName();
                var userEmail = CreateUserEmail();
                
                users.Add(new UserModel(userName, userEmail));
            }
            // Сортировка пользователей в лексикографическом порядке.
            users.Sort((user1, user2) => user1.Email.CompareTo(user2.Email));

            return users;
        }
        
        /// <summary>
        /// Метод, отвечающий за генерацию сообщений.
        /// </summary>
        /// <returns>Список сгенерированных сообщений.</returns>
        public static List<MessageModel> GenerateMessages()
        {
            s_userCount = 0;
            var messages = new List<MessageModel>();
            for (var i = 0; i < s_collectionLength; i++)
            {
                // Создание темы сообщения, текста сообщения, Id отправителя и получателя.
                var messageSubject = CreateMessageSubject();
                var message = CreateMessage();
                var (senderId, receiverId) = CreateAndReturnSenderAndReceiverIds();
                
                messages.Add(new MessageModel(messageSubject, message, senderId, receiverId));
            }

            return messages;
        }

        /// <summary>
        /// Метод, отвечающий за создание имени пользователя.
        /// </summary>
        /// <returns>Имя пользователя.</returns>
        private static string CreateUserName() => $"User{++s_userCount}";
        
        /// <summary>
        /// Метод, отвечающий за создание электронного адреса пользователя.
        /// </summary>
        /// <returns>Электронный адрес пользователя.</returns>
        private static string CreateUserEmail() => $"User{s_userCount}@{Domain}";

        /// <summary>
        /// Метод, отвечающий за создание темы сообщения.
        /// </summary>
        /// <returns>Тему сообщения.</returns>
        private static string CreateMessageSubject() => $"Это тема сообщения от User{++s_userCount}.";

        /// <summary>
        /// Метод, отвечающий за создание текста сообщения.
        /// </summary>
        /// <returns>Текст сообщения.</returns>
        private static string CreateMessage() => $"Это сообщение от User{s_userCount}! Здравствуйте!";

        /// <summary>
        /// Метод, отвечающий за создание индентификаторов отправителя и получателя.
        /// </summary>
        /// <returns>Id отправителя и получателя.</returns>
        private static (string senderId, string receiverId) CreateAndReturnSenderAndReceiverIds()
        {
            // Присвоение значения id отправителя в порядке очереди.
            var senderId = s_userCount;
            
            // Присвоение значения id получателя, не равного id отправителя.
            int receiverId;
            do
            {
                receiverId = s_random.Next(1, s_collectionLength + 1);
            } while (senderId == receiverId);

            return ($"User{senderId}@{Domain}", $"User{receiverId}@{Domain}");
        }
    }
}