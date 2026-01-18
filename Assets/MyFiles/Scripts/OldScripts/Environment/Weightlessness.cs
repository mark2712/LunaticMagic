using Mobs;
using UnityEngine;

namespace EnvironmentOld
{
    public class Weightlessness : MonoBehaviour
    {
        public EnvironmentState environmentState;
        private void Start()
        {
            CheckExistingObjectsInWater();
        }

        private void CheckExistingObjectsInWater()
        {
            Collider[] collidersInWater = Physics.OverlapBox(
                transform.position,
                GetComponent<Collider>().bounds.extents,
                transform.rotation,
                LayerMask.GetMask("Player", "NPC")
            );

            // foreach (var collider in collidersInWater)
            // {
            //     if (collider.TryGetComponent<Mob>(out var mob))
            //     {
            //         mob.Body.EnvironmentState = EnvironmentState.Water;
            //     }
            // }
        }

        private void OnTriggerEnter(Collider other)
        {
            // if (other.TryGetComponent<Mob>(out var mob))
            // {
            //     mob.Body.EnvironmentState = EnvironmentState.Water;
            // }
        }

        private void OnTriggerExit(Collider other)
        {
            // if (other.TryGetComponent<Mob>(out var mob))
            // {
            //     mob.Body.EnvironmentState = EnvironmentState.Ground;
            // }
        }

        // private void OnTriggerStay(Collider other)
        // {
        //     if (other.TryGetComponent<Mob>(out var mob))
        //     {
        //         mob.EnvironmentState = EnvironmentState.Water;
        //     }
        // }
    }
}