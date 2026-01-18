using Mobs.Physics;
using UnityEngine;

namespace Environment
{
    // решает, какое тело нужно на основе данных о нахождении в текущих средах EnvironmentData
    // так же тут содержится список тел для моба использующего этот EnvironmentBodySwitcher
    public interface IEnvironmentBodySwitcher
    {
        GameObject MobGameObject { get; }
        IPhysicsBody GetPhysicsBody(EnvironmentData EnvironmentData);
    }
}