using UnityEngine;

namespace AetherSurge.Core
{
    public class MockInputService : IInputService
    {
        public Vector2 MoveVector { get; set; }
        public bool JumpTriggered { get; set; }
        public bool JumpHeld { get; set; }
        public bool DashTriggered { get; set; }
        public bool AttackTriggered { get; set; }
        public bool SkillTriggered { get; set; }
        public bool ChargeHeld { get; set; }
        public bool PauseTriggered { get; set; }

        public void ResetInputs()
        {
            MoveVector = Vector2.zero;
            JumpTriggered = false;
            JumpHeld = false;
            DashTriggered = false;
            AttackTriggered = false;
            SkillTriggered = false;
            ChargeHeld = false;
            PauseTriggered = false;
        }
    }
}
