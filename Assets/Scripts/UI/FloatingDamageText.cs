using System.Collections;
using UnityEngine;
using TMPro;

namespace AetherSurge.UI
{
    public class FloatingDamageText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _floatSpeed = 1.5f;
        [SerializeField] private float _lifetime = 0.8f;

        private void Awake()
        {
            if (_text == null) _text = GetComponentInChildren<TMP_Text>();
        }

        public void Initialize(int damage, Color? customColor = null)
        {
            if (_text != null)
            {
                _text.text = damage.ToString();
                if (customColor.HasValue) _text.color = customColor.Value;
            }
            StartCoroutine(AnimateAndDestroy());
        }

        private IEnumerator AnimateAndDestroy()
        {
            float elapsed = 0f;
            Vector3 startPos = transform.position;

            while (elapsed < _lifetime)
            {
                elapsed += Time.deltaTime;
                transform.position = startPos + new Vector3(0, _floatSpeed * elapsed, 0);

                if (_text != null)
                {
                    Color c = _text.color;
                    c.a = Mathf.Lerp(1f, 0f, elapsed / _lifetime);
                    _text.color = c;
                }
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
