using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using LabApi.Features.Wrappers;
using Mirror;

namespace CustomItemLib.API
{
    /// <summary>
    /// The base class for all instances of any Item from any ItemDefinition <see cref="CustomItemBase{T}"/> 
    /// </summary>
    public abstract class ItemInstanceBase
    {
        /// <summary>
        /// The parent to which this <see cref="ItemInstanceBase"/> belongs to.
        /// </summary>
        /// <value>The parent as a generic <see cref="ICustomItem{object}"/> object.</value>
        public ICustomItem<object> Parent { get; set; }

        /// <summary>
        /// The namespace of the parent to which this <see cref="ItemInstanceBase"/> belongs to.
        /// </summary>
        /// <value>The parent's <see cref="ItemNamespace"/>.</value>
        public ItemNamespace Namespace { get; set; }
        /// <summary>
        /// The <see cref="LabApi.Features.Wrappers.Item"/> or <see cref="LabApi.Features.Wrappers.Pickup"/> serial this <see cref="ItemInstanceBase"/> belongs to.
        /// </summary>
        /// <value>The item / pickup serial.</value>
        public ushort Serial { get; set; }

        /// <summary>
        /// The internal Custom Item Instance ID for this object.
        /// </summary>
        /// <value>The Instance ID.</value>
        public ushort InstanceId { get; set; } = GetNextInstanceId();

        /// <summary>
        /// Destroys this <see cref="ItemInstanceBase"/>.
        /// </summary>
        /// <param name="force">Whether to bypass any decision made by this <see cref="ItemInstanceBase"/>'s Item Definition's components.</param>
        public void Destroy(bool force)
        {
            if (CustomItemManager.TryGetItem(Namespace, out var customItem))
            {
                if (!customItem.TryDestroyInstance(this, force))
                {
                    return;
                }
            }
            if (!Item.TryGet(Serial, out var item))
            {
                if (!Pickup.TryGet(Serial, out var pickup))
                {
                    return;
                }
                pickup.Destroy();
                return;
            }
            NetworkServer.Destroy(item.GameObject);
        }
        /// <summary>
        /// Checks whether an item / pickup serial matches this <see cref="ItemInstanceBase"/>'s serial.
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        public bool Check(ushort serial)
        {
            return Serial == serial;
        }

        /// <summary>
        /// Checks whether a <see cref="LabApi.Features.Wrappers.Item"/> matches this <see cref="ItemInstanceBase"/>'s serial.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Check(Item item) => Check(item.Serial);
        
        /// <summary>
        /// Checks whether a <see cref="LabApi.Features.Wrappers.Pickup"/> matches this <see cref="ItemInstanceBase"/>'s serial.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Check(Pickup item) => Check(item.Serial);
        
        /// <summary>
        /// Checks whether a <see cref="ItemBase"/> matches this <see cref="ItemInstanceBase"/>'s serial.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Check(ItemBase item) => Check(item.ItemSerial);
        
        /// <summary>
        /// Checks whether a <see cref="ItemPickupBase"/> matches this <see cref="ItemInstanceBase"/>'s serial.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Check(ItemPickupBase item) => Check(item.Info.Serial);

        protected bool Equals(ItemInstanceBase other)
        {
            return Serial == other.Serial || InstanceId == other.InstanceId;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ItemInstanceBase)obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return InstanceId.GetHashCode();
        }

        private static ushort _lastInstanceId = 0;

        public static ushort GetNextInstanceId()
        {
            return ++_lastInstanceId;
        }
        
        public static void ResetInstanceCounter(ushort newCount = 0)
        {
            _lastInstanceId = newCount;
        }
    }
}