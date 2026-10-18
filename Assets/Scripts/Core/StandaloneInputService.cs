using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace AetherSurge.Core
{
    public class StandaloneInputService : MonoBehaviour, IInputService
    {
        public Vector2 MoveVector
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                Vector2 move = Vector2.zero;
                if (Keyboard.current != null)
                {
                    if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) move.x -= 1f;
                    if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) move.x += 1f;
                    if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) move.y += 1f;
                    if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) move.y -= 1f;
                }
                if (Gamepad.current != null)
                {
                    Vector2 stick = Gamepad.current.leftStick.ReadValue();
                    if (stick.sqrMagnitude > 0.04f) move = stick;
                }
                return move;
#else
                float x = Input.GetAxisRaw("Horizontal");
                float y = Input.GetAxisRaw("Vertical");
                return new Vector2(x, y);
#endif
            }
        }

        public bool JumpTriggered
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                bool key = Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame);
                bool pad = Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;
                return key || pad;
#else
                return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W);
#endif
            }
        }

        public bool JumpHeld
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                bool key = Keyboard.current != null && (Keyboard.current.spaceKey.isPressed || Keyboard.current.wKey.isPressed);
                bool pad = Gamepad.current != null && Gamepad.current.buttonSouth.isPressed;
                return key || pad;
#else
                return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W);
#endif
            }
        }

        public bool DashTriggered
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                bool key = Keyboard.current != null && (Keyboard.current.leftShiftKey.wasPressedThisFrame || Keyboard.current.leftCtrlKey.wasPressedThisFrame || Keyboard.current.iKey.wasPressedThisFrame);
                bool pad = Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame;
                return key || pad;
#else
                return Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.I);
#endif
            }
        }

        public bool AttackTriggered
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                bool key = Keyboard.current != null && (Keyboard.current.jKey.wasPressedThisFrame || Keyboard.current.zKey.wasPressedThisFrame);
                bool mouse = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
                bool pad = Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame;
                return key || mouse || pad;
#else
                return Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0);
#endif
            }
        }

        public bool SkillTriggered
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                bool key = Keyboard.current != null && (Keyboard.current.kKey.wasPressedThisFrame || Keyboard.current.xKey.wasPressedThisFrame || Keyboard.current.uKey.wasPressedThisFrame);
                bool mouse = Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
                bool pad = Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame;
                return key || mouse || pad;
#else
                return Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.U) || Input.GetMouseButtonDown(1);
#endif
            }
        }

        public bool ChargeHeld
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                bool key = Keyboard.current != null && (Keyboard.current.lKey.isPressed || Keyboard.current.cKey.isPressed || Keyboard.current.vKey.isPressed);
                bool pad = Gamepad.current != null && (Gamepad.current.leftShoulder.isPressed || Gamepad.current.leftTrigger.isPressed);
                return key || pad;
#else
                return Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.V);
#endif
            }
        }

        public bool PauseTriggered
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                bool key = Keyboard.current != null && (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame);
                bool pad = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;
                return key || pad;
#else
                return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P);
#endif
            }
        }
    }
}
