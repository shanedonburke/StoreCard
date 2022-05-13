﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StoreCard.Models;
using StoreCard.Models.Items.Saved;

namespace StoreCard.Utils;

internal class AppData
{
    public static List<SavedItem> ReadItemsFromFile()
    {
        var filePath = GetItemsFilePath();

        if (!File.Exists(filePath)) return new List<SavedItem>();

        var json = File.ReadAllText(filePath);
        {
            List<SavedItem>? savedItems = null;
            try
            {
                savedItems = JsonConvert.DeserializeObject<List<SavedItem>>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
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
        var filePath = GetConfigFilePath();

        if (!File.Exists(filePath)) return new UserConfig();

        var json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<UserConfig>(json) ?? new UserConfig();
    }

    public static void SaveItemsToFile(List<SavedItem> items)
    {
        items.Sort();
        SerializeObjectToFile(items, GetItemsFilePath());
    }

    public static void SaveConfigToFile(UserConfig config)
    {
        SerializeObjectToFile(config, GetConfigFilePath());
    }

    public static void DeleteItemAndSave(SavedItem item)
    {
        var newItems = ReadItemsFromFile().Where(i => i.Id != item.Id).ToList();
        SaveItemsToFile(newItems);
    }

    private static void SerializeObjectToFile(object objectToSave, string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ??
                                  throw new InvalidOperationException($"Could not get directory name for {filePath}"));
        var json = JsonConvert.SerializeObject(objectToSave, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });
        File.WriteAllText(filePath, json);
    }

    private static string GetItemsFilePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "StoreCard",
            "Items.json");
    }

    private static string GetConfigFilePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "StoreCard",
            "Config.json");
    }
}