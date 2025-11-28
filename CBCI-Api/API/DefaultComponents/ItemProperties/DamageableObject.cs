using AdminToys;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;

namespace WackyGrenades.CustomItems.Components
{
    public class DamageableObject : MonoBehaviour, IDestructible
    {
        private PrimitiveObjectToy primitive;
        private ReferenceHub playerHolder;
        private Action<float> applyDamage;

        public uint NetworkId
        {
            get
            {
                return netId;
            }
        }
        private uint netId;

        public Vector3 CenterOfMass => primitive.transform.position;

        public void Init(PrimitiveObjectToy primitive, ReferenceHub playerHolder, Action<float> applyDamage, uint? netId = null)
        {
            this.primitive = primitive;
            this.playerHolder = playerHolder;
            this.applyDamage = applyDamage;
            this.netId = netId ?? primitive.netId;
        }

        public void Update()
        {
            if (!NetworkServer.active || primitive == null || playerHolder == null)
            {
                Destroy(this);
                return;
            }
        }

        public bool Damage(float damage, DamageHandlerBase handler, Vector3 exactHitPos)
        {
            if (
                handler is not AttackerDamageHandler attackerDamageHandler
                || (attackerDamageHandler.Attacker.Hub != playerHolder
                && !HitboxIdentity.IsDamageable(
                    attackerDamageHandler.Attacker.Role,
                    playerHolder.GetRoleId()
                ))
            )
            {
                return false;
            }

            if (attackerDamageHandler?.Attacker.Hub is not null)
            {
                Hitmarker.SendHitmarkerDirectly(attackerDamageHandler.Attacker.Hub, 1f, true);
            }

            applyDamage(damage);

            return true;
        }
    }
}