using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MessagesService
{
    /// <summary>
    /// Основной класс программы.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Основная точка программы, которая запускает ее.
        /// </summary>
        /// <param name="args">Аргументы.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Метод для создания и настройки объекта построителя.
        /// </summary>
        /// <param name="args">Аргументы.</param>
        /// <returns>Развернутое веб-приложение.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}