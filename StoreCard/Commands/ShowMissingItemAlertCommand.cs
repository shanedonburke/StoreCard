using System;
using StoreCard.Models.Items.Saved;
using StoreCard.Windows;

namespace StoreCard.Commands;

public class ShowMissingItemAlertCommand : IStoreCardCommand<bool>
{
    private readonly SavedItem _item;

    private readonly Action? _editAction;

    public ShowMissingItemAlertCommand(SavedItem item) {
        _item = item;
    }

    public ShowMissingItemAlertCommand(SavedItem item, Action editAction) : this(item)
    {
        _editAction = editAction;
    }

    public bool Execute() {
        if (_editAction != null)
        {
            new MissingItemWindow(_item, _editAction).ShowDialog();
        }
        else
        {
            new MissingItemWindow(_item).ShowDialog();
        }

        return true;
    }
}