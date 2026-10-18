using System;
using UnityEngine;

namespace AetherSurge.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerLocomotion : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 7.5f;
        [SerializeField] private float _jumpForce = 12.5f;
        [SerializeField] private float _dashSpeed = 16f;
        [SerializeField] private float _dashDuration = 0.25f;
        [SerializeField] private float _dodgeDuration = 0.40f;
        [SerializeField] private float _dodgeInvulnerabilityWindow = 0.20f;
        [SerializeField] private float _dodgeCooldown = 0.25f;

        private Rigidbody2D _rb;
        private bool _isGrounded = true;
        private bool _canAirDash = true;
        private bool _isDashing;
        private bool _isDodging;
        private bool _isInvulnerable;
        private float _stateTimer;
        private float _dodgeCooldownTimer;
        private int _facingDirection = 1;

        public bool IsGrounded
        {
            get { return _isGrounded; }
        }

        public bool CanAirDash
        {
            get { return _canAirDash; }
        }

        public bool IsDashing
        {
            get { return _isDashing; }
        }

        public bool IsDodging
        {
            get { return _isDodging; }
        }

        public bool IsInvulnerable
        {
            get { return _isInvulnerable; }
        }

        public int FacingDirection
        {
            get { return _facingDirection; }
        }

        public float MoveSpeed
        {
            get { return _moveSpeed; }
        }

        public event Action OnDodgeStarted;
        public event Action OnDodgeEnded;
        public event Action OnAirDashTriggered;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void SetGrounded(bool grounded)
        {
            _isGrounded = grounded;
            if (_isGrounded)
            {
                _canAirDash = true;
            }
        }

        public void Move(float horizontalInput)
        {
            if (_isDodging || _isDashing) return;

            float targetVelocityX = horizontalInput * _moveSpeed;
            _rb.linearVelocity = new Vector2(targetVelocityX, _rb.linearVelocity.y);

            if (horizontalInput > 0.05f)
            {
                _facingDirection = 1;
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (horizontalInput < -0.05f)
            {
                _facingDirection = -1;
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }

        public bool TryJump(bool jumpTriggered, bool jumpHeld)
        {
            if (_isDodging || _isDashing) return false;

            if (jumpTriggered && _isGrounded)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
                _isGrounded = false;
                return true;
            }

            if (!jumpHeld && _rb.linearVelocity.y > 0.5f)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * 0.6f);
            }

            return false;
        }

        public bool TryAirDash()
        {
            if (!_isGrounded && _canAirDash && !_isDashing && !_isDodging)
            {
                _canAirDash = false;
                _isDashing = true;
                _stateTimer = _dashDuration;
                _rb.gravityScale = 0f;
                _rb.linearVelocity = new Vector2(_facingDirection * _dashSpeed, 0f);
                if (OnAirDashTriggered != null) OnAirDashTriggered.Invoke();
                return true;
            }
            return false;
        }

        public bool TryDodge()
        {
            if (_dodgeCooldownTimer <= 0f && !_isDodging && !_isDashing && _isGrounded)
            {
                _isDodging = true;
                _isInvulnerable = true;
                _stateTimer = _dodgeDuration;
                _dodgeCooldownTimer = _dodgeDuration + _dodgeCooldown;
                _rb.linearVelocity = new Vector2(_facingDirection * _moveSpeed * 1.3f, _rb.linearVelocity.y);
                if (OnDodgeStarted != null) OnDodgeStarted.Invoke();
                return true;
            }
            return false;
        }

        public void UpdateLocomotion(float deltaTime)
        {
            if (_dodgeCooldownTimer > 0f)
            {
                _dodgeCooldownTimer -= deltaTime;
            }

            if (_isDodging)
            {
                _stateTimer -= deltaTime;
                float elapsed = _dodgeDuration - _stateTimer;

                if (elapsed >= _dodgeInvulnerabilityWindow)
                {
                    _isInvulnerable = false;
                }

                if (_stateTimer <= 0f)
                {
                    _isDodging = false;
                    _isInvulnerable = false;
                    if (OnDodgeEnded != null) OnDodgeEnded.Invoke();
                }
            }
            else if (_isDashing)
            {
                _stateTimer -= deltaTime;
                if (_stateTimer <= 0f)
                {
                    _isDashing = false;
                    _rb.gravityScale = 3f;
                }
            }
        }
    }
}
