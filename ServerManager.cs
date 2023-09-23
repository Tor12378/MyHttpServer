using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using MyHttpServer;
using Newtonsoft.Json;

namespace hw
{
    public class ServerManager
    {
        private readonly HttpListener _server;
        private readonly AppSettings _config;
        private bool _stopRequested;

        public ServerManager(HttpListener server, AppSettings config)
        {
            _server = server;
            _config = config;
            _stopRequested = false;
        }

        public void Start()
        {
            while (!_stopRequested)
            {
                try
                {
                    var context = _server.GetContext();
                    var request = context.Request;
                    var response = context.Response;

                    string requestUrl = request.Url.LocalPath;
                    if (requestUrl.EndsWith(".html"))
                    {
                        string filePath = Path.Combine(_config.StaticFilesPath, requestUrl.TrimStart('/'));
                        if (File.Exists(filePath))
                        {
                            byte[] buffer = File.ReadAllBytes(filePath);
                            response.ContentLength64 = buffer.Length;
                            using Stream output = response.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                            output.Flush();
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
                            output.Write(notFoundBuffer, 0, notFoundBuffer.Length);
                            output.Flush();
                            Console.WriteLine($"Файл не найден: {requestUrl}");
                        }
                    }
                    else
                    {
                        string filePath = Path.Combine(_config.StaticFilesPath, "Google.html");
                        if (File.Exists(filePath))
                        {
                            byte[] buffer = File.ReadAllBytes(filePath);
                            response.ContentLength64 = buffer.Length;
                            using Stream output = response.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                            output.Flush();
                            Console.WriteLine($"Запрос обработан: {requestUrl}");
                        }
                    }
                }

                catch (HttpListenerException ex) when (ex.ErrorCode == 995)
                {
                    break;
                }
            }
        }

        public void Stop()
        {
            Console.WriteLine("Получена команда на остановку сервера.");
            _server.Close();
            _stopRequested = true;
            Console.WriteLine("Сервер остановлен.");
        }
    }
}
