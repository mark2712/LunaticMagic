using UnityEngine;

namespace EnvironmentOld
{
    public class EnvironmentChecker
    {
        public virtual byte SlopeLimit => 55;
        public GroundState groundedRay = GroundState.Ground;
        public GroundState groundedSphereRay = GroundState.Ground;
        public Vector3 _platformVelocity;

        protected float _groundedRayLength;
        protected CapsuleCollider _mainCollider;
        protected Transform _main;
        protected RaycastHit _groundedRayHit;
        protected RaycastHit _groundedSphereRayHit;
        protected LayerMask layerMask;

        public EnvironmentChecker(Transform main)
        {
            _main = main;
            _mainCollider = main.GetComponent<CapsuleCollider>();
            // Transform main = characterCollider.Find("Main");
            _groundedRayLength = _mainCollider.height / 2 * 1.2f;
            layerMask = LayerMask.GetMask("Player");
        }

        protected void CheckGround()
        {
            if (Physics.SphereCast(_main.position, _mainCollider.radius, Vector3.down, out _groundedSphereRayHit, _groundedRayLength, ~layerMask))
            {
                groundedSphereRay = GroundState.Ground;
                _platformVelocity = Vector3.zero;

                Rigidbody rb = _groundedSphereRayHit.collider.GetComponentInParent<Rigidbody>();
                if (rb != null) { _platformVelocity = rb.linearVelocity; }

                Vector3 _groundNormal;
                Vector3 correctedRayOrigin = new Vector3(_groundedSphereRayHit.point.x, _main.position.y, _groundedSphereRayHit.point.z);
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

            if (Physics.Raycast(_main.position, Vector3.down, out _groundedRayHit, _groundedRayLength, ~layerMask))
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