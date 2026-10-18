using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AetherSurge.Player;
using AetherSurge.Dummy;
using AetherSurge.Enemy;
using AetherSurge.Core;

namespace AetherSurge.UI
{
    public class CombatHUD : MonoBehaviour
    {
        [Header("Player References")]
        [SerializeField] private PlayerCharacter _player;
        [SerializeField] private Slider _playerHpSlider;
        [SerializeField] private TMP_Text _playerHpText;
        [SerializeField] private Slider _energySlider;
        [SerializeField] private TMP_Text _energyText;

        [Header("Enemy References")]
        [SerializeField] private BasicEnemy _basicEnemy;
        [SerializeField] private TrainingDummy _dummy;
        [SerializeField] private Slider _enemyHpSlider;
        [SerializeField] private TMP_Text _enemyHpText;

        [Header("Match & Objective")]
        [SerializeField] private MatchManager _matchManager;
        [SerializeField] private TMP_Text _objectiveText;
        [SerializeField] private TMP_Text _outcomeBannerText;
        [SerializeField] private GameObject _outcomeBannerRoot;

        [Header("UI Controls")]
        [SerializeField] private Button _resetArenaButton;
        [SerializeField] private TMP_Text _fpsText;

        private float _fpsTimer;
        private int _frameCount;

        private void Start()
        {
            ResolveReferences();

            if (_resetArenaButton != null)
            {
                _resetArenaButton.onClick.AddListener(ResetArena);
            }

            if (_matchManager != null)
            {
                _matchManager.OnMatchStateChanged += HandleMatchStateChanged;
            }

            UpdateOutcomeBanner(MatchState.InProgress);
        }

        private void OnDestroy()
        {
            if (_matchManager != null)
            {
                _matchManager.OnMatchStateChanged -= HandleMatchStateChanged;
            }
        }

        public void ResolveReferences()
        {
            if (_player == null)
            {
#if UNITY_2023_1_OR_NEWER
                _player = Object.FindFirstObjectByType<PlayerCharacter>();
#else
                _player = Object.FindObjectOfType<PlayerCharacter>();
#endif
            }

            if (_basicEnemy == null)
            {
#if UNITY_2023_1_OR_NEWER
                _basicEnemy = Object.FindFirstObjectByType<BasicEnemy>();
#else
                _basicEnemy = Object.FindObjectOfType<BasicEnemy>();
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

            if (_matchManager == null)
            {
#if UNITY_2023_1_OR_NEWER
                _matchManager = Object.FindFirstObjectByType<MatchManager>();
#else
                _matchManager = Object.FindObjectOfType<MatchManager>();
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

            if (_playerHpText == null)
            {
                var playerHpObj = GameObject.Find("Text_PlayerHP");
                if (playerHpObj != null) _playerHpText = playerHpObj.GetComponent<TMP_Text>();
            }

            if (_enemyHpText == null)
            {
                var enemyHpObj = GameObject.Find("Text_DummyHP");
                if (enemyHpObj == null) enemyHpObj = GameObject.Find("Text_EnemyHP");
                if (enemyHpObj != null) _enemyHpText = enemyHpObj.GetComponent<TMP_Text>();
            }

            if (_objectiveText == null)
            {
                var objTextGo = GameObject.Find("Text_Objective");
                if (objTextGo != null) _objectiveText = objTextGo.GetComponent<TMP_Text>();
            }

            if (_outcomeBannerText == null)
            {
                var bannerGo = GameObject.Find("Text_OutcomeBanner");
                if (bannerGo != null) _outcomeBannerText = bannerGo.GetComponent<TMP_Text>();
            }

            if (_outcomeBannerRoot == null)
            {
                _outcomeBannerRoot = GameObject.Find("OutcomeBannerRoot");
            }
        }

        public void BindControls(
            Button resetButton,
            TMP_Text fpsText,
            TMP_Text energyText = null,
            TMP_Text dummyHpText = null,
            TMP_Text playerHpText = null,
            TMP_Text objectiveText = null,
            TMP_Text outcomeBanner = null,
            GameObject outcomeBannerRoot = null)
        {
            _resetArenaButton = resetButton;
            _fpsText = fpsText;
            _energyText = energyText;
            _enemyHpText = dummyHpText;
            _playerHpText = playerHpText;
            _objectiveText = objectiveText;
            _outcomeBannerText = outcomeBanner;
            _outcomeBannerRoot = outcomeBannerRoot;
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
                if (_playerHpText != null)
                {
                    _playerHpText.text = string.Format("Player HP: {0} / {1}", _player.CurrentHealth, _player.MaxHealth);
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

            if (_basicEnemy != null)
            {
                if (_enemyHpSlider != null)
                {
                    _enemyHpSlider.maxValue = _basicEnemy.MaxHealth;
                    _enemyHpSlider.value = _basicEnemy.CurrentHealth;
                }
                if (_enemyHpText != null)
                {
                    _enemyHpText.text = string.Format("Enemy HP: {0} / {1}", _basicEnemy.CurrentHealth, _basicEnemy.MaxHealth);
                }
            }
            else if (_dummy != null)
            {
                if (_enemyHpSlider != null)
                {
                    _enemyHpSlider.maxValue = _dummy.MaxHealth;
                    _enemyHpSlider.value = _dummy.CurrentHealth;
                }
                if (_enemyHpText != null)
                {
                    _enemyHpText.text = string.Format("Dummy HP: {0} / {1}", _dummy.CurrentHealth, _dummy.MaxHealth);
                }
            }
        }

        private void HandleMatchStateChanged(MatchState state)
        {
            UpdateOutcomeBanner(state);
        }

        private void UpdateOutcomeBanner(MatchState state)
        {
            if (_outcomeBannerRoot != null)
            {
                _outcomeBannerRoot.SetActive(state != MatchState.InProgress);
            }

            if (_outcomeBannerText != null)
            {
                switch (state)
                {
                    case MatchState.Victory:
                        _outcomeBannerText.text = "VICTORY!\nObjective Complete: Enemy Defeated";
                        _outcomeBannerText.color = new Color(1.0f, 0.85f, 0.2f); // Gold
                        _outcomeBannerText.gameObject.SetActive(true);
                        break;
                    case MatchState.Defeat:
                        _outcomeBannerText.text = "DEFEAT!\nPlayer Fallen in Combat";
                        _outcomeBannerText.color = new Color(0.95f, 0.25f, 0.25f); // Red
                        _outcomeBannerText.gameObject.SetActive(true);
                        break;
                    default:
                        _outcomeBannerText.gameObject.SetActive(false);
                        break;
                }
            }
        }

        public void ResetArena()
        {
            ResolveReferences();

            if (_matchManager != null)
            {
                _matchManager.RestartMatch();
            }
            else
            {
                if (_dummy != null) _dummy.ResetDummy();
                if (_basicEnemy != null) _basicEnemy.ResetEnemy();
                if (_player != null)
                {
                    _player.ResetPlayer(new Vector3(-3f, 1f, 0f));
                }
                if (Combat.HitStopManager.Instance != null)
                {
                    Combat.HitStopManager.Instance.ResetTimeScale();
                }
            }

            UpdateOutcomeBanner(MatchState.InProgress);
        }
    }
}
