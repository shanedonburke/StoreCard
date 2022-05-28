using System;
using System.Windows.Media.Imaging;

namespace StoreCard.Models.Items;

public interface IListBoxItem : IComparable<IListBoxItem>
{
    public string Name { get; }

    public BitmapSource? BitmapIcon { get; }
        
    public BitmapSource? PrefixIcon { get; }

    public string SecondaryText { get; }
}
