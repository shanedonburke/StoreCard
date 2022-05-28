﻿using StoreCard.Windows;

namespace StoreCard.Commands;

public class AddFileCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        new AddFileWindow().Show();
        return true;
    }
}
