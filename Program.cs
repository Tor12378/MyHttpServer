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
        static async Task Main(string[] args)
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

            bool stopRequested = false;

            Task.Run(() =>
            {
                while (true)
                {
                    string consoleInput = Console.ReadLine();
                    if (consoleInput == "stop")
                    {
                        Console.WriteLine("Получена команда на остановку сервера.");
                        stopRequested = true;
                        server.GetContext();
                        break;
                    }
                }
            });

            while (!stopRequested)
            {
                var context = await server.GetContextAsync();
                var request = context.Request;
                var response = context.Response;

                string requestUrl = request.Url.LocalPath;
                if ( requestUrl.EndsWith(".html"))
                {
                    string filePath = Path.Combine(config.StaticFilesPath, requestUrl.TrimStart('/'));
                    if (File.Exists(filePath))
                    {
                        byte[] buffer = File.ReadAllBytes(filePath);
                        response.ContentLength64 = buffer.Length;
                        using Stream output = response.OutputStream;
                        await output.WriteAsync(buffer);
                        await output.FlushAsync();
                        Console.WriteLine($"Запрос обработан: {requestUrl}");
                    }
                    else
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.ContentType = "text/plain; charset=utf-8";
                        string notFoundMessage = "404 Файл не найден";
                        byte[] notFoundBuffer = Encoding.UTF8.GetBytes(notFoundMessage);
                        response.ContentLength64 = notFoundBuffer.Length;
                        using Stream output = response.OutputStream;
                        await output.WriteAsync(notFoundBuffer);
                        await output.FlushAsync();
                        Console.WriteLine($"Файл не найден: {requestUrl}");
                    }
                }

            }

            server.Stop();
            Console.WriteLine("Сервер остановлен.");
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
