using System;
using UnityEngine;
using AetherSurge.Player;
using AetherSurge.Enemy;
using AetherSurge.Combat;

namespace AetherSurge.Core
{
    public enum MatchState
    {
        InProgress,
        Victory,
        Defeat
    }

    public class MatchManager : MonoBehaviour
    {
        [Header("Combatants")]
        [SerializeField] private PlayerCharacter _player;
        [SerializeField] private BasicEnemy _enemy;
        [SerializeField] private Vector3 _playerSpawnPoint = new Vector3(-3.0f, 1.0f, 0f);

        [Header("State")]
        [SerializeField] private MatchState _matchState = MatchState.InProgress;

        public MatchState CurrentMatchState
        {
            get { return _matchState; }
        }

        public PlayerCharacter Player
        {
            get { return _player; }
            set { _player = value; }
        }

        public BasicEnemy Enemy
        {
            get { return _enemy; }
            set { _enemy = value; }
        }

        public Vector3 PlayerSpawnPoint
        {
            get { return _playerSpawnPoint; }
            set { _playerSpawnPoint = value; }
        }

        public event Action<MatchState> OnMatchStateChanged;
        public event Action OnMatchRestarted;

        private void Start()
        {
            InitializeReferences();
            SubscribeEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        public void InitializeReferences()
        {
            if (_player == null)
            {
#if UNITY_2023_1_OR_NEWER
                _player = FindFirstObjectByType<PlayerCharacter>();
#else
                _player = FindObjectOfType<PlayerCharacter>();
#endif
            }

            if (_enemy == null)
            {
#if UNITY_2023_1_OR_NEWER
                _enemy = FindFirstObjectByType<BasicEnemy>();
#else
                _enemy = FindObjectOfType<BasicEnemy>();
#endif
            }
        }

        public void SubscribeEvents()
        {
            UnsubscribeEvents(); // Cleanly detach any previous listeners

            if (_player != null)
            {
                _player.OnDefeated += HandlePlayerDefeated;
            }

            if (_enemy != null)
            {
                _enemy.OnDefeated += HandleEnemyDefeated;
            }
        }

        public void UnsubscribeEvents()
        {
            if (_player != null)
            {
                _player.OnDefeated -= HandlePlayerDefeated;
            }

            if (_enemy != null)
            {
                _enemy.OnDefeated -= HandleEnemyDefeated;
            }
        }

        private void HandlePlayerDefeated()
        {
            if (_matchState != MatchState.InProgress) return;

            SetMatchState(MatchState.Defeat);
            if (_player != null)
            {
                _player.SetInputEnabled(false);
            }
        }

        private void HandleEnemyDefeated()
        {
            if (_matchState != MatchState.InProgress) return;

            SetMatchState(MatchState.Victory);
            if (_player != null)
            {
                _player.SetInputEnabled(false);
            }
        }

        public void SetMatchState(MatchState newState)
        {
            _matchState = newState;
            if (OnMatchStateChanged != null) OnMatchStateChanged.Invoke(_matchState);
        }

        public void RestartMatch()
        {
            _matchState = MatchState.InProgress;

            if (HitStopManager.Instance != null)
            {
                HitStopManager.Instance.ResetTimeScale();
            }

            if (_player != null)
            {
                _player.ResetPlayer(_playerSpawnPoint);
                _player.SetInputEnabled(true);
            }

            if (_enemy != null)
            {
                _enemy.ResetEnemy();
            }

            SubscribeEvents();

            if (OnMatchRestarted != null) OnMatchRestarted.Invoke();
            if (OnMatchStateChanged != null) OnMatchStateChanged.Invoke(_matchState);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartMatch();
            }
        }
    }
}
