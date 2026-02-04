using System.Reflection;
using CustomItemLib.API.Attributes;
using CustomItemLib.Helpers;
using InventorySystem;
using InventorySystem.Items.Pickups;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using PlayerRoles;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace CustomItemLib.API
{
    /// <summary>
    /// The base for most Custom Item Definitions.
    /// </summary>
    /// <typeparam name="T">An <see cref="ItemInstanceBase"/> used for each spawned item object.</typeparam>
    public abstract class CustomItemBase<T> : ICustomItem<T> where T : ItemInstanceBase
    {
        /// <inheritdoc/>
        public abstract string Name { get; }
        /// <inheritdoc/>
        public abstract string Description { get; }
        /// <inheritdoc/>
        public abstract string Id { get; }
        /// <inheritdoc/>
        public string PluginNamespace => GetType().Assembly.GetName().Name.ToSnakeCase();
        /// <inheritdoc/>
        public virtual ItemNamespace Namespace => ItemNamespace.Get(PluginNamespace, Id);

        private ItemType type = ItemType.None;
        /// <inheritdoc/>
        public virtual ItemType Type
        {
            get
            {
                if (type == ItemType.None)
                {
                    var attr = this.GetType().GetCustomAttribute<CustomItemAttribute>();
                    if (attr != null)
                    {
                        type = attr.ItemType;
                    }
                }
                return type;
            }
        }
        /// <summary>
        /// The list of attached <see cref="ICustomItemComponent{T}"/>
        /// </summary>
        public List<ICustomItemComponent<T>> ComponentAttributes = [];

        /// <summary>
        /// Inicializes a new instance of an Item Definition.
        /// </summary>
        public CustomItemBase()
        {
            ComponentAttributes = this.GetType().GetCustomAttributes<CustomItemAttributeBase>().Select(a =>
            {
                if (a.Component is not ICustomItemComponent<T> component)
                {
                    Logger.Error($"Failed to cast component of type {a.GetType()} to {typeof(ICustomItemComponent<T>)}.{(a.GetType().GetGenericArguments() != typeof(ICustomItemComponent<T>).GetGenericArguments() ? " Please check that your ICustomItemComponent<ItemInstanceBase> the ItemInstanceBase matches the one used for this item." : "")} This Component will not be added to the item and will be skipped!");
                    return null;
                }
                return component;
            }).Where(a => a != null).ToList();

            SubscribeEvents();
            ComponentAttributes.ForEach(c => c.InitComponent(this));
        }

        ~CustomItemBase()
        {
            ComponentAttributes.ForEach(c => c.DestroyComponent(this));
            UnsubscribeEvents();
        }

        /// <inheritdoc/>
        public List<ItemInstanceBase> Instances { get; } = [];

        private T CreateInstance()
        {
            var instance = Activator.CreateInstance(typeof(T));
            var typed = (T)instance;
            if (ComponentAttributes.Select(c => c.OnCreatingInstance(typed)).Any(b => !b))
            {
                return default;
            }
            Instances.Add(typed);
            ComponentAttributes.ForEach(c => c.OnCreatedInstance(typed));
            typed.Parent = this;
            return typed;
        }

        public bool TryDestroyInstance(ItemInstanceBase itemInstanceU, bool force = false)
        {
            if (itemInstanceU is not T itemInstance) return true;
            var results = ComponentAttributes.Select(c => c.OnDestroyingInstance(itemInstance));
            if (!force && results.Any(b => !b))
            {
                return false;
            }
            Instances.Remove(itemInstance);
            ComponentAttributes.ForEach(c => c.OnDestroyedInstance(itemInstance));
            return true;
        }

        /// <inheritdoc/>
        protected virtual Item CreateItem(Player player)
        {
            return CreateItem(player, 0);
        }

        /// <inheritdoc/>
        protected virtual Item CreateItem(Player player, ushort itemSerial)
        {
            return Item.Get(player.Inventory.ServerAddItem(Type, InventorySystem.Items.ItemAddReason.AdminCommand, itemSerial));
        }

        protected virtual Pickup CreatePickup(Vector3? position = null)
        {
            return CreatePickup(position, 0);
        }

        /// <inheritdoc/>
        protected virtual Pickup CreatePickup(Vector3? position, ushort itemSerial)
        {
            if (type == ItemType.None || !InventoryItemLoader.AvailableItems.TryGetValue(type, out var value))
            {
                return null;
            }

            ItemPickupBase itemPickupBase = InventoryExtensions.ServerCreatePickup(value, new PickupSyncInfo(type, value.Weight, itemSerial), position ?? Vector3.zero, Quaternion.identity, spawn: false);
            return Pickup.Get(itemPickupBase);
        }

        /// <summary>
        /// Tries to give this Item Definition's <see cref="ItemInstanceBase"/> to a specified <see cref="LabApi.Features.Wrappers.Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="LabApi.Features.Wrappers.Player"/> to which the item will be given.</param>
        /// <param name="itemSerial">The <see cref="ushort"/> item serial of the new.</param>
        /// <param name="itemInstance">The created <see cref="ItemInstanceBase"/>.</param>
        /// <returns>Whether or not the item was gives successfully.</returns>
        public bool TryGiveItem(Player player, ushort itemSerial, out T itemInstance)
        {
            itemInstance = CreateInstance();
            if (itemInstance == null) return false;
            itemInstance.Namespace = Namespace;
            var item = CreateItem(player, itemSerial);
            if (item == null)
            {
                itemInstance.Destroy(true);
                itemInstance = null;
                return false;
            }
            itemInstance.Serial = item.Serial;
            return true;
        }

        /// <summary>
        /// Tries to give this Item Definition's <see cref="ItemInstanceBase"/> to a specified <see cref="LabApi.Features.Wrappers.Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="LabApi.Features.Wrappers.Player"/> to which the item will be given.</param>
        /// <param name="itemInstance">The created <see cref="ItemInstanceBase"/>.</param>
        /// <returns>Whether or not the item was gives successfully.</returns>
        public bool TryGiveItem(Player player, out T itemInstance)
        {
            return TryGiveItem(player, 0, out itemInstance);
        }

        /// <summary>
        /// Tries to spawn this Item Definition's <see cref="ItemInstanceBase"/> at a specified <see cref="UnityEngine.Vector3"/> position.
        /// </summary>
        /// <param name="position">The <see cref="UnityEngine.Vector3"/> where the item will be spawned.</param>
        /// <param name="itemSerial">The <see cref="ushort"/> item serial of the new.</param>
        /// <param name="itemInstance">The created <see cref="ItemInstanceBase"/>.</param>
        /// <returns>Whether or not the item was spawned successfully.</returns>
        public bool TrySpawn(Vector3 position, ushort itemSerial, out T itemInstance)
        {
            itemInstance = CreateInstance();
            if (itemInstance == null) return false;
            itemInstance.Namespace = Namespace;
            var item = CreatePickup(position, itemSerial);
            if (item == null)
            {
                itemInstance.Destroy(true);
                itemInstance = null;
                return false;
            }
            itemInstance.Serial = item.Serial;
            return true;
        }

        /// <summary>
        /// Tries to spawn this Item Definition's <see cref="ItemInstanceBase"/> at a specified <see cref="UnityEngine.Vector3"/> position.
        /// </summary>
        /// <param name="position">The <see cref="UnityEngine.Vector3"/> where the item will be spawned.</param>
        /// <param name="itemInstance">The created <see cref="ItemInstanceBase"/>.</param>
        /// <returns>Whether or not the item was spawned successfully.</returns>
        public bool TrySpawn(Vector3 position, out T itemInstance)
        {
            return TrySpawn(position, 0, out itemInstance);
        }

        /// <summary>
        /// Tries to spawn this Item Definition's <see cref="ItemInstanceBase"/> at a specified <see cref="UnityEngine.Vector3"/> position without creating a new <see cref="LabApi.Features.Wrappers.Pickup"/>.
        /// </summary>
        /// <param name="position">The <see cref="UnityEngine.Vector3"/> where the item will be spawned.</param>
        /// <param name="pickup">The <see cref="LabApi.Features.Wrappers.Pickup"/> to use instead of creating a new one.</param>
        /// <param name="itemInstance">The created <see cref="ItemInstanceBase"/>.</param>
        /// <returns>Whether or not the item was spawned successfully.</returns>
        public bool TrySpawn(Vector3 position, Pickup pickup, out T itemInstance)
        {
            itemInstance = CreateInstance();
            if (itemInstance == null) return false;
            itemInstance.Namespace = Namespace;
            pickup.Position = position;
            if (!pickup.IsSpawned) pickup.Spawn();
            itemInstance.Serial = pickup.Serial;
            return true;
        }

        /// <summary>
        /// Tries to spawn this Item Definition's <see cref="ItemInstanceBase"/> at a specified <see cref="UnityEngine.Vector3"/> position without creating a new <see cref="LabApi.Features.Wrappers.Pickup"/>.
        /// </summary>
        /// <param name="position">The <see cref="UnityEngine.Vector3"/> where the item will be spawned.</param>
        /// <param name="item">The <see cref="LabApi.Features.Wrappers.Item"/> to use for creating the <see cref="LabApi.Features.Wrappers.Pickup"/>.</param>
        /// <param name="itemInstance">The created <see cref="ItemInstanceBase"/>.</param>
        /// <returns>Whether or not the item was spawned successfully.</returns>
        public bool TrySpawn(Vector3 position, Item item, out T itemInstance)
        {
            itemInstance = CreateInstance();
            if (itemInstance == null) return false;
            itemInstance.Namespace = Namespace;
            var pickup = item.DropItem();
            pickup.Position = position;
            if (!pickup.IsSpawned) pickup.Spawn();
            itemInstance.Serial = pickup.Serial;
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Check(Item item)
        {
            if (item == null) return false;
            return Check(item.Serial);
        }

        /// <inheritdoc/>
        public virtual bool Check(Pickup pickup)
        {
            return Check(pickup.Serial);
        }

        /// <inheritdoc/>
        public virtual bool Check(ushort itemSerial)
        {
            return Instances.Any(i => i.Serial == itemSerial);
        }

        /// <summary>
        /// Subscribes this Item Definition to some events. Should never have to be used.
        /// </summary>
        public virtual void SubscribeEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.ChangingRole += OnOwnerChangingRole;
            LabApi.Events.Handlers.PlayerEvents.Dying += OnOwnerDying;
            LabApi.Events.Handlers.PlayerEvents.Cuffing += OnOwnerCuffing;
            LabApi.Events.Handlers.PlayerEvents.Escaping += OnOwnerEscaping;
            LabApi.Events.Handlers.Scp914Events.ProcessingInventoryItem += OnUpgradingInventoryItem;
            LabApi.Events.Handlers.Scp914Events.ProcessingPickup += OnUpgradingPickup;
        }

        /// <summary>
        /// Unsubscribes this Item Definition from some events. Should never have to be used.
        /// </summary>
        public virtual void UnsubscribeEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.ChangingRole -= OnOwnerChangingRole;
            LabApi.Events.Handlers.PlayerEvents.Dying -= OnOwnerDying;
            LabApi.Events.Handlers.PlayerEvents.Cuffing -= OnOwnerCuffing;
            LabApi.Events.Handlers.PlayerEvents.Escaping -= OnOwnerEscaping;
            LabApi.Events.Handlers.Scp914Events.ProcessingInventoryItem -= OnUpgradingInventoryItem;
            LabApi.Events.Handlers.Scp914Events.ProcessingPickup -= OnUpgradingPickup;
        }

        /// <summary>
        /// Handles an event so that an <see cref="ItemInstanceBase"/> isn't lost.
        /// </summary>
        private void OnOwnerChangingRole(PlayerChangingRoleEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            if (ev.ChangeReason is RoleChangeReason.Escaped or RoleChangeReason.Destroyed or RoleChangeReason.LateJoin)
                return;

            List<Item> itemsCopy = [];
            ev.Player.Items.CopyTo(itemsCopy);
            foreach (Item item in itemsCopy)
            {
                if (!Check(item))
                    continue;

                Instances.RemoveAll(i => i is T typed && typed.Serial == item.Serial);

                ev.Player.RemoveItem(item);

                TrySpawn(ev.Player.Position, item, out _);
            }
        }

        /// <summary>
        /// Handles an event so that an <see cref="ItemInstanceBase"/> isn't lost.
        /// </summary>
        private void OnOwnerDying(PlayerDyingEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            List<Item> itemsCopy = [];
            ev.Player.Items.CopyTo(itemsCopy);
            foreach (Item item in itemsCopy)
            {
                if (!Check(item))
                    continue;

                ev.Player.RemoveItem(item);

                Instances.RemoveAll(i => i is T typed && typed.Serial == item.Serial);

                TrySpawn(ev.Player.Position, item, out _);
            }
        }

        /// <summary>
        /// Handles an event so that an <see cref="ItemInstanceBase"/> isn't lost.
        /// </summary>
        private void OnOwnerEscaping(PlayerEscapingEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            List<Item> itemsCopy = [];
            ev.Player.Items.CopyTo(itemsCopy);
            foreach (Item item in itemsCopy)
            {
                if (!Check(item))
                    continue;

                ev.Player.RemoveItem(item);

                Instances.RemoveAll(i => i is T typed && typed.Serial == item.Serial);

                Timing.CallDelayed(1.5f, () => TrySpawn(ev.Player.Position, item, out _));
            }
        }

        /// <summary>
        /// Handles an event so that an <see cref="ItemInstanceBase"/> isn't lost.
        /// </summary>
        private void OnOwnerCuffing(PlayerCuffingEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            List<Item> itemsCopy = [];
            ev.Player.Items.CopyTo(itemsCopy);
            foreach (Item item in itemsCopy)
            {
                if (!Check(item))
                    continue;

                ev.Target.RemoveItem(item);

                Instances.RemoveAll(i => i is T typed && typed.Serial == item.Serial);

                TrySpawn(ev.Target.Position, item, out _);
            }
        }

        /// <summary>
        /// Handles an event so that an <see cref="ItemInstanceBase"/> isn't lost.
        /// </summary>
        private void OnUpgradingInventoryItem(Scp914ProcessingInventoryItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            ev.IsAllowed = false;
        }

        /// <summary>
        /// Handles an event so that an <see cref="ItemInstanceBase"/> isn't lost.
        /// </summary>
        private void OnUpgradingPickup(Scp914ProcessingPickupEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc/>
        public bool TryGiveItem(Player player)
        {
            return TryGiveItem(player, out _);
        }

        /// <inheritdoc/>
        public bool TryGiveItem(Player player, ushort itemSerial)
        {
            return TryGiveItem(player, itemSerial, out _);
        }

        /// <inheritdoc/>
        public bool TrySpawn(Vector3 position)
        {
            return TrySpawn(position, out _);
        }

        /// <inheritdoc/>
        public bool TrySpawn(Vector3 position, Pickup item)
        {
            return TrySpawn(position, item, out _);
        }

        /// <inheritdoc/>
        public bool TrySpawn(Vector3 position, Item item)
        {
            return TrySpawn(position, item, out _);
        }

        /// <inheritdoc/>
        public bool TrySpawn(Vector3 position, ushort itemSerial)
        {
            return TrySpawn(position, itemSerial, out _);
        }
    }
}