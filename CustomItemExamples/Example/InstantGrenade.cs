using CustomItemLib.API;
using CustomItemLib.API.Attributes;
using CustomItemLib.API.CustomWeapons;
using CustomItemLib.API.DefaultComponents;

namespace CustomItemExamples.Example
{
    [CustomItemAttributeBase(typeof(ItemSelectionHintComponent<TestGrenadeInstance>))]
    [CustomItemAttributeBase(typeof(ColliderExplodeComponent<TestGrenadeInstance>))]
    public class InstantGrenade : CustomGrenade<TestGrenadeInstance>
    {
        public override string Name => "Instant Grenade";
        public override string Description => "Unknown";
        public override string Id => "instant_grenade";
    }

    public class TestGrenadeInstance : ItemInstanceBase {}
}