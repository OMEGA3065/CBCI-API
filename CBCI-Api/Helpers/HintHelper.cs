using LabApi.Features.Console;
using LabApi.Features.Wrappers;

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

        RueHelpers.Display(player, "CBCI-SelectionHint", hint, 200f, 3f);
    }
}

internal static class RueHelpers
{
    public static void Display(Player player, string tag, string hint, float position, float duration)
    {
        var display = RueI.API.RueDisplay.Get(player);
        display.Show(new RueI.API.Elements.Tag(tag), new RueI.API.Elements.BasicElement(position, hint), duration);
    }
}