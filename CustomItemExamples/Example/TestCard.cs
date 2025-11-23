using CustomItemLib.API;
using CustomItemLib.API.Attributes;
using CustomItemLib.API.CustomKeycards;
using CustomItemLib.API.DefaultComponents;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace CustomItemExamples.Example
{
    [CustomItemAttributeBase(typeof(TestComponent))]
    [CustomItemAttributeBase(typeof(ItemSelectionHintComponent<TestKeycardInstance>))]
    public class TestCard : CustomKeycardMetalCase<TestKeycardInstance>
    {
        public override string Name => "Testing Keycard";
        public override string Description => "Unknown";
        public override string Id => "test_item";

        public override string KeycardName => Name;

        public override string Label => "TEST CARD";
    }

    public class TestKeycardInstance : ItemInstanceBase
    {
        public PrimitiveObjectToy AttachedPrimitive { get; set; }
    }
}