using System;
using System.Collections;
using UnityEngine;
using AetherSurge.Combat;

namespace AetherSurge.Dummy
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class TrainingDummy : MonoBehaviour, IHurtbox
    {
        [Header("Dummy Health")]
        [SerializeField] private int _maxHealth = 500;
        [SerializeField] private int _currentHealth = 500;
        [SerializeField] private bool _autoRegen = true;
        [SerializeField] private float _regenDelay = 3.0f;
        [SerializeField] private float _regenRatePerSec = 50f;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private float _flashDuration = 0.06f; // 60ms
        [SerializeField] private GameObject _damageTextPrefab;

        private Rigidbody2D _rb;
        private Color _originalColor = Color.red;
        private float _timeSinceLastDamage;
        private Vector3 _spawnPosition;

        public int CurrentHealth
        {
            get { return _currentHealth; }
        }

        public int MaxHealth
        {
            get { return _maxHealth; }
        }

        public bool IsInvulnerable
        {
            get { return false; }
        }

        public Transform TransformRef
        {
            get { return transform; }
        }

        public event Action<int, int> OnHealthChanged;
        public event Action<int, Vector2> OnDamageTaken;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null) _originalColor = _spriteRenderer.color;
            _currentHealth = _maxHealth;
            _spawnPosition = transform.position;
        }

        private void Update()
        {
            if (_autoRegen && _currentHealth < _maxHealth)
            {
                _timeSinceLastDamage += Time.deltaTime;
                if (_timeSinceLastDamage >= _regenDelay)
                {
                    _currentHealth = Mathf.Min(_maxHealth, Mathf.RoundToInt(_currentHealth + _regenRatePerSec * Time.deltaTime));
                    if (OnHealthChanged != null) OnHealthChanged.Invoke(_currentHealth, _maxHealth);
                }
            }
        }

        public bool TakeDamage(DamageContext context)
        {
            _currentHealth = Mathf.Max(0, _currentHealth - context.Damage);
            _timeSinceLastDamage = 0f;

            if (_rb != null)
            {
                _rb.linearVelocity = Vector2.zero;
                _rb.AddForce(context.Knockback, ForceMode2D.Impulse);
            }

            if (_spriteRenderer != null)
            {
                StopAllCoroutines();
                StartCoroutine(DoFlash());
            }

            SpawnDamageText(context.Damage, transform.position + new Vector3(0, 1.2f, 0));

            if (OnHealthChanged != null) OnHealthChanged.Invoke(_currentHealth, _maxHealth);
            if (OnDamageTaken != null) OnDamageTaken.Invoke(context.Damage, context.Knockback);

            return true;
        }

        private IEnumerator DoFlash()
        {
            _spriteRenderer.color = _flashColor;
            yield return new WaitForSecondsRealtime(_flashDuration);
            _spriteRenderer.color = _originalColor;
        }

        private void SpawnDamageText(int damage, Vector3 position)
        {
            if (_damageTextPrefab != null)
            {
                var textObj = Instantiate(_damageTextPrefab, position, Quaternion.identity);
                var fct = textObj.GetComponent<UI.FloatingDamageText>();
                if (fct != null) fct.Initialize(damage);
            }
            else
            {
                var go = new GameObject("DamageText_Popup");
                go.transform.position = position;
                var tmp = go.AddComponent<TMPro.TextMeshPro>();
                tmp.text = damage.ToString();
                tmp.fontSize = 5;
                tmp.alignment = TMPro.TextAlignmentOptions.Center;
                tmp.color = Color.yellow;
                var fct = go.AddComponent<UI.FloatingDamageText>();
                fct.Initialize(damage);
            }
        }

        public void ResetDummy()
        {
            transform.position = _spawnPosition;
            if (_rb != null) _rb.linearVelocity = Vector2.zero;
            _currentHealth = _maxHealth;
            if (_spriteRenderer != null) _spriteRenderer.color = _originalColor;
            if (OnHealthChanged != null) OnHealthChanged.Invoke(_currentHealth, _maxHealth);
        }
    }
}
