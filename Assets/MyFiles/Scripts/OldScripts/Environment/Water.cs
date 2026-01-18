using UnityEngine;

namespace EnvironmentOld
{
    public class Water : MonoBehaviour
    {
        [Header("Water Settings")]
        public float density = 1000f;      // плотность жидкости
        public float drag = 3f;           // линейное сопротивление
        public float angularDrag = 1f;    // вращательное сопротивление
        public EnvironmentState environmentState = EnvironmentState.Water;

        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other.TryGetComponent<EnvironmentChecker>(out var checker))
        //     {
        //         checker.EnterEnvironment(this);
        //     }
        // }

        // private void OnTriggerExit(Collider other)
        // {
        //     if (other.TryGetComponent<EnvironmentChecker>(out var checker))
        //     {
        //         checker.ExitEnvironment(this);
        //     }
        // }
    }
}