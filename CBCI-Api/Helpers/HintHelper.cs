using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using RueI.API;
using RueI.API.Elements;

namespace CustomItemLib.Helpers;

public static class HintHelper
{
    private static bool _rueIPresent = false;

    public static void Load()
    {
        try
        {
            var type = typeof(RueDisplay);
            Logger.Debug($"Loaded compatibility with RueI. ({type.Name})");
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

        var display = RueDisplay.Get(player);
        display.Show(new Tag("CBCI-SelectionHint"), new BasicElement(200f, hint), 3f);
    }
}