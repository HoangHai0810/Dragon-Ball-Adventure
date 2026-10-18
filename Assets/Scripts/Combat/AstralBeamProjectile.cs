using UnityEngine;

namespace AetherSurge.Combat
{
    public class AstralBeamProjectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 22f;
        [SerializeField] private int _damage = 55;
        [SerializeField] private Vector2 _knockback = new Vector2(4f, 1.5f);
        [SerializeField] private float _lifetime = 3.0f;

        private Vector2 _direction = Vector2.right;
        private GameObject _owner;

        public void Initialize(Vector2 direction, GameObject owner)
        {
            _direction = direction.normalized;
            _owner = owner;
            Destroy(gameObject, _lifetime);
        }

        private void Update()
        {
            transform.Translate((Vector3)(_direction * _speed * Time.deltaTime), Space.World);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_owner != null && (collision.gameObject == _owner || collision.transform.IsChildOf(_owner.transform))) return;

            IHurtbox hurtbox = collision.GetComponent<IHurtbox>();
            if (hurtbox != null)
            {
                DamageContext ctx = new DamageContext(_damage, new Vector2(_direction.x * _knockback.x, _knockback.y), 0.08f, true, _owner);
                hurtbox.TakeDamage(ctx);
                Destroy(gameObject);
                return;
            }

            // Destroy upon impacting solid environment / arena boundaries
            if (collision.gameObject.layer == 8)
            {
                Destroy(gameObject);
            }
        }
    }
}
