using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.VK.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddKeyPerFile(this IConfigurationBuilder configBuilder, string directoryPath, string filePrefix)
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();

            foreach (var fileInfo in Directory.GetFiles(directoryPath)
                                              .Select(filePath => new FileInfo(filePath)))
            {
                string filePath = fileInfo.FullName;
                string fileName = fileInfo.Name;

                string rawKey = fileName.StartsWith(filePrefix) ? fileName.Substring(filePrefix.Length) : fileName;
                string key = rawKey.Replace("__", ":");
                string value = ReadFile(filePath);

                keyValues[key] = value;
            }

            return configBuilder.AddInMemoryCollection(keyValues);
        }

        private static string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
