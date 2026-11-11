using System;
using Mobs.Physics;

namespace Environment
{
    // определяет среду
    public class EnvironmentDetector : IEnvironmentDetector
    {
        public EnvironmentData EnvironmentData { get; private set; } = new();
        public event Action<EnvironmentData> OnEnvironmentChanged;
        private IScenePhysicsBody ScenePhysicsBody;

        public EnvironmentDetector(IScenePhysicsBody scenePhysicsBody)
        {
            ScenePhysicsBody = scenePhysicsBody;
        }

        public void FixedUpdate()
        {
            EnvironmentData oldEnvironmentData = EnvironmentData;
            var newEnvironmentData = Detect();

            if (!newEnvironmentData.Equals(oldEnvironmentData))
            {
                oldEnvironmentData = newEnvironmentData;
                OnEnvironmentChanged?.Invoke(newEnvironmentData);
            }
        }

        private EnvironmentData Detect()
        {
            // касты, триггеры, volume
            return new EnvironmentData();
        }
    }
}