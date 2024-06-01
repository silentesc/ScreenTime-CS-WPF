﻿using Newtonsoft.Json;
using ScreenTime.classes;
using System.IO;

namespace ScreenTime.utils
{
    internal class StorageUtils
    {
        private static readonly string directoryPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/ScreenTime";
        private static readonly string filePath = $"{directoryPath}/apps.json";

        public static void SaveAppsToFile(List<ScreenTimeApp> apps)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string json = JsonConvert.SerializeObject(apps);
            File.WriteAllText(filePath, json);
        }

        public static void LoadAppsFromFile()
        {
            if (!File.Exists(filePath)) return;

            string json = File.ReadAllText(filePath);
            JsonConvert.DeserializeObject<List<ScreenTimeApp>>(json);
        }
    }
}
