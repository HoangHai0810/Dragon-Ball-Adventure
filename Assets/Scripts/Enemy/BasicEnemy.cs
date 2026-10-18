using System;
using System.Collections;
using UnityEngine;
using AetherSurge.Combat;
using AetherSurge.Player;

namespace AetherSurge.Enemy
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Recovery,
        Defeated
    }

    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class BasicEnemy : MonoBehaviour, IHurtbox
    {
        [Header("Stats")]
        [SerializeField] private int _maxHealth = 250;
        [SerializeField] private int _currentHealth = 250;
        [SerializeField] private float _moveSpeed = 3.2f;

        [Header("Detection & Combat Ranges")]
        [SerializeField] private float _detectionRange = 10.0f;
        [SerializeField] private float _attackRange = 1.8f;
        [SerializeField] private float _attackWindup = 0.30f;
        [SerializeField] private float _attackDuration = 0.45f;
        [SerializeField] private float _attackCooldown = 1.2f;
        [SerializeField] private int _attackDamage = 20;
        [SerializeField] private Vector2 _attackKnockback = new Vector2(4.0f, 2.0f);
        [SerializeField] private Vector2 _attackHitboxSize = new Vector2(1.8f, 1.4f);
        [SerializeField] private Vector2 _attackHitboxOffset = new Vector2(1.0f, 0f);
        [SerializeField] private LayerMask _targetLayer; // Layer 6 (Player)

        [Header("Patrol Settings")]
        [SerializeField] private float _patrolDistance = 4.0f;
        [SerializeField] private float _idleDuration = 1.5f;

        [Header("Visual Feedback")]
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private float _flashDuration = 0.08f;

        private Rigidbody2D _rb;
        private Transform _targetTransform;
        private EnemyState _currentState = EnemyState.Idle;
        private float _stateTimer;
        private float _cooldownTimer;
        private int _facingDirection = -1; // -1 = left, +1 = right
        private Vector3 _spawnPosition;
        private bool _hasDealtDamageThisAttack;
        private Color _originalColor = new Color(0.9f, 0.2f, 0.2f);

        public EnemyState CurrentState
        {
            get { return _currentState; }
        }

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
            get { return _currentState == EnemyState.Defeated; }
        }

        public Transform TransformRef
        {
            get { return transform; }
        }

        public int FacingDirection
        {
            get { return _facingDirection; }
        }

        public float CooldownTimer
        {
            get { return _cooldownTimer; }
        }

        public bool IsDefeated
        {
            get { return _currentState == EnemyState.Defeated; }
        }

        public event Action<int, int> OnHealthChanged;
        public event Action<int, Vector2> OnDamageTaken;
        public event Action OnDefeated;
        public event Action<EnemyState> OnStateChanged;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_renderer == null) _renderer = GetComponentInChildren<Renderer>();
            if (_renderer != null)
            {
                if (_renderer is SpriteRenderer sr) _originalColor = sr.color;
                else if (_renderer.sharedMaterial != null) _originalColor = _renderer.sharedMaterial.color;
            }

            _currentHealth = _maxHealth;
            _spawnPosition = transform.position;
            _stateTimer = _idleDuration;

            if (_targetLayer.value == 0)
            {
                int playerLayer = LayerMask.NameToLayer("Player");
                if (playerLayer != -1)
                {
                    _targetLayer = 1 << playerLayer;
                }
                else
                {
                    _targetLayer = 1 << 6;
                }
            }
        }

        private void Start()
        {
            if (_targetTransform == null)
            {
#if UNITY_2023_1_OR_NEWER
                var player = FindFirstObjectByType<PlayerCharacter>();
#else
                var player = FindObjectOfType<PlayerCharacter>();
#endif
                if (player != null) _targetTransform = player.transform;
            }

            _stateTimer = _idleDuration;
        }

        private void Update()
        {
            UpdateState(Time.deltaTime);
        }

        /// <summary>
        /// Explicit deterministic state tick. Called automatically in Update,
        /// or can be driven manually by unit tests.
        /// </summary>
        public void UpdateState(float dt)
        {
            if (_currentState == EnemyState.Defeated)
            {
                if (_rb != null) _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
                return;
            }

            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= dt;
            }

            // Lazy find target if needed
            if (_targetTransform == null)
            {
#if UNITY_2023_1_OR_NEWER
                var player = FindFirstObjectByType<PlayerCharacter>();
#else
                var player = FindObjectOfType<PlayerCharacter>();
#endif
                if (player != null) _targetTransform = player.transform;
            }

            float distToTarget = float.MaxValue;
            if (_targetTransform != null)
            {
                distToTarget = Mathf.Abs(_targetTransform.position.x - transform.position.x);
            }

            switch (_currentState)
            {
                case EnemyState.Idle:
                    if (_rb != null) _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);

                    if (distToTarget <= _detectionRange)
                    {
                        ChangeState(EnemyState.Chase);
                    }
                    else
                    {
                        _stateTimer -= dt;
                        if (_stateTimer <= 0f)
                        {
                            _facingDirection *= -1;
                            _stateTimer = 2.0f;
                            ChangeState(EnemyState.Patrol);
                        }
                    }
                    break;

                case EnemyState.Patrol:
                    if (distToTarget <= _detectionRange)
                    {
                        ChangeState(EnemyState.Chase);
                        break;
                    }

                    if (_rb != null) _rb.linearVelocity = new Vector2(_facingDirection * (_moveSpeed * 0.6f), _rb.linearVelocity.y);

                    float distFromSpawn = transform.position.x - _spawnPosition.x;
                    if (Mathf.Abs(distFromSpawn) >= _patrolDistance)
                    {
                        _facingDirection = distFromSpawn > 0 ? -1 : 1;
                        _stateTimer = _idleDuration;
                        ChangeState(EnemyState.Idle);
                    }
                    break;

                case EnemyState.Chase:
                    if (_targetTransform != null)
                    {
                        float dir = Mathf.Sign(_targetTransform.position.x - transform.position.x);
                        _facingDirection = dir >= 0 ? 1 : -1;
                    }

                    float stopDistance = 2.2f;
                    if (distToTarget <= stopDistance)
                    {
                        if (_rb != null) _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);

                        if (_cooldownTimer <= 0f)
                        {
                            ChangeState(EnemyState.Attack);
                        }
                    }
                    else if (distToTarget > _detectionRange * 1.6f)
                    {
                        ChangeState(EnemyState.Idle);
                    }
                    else
                    {
                        if (_rb != null) _rb.linearVelocity = new Vector2(_facingDirection * _moveSpeed, _rb.linearVelocity.y);
                    }
                    break;

                case EnemyState.Attack:
                    _stateTimer -= dt;
                    float elapsedInAttack = _attackDuration - _stateTimer;

                    if (elapsedInAttack < _attackWindup)
                    {
                        // Telegraph: Flash warm orange/yellow to signal incoming attack!
                        if (_rb != null) _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
                        SetVisualColor(new Color(1f, 0.75f, 0.15f));
                    }
                    else if (!_hasDealtDamageThisAttack)
                    {
                        // Attack Impact: Lunge forward and strike!
                        SetVisualColor(new Color(1f, 0.1f, 0.1f));
                        if (_rb != null) _rb.linearVelocity = new Vector2(_facingDirection * 3.5f, _rb.linearVelocity.y);
                        ExecuteAttackHit();
                        _hasDealtDamageThisAttack = true;
                    }
                    else
                    {
                        if (_rb != null) _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
                    }

                    if (_stateTimer <= 0f)
                    {
                        _cooldownTimer = _attackCooldown;
                        _stateTimer = _attackCooldown * 0.5f;
                        ChangeState(EnemyState.Recovery);
                    }
                    break;

                case EnemyState.Recovery:
                    if (_rb != null) _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
                    // Tint darker grey to signal open punish window!
                    SetVisualColor(new Color(0.45f, 0.45f, 0.5f));

                    _stateTimer -= dt;
                    if (_stateTimer <= 0f)
                    {
                        SetVisualColor(_originalColor);
                        _hasDealtDamageThisAttack = false;
                        if (distToTarget <= _detectionRange)
                        {
                            ChangeState(EnemyState.Chase);
                        }
                        else
                        {
                            ChangeState(EnemyState.Idle);
                        }
                    }
                    break;
            }
        }

        private void SetVisualColor(Color color)
        {
            if (_renderer != null)
            {
                if (_renderer is SpriteRenderer sr) sr.color = color;
                else if (_renderer.material != null) _renderer.material.color = color;
            }
        }

        private void ExecuteAttackHit()
        {
            Vector2 origin = (Vector2)transform.position + new Vector2(_attackHitboxOffset.x * _facingDirection, _attackHitboxOffset.y);
            Collider2D[] hits = Physics2D.OverlapBoxAll(origin, _attackHitboxSize, 0f, _targetLayer);

            foreach (var hit in hits)
            {
                if (hit.gameObject == gameObject) continue;

                IHurtbox hurtbox = hit.GetComponent<IHurtbox>() ?? hit.GetComponentInParent<IHurtbox>();
                if (hurtbox != null)
                {
                    Vector2 knockback = new Vector2(_attackKnockback.x * _facingDirection, _attackKnockback.y);
                    DamageContext ctx = new DamageContext(_attackDamage, knockback, 0.08f, true, gameObject);
                    hurtbox.TakeDamage(ctx);
                }
            }
        }

        public bool TakeDamage(DamageContext context)
        {
            if (_currentState == EnemyState.Defeated)
            {
                return false;
            }

            _currentHealth = Mathf.Max(0, _currentHealth - context.Damage);

            if (_rb != null)
            {
                _rb.linearVelocity = Vector2.zero;
                _rb.AddForce(context.Knockback, ForceMode2D.Impulse);
            }

            StopAllCoroutines();
            StartCoroutine(DoFlash());

            SpawnDamageText(context.Damage, transform.position + new Vector3(0, 1.2f, 0));

            if (OnHealthChanged != null) OnHealthChanged.Invoke(_currentHealth, _maxHealth);
            if (OnDamageTaken != null) OnDamageTaken.Invoke(context.Damage, context.Knockback);

            if (_currentHealth <= 0)
            {
                ChangeState(EnemyState.Defeated);
                if (_rb != null) _rb.linearVelocity = Vector2.zero;
                if (OnDefeated != null) OnDefeated.Invoke();
            }

            return true;
        }

        private void ChangeState(EnemyState newState)
        {
            if (_currentState == newState) return;

            _currentState = newState;
            switch (_currentState)
            {
                case EnemyState.Idle:
                    _stateTimer = _idleDuration;
                    SetVisualColor(_originalColor);
                    break;
                case EnemyState.Attack:
                    _stateTimer = _attackDuration;
                    _hasDealtDamageThisAttack = false;
                    break;
                case EnemyState.Chase:
                    SetVisualColor(_originalColor);
                    break;
                case EnemyState.Defeated:
                    SetVisualColor(new Color(0.3f, 0.3f, 0.3f, 0.5f));
                    break;
            }

            if (OnStateChanged != null) OnStateChanged.Invoke(_currentState);
        }

        private IEnumerator DoFlash()
        {
            SetVisualColor(_flashColor);
            yield return new WaitForSeconds(0.08f);
            SetVisualColor(_currentState == EnemyState.Recovery ? new Color(0.45f, 0.45f, 0.5f) : _originalColor);
        }

        private void SpawnDamageText(int damage, Vector3 position)
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

        public void ResetEnemy()
        {
            _currentHealth = _maxHealth;
            _currentState = EnemyState.Idle;
            _stateTimer = _idleDuration;
            _cooldownTimer = 0f;
            _hasDealtDamageThisAttack = false;
            transform.position = _spawnPosition;

            if (_rb != null) _rb.linearVelocity = Vector2.zero;
            SetVisualColor(_originalColor);

            if (OnHealthChanged != null) OnHealthChanged.Invoke(_currentHealth, _maxHealth);
        }

        public void SetTarget(Transform target)
        {
            _targetTransform = target;
        }

        public void ForceState(EnemyState state)
        {
            ChangeState(state);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);

            Vector2 origin = (Vector2)transform.position + new Vector2(_attackHitboxOffset.x * _facingDirection, _attackHitboxOffset.y);
            Gizmos.DrawWireCube(origin, _attackHitboxSize);
        }
    }
}
