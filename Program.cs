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
            string appSettingsPath = "appsetting.json";
            string json = File.ReadAllText("C:\\Users\\tupae\\source\\repos\\MyHttpServer\\appsetting.json");
            AppSettings settings = JsonConvert.DeserializeObject<AppSettings>(json);
            string prefix = $"{settings.Address}:{settings.Port}/";
            HttpListener server = new HttpListener();
            server.Prefixes.Add(prefix);
            server.Start();
            Console.WriteLine($"Сервер запущен на {prefix}");
            Console.WriteLine("Для остановки сервера введите '/stop' и нажмите Enter.");
            while (true)
            {
                var context = await server.GetContextAsync();
                var request = context.Request;
                var response = context.Response;
                if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/stop")
                {
                    Console.WriteLine("Получен запрос на остановку сервера.");
                    server.Stop();
                    Console.WriteLine("Сервер остановлен.");
                    break;
                }
                StreamReader reader = new StreamReader("C:\\Users\\tupae\\OneDrive\\Рабочий стол\\ITISHW\\Google\\index.html");
                string responseText = await reader.ReadToEndAsync();
                byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                response.ContentLength64 = buffer.Length;
                using Stream output = response.OutputStream;
                await output.WriteAsync(buffer);
                await output.FlushAsync();
                Console.WriteLine("Запрос обработан");
            }
        }
    }
}