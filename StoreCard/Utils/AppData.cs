#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StoreCard.Models;
using StoreCard.Models.Items.Saved;
using StoreCard.Services;
using StoreCard.Static;

#endregion

namespace StoreCard.Utils;

/// <summary>
/// Utilities for interacting with stored application data, e.g., user config and saved items.
/// </summary>
public class AppData
{
    /// <summary>
    /// Path to saved items JSON file.
    /// </summary>
    private static readonly string s_itemsFilePath = Path.Combine(
        FolderPaths.ApplicationData,
        "StoreCard",
        "Items.json");

    /// <summary>
    /// Path to user config JSON file.
    /// </summary>
    private static readonly string s_configFilePath = Path.Combine(
        FolderPaths.ApplicationData,
        "StoreCard",
        "Config.json");

    private static readonly JsonSerializerSettings s_serializerSettings =
        new() {TypeNameHandling = TypeNameHandling.All};

    /// <summary>
    /// Read the list of saved items from the file system.
    /// </summary>
    /// <returns>The saved items as a list</returns>
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

    /// <summary>
    /// Read the user config from the file system.
    /// </summary>
    /// <returns>The user config</returns>
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

    /// <summary>
    /// Write the given list of saved items to the file system.
    /// </summary>
    /// <param name="items">Items to save</param>
    public static void SaveItemsToFile(List<SavedItem> items)
    {
        items.Sort();
        SerializeObjectToFile(items, s_itemsFilePath);
    }

    /// <summary>
    /// Write the given user config to the file system.
    /// </summary>
    /// <param name="config">User config to save</param>
    public static void SaveConfigToFile(UserConfig config)
    {
        SerializeObjectToFile(config, s_configFilePath);
        HotKeyService.Instance.UpdateHotKey();
    }

    /// <summary>
    /// Delete the given item, then save the new list of saved items to the file system.
    /// </summary>
    /// <param name="item"></param>
    public static void DeleteItemAndSave(SavedItem item)
    {
        var newItems = ReadItemsFromFile().Where(i => i.Id != item.Id).ToList();
        SaveItemsToFile(newItems);
    }

    /// <summary>
    /// Execute the given action to update the saved item with the given ID, then write
    /// the updated item list to the file system.
    /// </summary>
    /// <typeparam name="T">Type of the saved item to update</typeparam>
    /// <param name="id">ID of the item to update</param>
    /// <param name="updateAction">Action that updates the item</param>
    /// <returns></returns>
    public static T? UpdateSavedItemById<T>(string id, Action<T> updateAction) where T : SavedItem
    {
        List<SavedItem> savedItems = ReadItemsFromFile();

        T? item = FindSavedItemById<T>(savedItems, id);

        if (item == null)
        {
            Logger.Log($"Tried to update saved item with ID {id}, but the item was not found.");
            return null;
        }

        updateAction(item);
        SaveItemsToFile(savedItems);
        return item;
    }

    /// <summary>
    /// Find a saved item by its ID.
    /// </summary>
    /// <typeparam name="T">Type of the saved item to find</typeparam>
    /// <param name="items">List of saved items from the file system</param>
    /// <param name="id">ID of the item to find</param>
    /// <returns></returns>
    public static T? FindSavedItemById<T>(List<SavedItem> items, string id) where T : SavedItem =>
        items.FirstOrDefault(i => i.Id == id) as T;

    /// <summary>
    /// Serialize the given object as JSON and write it to the file system.
    /// </summary>
    /// <param name="objectToSave">Object to serialize</param>
    /// <param name="filePath">Absolute path to the target file</param>
    /// <exception cref="InvalidOperationException">If the file name doesn't include a directory</exception>
    private static void SerializeObjectToFile(object objectToSave, string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ??
                                  throw new InvalidOperationException($"Could not get directory name for {filePath}"));
        string json = JsonConvert.SerializeObject(objectToSave, s_serializerSettings);
        File.WriteAllText(filePath, json);
    }
}
