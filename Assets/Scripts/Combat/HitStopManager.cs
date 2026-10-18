using System.Collections;
using UnityEngine;

namespace AetherSurge.Combat
{
    public class HitStopManager : MonoBehaviour
    {
        private static HitStopManager _instance;
        public static HitStopManager Instance
        {
            get { return _instance; }
        }

        private Coroutine _activeCoroutine;
        private bool _isFrozen;

        public bool IsFrozen
        {
            get { return _isFrozen; }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        public void TriggerHitStop(float durationRealtime, float frozenScale = 0.05f)
        {
            if (!isActiveAndEnabled) return;

            if (_activeCoroutine != null)
            {
                StopCoroutine(_activeCoroutine);
                Time.timeScale = 1.0f;
            }

            _activeCoroutine = StartCoroutine(DoHitStop(durationRealtime, frozenScale));
        }

        private IEnumerator DoHitStop(float durationRealtime, float frozenScale)
        {
            _isFrozen = true;
            Time.timeScale = frozenScale;
            yield return new WaitForSecondsRealtime(durationRealtime);
            Time.timeScale = 1.0f;
            _isFrozen = false;
            _activeCoroutine = null;
        }

        public void ResetTimeScale()
        {
            if (_activeCoroutine != null)
            {
                StopCoroutine(_activeCoroutine);
                _activeCoroutine = null;
            }
            Time.timeScale = 1.0f;
            _isFrozen = false;
        }

        private void OnDisable()
        {
            ResetTimeScale();
        }

        private void OnDestroy()
        {
            ResetTimeScale();
            if (_instance == this) _instance = null;
        }
    }
}
