using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace CustomItemLib.API.CustomKeycards
{
    /// <summary>
    /// The base for Keycards of the TaskForce-shape.
    /// TaskForce-shaped keycard examples: <see cref="ItemType.KeycardMTFPrivate"/>, <see cref="ItemType.KeycardMTFOperative"/>, <see cref="ItemType.KeycardMTFCaptain"/>
    /// </summary>
    /// <typeparam name="T"><inheritdoc/></typeparam>
    public abstract class CustomKeycardTaskForce<T> : CustomItemBase<T> where T : ItemInstanceBase
    {
        /// <inheritdoc/>
        public override ItemType Type => ItemType.KeycardCustomTaskForce;

        /// <summary>
        /// The name shown in the <see cref="LabApi.Features.Wrappers.Player"/>'s inventory.
        /// </summary>
        /// <value>The keycard's name.</value>
        public abstract string KeycardName { get; }

        /// <summary>
        /// The name shown BELOW the keycard's <see cref="LabApi.Features.Wrappers.Pickup"/>'s <c>Card Holder</c> row.
        /// </summary>
        /// <value>The name of the keycard's holder.</value>
        public virtual string KeycardHolder => "";

        /// <summary>
        /// The <see cref="string"/> shown in the keycard's <see cref="LabApi.Features.Wrappers.Pickup"/>'s <c>Card Holder</c> row.
        /// </summary>
        /// <value>The <see cref="string"/> seen next to the <c>Keycard Holder</c> tag.</value>
        public virtual string SerialLabel => "0000000000000";

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
        /// The number of circles around the hole in the keycard's <see cref="LabApi.Features.Wrappers.Pickup"/>.
        /// </summary>
        /// <value>The keycard's number of circles seen on it around the hole.</value>
        public virtual int RankIndex => 0;

        /// <inheritdoc/>
        protected override Item CreateItem(Player player)
        {
            return KeycardItem.CreateCustomKeycardTaskForce(player, KeycardName, KeycardHolder, KeycardPermissions, KeycardColor, PermissionColor, SerialLabel, RankIndex);
        }

        /// <inheritdoc/>
        protected override Pickup CreatePickup(Vector3? position = null)
        {
            return KeycardItem.CreateCustomKeycardTaskForce(Player.Host, KeycardName, KeycardHolder, KeycardPermissions, KeycardColor, PermissionColor, SerialLabel, RankIndex).DropItem();
        }
    }
}