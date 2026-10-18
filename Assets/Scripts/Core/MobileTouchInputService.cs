using UnityEngine;

namespace AetherSurge.Core
{
    public class MobileTouchInputService : MonoBehaviour, IInputService
    {
        [SerializeField] private Vector2 _touchMoveVector;
        private bool _jumpTriggered;
        private bool _jumpHeld;
        private bool _dashTriggered;
        private bool _attackTriggered;
        private bool _skillTriggered;
        private bool _chargeHeld;
        private bool _pauseTriggered;

        public Vector2 MoveVector
        {
            get { return _touchMoveVector; }
        }

        public bool JumpTriggered
        {
            get { return _jumpTriggered; }
        }

        public bool JumpHeld
        {
            get { return _jumpHeld; }
        }

        public bool DashTriggered
        {
            get { return _dashTriggered; }
        }

        public bool AttackTriggered
        {
            get { return _attackTriggered; }
        }

        public bool SkillTriggered
        {
            get { return _skillTriggered; }
        }

        public bool ChargeHeld
        {
            get { return _chargeHeld; }
        }

        public bool PauseTriggered
        {
            get { return _pauseTriggered; }
        }

        public void SetMoveVector(Vector2 move)
        {
            _touchMoveVector = move;
        }

        public void TriggerJump()
        {
            _jumpTriggered = true;
            _jumpHeld = true;
        }

        public void ReleaseJump()
        {
            _jumpHeld = false;
        }

        public void TriggerDash()
        {
            _dashTriggered = true;
        }

        public void TriggerAttack()
        {
            _attackTriggered = true;
        }

        public void TriggerSkill()
        {
            _skillTriggered = true;
        }

        public void SetChargeHeld(bool held)
        {
            _chargeHeld = held;
        }

        public void TriggerPause()
        {
            _pauseTriggered = true;
        }

        private void LateUpdate()
        {
            _jumpTriggered = false;
            _dashTriggered = false;
            _attackTriggered = false;
            _skillTriggered = false;
            _pauseTriggered = false;
        }
    }
}
