using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StoreCard.Models;
using StoreCard.Models.Items.Saved;
using StoreCard.Services;
using StoreCard.Static;

namespace StoreCard.Utils;

public class AppData
{
    private static readonly string s_itemsFilePath = Path.Combine(
        FolderPaths.ApplicationData,
        "StoreCard",
        "Items.json");

    private static readonly string s_configFilePath = Path.Combine(
        FolderPaths.ApplicationData,
        "StoreCard",
        "Config.json");

    private static readonly JsonSerializerSettings s_serializerSettings =
        new() {TypeNameHandling = TypeNameHandling.All};

    public static List<SavedItem> ReadItemsFromFile()
    {
        if (!File.Exists(s_itemsFilePath))
        {
            return new List<SavedItem>();
        }

        string json = File.ReadAllText(s_itemsFilePath);
        List<SavedItem>? savedItems = null;

        try
        {
            savedItems = JsonConvert.DeserializeObject<List<SavedItem>>(json, s_serializerSettings);
        }
        catch (JsonSerializationException e)
        {
            Logger.LogExceptionMessage("Failed to deserialize list of saved items", e);
        }

        return savedItems ?? new List<SavedItem>();
    }

    public static UserConfig ReadConfigFromFile()
    {
        if (!File.Exists(s_configFilePath))
        {
            return new UserConfig();
        }

        string json = File.ReadAllText(s_configFilePath);
        UserConfig? config = null;

        try
        {
            config = JsonConvert.DeserializeObject<UserConfig>(json);
        }
        catch (JsonSerializationException e)
        {
            Logger.LogExceptionMessage("Failed to deserialize user config", e);
        }

        return config ?? new UserConfig();
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
        List<SavedItem> savedItems = ReadItemsFromFile();

        T? item = FindSavedItemById<T>(savedItems, id);
        if (item != null)
        {
            updateAction(item);
            SaveItemsToFile(savedItems);
            return item;
        }

        Logger.Log($"Tried to update saved item with ID {id}, but the item was not found.");
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
        string json = JsonConvert.SerializeObject(objectToSave, s_serializerSettings);
        File.WriteAllText(filePath, json);
    }
}
