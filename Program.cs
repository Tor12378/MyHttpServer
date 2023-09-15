
using System.IO;
using System.Net;
using System.Text;

HttpListener server = new HttpListener();
// установка адресов прослушки
server.Prefixes.Add();
server.Start(); // начинаем прослушивать входящие подключения


// получаем контекст
var context = await server.GetContextAsync();

var response = context.Response;
// отправляемый в ответ код htmlвозвращает

StreamReader reader = new StreamReader("C:\\Users\\tupae\\OneDrive\\Рабочий стол\\ITISHW\\Google\\index.html");
string responseText = await reader.ReadToEndAsync();
//
byte[] buffer = Encoding.UTF8.GetBytes(responseText);
// получаем поток ответа и пишем в него ответ
response.ContentLength64 = buffer.Length;
using Stream output = response.OutputStream;
// отправляем данные
await output.WriteAsync(buffer);
await output.FlushAsync();

Console.WriteLine("Запрос обработан");

server.Stop();
Console.WriteLine("Server ended");