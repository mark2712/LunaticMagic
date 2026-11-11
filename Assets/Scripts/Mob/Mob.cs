using System;
using UnityEngine;
using Environment;
using Mobs.Physics;

namespace Mobs
{
    public class Mob
    {
        public string MobName { get; private set; } // имя моба (бочка, игрок, гоблин)
        public string MobId { get; private set; } // уникальный id
        public GameObject MobGameObject { get; }
        public Entities_1.BaseEntity Entity { get; set; } // статы, получение урона, сопротивления, баффы
        public PhysicsBodyController PhysicsBodyController { get; private set; } // смена физ тел, сканирование среды

        public Mob()
        {
            MobId = Guid.NewGuid().ToString();
            PhysicsBodyController = new(new PlayerBodySwitcher(MobGameObject), new EnvironmentData());
        }

        public void Update()
        {
            Vector3 input = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if (input != Vector3.zero)
            {
                PhysicsBodyController.SendCommand(
                    new MoveCommand { MoveInput = input }
                );
            }
        }

        public void FixedUpdate()
        {
            PhysicsBodyController.FixedUpdate();
        }
    }
}