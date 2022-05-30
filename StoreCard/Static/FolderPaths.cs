#region

using System;

#endregion

namespace StoreCard.Static;

/// <summary>
/// Paths to common system folders.
/// </summary>
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
