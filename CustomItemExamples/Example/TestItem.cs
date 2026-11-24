using CustomItemLib.API;
using CustomItemLib.API.Attributes;
using CustomItemLib.API.DefaultComponents;
using CustomItemLib.Helpers;
using InventorySystem.Items;
using UnityEngine;
using Utils;

namespace CustomItemExamples.Example
{
    [CustomItem(ItemType.Medkit)]
    [CustomItemAttributeBase(typeof(ItemOutlineComponent<TestItemInstance>))]
    [CustomItemAttributeBase(typeof(ItemSelectionDisplayComponent))]
    public class TestItem : CustomItemBase<TestItemInstance>
    {
        public override string Name => "Custom Medkit";
        public override string Description => "Unknown";
        public override string Id => "custom_medkit";
    }

    public class TestItemInstance : ItemInstanceBase, IItemOutline, IItemAnimatedOutlineColor
    {
        public Color ItemOutlineColor => Color.green;

        public float ItemOutlineWidth => 3f;

        public bool ItemOutlineFull => false;

        public AnimationColorCurve ItemOutlineColorCurve => AnimationCurveHelpers.GetPhaseInOutCurve(ItemOutlineColor, 1f, 1f);
    }
}