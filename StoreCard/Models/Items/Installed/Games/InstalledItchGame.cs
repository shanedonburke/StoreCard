﻿#region

using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.Games;
using StoreCard.Static;

#endregion

namespace StoreCard.Models.Items.Installed.Games;

/// <summary>
/// Represents an installed itch game.
/// </summary>
public sealed class InstalledItchGame : InstalledGame
{
    /// <summary>
    /// Cave ID used to launch the game.
    /// </summary>
    public readonly string CaveId;

    public InstalledItchGame(
        string name,
        BitmapSource? bitmapIcon,
        string caveId) : base(name,
        bitmapIcon) =>
        CaveId = caveId;

    public override string SecondaryText => GamePlatformNames.Itch;

    public override SavedItem SavedItem => new SavedItchGame(this);
}
