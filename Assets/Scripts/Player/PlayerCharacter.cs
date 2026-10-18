using UnityEngine;
using AetherSurge.Core;
using AetherSurge.Combat;

namespace AetherSurge.Player
{
    [RequireComponent(typeof(PlayerLocomotion), typeof(CombatStateMachine), typeof(EnergyController))]
    public class PlayerCharacter : MonoBehaviour, IHurtbox
    {
        [Header("Combat Detection")]
        [SerializeField] private Vector2 _attackHitboxSize = new Vector2(1.8f, 1.4f);
        [SerializeField] private Vector2 _attackHitboxOffset = new Vector2(1.0f, 0f);
        [SerializeField] private LayerMask _targetLayer;
        [SerializeField] private GameObject _astralBeamPrefab;
        [SerializeField] private int _maxHealth = 200;

        private PlayerLocomotion _locomotion;
        private CombatStateMachine _combat;
        private EnergyController _energy;
        private IInputService _input;
        private int _currentHealth;
        private bool _isCharging;
        private bool _inputEnabled = true;
        private bool _isDefeated;

        public event System.Action<int, int> OnHealthChanged;
        public event System.Action OnDefeated;

        public PlayerLocomotion Locomotion
        {
            get { return _locomotion; }
        }

        public CombatStateMachine Combat
        {
            get { return _combat; }
        }

        public EnergyController Energy
        {
            get { return _energy; }
        }

        public int CurrentHealth
        {
            get { return _currentHealth; }
        }

        public int MaxHealth
        {
            get { return _maxHealth; }
        }

        public bool IsCharging
        {
            get { return _isCharging; }
        }

        public bool IsDefeated
        {
            get { return _isDefeated; }
        }

        public bool IsInputEnabled
        {
            get { return _inputEnabled; }
        }

        public bool IsInvulnerable
        {
            get { return _locomotion != null && _locomotion.IsInvulnerable; }
        }

        public LayerMask TargetLayer
        {
            get { return _targetLayer; }
            set { _targetLayer = value; }
        }

        public Transform TransformRef
        {
            get { return transform; }
        }

        private void Awake()
        {
            _locomotion = GetComponent<PlayerLocomotion>();
            _combat = GetComponent<CombatStateMachine>();
            _energy = GetComponent<EnergyController>();
            _currentHealth = _maxHealth;

            if (_targetLayer.value == 0)
            {
                int enemyLayer = LayerMask.NameToLayer("Enemy");
                if (enemyLayer != -1)
                {
                    _targetLayer = 1 << enemyLayer;
                }
            }

            _combat.OnAttackExecuted += HandleAttackExecuted;
        }

        public void SetInputService(IInputService inputService)
        {
            _input = inputService;
        }

        private void Start()
        {
            if (_input == null)
            {
                _input = GetComponent<IInputService>();
                if (_input == null)
                {
#if UNITY_2023_1_OR_NEWER
                    _input = Object.FindFirstObjectByType<MonoBehaviour>() as IInputService;
#else
                    _input = Object.FindObjectOfType<MonoBehaviour>() as IInputService;
#endif
                }
                if (_input == null)
                {
                    _input = gameObject.AddComponent<StandaloneInputService>();
                }
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            _locomotion.UpdateLocomotion(dt);
            _combat.UpdateStateMachine(dt);

            if (!_inputEnabled || _isDefeated)
            {
                _locomotion.Move(0f);
                return;
            }

            if (_input == null) return;

            if (_input.ChargeHeld && _locomotion.IsGrounded && !_combat.IsAttacking && !_locomotion.IsDodging)
            {
                _isCharging = true;
                _locomotion.Move(0f);
                _energy.Charge(dt);
            }
            else
            {
                _isCharging = false;
            }

            if (!_isCharging && !_combat.IsAttacking)
            {
                _locomotion.Move(_input.MoveVector.x);
                _locomotion.TryJump(_input.JumpTriggered, _input.JumpHeld);

                if (_input.DashTriggered)
                {
                    if (_locomotion.IsGrounded)
                    {
                        _locomotion.TryDodge();
                    }
                    else
                    {
                        _locomotion.TryAirDash();
                    }
                }
            }
            else if (_combat.IsAttacking)
            {
                _locomotion.Move(0f);
            }

            if (_input.AttackTriggered && !_isCharging)
            {
                _combat.TryAttack();
            }

            if (_input.SkillTriggered && !_isCharging && !_combat.IsAttacking && !_locomotion.IsDodging)
            {
                if (_energy.TryCastAstralBeam())
                {
                    SpawnAstralBeam();
                }
            }
        }

        private void HandleAttackExecuted(CombatStateMachine.AttackStage stage, DamageContext context)
        {
            Vector2 origin = (Vector2)transform.position + new Vector2(_attackHitboxOffset.x * _locomotion.FacingDirection, _attackHitboxOffset.y);
            Collider2D[] hits = Physics2D.OverlapBoxAll(origin, _attackHitboxSize, 0f, _targetLayer);

            bool hitAny = false;
            foreach (var hit in hits)
            {
                if (hit.gameObject == gameObject) continue;

                IHurtbox hurtbox = hit.GetComponent<IHurtbox>();
                if (hurtbox != null)
                {
                    Vector2 knockback = new Vector2(context.Knockback.x * _locomotion.FacingDirection, context.Knockback.y);
                    DamageContext directedContext = new DamageContext(context.Damage, knockback, context.HitStopDuration, context.CausesStagger, gameObject);

                    if (hurtbox.TakeDamage(directedContext))
                    {
                        hitAny = true;
                    }
                }
            }

            if (hitAny && HitStopManager.Instance != null)
            {
                HitStopManager.Instance.TriggerHitStop(context.HitStopDuration);
            }
        }

        private void SpawnAstralBeam()
        {
            Vector2 spawnPos = (Vector2)transform.position + new Vector2(1.2f * _locomotion.FacingDirection, 0.2f);
            GameObject projObj;
            if (_astralBeamPrefab != null)
            {
                projObj = Instantiate(_astralBeamPrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                projObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                projObj.transform.position = spawnPos;
                projObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                var col = projObj.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                projObj.AddComponent<AstralBeamProjectile>();
            }

            AstralBeamProjectile projectile = projObj.GetComponent<AstralBeamProjectile>();
            if (projectile != null)
            {
                projectile.Initialize(new Vector2(_locomotion.FacingDirection, 0f), gameObject);
            }
        }

        public bool TakeDamage(DamageContext context)
        {
            if (IsInvulnerable || _isDefeated)
            {
                return false;
            }

            _currentHealth = Mathf.Max(0, _currentHealth - context.Damage);
            if (OnHealthChanged != null) OnHealthChanged.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0 && !_isDefeated)
            {
                _isDefeated = true;
                _locomotion.Move(0f);
                if (OnDefeated != null) OnDefeated.Invoke();
            }

            return true;
        }

        public void SetInputEnabled(bool enabled)
        {
            _inputEnabled = enabled;
            if (!_inputEnabled && _locomotion != null)
            {
                _locomotion.Move(0f);
            }
        }

        public void ResetPlayer(Vector3 spawnPosition)
        {
            _currentHealth = _maxHealth;
            _isDefeated = false;
            _inputEnabled = true;
            _isCharging = false;
            transform.position = spawnPosition;

            var rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            if (_locomotion != null)
            {
                _locomotion.Move(0f);
                _locomotion.SetGrounded(true);
            }

            if (_combat != null)
            {
                _combat.ResetCombo();
            }

            if (_energy != null)
            {
                _energy.Initialize(50f, 100f);
            }

            if (OnHealthChanged != null) OnHealthChanged.Invoke(_currentHealth, _maxHealth);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            float dir = _locomotion != null ? _locomotion.FacingDirection : 1;
            Vector2 origin = (Vector2)transform.position + new Vector2(_attackHitboxOffset.x * dir, _attackHitboxOffset.y);
            Gizmos.DrawWireCube(origin, _attackHitboxSize);
        }
    }
}
