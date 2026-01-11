using CustomItemLib.API;
using CustomItemLib.Compat;
using HarmonyLib;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using MEC;
using Mirror;
using Utils.NonAllocLINQ;

namespace CustomItemLib
{
    public class CustomItemLibPlugin : Plugin<Config>
    {
        public static CustomItemLibPlugin Instance { get; private set; }
        public override string Name => "Component Based Custom Item API";
        public override string Description => "An API for creating and managing custom items.";

        public override string Author => "OMEGA3065";

        public override Version Version => new(0, 1, 4);
        public override LoadPriority Priority => LoadPriority.Highest;
        public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

        public bool RaCustomMenuFound = false;
        
        public CustomItemLibPlugin()
        {
            Instance = this;
        }

        private Harmony _harmony;

        private IEnumerator<float> AddPluginCompatibility(string pluginNameStart, Action onFound, float maxDuration = 5f, float waitSeconds = 0.2f)
        {
            double endTime = NetworkTime.time + maxDuration;
            while (endTime > NetworkTime.time)
            {
                if (LabApi.Loader.PluginLoader.EnabledPlugins.Any(p => p.Name.StartsWith(pluginNameStart)))
                {
                    onFound();
                    Logger.Info($"Plugin {pluginNameStart}... has been found and it's compatibility layer has been enabled!");
                    yield break;
                }
                yield return Timing.WaitForSeconds(waitSeconds);
            }
            // Logger.Warn($"Could not load {pluginNameStart}... Plugin in time. Make sure it is enabled if you want to use it.");
        }

        public override void Enable()
        {
            _harmony = new Harmony("omega3065.custom_item_lib");
            _harmony.PatchAll();

            Timing.RunCoroutine(AddPluginCompatibility("RA Custom Menu", () => {
                RaCustomMenuFound = true;
                RaCustomMenuCompat.Init();
            }));
        }

        public override void Disable()
        {
            _harmony.UnpatchAll();
            RaCustomMenuFound = false;
        }
    }
}