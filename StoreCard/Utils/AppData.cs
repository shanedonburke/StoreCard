using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StoreCard.Models;
using StoreCard.Models.Items.Saved;
using StoreCard.Services;
using StoreCard.Static;

namespace StoreCard.Utils;

internal class AppData
{
    private static readonly string s_itemsFilePath = Path.Combine(
        FolderPaths.ApplicationData,
        "StoreCard",
        "Items.json");

    private static readonly string s_configFilePath = Path.Combine(
        FolderPaths.ApplicationData,
        "StoreCard",
        "Config.json");

    public static List<SavedItem> ReadItemsFromFile()
    {
        if (!File.Exists(s_itemsFilePath))
        {
            return new List<SavedItem>();
        }

        string json = File.ReadAllText(s_itemsFilePath);
        {
            List<SavedItem>? savedItems = null;
            try
            {
                savedItems = JsonConvert.DeserializeObject<List<SavedItem>>(json,
                    new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return savedItems ?? new List<SavedItem>();
        }
    }

    public static UserConfig ReadConfigFromFile()
    {
        if (!File.Exists(s_configFilePath)) return new UserConfig();

        string json = File.ReadAllText(s_configFilePath);
        return JsonConvert.DeserializeObject<UserConfig>(json) ?? new UserConfig();
    }

    public static void SaveItemsToFile(List<SavedItem> items)
    {
        items.Sort();
        SerializeObjectToFile(items, s_itemsFilePath);
    }

    public static void SaveConfigToFile(UserConfig config)
    {
        SerializeObjectToFile(config, s_configFilePath);
        HotKeyService.Instance.UpdateHotKey();
    }

    public static void DeleteItemAndSave(SavedItem item)
    {
        var newItems = ReadItemsFromFile().Where(i => i.Id != item.Id).ToList();
        SaveItemsToFile(newItems);
    }

    public static T? UpdateSavedItemById<T>(string id, Action<T> updateAction) where T : SavedItem
    {
        var savedItems = ReadItemsFromFile();

        var item = FindSavedItemById<T>(savedItems, id);
        if (item != null)
        {
            updateAction(item);
            SaveItemsToFile(savedItems);
            return item;
        }

        Debug.WriteLine($"Tried to update saved item with ID {id}, but the item was not found.");
        return null;
    }

    public static T? FindSavedItemById<T>(List<SavedItem> items, string id) where T : SavedItem
    {
        return items.FirstOrDefault(i => i.Id == id) as T;
    }

    private static void SerializeObjectToFile(object objectToSave, string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ??
                                  throw new InvalidOperationException($"Could not get directory name for {filePath}"));
        string json = JsonConvert.SerializeObject(objectToSave,
            new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});
        File.WriteAllText(filePath, json);
    }
}
