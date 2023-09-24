using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MyHttpServer;
using Newtonsoft.Json;

namespace MyHttpServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            AppSettings config = ServerConfigManager.LoadServerConfig();
            if (config == null)
            {
                Console.WriteLine("Ошибка при загрузке конфигурации. Сервер не может быть запущен.");
                return;
            }

            string prefix = $"{config.Address}:{config.Port}/";
            HttpListener server = new HttpListener();
            server.Prefixes.Add(prefix);
            server.Start();
            Console.WriteLine($"Сервер запущен на {prefix}");
            Console.WriteLine("Для остановки сервера введите 'stop' в консоль и нажмите Enter.");

            var serverManager = new ServerManager(server, config);
            Task.Run(() =>
            {
                while (true)
                {
                    string consoleInput = Console.ReadLine();
                    if (consoleInput == "stop")
                    {
                        serverManager.Stop();
                        break;
                    }
                }
            });
            serverManager.Start();
        }
    }
}
