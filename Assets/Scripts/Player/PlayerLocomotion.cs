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

        [Header("Ground Detection")]
        [SerializeField] private LayerMask _groundLayer = 1 << 8; // Layer 8 = Ground
        [SerializeField] private float _groundCheckDistance = 0.15f;
        [SerializeField] private float _groundCheckWidth = 0.8f;
        [SerializeField] private float _coyoteTimeDuration = 0.08f;

        private Rigidbody2D _rb;
        private BoxCollider2D _collider;
        private bool _isGrounded = true;
        private bool _wasGroundedLastFrame = true;
        private bool _canAirDash = true;
        private bool _isDashing;
        private bool _isDodging;
        private bool _isInvulnerable;
        private float _stateTimer;
        private float _dodgeCooldownTimer;
        private float _coyoteTimer;
        private int _facingDirection = 1;
        private float _defaultGravityScale = 3.5f;
        private bool _useAutoGroundDetection = true;

        public bool IsGrounded
        {
            get { return _isGrounded; }
        }

        public bool CanJump
        {
            get { return _isGrounded || _coyoteTimer > 0f; }
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

        public LayerMask GroundLayer
        {
            get { return _groundLayer; }
            set { _groundLayer = value; }
        }

        public event Action OnDodgeStarted;
        public event Action OnDodgeEnded;
        public event Action OnAirDashTriggered;
        public event Action OnLanded;
        public event Action OnBecameAirborne;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _defaultGravityScale = _rb.gravityScale;
        }

        /// <summary>
        /// Manually set grounded state. When called, disables automatic ground detection
        /// for that frame to allow unit tests to control state directly.
        /// </summary>
        public void SetGrounded(bool grounded)
        {
            bool wasGrounded = _isGrounded;
            _isGrounded = grounded;
            _useAutoGroundDetection = false;

            if (_isGrounded && !wasGrounded)
            {
                _canAirDash = true;
                if (OnLanded != null) OnLanded.Invoke();
            }
            else if (!_isGrounded && wasGrounded)
            {
                _coyoteTimer = _coyoteTimeDuration;
                if (OnBecameAirborne != null) OnBecameAirborne.Invoke();
            }
        }

        /// <summary>
        /// Enables automatic ground detection via physics raycasts.
        /// Called automatically each FixedUpdate, but can be toggled for tests.
        /// </summary>
        public void EnableAutoGroundDetection(bool enable)
        {
            _useAutoGroundDetection = enable;
        }

        private void FixedUpdate()
        {
            if (_useAutoGroundDetection && _collider != null)
            {
                PerformGroundCheck();
            }
        }

        private void PerformGroundCheck()
        {
            if (_isDashing || _collider == null) return;

            bool wasGrounded = _isGrounded;
            bool groundDetected = false;

            Vector2 boxCenter = (Vector2)transform.position + _collider.offset + Vector2.down * (_collider.size.y * 0.5f + _groundCheckDistance * 0.5f);
            Vector2 boxSize = new Vector2(_groundCheckWidth, _groundCheckDistance);

            Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0f, _groundLayer);
            groundDetected = hit != null;

            // Only accept ground if vertical velocity is not strongly upward (avoid grounding during jump ascent)
            if (groundDetected && _rb.linearVelocity.y > 1.0f)
            {
                groundDetected = false;
            }

            _isGrounded = groundDetected;

            if (_isGrounded && !wasGrounded)
            {
                // Just landed
                _canAirDash = true;
                _coyoteTimer = 0f;
                if (OnLanded != null) OnLanded.Invoke();
            }
            else if (!_isGrounded && wasGrounded)
            {
                // Just became airborne (walked off edge or jumped)
                _coyoteTimer = _coyoteTimeDuration;
                if (OnBecameAirborne != null) OnBecameAirborne.Invoke();
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

            // Allow jump during coyote time
            bool canJump = _isGrounded || _coyoteTimer > 0f;

            if (jumpTriggered && canJump)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
                _isGrounded = false;
                _coyoteTimer = 0f; // Consume coyote time
                return true;
            }

            // Variable jump height: releasing jump button cuts upward velocity
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

            if (_coyoteTimer > 0f)
            {
                _coyoteTimer -= deltaTime;
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
                    _rb.gravityScale = _defaultGravityScale;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize ground check area
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col != null)
            {
                Vector2 boxCenter = (Vector2)transform.position + col.offset + Vector2.down * (col.size.y * 0.5f + _groundCheckDistance * 0.5f);
                Vector2 boxSize = new Vector2(_groundCheckWidth, _groundCheckDistance);
                Gizmos.DrawWireCube(boxCenter, boxSize);
            }
        }
    }
}
