using UnityEngine;

namespace Player
{
    public class GroundPlayerController : PlayerController, IPlayerController
    {
        public float MoveInputUp { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public float MoveInputDown { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override float NowMoveSpeed { get; set; }
        protected float jumpForce = 9f;
        protected float gravityScaleNow = 2f;

        protected bool isMoveOn; // движение разрешено
        protected bool isJumpOn; // прыжок разрешен
        protected Vector3 move; // хранит информацию о движении, отсюда можно получить направление движения

        public GroundPlayerController(GameObject Player) : base(Player) { }

        public override void Update()
        {
            SyncCamera();
        }

        public override void FixedUpdate()
        {
            jumpForce = PlayerControllerRigidbodyMB.self.jumpForce;
            gravityScaleNow = PlayerControllerRigidbodyMB.self.gravityScale;

            groundedSphereRay = GroundState.Ground;
            groundedRay = GroundState.Ground;
            isJumpOn = true;
            isMoveOn = true;

            move = (player.right * MoveInput.x + player.forward * MoveInput.y).normalized * NowMoveSpeed;
            IsMove = move.magnitude > 0.01f;

            if (IsMove) // вращение персонажа в сторону камеры происходит только при движении
            {
                characterRigidbody.MoveRotation(CameraPlayerController.self.playerCameraBody.rotation);
                // characterRigidbody.MoveRotation(Quaternion.Slerp(characterRigidbody.rotation, CameraPlayerController.self.playerCameraBody.rotation, Time.deltaTime * 20f));
            }

            CheckGround();
            IsGround = groundedRay == GroundState.Ground || groundedSphereRay == GroundState.Ground;

            if (groundedSphereRay == GroundState.Wall)
            {
                if (groundedRay != GroundState.Ground)
                {
                    isJumpOn = false;// прыжок запрещен
                    isMoveOn = false; // движение ограничено
                }
            }

            if (slopeLimitCollisionOn)
            {
                if (!IsGround)
                {
                    isJumpOn = false; // прыжок запрещен
                    isMoveOn = false; // движение ограничено
                }
            }

            PhysicsMaterial MoveMaterial()
            {
                if (IsMove)
                {
                    return _playerSkinMaterial;
                }
                else
                {
                    if (isMoveOn)
                    {
                        return _playerMaterial;
                    }
                    else
                    {
                        return _playerSkinMaterial;
                    }
                }
            }

            _characterBottomCollider.sharedMaterial = MoveMaterial();

            Move();
            JumpSpase();
            ApplyGravity();

            MoveInput = Vector2.zero;
        }

        protected void Move()
        {
            if (move != Vector3.zero)
            {
                if (!isMoveOn)
                {
                    move /= 40;
                }
                _characterBottomCollider.sharedMaterial = _playerSkinMaterial;

                Vector3 velocity = characterRigidbody.linearVelocity;
                Vector3 relativeMove = move + new Vector3(_platformVelocity.x, 0, _platformVelocity.z);
                Vector3 velocityChange = relativeMove - new Vector3(velocity.x, 0, velocity.z);

                if (IsGround)
                {
                    float stepeHight = CheckForSteps(move);
                    if (stepeHight > 0)
                    {
                        JumpStep(Mathf.Min(stepeHight, _characterCollider.height / 2) * gravityScaleNow);
                    }
                }
                else
                {
                    velocityChange = Vector3.Lerp(Vector3.zero, velocityChange, 0.1f);
                }

                characterRigidbody.AddForce(velocityChange / Time.fixedDeltaTime, ForceMode.Acceleration);
            }
        }

        public override void Jump() { IsJump = true; }
        protected void JumpSpase()
        {
            if (IsJump)
            {
                IsJump = false;
                JumpExec(jumpForce);
            }
        }
        protected void JumpStep(float jumpForce)
        {
            if (IsGround)
            {
                JumpExec(jumpForce);
            }
        }
        protected void JumpExec(float jumpForce)
        {
            if (IsGround && isJumpOn)
            {
                float time = Time.time - LastJumpTime;
                if (time > 0.1f)
                {
                    LastJumpTime = Time.time;
                    float currentVerticalSpeed = characterRigidbody.linearVelocity.y;
                    if (currentVerticalSpeed > 0) { currentVerticalSpeed = 0; }
                    characterRigidbody.AddForce(Vector3.up * (jumpForce * characterRigidbody.mass - currentVerticalSpeed * characterRigidbody.mass), ForceMode.Impulse);
                }
            }
        }

        protected void ApplyGravity()
        {
            Vector3 gravity = Physics.gravity * gravityScaleNow; // Увеличиваем стандартную гравитацию
            characterRigidbody.AddForce(gravity, ForceMode.Acceleration);
        }

        protected float CheckForSteps(Vector3 move)
        {
            _MoveDirection2D.rotation = Quaternion.LookRotation(move, Vector3.up);

            if (Physics.Raycast(stepOffset1.position, Vector3.down, out RaycastHit hit, rayStepHeight, ~layerMask))
            {
                float step = hit.point.y - stepOffsetBottom.position.y;
                return step;
            }

            return 0f; // Нет препятствия
        }

    }
}





// public override void Jump() { IsJump = true; }
// protected void JumpSpase()
// {
//     if (IsJump)
//     {
//         IsJump = false;
//         float time = Time.time - LastJumpTime;
//         if (JumpCount < 2 && time > 0.1f)
//         {
//             float jumpMultiplier = 1;
//             // если сделать второй прыжок сразу то скорость не просто складывается а почему то как будто умножается. Поэтому чем меньше прошло времени с момента последнего прыжка тем меньше будет сила прыжка
//             if (JumpCount == 1) // только для второго прыжка
//             {
//                 jumpMultiplier = Mathf.Clamp01(Time.time - LastJumpTime + 0.3f) * 0.95f;
//             }
//             LastJumpTime = Time.time;
//             JumpCount++;
//             JumpExec(jumpForce * jumpMultiplier);
//         }
//     }
// }
// protected void JumpStep(float jumpForce)
// {
//     if (IsGround)
//     {
//         JumpExec(jumpForce);
//     }
// }
// protected void JumpExec(float jumpForce)
// {
//     float currentVerticalSpeed = characterRigidbody.linearVelocity.y;
//     if (currentVerticalSpeed > 0) { currentVerticalSpeed = 0; } // если платформа движется вниз то прыжок будет как со статичной поверхности

//     // Если падение слишком быстрое то второй прыжок в воздухе не даёт результата
//     if (currentVerticalSpeed < -7 && !IsGround)
//     {
//         float speedFactor = Mathf.InverseLerp(-9f, -20f, currentVerticalSpeed);
//         currentVerticalSpeed = Mathf.Lerp(currentVerticalSpeed, 0f, speedFactor);
//     }

//     characterRigidbody.AddForce(Vector3.up * (jumpForce * characterRigidbody.mass - currentVerticalSpeed * characterRigidbody.mass), ForceMode.Impulse);
// }