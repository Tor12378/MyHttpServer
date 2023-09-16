using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace hw
{
    public class AppSettings
    {
        public int Port { get; set; }
        public string Address { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            string appSettingsPath = "C:\\Users\\tupae\\source\\repos\\MyHttpServer\\appsetting.json";
            string json = File.ReadAllText(appSettingsPath);
            AppSettings settings = JsonConvert.DeserializeObject<AppSettings>(json);
            string prefix = $"{settings.Address}:{settings.Port}/";
            HttpListener server = new HttpListener();
            server.Prefixes.Add(prefix);
            server.Start();
            Console.WriteLine($"Сервер запущен на {prefix}");
            Console.WriteLine("Для остановки сервера введите '/stop' в консоль и нажмите Enter.");

            bool stopRequested = false;

            Task.Run(() =>
            {
                while (true)
                {
                    string consoleInput = Console.ReadLine();
                    if (consoleInput == "/stop")
                    {
                        Console.WriteLine("Получена команда на остановку сервера.");
                        stopRequested = true;
                        break;
                    }
                }
            });

            while (!stopRequested)
            {
                var context = await server.GetContextAsync();
                var request = context.Request;
                var response = context.Response;

                string filePath = "C:\\Users\\tupae\\OneDrive\\Рабочий стол\\ITISHW\\Google\\index.html";
                byte[] buffer = File.ReadAllBytes(filePath);
                response.ContentLength64 = buffer.Length;
                using Stream output = response.OutputStream;
                await output.WriteAsync(buffer);
                await output.FlushAsync();
                Console.WriteLine("Запрос обработан");
            }

            server.Stop();
            Console.WriteLine("Сервер остановлен.");
        }
    }
}
