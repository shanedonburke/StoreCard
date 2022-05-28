// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace StoreCard.Static;

public static class FolderPaths
{
    public static readonly string UserProfile =
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public static readonly string CommonApplicationData =
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

    public static readonly string ApplicationData =
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    public static readonly string Windows =
        Environment.GetFolderPath(Environment.SpecialFolder.Windows);

    public static readonly string Startup =
        Environment.GetFolderPath(Environment.SpecialFolder.Startup);
}
