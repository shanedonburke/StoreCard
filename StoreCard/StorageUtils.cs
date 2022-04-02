using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCard
{
    internal class StorageUtils
    {
        public static List<SavedItem> ReadItemsFromFile()
        {
            string filePath = GetFilePath();
            var json = System.IO.File.ReadAllText(filePath);
            if (json != null)
            {
                // TODO: try-catch
                List<SavedItem>? savedItems = JsonConvert.DeserializeObject<List<SavedItem>>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
                return savedItems ?? new List<SavedItem>();
            }
            return new List<SavedItem>();
        }

        public static bool SaveItemsToFile(List<SavedItem> items)
        {
            var filePath = GetFilePath();
            var json = JsonConvert.SerializeObject(items, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            if (json != null)
            {
                System.IO.File.WriteAllText(filePath, json);
                return true;
            }
            return false;
        }

        private static string GetFilePath()
        {
            return System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "StoreCard.json");
        }
    }
}
