using UnityEngine;


namespace Player
{
    public class PlayerControllerRigidbodyMB : MonoBehaviour
    {
        public static PlayerControllerRigidbodyMB self;
        public float moveSpeed = 4f;
        public float runSpeed = 7f;
        public float jumpForce = 9f;
        public float gravityScale = 2f;

        void Awake()
        {
            self = this;
        }

        public void OnCollisionStay(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                float surfaceAngle = Vector3.Angle(contact.normal, Vector3.up);
                PlayerController playerController = GameContext.PlayerController;

                // Если угол между 70° и 90°, это стена
                if (surfaceAngle >= 70f && surfaceAngle <= 89f)
                {
                    playerController.slopeLimitCollisionOn = true;
                    return; // Достаточно одного контакта
                }
                playerController.slopeLimitCollisionOn = false;
            }
        }
    }
}