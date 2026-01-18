using System;
using UnityEngine;


namespace Player
{
    public abstract class PlayerController
    {
        public virtual float NowMoveSpeed { get; set; }
        public virtual byte SlopeLimit => 55;

        public event Action<bool> OnMoveChanged;
        protected bool _isMove;
        public virtual bool IsMove // персонаж идёт (обычно определяется как move.magnitude на основе данных ввода)
        {
            get => _isMove;
            protected set
            {
                if (_isMove != value)
                {
                    _isMove = value;
                    OnMoveChanged?.Invoke(_isMove);
                }
            }
        }

        public event Action<bool> OnGroundChanged;
        protected bool _isGround;
        public virtual bool IsGround // стоит ли персонаж на земле
        {
            get => _isGround;
            protected set
            {
                if (_isGround != value)
                {
                    _isGround = value;
                    OnGroundChanged?.Invoke(_isGround);
                }
            }
        }

        public virtual bool IsJump { get; set; } // можно ли выпонить прыжок в этом кадре?
        public float LastJumpTime { get; set; } // Время последнего прыжка

        public enum GroundState { Ground, Wall, Air }
        public GroundState groundedRay = GroundState.Ground;
        public GroundState groundedSphereRay = GroundState.Ground;
        public float _groundedRayLength;
        public bool slopeLimitCollisionOn = false;

        public Vector2 MoveInput { get; set; }
        protected Vector3 _platformVelocity;

        public Transform player; // Тело игрока
        protected CapsuleCollider _characterCollider;
        public Rigidbody characterRigidbody;
        protected CapsuleCollider _characterBottomCollider;
        protected Transform _MoveDirection2D;
        protected Transform stepOffset1;
        protected Transform stepOffset2;
        protected Transform stepOffsetBottom;
        protected float rayStepHeight;
        protected RaycastHit _groundedRayHit;
        protected RaycastHit _groundedSphereRayHit;
        protected LayerMask layerMask;

        protected PhysicsMaterial _playerSkinMaterial;
        protected PhysicsMaterial _playerMaterial;
        public Transform playerModel;

        public PlayerController(GameObject Player)
        {
            player = Player.transform;
        }

        public void Awake()
        {
            _characterCollider = player.GetComponent<CapsuleCollider>();
            if (_characterCollider == null) Debug.LogError("Character CapsuleCollider не найден! Убедитесь, что он находится на объекте Player.");

            characterRigidbody = player.GetComponent<Rigidbody>();
            if (characterRigidbody == null) Debug.LogError("Character Rigidbody не найден! Убедитесь, что он находится на объекте Player.");

            _characterBottomCollider = player.Find("ColliderBottom").GetComponent<CapsuleCollider>();
            if (_characterBottomCollider == null) Debug.LogError("CharacterBottom CapsuleCollider не найден! Убедитесь, что он находится на объекте Player.");

            playerModel = player.Find("PlayerModel");
            if (playerModel == null) Debug.LogError("PlayerModel не найден!");

            _MoveDirection2D = player.Find("MoveDirection2D");
            stepOffset1 = _MoveDirection2D.Find("StepOffset1");
            stepOffset2 = _MoveDirection2D.Find("StepOffset2");
            stepOffsetBottom = _MoveDirection2D.Find("StepOffsetBottom");
            rayStepHeight = stepOffset1.position.y - stepOffset2.position.y;

            _playerSkinMaterial = _characterCollider.sharedMaterial;
            _playerMaterial = _characterBottomCollider.sharedMaterial;


            layerMask = LayerMask.GetMask("Player");

            Transform playerBottomPoint = player.Find("PlayerBottomPoint");
            _groundedRayLength = player.position.y - playerBottomPoint.position.y;
        }

        public virtual void FixedUpdate() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }

        public void SetMoveInput(Vector2 moveInput) { MoveInput = moveInput; }
        public void SetMoveInput() { MoveInput = GameContext.InputController.Move.Value; } // зависит от InputController
        public void SyncCamera() { CameraPlayerController.self.playerCameraBody.position = player.position; }

        public virtual void Jump() { }

        protected void CheckGround()
        {
            if (Physics.SphereCast(player.position, _characterCollider.radius * 0.48f, Vector3.down, out _groundedSphereRayHit, _groundedRayLength, ~layerMask))
            {
                groundedSphereRay = GroundState.Ground;
                _platformVelocity = Vector3.zero;

                Rigidbody rb = _groundedSphereRayHit.collider.GetComponentInParent<Rigidbody>();
                if (rb != null) { _platformVelocity = rb.linearVelocity; }

                Vector3 _groundNormal;
                Vector3 correctedRayOrigin = new Vector3(_groundedSphereRayHit.point.x, player.position.y, _groundedSphereRayHit.point.z);
                if (Physics.Raycast(correctedRayOrigin, Vector3.down, out RaycastHit correctedHit, _groundedRayLength, ~layerMask))
                {
                    _groundNormal = correctedHit.normal;
                }
                else
                {
                    _groundNormal = _groundedSphereRayHit.normal;
                }

                float surfaceAngle = Vector3.Angle(_groundNormal, Vector3.up);
                if (surfaceAngle > SlopeLimit && surfaceAngle < 90)
                {
                    groundedSphereRay = GroundState.Wall;
                }
            }
            else
            {
                groundedSphereRay = GroundState.Air;
            }

            if (Physics.Raycast(player.position, Vector3.down, out _groundedRayHit, _groundedRayLength, ~layerMask))
            {
                groundedRay = GroundState.Ground;

                float surfaceAngle = Vector3.Angle(_groundedRayHit.normal, Vector3.up);
                if (surfaceAngle > SlopeLimit)
                {
                    groundedRay = GroundState.Wall;
                }
            }
            else
            {
                groundedRay = GroundState.Air;
            }
        }
    }
}