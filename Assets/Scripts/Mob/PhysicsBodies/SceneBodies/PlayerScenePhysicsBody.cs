using UnityEngine;

namespace Mobs.Physics
{
    public class PlayerScenePhysicsBody : IScenePhysicsBody
    {
        public GameObject MobGameObject { get; protected set; }
        public Rigidbody MobRigidbody => MobGameObject.GetComponent<Rigidbody>();

        public PlayerScenePhysicsBody(GameObject mobGameObject)
        {
            MobGameObject = mobGameObject;
        }

        public void Activate()
        {
            
        }

        public void Deactivate()
        {
            
        }
    }
}