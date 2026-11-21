using CustomItemLib.DisplayKit;
using CustomItemLib.Helpers;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;

namespace CustomItemLib.API.DefaultComponents;

/// <summary>
/// A component used for giving the attached <see cref="CustomItemBase{T}"/>'s <see cref="LabApi.Features.Wrappers.Item"/> an on-screen display for the player who is holding the item.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public class ItemSelectionDisplayComponent : ComponentBase<ItemInstanceBase>
{
    private const float DisplayTimeSeconds = 3f;

    /// <inheritdoc/>
    public override void SubscribeEvents(ItemInstanceBase itemInstance)
    {
        base.SubscribeEvents(itemInstance);
        PlayerEvents.ChangedItem += GetLabEvent<PlayerChangedItemEventArgs>(itemInstance, OnOwnerChangedItem, "ownerChangedItem");
    }

    /// <inheritdoc/>
    public override void UnsubscribeEvents(ItemInstanceBase itemInstance)
    {
        base.UnsubscribeEvents(itemInstance);
        PlayerEvents.ChangedItem -= GetLabEvent<PlayerChangedItemEventArgs>(itemInstance, OnOwnerChangedItem, "ownerChangedItem");
    }

    private static readonly Dictionary<Player, CanvasHandler<SelectedItemDisplay>> SavedReferences = [];

    internal static void Init()
    {
        ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
        PlayerEvents.Left += OnPlayerLeft;
    }

    private static void OnPlayerLeft(PlayerLeftEventArgs ev)
    {
        SavedReferences.Remove(ev.Player);
    }

    private static void OnWaitingForPlayers()
    {
        SavedReferences.Clear();
    }

    protected void OnOwnerChangedItem(PlayerChangedItemEventArgs ev, ItemInstanceBase instance)
    {
        if (ev.NewItem is null) return;
        if (ev.NewItem.Serial != instance.Serial)
        {
            if (!SavedReferences.TryGetValue(ev.Player, out var handler2) || !ReferenceEquals(handler2.Reference.LastOwner, this))
                return;
            // no need for ::Effect() since Canvas is already hidden by default if not constructed yet.
            handler2.Reference.Canvas.Hide();
            return;
        }

        if (!SavedReferences.TryGetValue(ev.Player, out var handler))
            SavedReferences[ev.Player] = handler = new CanvasHandler<SelectedItemDisplay>(new SelectedItemDisplay(ev.Player));

        var reference = handler.Reference;
        handler.Effect(() =>
        {
            reference.Item = instance.Parent.Name.ToUpper();
            reference.ItemColor = instance.Parent.NameColor;
            reference.Description = instance.Parent.Description;
            reference.LastOwner = instance;
            reference.Canvas.Show();

            Timing.CallDelayed(DisplayTimeSeconds, () =>
            {
                if (!ReferenceEquals(reference.LastOwner, instance))
                    return;

                reference.Canvas.Hide();
            }, ev.Player.GameObject);
        });
    }
}
