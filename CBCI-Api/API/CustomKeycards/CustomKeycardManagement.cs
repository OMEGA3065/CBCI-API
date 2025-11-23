using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace CustomItemLib.API.CustomKeycards
{
    /// <summary>
    /// The base for Keycards of the Management-shape.
    /// Management-shaped keycard examples: <see cref="ItemType.KeycardFacilityManager"/>, <see cref="ItemType.KeycardZoneManager"/>
    /// </summary>
    /// <typeparam name="T"><inheritdoc/></typeparam>
    public abstract class CustomKeycardManagement<T> : CustomItemBase<T> where T : ItemInstanceBase
    {
        /// <inheritdoc/>
        public override ItemType Type => ItemType.KeycardCustomManagement;

        /// <summary>
        /// The name shown in the <see cref="LabApi.Features.Wrappers.Player"/>'s inventory.
        /// </summary>
        /// <value>The keycard's name.</value>
        public abstract string KeycardName { get; }
        
        /// <summary>
        /// The name shown on the keycard's <see cref="LabApi.Features.Wrappers.Pickup"/>.
        /// </summary>
        /// <value>The keycard's label.</value>
        public abstract string Label { get; }

        /// <summary>
        /// The permissions of this keycard. No permissions by default.
        /// </summary>
        /// <value>The keycard's permissions.</value>
        public virtual KeycardLevels KeycardPermissions => new();

        /// <summary>
        /// The background color of the keycard <see cref="LabApi.Features.Wrappers.Pickup"/>.
        /// </summary>
        /// <value>The keycard's background color.</value>
        public virtual Color KeycardColor => Color.black;

        /// <summary>
        /// The color of the keycard <see cref="LabApi.Features.Wrappers.Pickup"/>'s permission circles..
        /// </summary>
        /// <value>The keycard's permission circles' color.</value>
        public virtual Color PermissionColor => Color.white;

        /// <summary>
        /// The color of the keycard <see cref="LabApi.Features.Wrappers.Pickup"/>'s <see cref="Label"/>.
        /// </summary>
        /// <value>The keycard's label's color.</value>
        public virtual Color LabelColor => Color.white;

        /// <inheritdoc/>
        protected override Item CreateItem(Player player)
        {
            return KeycardItem.CreateCustomKeycardManagement(player, KeycardName, Label, KeycardPermissions, KeycardColor, PermissionColor, LabelColor);
        }

        /// <inheritdoc/>
        protected override Pickup CreatePickup(Vector3? position = null)
        {
            return KeycardItem.CreateCustomKeycardManagement(Player.Host, KeycardName, Label, KeycardPermissions, KeycardColor, PermissionColor, LabelColor).DropItem();
        }
    }
}