using UnityEngine;

namespace AetherSurge.Core
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform _target;

        [Header("Follow Settings")]
        [SerializeField] private float _smoothSpeed = 8f;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 2.5f, -10f);

        [Header("Arena Bounds")]
        [SerializeField] private bool _clampHorizontal = true;
        [SerializeField] private float _minX = -10f;
        [SerializeField] private float _maxX = 10f;
        [SerializeField] private bool _clampVertical = true;
        [SerializeField] private float _minY = 0f;
        [SerializeField] private float _maxY = 10f;

        private void Start()
        {
            if (_target == null)
            {
#if UNITY_2023_1_OR_NEWER
                var player = Object.FindFirstObjectByType<Player.PlayerCharacter>();
#else
                var player = Object.FindObjectOfType<Player.PlayerCharacter>();
#endif
                if (player != null) _target = player.transform;
            }

            if (_target != null)
            {
                // Snap to target immediately on start
                Vector3 desired = _target.position + _offset;
                transform.position = ClampPosition(desired);
            }
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 desiredPosition = _target.position + _offset;
            desiredPosition = ClampPosition(desiredPosition);

            Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
            transform.position = smoothed;
        }

        private Vector3 ClampPosition(Vector3 pos)
        {
            if (_clampHorizontal)
            {
                pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
            }
            if (_clampVertical)
            {
                pos.y = Mathf.Clamp(pos.y, _minY, _maxY);
            }
            return pos;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }
    }
}
