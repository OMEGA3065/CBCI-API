using LabApi.Features.Wrappers;
using UnityEngine;

namespace CustomItemLib.API
{
    /// <summary>
    /// The base interface defining the base features of an Item Definition. <see cref="CustomItemBase{T}"/>
    /// </summary>
    /// <typeparam name="T">The <see cref="ItemInstanceBase"/> to use for this Item Definition.</typeparam>
    public interface ICustomItem<out T>
    {
        /// <summary>
        /// The name of this Item.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The description of this Item.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The ID part of this Item's namespace (ex. pluginNamespace:ID).
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The <see cref="ItemNamespace"/> of this Item.
        /// </summary>
        public ItemNamespace Namespace { get; }

        /// <summary>
        /// The <see cref="ItemType"/> of this Item.
        /// </summary>
        /// <value></value>
        public ItemType Type { get; }

        /// <summary>
        /// The list of <see cref="ItemInstanceBase"/> handled by this Item.
        /// </summary>
        public List<ItemInstanceBase> Instances { get; }

        /// <summary>
        /// Tries to destroy the specified <see cref="ItemInstanceBase"/>.
        /// </summary>
        /// <param name="itemInstanceU">The <see cref="ItemInstanceBase"/> to destroy.</param>
        /// <param name="force">Forces the destruction of the item instance bypassing any decisions made by it's components.</param>
        /// <returns>Whether or not the <see cref="ItemInstanceBase"/> has been destroyed successfully.</returns>
        public bool TryDestroyInstance(ItemInstanceBase itemInstance, bool force = false);

        /// <summary>
        /// Checks whether or not the specified <see cref="LabApi.Features.Wrappers.Item"/> is an instance of this Item Definition.
        /// </summary>
        /// <param name="item">The <see cref="LabApi.Features.Wrappers.Item"/> to check.</param>
        /// <returns>Whether or not the <see cref="LabApi.Features.Wrappers.Item"/> was an instance of this Item Definition.</returns>
        public bool Check(Item item);

        /// <summary>
        /// Checks whether or not the specified <see cref="LabApi.Features.Wrappers.Pickup"/> is an instance of this Item Definition.
        /// </summary>
        /// <param name="pickup">The <see cref="LabApi.Features.Wrappers.Pickup"/> to check.</param>
        /// <returns>Whether or not the <see cref="LabApi.Features.Wrappers.Pickup"/> was an instance of this Item Definition.</returns>
        public bool Check(Pickup pickup);

        /// <summary>
        /// Checks whether or not the specified <see cref="ushort"/> item serial is an instance of this Item Definition.
        /// </summary>
        /// <param name="itemSerial">The <see cref="ushort"/> item serial to check.</param>
        /// <returns>Whether or not the <see cref="ushort"/> item serial was an instance of this Item Definition.</returns>
        public bool Check(ushort itemSerial);

        /// <summary>
        /// Tries to give this Item Definition's <see cref="ItemInstanceBase"/> to a specified <see cref="LabApi.Features.Wrappers.Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="LabApi.Features.Wrappers.Player"/> to which the item will be given.</param>
        /// <returns>Whether or not the item was given successfully.</returns>
        public bool TryGiveItem(Player player);

        /// <summary>
        /// Tries to give this Item Definition's <see cref="ItemInstanceBase"/> to a specified <see cref="LabApi.Features.Wrappers.Player"/> with a specific <see cref="ushort"/> item serial.
        /// </summary>
        /// <param name="player">The <see cref="LabApi.Features.Wrappers.Player"/> to which the item will be given.</param>
        /// <param name="itemSerial">The <see cref="ushort"/> item serial to use for the given item.</param>
        /// <returns>Whether or not the item was given successfully.</returns>
        public bool TryGiveItem(Player player, ushort itemSerial);

        /// <summary>
        /// Tries to spawn this Item Definition's <see cref="ItemInstanceBase"/> at a specified <see cref="UnityEngine.Vector3"/> position.
        /// </summary>
        /// <param name="position">The <see cref="UnityEngine.Vector3"/> where the item will be spawned.</param>
        /// <returns>Whether or not the item was spawned successfully.</returns>
        public bool TrySpawn(Vector3 position);

        /// <summary>
        /// Tries to spawn this Item Definition's <see cref="ItemInstanceBase"/> at a specified <see cref="UnityEngine.Vector3"/> position without creating a new <see cref="LabApi.Features.Wrappers.Pickup"/>.
        /// </summary>
        /// <param name="position">The <see cref="UnityEngine.Vector3"/> where the item will be spawned.</param>
        /// <param name="item">The <see cref="LabApi.Features.Wrappers.Pickup"/> to use instead of creating a new one.</param>
        /// <returns>Whether or not the item was spawned successfully.</returns>
        public bool TrySpawn(Vector3 position, Pickup item);

        /// <summary>
        /// Tries to spawn this Item Definition's <see cref="ItemInstanceBase"/> at a specified <see cref="UnityEngine.Vector3"/> position without creating a new <see cref="LabApi.Features.Wrappers.Pickup"/>.
        /// </summary>
        /// <param name="position">The <see cref="UnityEngine.Vector3"/> where the item will be spawned.</param>
        /// <param name="item">The <see cref="LabApi.Features.Wrappers.Item"/> to use for creating the <see cref="LabApi.Features.Wrappers.Pickup"/>.</param>
        /// <returns>Whether or not the item was spawned successfully.</returns>
        public bool TrySpawn(Vector3 position, Item item);

        /// <summary>
        /// Tries to spawn this Item Definition's <see cref="ItemInstanceBase"/> at a specified <see cref="UnityEngine.Vector3"/> position without creating a new <see cref="LabApi.Features.Wrappers.Pickup"/>.
        /// </summary>
        /// <param name="position">The <see cref="UnityEngine.Vector3"/> where the item will be spawned.</param>
        /// <param name="itemSerial">The <see cref="ushort"/> item serial to use for creating the <see cref="LabApi.Features.Wrappers.Pickup"/>.</param>
        /// <returns>Whether or not the item was spawned successfully.</returns>
        public bool TrySpawn(Vector3 position, ushort itemSerial);
    }
}