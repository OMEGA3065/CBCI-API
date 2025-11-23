using CustomItemExamples.Example;
using CustomItemLib.API;
using CustomItemLib.API.Attributes;
using CustomItemLib.API.CustomWeapons;
using CustomItemLib.API.DefaultComponents;
using InventorySystem.Items.Firearms.Attachments;
using UnityEngine;

namespace CustomItemLib.Example
{
    [CustomItem(ItemType.GunE11SR)]
    [CustomItemAttributeBase(typeof(ItemLightComponent<TestWeaponInstance>))]
    [CustomItemAttributeBase(typeof(ItemSelectionHintComponent<TestWeaponInstance>))]
    public class TestWeapon : CustomFirearm<TestWeaponInstance>
    {
        public override string Name => "Testing Gun";
        public override string Description => "Unknown";
        public override string Id => "test_gun";
        public override int? MagazineSize => 100;
        public override AttachmentName[] Attachments => [
            AttachmentName.ScopeSight,
            AttachmentName.Flashlight,
            AttachmentName.RifleBody
        ];
    }

    public class TestWeaponInstance : ItemInstanceBase, IItemLight
    {
        public Color ItemLightColor => Color.blue;

        public float ItemLightRange => 4f;

        public float ItemLightIntensity => 7f;
    }
}