using System;
using UnityEngine;

namespace AetherSurge.Combat
{
    public class CombatStateMachine : MonoBehaviour
    {
        public enum AttackStage { None = 0, Attack1 = 1, Attack2 = 2, Attack3 = 3, Attack4 = 4 }

        [Header("Combo Timings")]
        [SerializeField] private float _attack1Duration = 0.33f;
        [SerializeField] private float _attack2Duration = 0.32f;
        [SerializeField] private float _attack3Duration = 0.42f;
        [SerializeField] private float _attack4Duration = 0.67f;
        [SerializeField] private float _comboWindow = 0.25f; // Max wait time between attacks before resetting
        [SerializeField] private float _inputBufferWindow = 0.15f; // 150ms input buffer

        [Header("Damage & Knockback Config")]
        [SerializeField] private int _attack1Damage = 20;
        [SerializeField] private int _attack2Damage = 25;
        [SerializeField] private int _attack3Damage = 35;
        [SerializeField] private int _attack4Damage = 60;
        [SerializeField] private Vector2 _attack1Knockback = new Vector2(1.5f, 0.5f);
        [SerializeField] private Vector2 _attack2Knockback = new Vector2(2.0f, 0.5f);
        [SerializeField] private Vector2 _attack3Knockback = new Vector2(2.5f, 1.0f);
        [SerializeField] private Vector2 _attack4Knockback = new Vector2(8.0f, 3.5f);

        private AttackStage _currentStage = AttackStage.None;
        private float _stageTimer;
        private float _comboTimeoutTimer;
        private float _bufferedAttackTimer;
        private bool _isAttacking;

        public AttackStage CurrentStage
        {
            get { return _currentStage; }
        }

        public bool IsAttacking
        {
            get { return _isAttacking; }
        }

        public event Action<AttackStage, DamageContext> OnAttackExecuted;
        public event Action OnComboReset;

        public void BufferAttackInput()
        {
            _bufferedAttackTimer = _inputBufferWindow;
        }

        public void UpdateStateMachine(float deltaTime)
        {
            if (_bufferedAttackTimer > 0)
            {
                _bufferedAttackTimer -= deltaTime;
            }

            if (_isAttacking)
            {
                _stageTimer -= deltaTime;
                if (_stageTimer <= 0)
                {
                    _isAttacking = false;
                    _comboTimeoutTimer = _comboWindow;
                }
            }
            else
            {
                if (_currentStage != AttackStage.None)
                {
                    _comboTimeoutTimer -= deltaTime;
                    if (_comboTimeoutTimer <= 0)
                    {
                        ResetCombo();
                    }
                }
            }

            // Check if we can transition or consume buffered input
            if (_bufferedAttackTimer > 0 && !_isAttacking)
            {
                ExecuteNextAttack();
                _bufferedAttackTimer = 0f;
            }
        }

        public bool TryAttack()
        {
            if (!_isAttacking)
            {
                ExecuteNextAttack();
                return true;
            }
            else
            {
                // Buffer input for seamless branching
                BufferAttackInput();
                return false;
            }
        }

        private void ExecuteNextAttack()
        {
            switch (_currentStage)
            {
                case AttackStage.None:
                    _currentStage = AttackStage.Attack1;
                    _stageTimer = _attack1Duration;
                    TriggerAttackEvent(AttackStage.Attack1, _attack1Damage, _attack1Knockback);
                    break;
                case AttackStage.Attack1:
                    _currentStage = AttackStage.Attack2;
                    _stageTimer = _attack2Duration;
                    TriggerAttackEvent(AttackStage.Attack2, _attack2Damage, _attack2Knockback);
                    break;
                case AttackStage.Attack2:
                    _currentStage = AttackStage.Attack3;
                    _stageTimer = _attack3Duration;
                    TriggerAttackEvent(AttackStage.Attack3, _attack3Damage, _attack3Knockback);
                    break;
                case AttackStage.Attack3:
                    _currentStage = AttackStage.Attack4;
                    _stageTimer = _attack4Duration;
                    TriggerAttackEvent(AttackStage.Attack4, _attack4Damage, _attack4Knockback);
                    break;
                case AttackStage.Attack4:
                    // After finisher, combo loops back to attack 1
                    _currentStage = AttackStage.Attack1;
                    _stageTimer = _attack1Duration;
                    TriggerAttackEvent(AttackStage.Attack1, _attack1Damage, _attack1Knockback);
                    break;
            }
            _isAttacking = true;
            _comboTimeoutTimer = _comboWindow;
        }

        private void TriggerAttackEvent(AttackStage stage, int damage, Vector2 knockback)
        {
            DamageContext ctx = new DamageContext(damage, knockback, 0.06f, true, gameObject);
            OnAttackExecuted?.Invoke(stage, ctx);
        }

        public void ResetCombo()
        {
            _currentStage = AttackStage.None;
            _isAttacking = false;
            _stageTimer = 0f;
            _comboTimeoutTimer = 0f;
            _bufferedAttackTimer = 0f;
            OnComboReset?.Invoke();
        }
    }
}
