﻿using System;
using System.Diagnostics;

namespace StoreCard
{
    internal class SavedFolder : SavedItem
    {
        public string Path { get; }

        public SavedFolder(string name, string? base64Icon, string path) : base(name, base64Icon) {
            Path = path;
        }

        public override ItemCategory Category => ItemCategory.Folder;

        public override void Open() {
            ProcessUtils.OpenInDefaultProgram(Path);
        }
    }
}