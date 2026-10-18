using UnityEngine;

namespace AetherSurge.Core
{
    public interface IInputService
    {
        Vector2 MoveVector { get; }
        bool JumpTriggered { get; }
        bool JumpHeld { get; }
        bool DashTriggered { get; }
        bool AttackTriggered { get; }
        bool SkillTriggered { get; }
        bool ChargeHeld { get; }
        bool PauseTriggered { get; }
    }
}
