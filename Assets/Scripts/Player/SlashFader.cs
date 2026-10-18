using UnityEngine;

namespace AetherSurge.Player
{
    public class SlashFader : MonoBehaviour
    {
        private float _lifetime = 0.12f;
        private float _timer;
        private Vector3 _initialScale;
        private Material _material;
        private Color _initialColor;

        public void Init(float lifetime)
        {
            _lifetime = lifetime;
            _timer = lifetime;
            _initialScale = transform.localScale;
            var mr = GetComponent<MeshRenderer>();
            if (mr != null && mr.material != null)
            {
                _material = mr.material;
                _initialColor = _material.color;
            }
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            float progress = Mathf.Clamp01(_timer / _lifetime);

            // Scale up slightly and flatten as it slashes
            transform.localScale = new Vector3(_initialScale.x * (1f + (1f - progress) * 0.4f), _initialScale.y * progress, _initialScale.z);

            if (_material != null)
            {
                Color c = _initialColor;
                c.a = progress;
                _material.color = c;
            }

            if (_timer <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
