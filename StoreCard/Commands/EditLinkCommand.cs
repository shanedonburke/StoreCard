using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCard.Models.Items.Saved;
using StoreCard.Windows;

namespace StoreCard.Commands;

internal class EditLinkCommand : IStoreCardCommand<bool>
{
    private readonly SavedLink _link;

    public EditLinkCommand(SavedLink link)
    {
        _link = link;
    }

    public bool Execute()
    {
        new EditLinkWindow(_link).Show();
        return true;
    }
}