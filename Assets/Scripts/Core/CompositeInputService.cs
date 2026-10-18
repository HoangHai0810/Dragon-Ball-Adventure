using UnityEngine;

namespace AetherSurge.Core
{
    public class CompositeInputService : MonoBehaviour, IInputService
    {
        [SerializeField] private StandaloneInputService _standalone;
        [SerializeField] private MobileTouchInputService _mobile;

        public Vector2 MoveVector
        {
            get
            {
                Vector2 s = _standalone != null ? _standalone.MoveVector : Vector2.zero;
                Vector2 m = _mobile != null ? _mobile.MoveVector : Vector2.zero;
                return s.sqrMagnitude > m.sqrMagnitude ? s : m;
            }
        }

        public bool JumpTriggered
        {
            get { return (_standalone != null && _standalone.JumpTriggered) || (_mobile != null && _mobile.JumpTriggered); }
        }

        public bool JumpHeld
        {
            get { return (_standalone != null && _standalone.JumpHeld) || (_mobile != null && _mobile.JumpHeld); }
        }

        public bool DashTriggered
        {
            get { return (_standalone != null && _standalone.DashTriggered) || (_mobile != null && _mobile.DashTriggered); }
        }

        public bool AttackTriggered
        {
            get { return (_standalone != null && _standalone.AttackTriggered) || (_mobile != null && _mobile.AttackTriggered); }
        }

        public bool SkillTriggered
        {
            get { return (_standalone != null && _standalone.SkillTriggered) || (_mobile != null && _mobile.SkillTriggered); }
        }

        public bool ChargeHeld
        {
            get { return (_standalone != null && _standalone.ChargeHeld) || (_mobile != null && _mobile.ChargeHeld); }
        }

        public bool PauseTriggered
        {
            get { return (_standalone != null && _standalone.PauseTriggered) || (_mobile != null && _mobile.PauseTriggered); }
        }

        public void Initialize(StandaloneInputService standalone, MobileTouchInputService mobile)
        {
            _standalone = standalone;
            _mobile = mobile;
        }

        private void Awake()
        {
            if (_standalone == null) _standalone = GetComponent<StandaloneInputService>();
            if (_mobile == null) _mobile = GetComponent<MobileTouchInputService>();
        }
    }
}
