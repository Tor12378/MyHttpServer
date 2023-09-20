using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MyHttpServer;
using Newtonsoft.Json;

namespace hw
{
    class Program
    {
        static void Main(string[] args)
        {
            AppSettings config = LoadServerConfig();
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

            var serverManager = new ServerManager(server,config);
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

        private static AppSettings LoadServerConfig()
        {
            string appSettingsPath = @".\appsettings.json";
            try
            {
                string json = File.ReadAllText(appSettingsPath);
                var config = JsonConvert.DeserializeObject<AppSettings>(json);
                EnsureStaticFilesPath(config);
                return config;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке конфигурации из файла appsettings.json: {ex.Message}");
                return null;
            }
        }

        private static void EnsureStaticFilesPath(AppSettings config)
        {
            if (!Directory.Exists(config.StaticFilesPath))
            {
                try
                {
                    Directory.CreateDirectory(config.StaticFilesPath);
                    Console.WriteLine($"Создана папка для статических файлов: {config.StaticFilesPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при создании папки для статических файлов: {ex.Message}");
                }
            }
        }
    }
}
