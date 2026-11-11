using System;
using System.Collections.Generic;

namespace InputLayer
{
    public sealed class AxisInput<T> : IAxisInput<T>
    {
        public T Value { get; private set; }
        public T DefaultValue { get; }

        public event Action<T> OnChanged;
        public event Action<T> OnDown;
        public event Action<T> OnUp;

        private readonly IEqualityComparer<T> comparer;

        public AxisInput(
            T defaultValue,
            IEqualityComparer<T> comparer = null)
        {
            DefaultValue = defaultValue;
            Value = defaultValue;
            this.comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public void SetValue(T newValue)
        {
            if (comparer.Equals(Value, newValue))
                return;

            bool wasDefault = comparer.Equals(Value, DefaultValue);
            bool nowDefault = comparer.Equals(newValue, DefaultValue);

            Value = newValue;
            OnChanged?.Invoke(Value);

            if (wasDefault && !nowDefault)
                OnDown?.Invoke(Value);

            if (!wasDefault && nowDefault)
                OnUp?.Invoke(DefaultValue);
        }
    }
}