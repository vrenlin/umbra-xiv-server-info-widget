using System.Collections.Generic;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Interface;
using Umbra.Common;
using Umbra.Game;
using Umbra.Widgets;

namespace ServerInfoMenu.Widgets;

/// <summary>
/// A toolbar widget that mirrors the native Server Info Bar (DTR bar), but
/// instead of laying every entry out in the toolbar, it collapses them
/// behind a single button. Clicking it opens a dynamic menu populated with
/// every currently active server info bar entry, each of which remains
/// fully interactable (click, tooltip, etc.) just like the native bar.
/// </summary>
[ToolbarWidget(
    "ServerInfoMenu",
    "Server Info Menu",
    "Collapses the Server Info Bar (DTR bar) into a single button that opens a dynamic menu with all of its entries.",
    ["dtr", "server", "info", "bar", "menu"]
)]
public sealed partial class ServerInfoMenuWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.SubText |
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    protected override string          DefaultIconType        => IconTypeFontAwesome;
    protected override FontAwesomeIcon DefaultFontAwesomeIcon => FontAwesomeIcon.Bars;

    /// <inheritdoc/>
    public override MenuPopup Popup { get; } = new();

    private IDtrBarEntryRepository? _repository;

    private readonly Dictionary<string, MenuPopup.Button> _buttons = [];

    /// <inheritdoc/>
    protected override void OnLoad()
    {
        _repository = Framework.Service<IDtrBarEntryRepository>();

        _repository.OnEntryAdded   += OnEntryAdded;
        _repository.OnEntryRemoved += OnEntryRemoved;
        _repository.OnEntryUpdated += OnEntryUpdated;

        foreach (var entry in _repository.GetEntries()) OnEntryAdded(entry);

        SetText("Server Info");
    }

    /// <inheritdoc/>
    protected override void OnDraw()
    {
        UpdateNativeServerInfoBarVisibility();

        // Some plugins update their DTR entry's text every tick without
        // firing the "OnEntryUpdated" event, so refresh visible buttons
        // every frame to keep them in sync, mirroring the native bar.
        int visibleCount = 0;

        foreach ((string name, MenuPopup.Button button) in _buttons) {
            var entry = _repository?.Get(name);
            if (entry is null) continue;

            ApplyEntryToButton(entry, button);

            if (entry.IsVisible) visibleCount++;
        }

        SetSubText(visibleCount == 1 ? "1 entry" : $"{visibleCount} entries");
    }

    /// <inheritdoc/>
    protected override void OnUnload()
    {
        if (_repository is not null) {
            _repository.OnEntryAdded   -= OnEntryAdded;
            _repository.OnEntryRemoved -= OnEntryRemoved;
            _repository.OnEntryUpdated -= OnEntryUpdated;
        }

        Popup.Clear(true);
        _buttons.Clear();

        SetNativeServerInfoBarVisibility(true);
    }

    private void OnEntryAdded(DtrBarEntry entry)
    {
        if (_buttons.ContainsKey(entry.Name)) {
            OnEntryUpdated(entry);
            return;
        }

        var button = new MenuPopup.Button(entry.Name);

        ApplyEntryToButton(entry, button);

        _buttons.Add(entry.Name, button);
        Popup.Add(button);
    }

    private void OnEntryRemoved(DtrBarEntry entry)
    {
        if (!_buttons.TryGetValue(entry.Name, out var button)) return;

        Popup.Remove(button, true);
        _buttons.Remove(entry.Name);
    }

    private void OnEntryUpdated(DtrBarEntry entry)
    {
        if (!_buttons.TryGetValue(entry.Name, out var button)) {
            OnEntryAdded(entry);
            return;
        }

        ApplyEntryToButton(entry, button);
    }

    private void ApplyEntryToButton(DtrBarEntry entry, MenuPopup.Button button)
    {
        string? text = entry.Text?.TextValue;

        button.IsVisible = entry.IsVisible;
        button.SortIndex = entry.SortIndex;
        button.Label     = BuildLabel(entry.Name, text);
        button.AltText   = string.Empty;

        button.Node.Tooltip = entry.TooltipText?.TextValue;

        button.IsDisabled        = !entry.IsInteractive;
        button.ClosePopupOnClick = GetConfigValue<bool>(CvarNameCloseMenuOnClick);

        button.OnClick = entry.IsInteractive
            ? () => entry.InvokeClickAction(MouseClickType.Left, ClickModifierKeys.None)
            : null;
    }

    private string BuildLabel(string name, string? text)
    {
        if (string.IsNullOrEmpty(text)) return name;

        if (!GetConfigValue<bool>(CvarNamePrefixLabelWithName)) return text;

        string value = text.TrimStart(':', ' ').Trim();

        return string.IsNullOrEmpty(value) ? name : $"{name} : {value}";
    }
}
