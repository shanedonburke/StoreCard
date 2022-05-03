using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;

namespace StoreCard.Models.Items.Installed;

internal class InstalledEpicGame : InstalledGame
{
    public InstalledEpicGame(
        string name,
        ImageSource bitmapIcon,
        string appName) : base(name, bitmapIcon)
    {
        AppName = appName;
    }

    public string AppName { get;  }

    public override SavedItem SavedItem => new SavedEpicGame(this);
}