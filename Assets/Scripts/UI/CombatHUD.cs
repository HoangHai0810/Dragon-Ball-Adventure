using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AetherSurge.Player;
using AetherSurge.Dummy;

namespace AetherSurge.UI
{
    public class CombatHUD : MonoBehaviour
    {
        [Header("Player References")]
        [SerializeField] private PlayerCharacter _player;
        [SerializeField] private Slider _playerHpSlider;
        [SerializeField] private Slider _energySlider;
        [SerializeField] private TMP_Text _energyText;

        [Header("Dummy References")]
        [SerializeField] private TrainingDummy _dummy;
        [SerializeField] private Slider _dummyHpSlider;
        [SerializeField] private TMP_Text _dummyHpText;

        [Header("UI Controls")]
        [SerializeField] private Button _resetArenaButton;
        [SerializeField] private TMP_Text _fpsText;

        private float _fpsTimer;
        private int _frameCount;

        private void Start()
        {
            if (_player == null)
            {
#if UNITY_2023_1_OR_NEWER
                _player = Object.FindFirstObjectByType<PlayerCharacter>();
#else
                _player = Object.FindObjectOfType<PlayerCharacter>();
#endif
            }

            if (_dummy == null)
            {
#if UNITY_2023_1_OR_NEWER
                _dummy = Object.FindFirstObjectByType<TrainingDummy>();
#else
                _dummy = Object.FindObjectOfType<TrainingDummy>();
#endif
            }

            if (_resetArenaButton == null)
            {
                var resetObj = GameObject.Find("Btn_ResetArena");
                if (resetObj != null) _resetArenaButton = resetObj.GetComponent<Button>();
            }

            if (_fpsText == null)
            {
                var fpsObj = GameObject.Find("Text_FPS");
                if (fpsObj != null) _fpsText = fpsObj.GetComponent<TMP_Text>();
            }

            if (_energyText == null)
            {
                var energyObj = GameObject.Find("Text_Energy");
                if (energyObj != null) _energyText = energyObj.GetComponent<TMP_Text>();
            }

            if (_dummyHpText == null)
            {
                var dummyHpObj = GameObject.Find("Text_DummyHP");
                if (dummyHpObj != null) _dummyHpText = dummyHpObj.GetComponent<TMP_Text>();
            }

            if (_resetArenaButton != null)
            {
                _resetArenaButton.onClick.AddListener(ResetArena);
            }
        }

        public void BindControls(Button resetButton, TMP_Text fpsText, TMP_Text energyText = null, TMP_Text dummyHpText = null)
        {
            _resetArenaButton = resetButton;
            _fpsText = fpsText;
            _energyText = energyText;
            _dummyHpText = dummyHpText;
        }

        private void Update()
        {
            _frameCount++;
            _fpsTimer += Time.unscaledDeltaTime;
            if (_fpsTimer >= 0.5f)
            {
                int fps = Mathf.RoundToInt(_frameCount / _fpsTimer);
                if (_fpsText != null) _fpsText.text = string.Format("FPS: {0}", fps);
                _frameCount = 0;
                _fpsTimer = 0f;
            }

            if (_player != null)
            {
                if (_playerHpSlider != null)
                {
                    _playerHpSlider.maxValue = _player.MaxHealth;
                    _playerHpSlider.value = _player.CurrentHealth;
                }
                if (_energySlider != null && _player.Energy != null)
                {
                    _energySlider.maxValue = _player.Energy.MaxEnergy;
                    _energySlider.value = _player.Energy.CurrentEnergy;
                }
                if (_energyText != null && _player.Energy != null)
                {
                    _energyText.text = string.Format("Energy: {0} / {1}", Mathf.RoundToInt(_player.Energy.CurrentEnergy), _player.Energy.MaxEnergy);
                }
            }

            if (_dummy != null)
            {
                if (_dummyHpSlider != null)
                {
                    _dummyHpSlider.maxValue = _dummy.MaxHealth;
                    _dummyHpSlider.value = _dummy.CurrentHealth;
                }
                if (_dummyHpText != null)
                {
                    _dummyHpText.text = string.Format("Dummy HP: {0} / {1}", _dummy.CurrentHealth, _dummy.MaxHealth);
                }
            }
        }

        public void ResetArena()
        {
            if (_dummy == null)
            {
#if UNITY_2023_1_OR_NEWER
                _dummy = Object.FindFirstObjectByType<TrainingDummy>();
#else
                _dummy = Object.FindObjectOfType<TrainingDummy>();
#endif
            }
            if (_player == null)
            {
#if UNITY_2023_1_OR_NEWER
                _player = Object.FindFirstObjectByType<PlayerCharacter>();
#else
                _player = Object.FindObjectOfType<PlayerCharacter>();
#endif
            }

            if (_dummy != null) _dummy.ResetDummy();
            if (_player != null)
            {
                _player.Locomotion.Move(0f);
                _player.transform.position = new Vector3(-3f, 0f, 0f);
                _player.Combat.ResetCombo();
                _player.Energy.Initialize(50f, 100f);
            }
            if (Combat.HitStopManager.Instance != null)
            {
                Combat.HitStopManager.Instance.ResetTimeScale();
            }
        }
    }
}
