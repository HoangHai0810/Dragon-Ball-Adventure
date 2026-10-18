using UnityEngine;

namespace AetherSurge.Combat
{
    public interface IHurtbox
    {
        bool IsInvulnerable { get; }
        bool TakeDamage(DamageContext context);
        Transform TransformRef { get; }
    }
}
