﻿using System.Windows.Media;

namespace StoreCard.Models.Items;

public interface IListBoxItem
{
    public string Name { get; }

    public ImageSource? BitmapIcon { get; }
        
    public ImageSource? PrefixIcon { get; }
}