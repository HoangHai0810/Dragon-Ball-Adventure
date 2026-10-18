using UnityEngine;
using AetherSurge.Core;

namespace AetherSurge.UI
{
    public class MobileControlsUI : MonoBehaviour
    {
        [Header("Touch Inputs")]
        [SerializeField] private MobileTouchInputService _touchService;
        [SerializeField] private VirtualJoystick _joystick;
        [SerializeField] private VirtualTouchButton _attackButton;
        [SerializeField] private VirtualTouchButton _jumpButton;
        [SerializeField] private VirtualTouchButton _dashButton;
        [SerializeField] private VirtualTouchButton _skillButton;
        [SerializeField] private VirtualTouchButton _chargeButton;

        public MobileTouchInputService TouchService
        {
            get { return _touchService; }
            set { _touchService = value; }
        }

        private void Start()
        {
            if (_touchService == null)
            {
#if UNITY_2023_1_OR_NEWER
                _touchService = Object.FindFirstObjectByType<MobileTouchInputService>();
#else
                _touchService = Object.FindObjectOfType<MobileTouchInputService>();
#endif
            }

            if (_joystick != null)
            {
                _joystick.OnValueChanged += HandleJoystickMoved;
            }

            if (_attackButton != null)
            {
                _attackButton.OnPressed += HandleAttackPressed;
            }

            if (_jumpButton != null)
            {
                _jumpButton.OnPressed += HandleJumpPressed;
                _jumpButton.OnReleased += HandleJumpReleased;
            }

            if (_dashButton != null)
            {
                _dashButton.OnPressed += HandleDashPressed;
            }

            if (_skillButton != null)
            {
                _skillButton.OnPressed += HandleSkillPressed;
            }

            if (_chargeButton != null)
            {
                _chargeButton.OnPressed += HandleChargePressed;
                _chargeButton.OnReleased += HandleChargeReleased;
            }
        }

        private void OnDestroy()
        {
            if (_joystick != null) _joystick.OnValueChanged -= HandleJoystickMoved;
            if (_attackButton != null) _attackButton.OnPressed -= HandleAttackPressed;
            if (_jumpButton != null)
            {
                _jumpButton.OnPressed -= HandleJumpPressed;
                _jumpButton.OnReleased -= HandleJumpReleased;
            }
            if (_dashButton != null) _dashButton.OnPressed -= HandleDashPressed;
            if (_skillButton != null) _skillButton.OnPressed -= HandleSkillPressed;
            if (_chargeButton != null)
            {
                _chargeButton.OnPressed -= HandleChargePressed;
                _chargeButton.OnReleased -= HandleChargeReleased;
            }
        }

        private void HandleJoystickMoved(Vector2 move)
        {
            if (_touchService != null) _touchService.SetMoveVector(move);
        }

        private void HandleAttackPressed()
        {
            if (_touchService != null) _touchService.TriggerAttack();
        }

        private void HandleJumpPressed()
        {
            if (_touchService != null) _touchService.TriggerJump();
        }

        private void HandleJumpReleased()
        {
            if (_touchService != null) _touchService.ReleaseJump();
        }

        private void HandleDashPressed()
        {
            if (_touchService != null) _touchService.TriggerDash();
        }

        private void HandleSkillPressed()
        {
            if (_touchService != null) _touchService.TriggerSkill();
        }

        private void HandleChargePressed()
        {
            if (_touchService != null) _touchService.SetChargeHeld(true);
        }

        private void HandleChargeReleased()
        {
            if (_touchService != null) _touchService.SetChargeHeld(false);
        }

        public void BindControls(
            MobileTouchInputService service,
            VirtualJoystick joystick,
            VirtualTouchButton attack,
            VirtualTouchButton jump,
            VirtualTouchButton dash,
            VirtualTouchButton skill,
            VirtualTouchButton charge)
        {
            _touchService = service;
            _joystick = joystick;
            _attackButton = attack;
            _jumpButton = jump;
            _dashButton = dash;
            _skillButton = skill;
            _chargeButton = charge;
        }
    }
}
