using UnityEngine;

public class MovingPlatformMain : MonoBehaviour
{
    public MovingPlatform movingPlatform;
    private Rigidbody rb;

    private void Start()
    {
        movingPlatform = transform.parent.GetComponent<MovingPlatform>();
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            // AddMovingPlatformObj(rb.transform);
        }

        // AddMovingPlatformObjRecursively(transform, rb);
    }

    // private void AddMovingPlatformObjRecursively(Transform parent, Rigidbody rb)
    // {
    //     foreach (Transform child in parent)
    //     {
    //         MovingPlatformObj childRb = child.GetComponent<MovingPlatformObj>();

    //         if (childRb == null)
    //         {
    //             AddMovingPlatformObj(child);
    //         }

    //         AddMovingPlatformObjRecursively(child, rb);
    //     }
    // }

    // private void AddMovingPlatformObj(Transform obj)
    // {
    //     MovingPlatformObj movingPlatformObj = obj.gameObject.AddComponent<MovingPlatformObj>();
    //     movingPlatformObj.rb = rb;
    //     movingPlatformObj.movingPlatform = movingPlatform;
    // }

    private void FixedUpdate()
    {
        rb.MovePosition(movingPlatform.currentPosition);
    }
}

