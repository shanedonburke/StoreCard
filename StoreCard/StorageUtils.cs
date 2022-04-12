using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace StoreCard;

internal class StorageUtils
{
    public static List<SavedItem> ReadItemsFromFile()
    {
        var filePath = GetFilePath();

        if (!File.Exists(filePath)) return new List<SavedItem>();

        var json = File.ReadAllText(filePath);
        {
            // TODO: try-catch
            var savedItems = JsonConvert.DeserializeObject<List<SavedItem>>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            return savedItems ?? new List<SavedItem>();
        }
    }

    public static bool SaveItemsToFile(List<SavedItem> items)
    {
        items.Sort();
        var filePath = GetFilePath();
        var json = JsonConvert.SerializeObject(items, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });
        File.WriteAllText(filePath, json);
        return true;
    }

    private static string GetFilePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "StoreCard.json");
    }
}