using System;
using UnityEngine;

namespace AetherSurge.Combat
{
    public class EnergyController : MonoBehaviour
    {
        [Header("Energy Settings")]
        [SerializeField] private float _maxEnergy = 100f;
        [SerializeField] private float _chargeRatePerSecond = 25f;
        [SerializeField] private float _astralBeamCost = 30f;
        [SerializeField] private float _currentEnergy = 50f;

        public float CurrentEnergy
        {
            get { return _currentEnergy; }
        }

        public float MaxEnergy
        {
            get { return _maxEnergy; }
        }

        public float AstralBeamCost
        {
            get { return _astralBeamCost; }
        }

        public event Action<float, float> OnEnergyChanged;
        public event Action OnSkillCastSuccess;
        public event Action OnSkillCastFailed;

        public void Initialize(float startingEnergy, float maxEnergy)
        {
            _maxEnergy = maxEnergy;
            _currentEnergy = Mathf.Clamp(startingEnergy, 0f, _maxEnergy);
            OnEnergyChanged?.Invoke(_currentEnergy, _maxEnergy);
        }

        public void Charge(float deltaTime)
        {
            if (_currentEnergy < _maxEnergy)
            {
                _currentEnergy = Mathf.Min(_maxEnergy, _currentEnergy + _chargeRatePerSecond * deltaTime);
                OnEnergyChanged?.Invoke(_currentEnergy, _maxEnergy);
            }
        }

        public bool TryCastAstralBeam()
        {
            if (_currentEnergy >= _astralBeamCost)
            {
                _currentEnergy -= _astralBeamCost;
                OnEnergyChanged?.Invoke(_currentEnergy, _maxEnergy);
                OnSkillCastSuccess?.Invoke();
                return true;
            }
            else
            {
                OnSkillCastFailed?.Invoke();
                return false;
            }
        }

        public void RestoreEnergy(float amount)
        {
            _currentEnergy = Mathf.Min(_maxEnergy, _currentEnergy + amount);
            OnEnergyChanged?.Invoke(_currentEnergy, _maxEnergy);
        }
    }
}
