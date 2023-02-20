using System;
using System.Collections.Generic;
using System.IO;
using MessagesService.Models;
using Newtonsoft.Json;

namespace MessagesService.Validators
{
    /// <summary>
    /// Класс, отвечающий за валидацию данных входных файлов с информацией в json формате.
    /// </summary>
    public static class InputFilesDataValidator
    {
        /// <summary>
        /// Валидирует данные входных файлов с информацией в json формате.
        /// </summary>
        /// <param name="usersFilePath">Путь к файлу с информацией о пользователях.</param>
        /// <param name="messagesFilePath">Путь к файлу с информацией о сообщениях.</param>
        /// <returns>True, если формат входных файлов корректен, false в противном случае.</returns>
        public static bool IsValid(string usersFilePath, string messagesFilePath)
            => UsersFileValidator.IsValidUsersFile(usersFilePath) &&
               MessagesFileValidator.IsValidMessagesFile(messagesFilePath);

        /// <summary>
        /// Приватный статический класс, инкапсулирующий валидацию файла с пользователями.
        /// </summary>
        private static class UsersFileValidator
        {
            /// <summary>
            /// Валидирует данные о пользователях.
            /// </summary>
            /// <param name="path">Путь к файлу с информацией о пользователях.</param>
            /// <returns>True, если формат входных файлов в json формате, false в противном случае.</returns>
            public static bool IsValidUsersFile(string path)
            {
                if (!File.Exists(path))
                {
                    return true;
                }
                try
                {
                    var _ = JsonConvert.DeserializeObject<List<UserModel>>(File.ReadAllText(path));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Приватный статический класс, инкапсулирующий валидацию файла с сообщениями.
        /// </summary>
        private static class MessagesFileValidator
        {
            /// <summary>
            /// Валидирует данные о сообщениях.
            /// </summary>
            /// <param name="path">Путь к файлу с информацией о сообщениях.</param>
            /// <returns>True, если формат входных файлов в json формате, false в противном случае.</returns>
            public static bool IsValidMessagesFile(string path)
            {
                if (!File.Exists(path))
                {
                    return true;
                }
                try
                {
                    var _ = JsonConvert.DeserializeObject<List<MessageModel>>(File.ReadAllText(path));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}