using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AetherSurge.UI
{
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _background;
        [SerializeField] private RectTransform _handle;
        [SerializeField] private float _handleRange = 60f;

        private Vector2 _inputVector = Vector2.zero;

        public Vector2 InputVector
        {
            get { return _inputVector; }
        }

        public event Action<Vector2> OnValueChanged;

        private void Awake()
        {
            if (_background == null) _background = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_background == null) return;

            Vector2 position;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_background, eventData.position, eventData.pressEventCamera, out position))
            {
                float bgWidth = _background.sizeDelta.x > 0 ? _background.sizeDelta.x : 120f;
                float bgHeight = _background.sizeDelta.y > 0 ? _background.sizeDelta.y : 120f;

                position.x = (position.x / bgWidth) * 2f;
                position.y = (position.y / bgHeight) * 2f;

                _inputVector = new Vector2(position.x, position.y);
                _inputVector = (_inputVector.magnitude > 1.0f) ? _inputVector.normalized : _inputVector;

                if (_handle != null)
                {
                    _handle.anchoredPosition = new Vector2(_inputVector.x * _handleRange, _inputVector.y * _handleRange);
                }

                if (OnValueChanged != null) OnValueChanged.Invoke(_inputVector);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ResetJoystick();
        }

        public void ResetJoystick()
        {
            _inputVector = Vector2.zero;
            if (_handle != null)
            {
                _handle.anchoredPosition = Vector2.zero;
            }
            if (OnValueChanged != null) OnValueChanged.Invoke(Vector2.zero);
        }

        private void OnDisable()
        {
            ResetJoystick();
        }
    }
}
