using UnityEngine;

namespace AetherSurge.Combat
{
    public struct DamageContext
    {
        public int Damage;
        public Vector2 Knockback;
        public float HitStopDuration;
        public bool CausesStagger;
        public GameObject Source;

        public DamageContext(int damage, Vector2 knockback, float hitStopDuration = 0.06f, bool causesStagger = true, GameObject source = null)
        {
            Damage = damage;
            Knockback = knockback;
            HitStopDuration = hitStopDuration;
            CausesStagger = causesStagger;
            Source = source;
        }
    }
}
