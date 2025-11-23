using CustomItemLib.API;
using HarmonyLib;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;

namespace CustomItemLib
{
    public class CustomItemLibPlugin : Plugin<Config>
    {
        public static CustomItemLibPlugin Instance { get; private set; }
        public override string Name => "Component Based Custom Item API";
        public override string Description => "An API for creating and managing custom items.";

        public override string Author => "OMEGA3065";

        public override Version Version => new(0, 1, 0);
        public override LoadPriority Priority => LoadPriority.High;
        public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);
        
        public CustomItemLibPlugin()
        {
            Instance = this;
        }

        private Harmony _harmony;

        public override void Enable()
        {
            _harmony = new Harmony("omega3065.custom_item_lib");
            _harmony.PatchAll();
        }

        public override void Disable()
        {
            _harmony.UnpatchAll();
        }
    }
}