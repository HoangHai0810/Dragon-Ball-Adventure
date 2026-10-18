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

        public void SetMoveVector(Vector2 move) { MoveVector = move; }
        public void TriggerJump() { JumpTriggered = true; }
        public void TriggerAttack() { AttackTriggered = true; }
        public void TriggerDash() { DashTriggered = true; }
        public void TriggerSkill() { SkillTriggered = true; }

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
