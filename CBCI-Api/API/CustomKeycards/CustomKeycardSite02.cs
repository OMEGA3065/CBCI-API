using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace CustomItemLib.API.CustomKeycards
{
    /// <summary>
    /// The base for Keycards of the Site02-shape. This is the most basic keycard shape.
    /// Site02-shaped keycard examples: <see cref="ItemType.KeycardJanitor"/>, <see cref="ItemType.KeycardScientist"/>, <see cref="ItemType.KeycardResearchCoordinator"/>
    /// </summary>
    /// <typeparam name="T"><inheritdoc/></typeparam>
    public abstract class CustomKeycardSite02<T> : CustomItemBase<T> where T : ItemInstanceBase
    {
        /// <inheritdoc/>
        public override ItemType Type => ItemType.KeycardCustomSite02;

        /// <summary>
        /// The name shown in the <see cref="LabApi.Features.Wrappers.Player"/>'s inventory.
        /// </summary>
        /// <value>The keycard's name.</value>
        public abstract string KeycardName { get; }

        /// <summary>
        /// The name shown in the keycard's <see cref="LabApi.Features.Wrappers.Pickup"/>'s <c>Card Holder</c> row.
        /// </summary>
        /// <value>The name of the keycard's holder.</value>
        public virtual string KeycardHolder => "";
        
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
        
        /// <summary>
        /// The amount of wear the keycard's <see cref="LabApi.Features.Wrappers.Pickup"/> will show.
        /// </summary>
        /// <value>The amount of visible wear on the keycard.</value>
        public virtual byte WearLevel => 0;

        /// <inheritdoc/>
        protected override Item CreateItem(Player player)
        {
            return KeycardItem.CreateCustomKeycardSite02(player, KeycardName, KeycardHolder, Label, KeycardPermissions, KeycardColor, PermissionColor, LabelColor, WearLevel);
        }

        /// <inheritdoc/>
        protected override Pickup CreatePickup(Vector3? position = null)
        {
            return KeycardItem.CreateCustomKeycardSite02(Player.Host, KeycardName, KeycardHolder, Label, KeycardPermissions, KeycardColor, PermissionColor, LabelColor, WearLevel).DropItem();
        }
    }
}