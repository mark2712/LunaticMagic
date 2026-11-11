using System;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace Stats
{
    [System.Serializable]
    public class FloatStat
    {
        private float _maxValue = 100;
        private float _regenRate = 1;
        private float _nowValue = 50;
        public float MaxValue
        {
            get => _maxValue;
            set
            {
                if (value > 100000000)
                {
                    _maxValue = 100000000;
                }
                else
                {
                    _maxValue = value;
                }
            }
        }
        public float RegenRate
        {
            get => _regenRate;
            set
            {
                if (value > 100000000)
                {
                    _regenRate = 100000000;
                }
                else
                {
                    _regenRate = value;
                }
            }
        }
        public float NowValue
        {
            get => _nowValue;
            private set
            {
                _nowValue = value;
            }
        }

        public float RegenDelaySeconds = 3f;
        public bool CanRegenWhenNegative = false;

        private float regenDelayTimer = 0f;
        private bool wasKnockedOut = false;

        // Событие: (newValue, oldValue, MaxValue)
        public Action<float, float, float> OnNowValueChanged; // любое изменение здоровья
        public Action<float, float, float> OnKnockout; // здоровье меньше 0
        public Action<float, float, float> OnRecovered; // здоровье снова больше 0

        public FloatStat(float maxValue, float regenRate, float nowValue)
        {
            MaxValue = maxValue;
            RegenRate = regenRate;
            NowValue = nowValue > maxValue ? maxValue : nowValue;
        }

        public void ChangeNowValue(float amount, bool interruptRegen = true)
        {
            float old = NowValue;

            NowValue += amount;
            NowValue = Mathf.Clamp(NowValue, -MaxValue * 2f, MaxValue);

            if (interruptRegen) // блокировать регенерацию 
                regenDelayTimer = RegenDelaySeconds;

            if (NowValue != old)
            {
                OnNowValueChanged?.Invoke(NowValue, old, MaxValue);

                if (!wasKnockedOut && old > 0 && NowValue <= 0)
                {
                    wasKnockedOut = true;
                    OnKnockout?.Invoke(NowValue, old, MaxValue);
                }
                else if (wasKnockedOut && old <= 0 && NowValue > 0)
                {
                    wasKnockedOut = false;
                    OnRecovered?.Invoke(NowValue, old, MaxValue);
                }
            }
        }

        public void OverrideNowValue(float value, bool interruptRegen = false)
        {
            float amount = NowValue - value;
            ChangeNowValue(amount, interruptRegen);
        }

        public void ChangeMaxValue(float amount)
        {
            MaxValue += amount;
            if (NowValue > MaxValue)
                NowValue = MaxValue;
        }

        public void Regenerate(float deltaTime)
        {
            if (regenDelayTimer > 0f)
            {
                regenDelayTimer -= deltaTime;
                return;
            }

            if (NowValue < MaxValue)
            {
                if (NowValue > 0 || CanRegenWhenNegative)
                    ChangeNowValue(RegenRate * deltaTime, false);
            }
        }
    }
}