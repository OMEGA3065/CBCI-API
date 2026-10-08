using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;

namespace CustomItemLib.Helpers;

public static class HintHelper
{
    private static bool _rueIPresent;

    public static void Load()
    {
        try
        {
            var type = LabApi.Loader.PluginLoader.Plugins.Values.Any(
                    a => a.DefinedTypes.Any(t => t.FullName == "RueI.API.RueDisplay")
                );
            Logger.Debug($"Loaded compatibility with RueI. ({type})");
            _rueIPresent = true;
        }
        catch (Exception)
        {
            Logger.Debug("Could not load compatibility with RueI.");
            _rueIPresent = false;
        }
    }

    public static void SendHintSpecial(this Player player, string hint)
    {
        if (!_rueIPresent)
        {
            player.SendHint(hint);
            return;
        }

        RueHelpers.SetDisplay(player, "CBCI-SelectionHint", hint, 350f, 3f);
    }
}

internal static class RueHelpers
{
    public static void SetDisplay(Player player, string tag, string hint, float position, float duration)
    {
        var display = RueI.API.RueDisplay.Get(player);
        var t = new RueI.API.Elements.Tag(tag);
        display.Remove(t);
        Timing.CallDelayed(0, () =>
        {
            if (player.IsDestroyed) return;
            display = RueI.API.RueDisplay.Get(player);
            display.Show(t, new RueI.API.Elements.BasicElement(position, hint), duration);
        });
    }
}