using System.Reflection;
using CustomItemExamples.Example;
using CustomItemLib;
using CustomItemLib.API;
using CustomItemLib.Example;
using HarmonyLib;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;

namespace CustomItemExamples
{
    public class CustomItemLibPlugin : Plugin<Config>
    {
        public static CustomItemLibPlugin Instance { get; private set; }
        public override string Name => "CBCI API Examples";
        public override string Description => "Example projects for the CBCI API.";

        public override string Author => "OMEGA3065";

        public List<ICustomItem<object>> items;

        public override Version Version => new(0, 1, 0);
        public override LoadPriority Priority => LoadPriority.Highest;
        public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);
        
        public CustomItemLibPlugin()
        {
            Instance = this;
        }

        private Harmony _harmony;

        public override void Enable()
        {
            _harmony = new Harmony("omega3065.cbciapi");
            _harmony.PatchAll();
            
            // You can register items individually
            // items = [
            //     new TestItem(),
            //     new TestCard(),
            //     new TestWeapon(),
            //     new InstantGrenade()
            // ];
            // items.ForEach(item => CustomItemManager.RegisterItem(item));

            CustomItemManager.RegisterAllItems();
        }

        public override void Disable()
        {
            CustomItemManager.UnregisterAllItems();
            _harmony.UnpatchAll();
        }
    }
}