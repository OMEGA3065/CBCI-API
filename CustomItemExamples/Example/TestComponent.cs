using AdminToys;
using CustomItemLib.API.DefaultComponents;
using InventorySystem.Items.Pickups;
using MEC;
using UnityEngine;
using PrimitiveObjectToy = LabApi.Features.Wrappers.PrimitiveObjectToy;

namespace CustomItemExamples.Example;

public class TestComponent : ComponentBase<TestKeycardInstance>
{
    public override void SubscribeEvents(TestKeycardInstance itemInstance)
    {
        ItemPickupBase.OnPickupAdded += GetEvent<ItemPickupBase>(itemInstance, OnPickupCreated, "OnPickupCreated");
        ItemPickupBase.OnPickupDestroyed += GetEvent<ItemPickupBase>(itemInstance, OnPickupDestroyed, "OnPickupDestroyed");
    }

    public override void UnsubscribeEvents(TestKeycardInstance itemInstance)
    {
        ItemPickupBase.OnPickupAdded -= GetEvent<ItemPickupBase>(itemInstance, OnPickupCreated, "OnPickupCreated");
        ItemPickupBase.OnPickupDestroyed -= GetEvent<ItemPickupBase>(itemInstance, OnPickupDestroyed, "OnPickupDestroyed");
    }

    public void OnPickupCreated(ItemPickupBase pickup, TestKeycardInstance itemInstance)
    {
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (!itemInstance.Check(pickup)) return;
            var primitive = PrimitiveObjectToy.Create(pickup.transform, false)!;
            primitive.Type = PrimitiveToyType.Sphere;
            primitive.Color = Color.red;
            primitive.Scale = new Vector3(0.2f, 0.2f, 0.2f);
            primitive.Flags = PrimitiveFlags.Visible;
            primitive.Transform.localPosition = Vector3.zero;
            itemInstance.AttachedPrimitive = primitive;
            primitive.Spawn();
        });
    }

    public void OnPickupDestroyed(ItemPickupBase pickup, TestKeycardInstance itemInstance)
    {
        if (!itemInstance.Check(pickup)) return;
        itemInstance.AttachedPrimitive?.Destroy();
    }
}
