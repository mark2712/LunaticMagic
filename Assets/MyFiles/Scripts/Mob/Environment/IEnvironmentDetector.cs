using System;

namespace Environment
{
    public interface IEnvironmentDetector
    {
        EnvironmentData EnvironmentData { get; }
        event Action<EnvironmentData> OnEnvironmentChanged;
        void FixedUpdate();
    }
}