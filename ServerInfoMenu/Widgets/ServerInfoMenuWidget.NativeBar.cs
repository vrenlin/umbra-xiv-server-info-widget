using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Umbra;
using Umbra.Common;

namespace ServerInfoMenu.Widgets;

public sealed unsafe partial class ServerInfoMenuWidget
{
    private IGameGui? _gameGui;

    private void UpdateNativeServerInfoBarVisibility()
    {
        _gameGui ??= Framework.Service<IGameGui>();

        if (!GetConfigValue<bool>(CvarNameHideNative)) {
            SetNativeServerInfoBarVisibility(true);
            return;
        }

        SetNativeServerInfoBarVisibility(!Framework.Service<UmbraVisibility>().IsToolbarVisible());
    }

    private void SetNativeServerInfoBarVisibility(bool isVisible)
    {
        _gameGui ??= Framework.Service<IGameGui>();

        var dtrBar = (AtkUnitBase*) _gameGui.GetAddonByName("_DTR").Address;
        if (dtrBar != null && dtrBar->IsVisible != isVisible) {
            dtrBar->IsVisible = isVisible;
        }
    }
}
