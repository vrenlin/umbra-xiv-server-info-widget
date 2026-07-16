using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Widgets;

namespace ServerInfoMenu.Widgets;

public sealed partial class ServerInfoMenuWidget
{
    private const string CvarNameHideNative          = "HideNative";
    private const string CvarNameCloseMenuOnClick    = "CloseMenuOnClick";
    private const string CvarNamePrefixLabelWithName = "PrefixLabelWithName";
    private const string CvarNameKnownEntries        = "KnownEntryNames";
    private const string CvarCategoryEntries         = "Server Bar Entries";
    private const string CvarPrefixEntryEnabled      = "EntryEnabled::";
    private const string CvarPrefixEntryPriority     = "EntryPriority::";

    /// <summary>
    /// Entry names are joined using this separator when persisted, since it
    /// is not expected to ever appear within a DTR entry's name.
    /// </summary>
    private const char KnownEntryNamesSeparator = '\u001F';

    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        List<IWidgetConfigVariable> variables = [
            // Always include the base variables when extending StandardToolbarWidget.
            ..base.GetConfigVariables(),

            new BooleanWidgetConfigVariable(
                CvarNameHideNative,
                "Hide native Server Info Bar",
                "Hides the game's native Server Info Bar (DTR bar) while this widget is active, since its entries are now available from this widget's menu.",
                false
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

            new StringWidgetConfigVariable(CvarNameKnownEntries, "", null, "", 0) { IsHidden = true },
        ];

        foreach (string name in _knownEntryNames.OrderBy(n => n, StringComparer.OrdinalIgnoreCase)) {
            int defaultPriority = _knownEntryNames.IndexOf(name) * 10;

            variables.Add(
                new BooleanWidgetConfigVariable(
                    EntryEnabledCvarName(name),
                    "Show entry",
                    $"Whether the \"{name}\" entry should be shown in the Server Info Menu popup.",
                    true
                ) { Category = CvarCategoryEntries, Group = name }
            );

            variables.Add(
                new IntegerWidgetConfigVariable(
                    EntryPriorityCvarName(name),
                    "Priority",
                    "Determines the sort order of this entry within the popup menu. Entries with a lower priority are shown first.",
                    defaultPriority,
                    0,
                    9999
                ) { Category = CvarCategoryEntries, Group = name }
            );
        }

        return variables;
    }

    private static string EntryEnabledCvarName(string name)  => CvarPrefixEntryEnabled + name;
    private static string EntryPriorityCvarName(string name) => CvarPrefixEntryPriority + name;

    private static string EncodeKnownEntryNames(IEnumerable<string> names) =>
        string.Join(KnownEntryNamesSeparator, names);

    private static List<string> DecodeKnownEntryNames(string? raw) =>
        string.IsNullOrEmpty(raw)
            ? []
            : raw.Split(KnownEntryNamesSeparator).Where(n => n.Length > 0).ToList();
}
