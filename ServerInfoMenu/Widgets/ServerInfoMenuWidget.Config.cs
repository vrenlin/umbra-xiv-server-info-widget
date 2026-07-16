using System.Collections.Generic;
using Umbra.Widgets;

namespace ServerInfoMenu.Widgets;

public sealed partial class ServerInfoMenuWidget
{
    private const string CvarNameHideNative          = "HideNative";
    private const string CvarNameCloseMenuOnClick    = "CloseMenuOnClick";
    private const string CvarNamePrefixLabelWithName = "PrefixLabelWithName";

    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            // Always include the base variables when extending StandardToolbarWidget.
            ..base.GetConfigVariables(),

            new BooleanWidgetConfigVariable(
                CvarNameHideNative,
                "Hide native Server Info Bar",
                "Hides the game's native Server Info Bar (DTR bar) while this widget is active, since its entries are now available from this widget's menu.",
                true
            ) { Category = "Server Info Menu" },

            new BooleanWidgetConfigVariable(
                CvarNameCloseMenuOnClick,
                "Close menu after clicking an entry",
                "Closes the popup menu automatically after clicking an interactable entry. Disable this to keep the menu open, for example to click multiple entries in a row.",
                true
            ) { Category = "Server Info Menu" },

            new BooleanWidgetConfigVariable(
                CvarNamePrefixLabelWithName,
                "Prefix entry text with its name",
                "Prefixes each menu entry's text with its name, e.g. \"Wrath Combo : On\" instead of just \": On\".",
                true
            ) { Category = "Server Info Menu" },
        ];
    }
}
