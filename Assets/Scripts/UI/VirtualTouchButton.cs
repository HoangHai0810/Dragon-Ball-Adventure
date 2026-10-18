using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AetherSurge.UI
{
    public class VirtualTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action OnPressed;
        public event Action OnReleased;

        private bool _isHeld;

        public bool IsHeld
        {
            get { return _isHeld; }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isHeld = true;
            if (OnPressed != null) OnPressed.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isHeld = false;
            if (OnReleased != null) OnReleased.Invoke();
        }

        private void OnDisable()
        {
            if (_isHeld)
            {
                _isHeld = false;
                if (OnReleased != null) OnReleased.Invoke();
            }
        }
    }
}
