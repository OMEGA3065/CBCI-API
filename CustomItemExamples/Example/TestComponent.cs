using CustomItemLib.API.DefaultComponents;
using InventorySystem.Items.Pickups;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;

namespace CustomItemExamples.Example;

public class TestComponent : ComponentBase<TestKeycardInstance>
{
    public override void SubscribeEvents(TestKeycardInstance itemInstance)
    {
        ItemPickupBase.OnPickupAdded += (ev) => OnPickupCreated(ev, itemInstance);
        ItemPickupBase.OnPickupDestroyed += (ev) => OnPickupDestroyed(ev, itemInstance);
    }

    public override void UnsubscribeEvents(TestKeycardInstance itemInstance)
    {
        ItemPickupBase.OnPickupAdded -= (ev) => OnPickupCreated(ev, itemInstance);
        ItemPickupBase.OnPickupDestroyed -= (ev) => OnPickupDestroyed(ev, itemInstance);
    }

    public void OnPickupCreated(ItemPickupBase pickup, TestKeycardInstance itemInstance)
    {
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (!itemInstance.Check(pickup)) return;
            var primitive = PrimitiveObjectToy.Create(pickup.transform, false);
            primitive.Type = PrimitiveType.Sphere;
            primitive.Color = Color.red;
            primitive.Scale = new(0.2f, 0.2f, 0.2f);
            primitive.Flags = AdminToys.PrimitiveFlags.Visible;
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
