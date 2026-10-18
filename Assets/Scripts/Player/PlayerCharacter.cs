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
        private Renderer _visualRenderer;
        private Color _playerBaseColor = new Color(0.2f, 0.55f, 0.95f);
        private Vector3 _baseVisualScale = Vector3.one;
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
            _visualRenderer = GetComponentInChildren<Renderer>();
            if (_visualRenderer != null)
            {
                if (_visualRenderer.sharedMaterial != null) _playerBaseColor = _visualRenderer.sharedMaterial.color;
                _baseVisualScale = _visualRenderer.transform.localScale;
            }

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
                _input = GetComponent<CompositeInputService>();
                if (_input == null) _input = GetComponent<IInputService>();
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
                UpdateChargeVisual(false);
                return;
            }

            if (_input == null) return;

            if (_input.ChargeHeld && _locomotion.IsGrounded && !_combat.IsAttacking && !_locomotion.IsDodging)
            {
                _isCharging = true;
                _locomotion.Move(0f);
                _energy.Charge(dt);
                UpdateChargeVisual(true);
            }
            else
            {
                if (_isCharging)
                {
                    _isCharging = false;
                    UpdateChargeVisual(false);
                }
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
                // Fluid minor directional adjustments during attack
                _locomotion.Move(_input.MoveVector.x * 0.4f);
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

        private void UpdateChargeVisual(bool isCharging)
        {
            if (_visualRenderer == null) return;

            if (isCharging)
            {
                float pulse = (Mathf.Sin(Time.time * 25f) + 1f) * 0.5f;
                _visualRenderer.material.color = Color.Lerp(_playerBaseColor, new Color(1f, 0.9f, 0.2f), pulse);
                _visualRenderer.transform.localScale = _baseVisualScale * (1f + pulse * 0.12f);
            }
            else
            {
                _visualRenderer.material.color = _playerBaseColor;
                _visualRenderer.transform.localScale = _baseVisualScale;
            }
        }

        private void SpawnAttackSlashEffect(CombatStateMachine.AttackStage stage)
        {
            float dir = _locomotion != null ? _locomotion.FacingDirection : 1;
            Vector3 slashPos = transform.position + new Vector3(1.1f * dir, 0.1f, 0f);
            var slashObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            slashObj.name = "AttackSlash_FX";
            slashObj.transform.position = slashPos;
            slashObj.transform.localScale = new Vector3(1.6f, 0.9f, 1f);
            if (dir < 0) slashObj.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            Object.DestroyImmediate(slashObj.GetComponent<Collider>());

            var mr = slashObj.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                mr.material.color = stage == CombatStateMachine.AttackStage.Attack4 ? new Color(1.0f, 0.85f, 0.2f, 0.95f) : new Color(0.3f, 0.85f, 1.0f, 0.9f);
            }

            var fader = slashObj.AddComponent<SlashFader>();
            fader.Init(0.12f);
        }

        private void HandleAttackExecuted(CombatStateMachine.AttackStage stage, DamageContext context)
        {
            if (_locomotion != null)
            {
                _locomotion.ApplyAttackLunge(2.2f);
            }

            SpawnAttackSlashEffect(stage);

            float dir = _locomotion != null ? _locomotion.FacingDirection : 1;
            Vector2 origin = (Vector2)transform.position + new Vector2(_attackHitboxOffset.x * dir, _attackHitboxOffset.y);
            Collider2D[] hits = Physics2D.OverlapBoxAll(origin, _attackHitboxSize, 0f, _targetLayer);

            bool hitAny = false;
            foreach (var hit in hits)
            {
                if (hit.gameObject == gameObject) continue;

                IHurtbox hurtbox = hit.GetComponent<IHurtbox>() ?? hit.GetComponentInParent<IHurtbox>();
                if (hurtbox != null)
                {
                    Vector2 knockback = new Vector2(context.Knockback.x * dir, context.Knockback.y);
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
            float dir = _locomotion != null ? _locomotion.FacingDirection : 1;
            Vector2 spawnPos = (Vector2)transform.position + new Vector2(1.2f * dir, 0.1f);
            GameObject projObj;
            if (_astralBeamPrefab != null)
            {
                projObj = Instantiate(_astralBeamPrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                projObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                projObj.name = "AstralBeam_Projectile";
                projObj.transform.position = spawnPos;
                projObj.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                projObj.transform.localScale = new Vector3(0.7f, 1.8f, 0.7f);
                Object.DestroyImmediate(projObj.GetComponent<Collider>());

                var mr = projObj.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.material.color = new Color(0.1f, 0.95f, 1.0f); // Bright glowing cyan energy
                }

                var col = projObj.AddComponent<BoxCollider2D>();
                col.size = new Vector2(2.0f, 0.9f);
                col.isTrigger = true;

                var rb = projObj.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;

                projObj.AddComponent<AstralBeamProjectile>();
            }

            AstralBeamProjectile projectile = projObj.GetComponent<AstralBeamProjectile>();
            if (projectile != null)
            {
                projectile.Initialize(new Vector2(dir, 0f), gameObject);
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
