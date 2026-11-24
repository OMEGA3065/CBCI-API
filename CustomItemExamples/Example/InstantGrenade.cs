using CustomItemLib.API;
using CustomItemLib.API.Attributes;
using CustomItemLib.API.CustomWeapons;
using CustomItemLib.API.DefaultComponents;
using CustomItemLib.Helpers;
using InventorySystem.Items;
using UnityEngine;
using Utils;

namespace CustomItemExamples.Example
{
    [CustomItem(ItemType.GrenadeHE)]
    [CustomItemAttributeBase(typeof(ItemSelectionDisplayComponent))]
    [CustomItemAttributeBase(typeof(ItemOutlineComponent<TestGrenadeInstance>))]
    [CustomItemAttributeBase(typeof(ColliderExplodeComponent<TestGrenadeInstance>))]
    public class InstantGrenade : CustomGrenade<TestGrenadeInstance>
    {
        public override string Name => "Instant Grenade";
        public override string Description => "Unknown";
        public override string Id => "instant_grenade";
    }

    public class TestGrenadeInstance : ItemInstanceBase, IItemOutline, IItemAnimatedOutlineColor
    {
        public Color ItemOutlineColor => Color.blue;

        public float ItemOutlineWidth => 3f;

        public bool ItemOutlineFull => false;

        public AnimationColorCurve ItemOutlineColorCurve => AnimationCurveHelpers.GetRainbowCurve(8f);
    }
}