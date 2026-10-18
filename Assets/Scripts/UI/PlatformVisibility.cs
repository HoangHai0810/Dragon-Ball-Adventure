using UnityEngine;

namespace AetherSurge.UI
{
    /// <summary>
    /// Controls GameObject visibility based on current runtime platform or debug override.
    /// Shows mobile touch controls on iOS/Android and in Unity Editor (for playtesting),
    /// while hiding them on desktop standalone builds (with optional F1 toggle).
    /// </summary>
    public class PlatformVisibility : MonoBehaviour
    {
        [Header("Platform Target")]
        [SerializeField] private bool _showOnMobile = true;
        [SerializeField] private bool _showOnDesktop = false;
        [SerializeField] private bool _showInEditor = true;

        [Header("Debug")]
        [SerializeField] private KeyCode _toggleKey = KeyCode.F1;
        [SerializeField] private bool _allowDebugToggle = true;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            ApplyPlatformVisibility();
        }

        private void Update()
        {
            if (_allowDebugToggle && Input.GetKeyDown(_toggleKey))
            {
                ToggleVisibility();
            }
        }

        public void ApplyPlatformVisibility()
        {
            bool shouldShow = false;

#if UNITY_EDITOR
            shouldShow = _showInEditor;
#elif UNITY_IOS || UNITY_ANDROID
            shouldShow = _showOnMobile;
#else
            shouldShow = _showOnDesktop;
#endif

            SetVisible(shouldShow);
        }

        public void SetVisible(bool visible)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = visible ? 1f : 0f;
                _canvasGroup.interactable = visible;
                _canvasGroup.blocksRaycasts = visible;
            }
            else
            {
                gameObject.SetActive(visible);
            }
        }

        public void ToggleVisibility()
        {
            if (_canvasGroup != null)
            {
                SetVisible(_canvasGroup.alpha < 0.5f);
            }
            else
            {
                SetVisible(!gameObject.activeSelf);
            }
        }
    }
}
