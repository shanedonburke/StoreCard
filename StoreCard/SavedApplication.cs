﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StoreCard
{
    internal class SavedApplication : SavedItem
    {
        public string AppUserModelId { get; private set; }

        public override ImageSource BitmapIcon => ImageUtils.Base64ToImage(Base64Icon);

        [JsonConstructor]
        public SavedApplication(string name, string base64Icon, string appUserModelId) : base(name, base64Icon)
        {
            AppUserModelId = appUserModelId;
        }

        public SavedApplication(InstalledApplication installedApplication) : base(
            installedApplication.Name,
            ImageUtils.ImageToBase64(installedApplication.Icon))
        {
            AppUserModelId = installedApplication.AppUserModelId;
        }

        public override void Open()
        {
            throw new NotImplementedException();
        }
    }
}
