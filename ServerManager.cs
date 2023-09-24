using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading;
using MyHttpServer;
using Newtonsoft.Json;

namespace MyHttpServer
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

                    if (requestUrl.StartsWith("/static/") && (requestUrl.EndsWith(".html") || requestUrl.EndsWith(".png") || requestUrl.EndsWith(".svg")))
                    {
                        string filePath = Path.Combine(_config.StaticFilesPath, requestUrl.Substring(8));
                        if (File.Exists(filePath))
                        {
                            byte[] buffer = File.ReadAllBytes(filePath);
                            response.ContentLength64 = buffer.Length;
                            string contentType;
                            switch (Path.GetExtension(requestUrl).ToLower())
                            {
                                case ".html":
                                    contentType = "text/html; charset=utf-8";
                                    break;
                                case ".png":
                                    contentType = "image/png";
                                    break;
                                case ".svg":
                                    contentType = "image/svg+xml";
                                    break;
                                default:
                                    contentType = "text/plain; charset=utf-8"; // Значение по умолчанию для неизвестных типов файлов
                                    break;
                            }
                            response.ContentType = contentType;
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
                        string filePath = Path.Combine(_config.StaticFilesPath, "google.html");
                        if (File.Exists(filePath))
                        {
                            byte[] buffer = File.ReadAllBytes(filePath);
                            response.ContentLength64 = buffer.Length;
                            response.ContentType = "text/html; charset=utf-8"; 
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
