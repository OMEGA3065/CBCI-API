using CustomItemLib.API;
using CustomItemLib.API.Attributes;
using CustomItemLib.API.DefaultComponents;
using UnityEngine;

namespace CustomItemExamples.Example
{
    [CustomItem(ItemType.Medkit)]
    [CustomItemAttributeBase(typeof(ItemLightComponent<TestItemInstance>))]
    [CustomItemAttributeBase(typeof(ItemSelectionHintComponent<TestItemInstance>))]
    public class TestItem : CustomItemBase<TestItemInstance>
    {
        public override string Name => "Custom Medkit";
        public override string Description => "Unknown";
        public override string Id => "custom_medkit";
    }

    public class TestItemInstance : ItemInstanceBase, IItemLight
    {
        public TestItemInstance()
        {
            Health = UnityEngine.Random.Range(0f, 100f);
        }

        public float Health { get; set; } = 100f;

        public Color ItemLightColor => Color.green;

        public float ItemLightRange => 4f;

        public float ItemLightIntensity => 2f;
    }
}