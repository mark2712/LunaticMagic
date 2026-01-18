using Environment;
using UnityEngine;

namespace Mobs.Physics
{
    public class PhysicsBodyController
    {
        public IPhysicsBody PhysicsBody { get; private set; }
        public readonly IEnvironmentBodySwitcher EnvironmentBodySwitcher;
        private IEnvironmentDetector EnvironmentDetector;
        public EnvironmentData EnvironmentData { get; private set; }

        public PhysicsBodyController(IEnvironmentBodySwitcher environmentBodySwitcher, EnvironmentData initialEnvironmentData = null)
        {
            EnvironmentData = initialEnvironmentData ?? new();
            EnvironmentBodySwitcher = environmentBodySwitcher;
            var startPhysicsBody = EnvironmentBodySwitcher.GetPhysicsBody(EnvironmentData);
            ChangePhysicsBody(startPhysicsBody);
        }

        public void OnEnvironmentChanged(EnvironmentData environmentData)
        {
            EnvironmentData = environmentData;
            var next = EnvironmentBodySwitcher.GetPhysicsBody(EnvironmentData);

            if (next != PhysicsBody)
            {
                ChangePhysicsBody(next);
            }
        }

        private void ChangePhysicsBody(IPhysicsBody newPhysicsBody)
        {
            if (newPhysicsBody == null || newPhysicsBody == PhysicsBody)
                return;

            // 1. Отписываемся от старого detector
            if (EnvironmentDetector != null)
            {
                EnvironmentDetector.OnEnvironmentChanged -= OnEnvironmentChanged;
            }

            // 2. Деактивируем старое тело
            PhysicsBody?.Deactivate();

            // 3. Назначаем новое тело
            PhysicsBody = newPhysicsBody;
            PhysicsBody.Activate();

            // 4. Подписываемся на detector нового тела
            EnvironmentDetector = PhysicsBody.EnvironmentDetector;
            if (EnvironmentDetector != null)
            {
                EnvironmentDetector.OnEnvironmentChanged += OnEnvironmentChanged;
                // EnvironmentData = EnvironmentDetector.EnvironmentData; // а нужно ли?
            }
        }

        public void SendCommand(IBodyCommand command)
        {
            command.ExecuteCommand(PhysicsBody);
        }

        public void FixedUpdate() { PhysicsBody.FixedUpdate(); }
        public void Update() { PhysicsBody.Update(); }
        public void LateUpdate() { PhysicsBody.LateUpdate(); }

        public void Dispose()
        {
            if (EnvironmentDetector != null)
            {
                EnvironmentDetector.OnEnvironmentChanged -= OnEnvironmentChanged;
                EnvironmentDetector = null;
            }
            PhysicsBody = null;
        }
    }
}