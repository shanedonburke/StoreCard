#region

using StoreCard.Models.Items.Saved;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

public class EditLinkCommand : IStoreCardCommand<bool>
{
    private readonly SavedLink _link;

    public EditLinkCommand(SavedLink link) => _link = link;

    public bool Execute()
    {
        new EditLinkWindow(_link).Show();
        return true;
    }
}
