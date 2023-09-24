using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer
{
    public class ServerConfigManager
    {
        public static AppSettings LoadServerConfig()
        {
            try
            {
                string appSettingsPath = "./appsettings.json";
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
            string projectDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string staticFolderPath = Path.Combine(projectDirectory, config.StaticFilesPath);

            if (!Directory.Exists(staticFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(staticFolderPath);
                    Console.WriteLine($"Создана папка для статических файлов: {staticFolderPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при создании папки для статических файлов: {ex.Message}");
                }
            }
        }
    }
}
