using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace TalkGPT.Utils
{
    public static class FileHelper
    {
        private static string BasePath { get
            {
                return FileSystem.AppDataDirectory;
            } 
        }
        public static T ReadJsonFile<T>(string filePath)
        {
            var path = Path.Combine(BasePath, filePath);
            var parentPath = Path.Combine(path, "..");
            if (!Directory.Exists(parentPath))
            {
                Directory.CreateDirectory(parentPath);
            }
            if (!File.Exists(path))
            {
                return default;
            }
            else
            {
                using var fs = new FileStream(path, FileMode.Open);
                using var sr = new StreamReader(fs);
                string json = sr.ReadToEnd();
                return JsonSerializer.Deserialize<T>(json);
            }
        }
        public static void WriteJsonFile(string filePath, object obj)
        {
            var path = Path.Combine(BasePath, filePath);
            var parentPath = Path.Combine(path, "..");
            if (!Directory.Exists(parentPath))
            {
                Directory.CreateDirectory(parentPath);
            }
            using var fs = new FileStream(path, FileMode.Create);
            using var sr = new StreamWriter(fs);
            string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                }
            );
            sr.Write(json);
        }
    }
}
