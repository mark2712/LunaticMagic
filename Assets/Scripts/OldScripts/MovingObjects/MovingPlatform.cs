using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [System.Serializable]
    public struct WaypointData
    {
        public Vector3 localPosition;
        public float localSpeed;
    }

    public WaypointData[] waypoints;
    public float globalSpeed = 2.0f;
    public MovementType movementType;

    private int currentWaypointIndex = 0;
    private bool isMovingForward = true;

    public enum MovementType
    {
        StopAtEnd,
        Loop,
        PingPong
    }

    public Vector3 currentPosition; // текущая позиция 
    public Vector3 futurePosition; // будет на след. кадре 

    private void Start()
    {
        if (waypoints.Length != 0)
        {
            currentPosition = transform.TransformPoint(waypoints[0].localPosition);
            futurePosition = currentPosition;
        }
        else
        {
            Debug.LogError("waypoints.Length == 0");
        }
    }


    private void FixedUpdate()
    {
        currentPosition = futurePosition;
        futurePosition = CalculateNextPosition();
    }

    private Vector3 CalculateNextPosition()
    {
        WaypointData nextWaypoint = GetNextWaypoint();

        float segmentSpeed = globalSpeed * nextWaypoint.localSpeed;
        Vector3 targetPosition = transform.TransformPoint(nextWaypoint.localPosition);

        // if (currentPosition == targetPosition) // Если достигли цели
        if (Vector3.Distance(currentPosition, targetPosition) < 0.01f)
        {
            AdvanceToNextWaypoint(); // Переход к следующей точке
        }

        return Vector3.MoveTowards(currentPosition, targetPosition, segmentSpeed * Time.fixedDeltaTime);
    }

    private WaypointData GetNextWaypoint()
    {
        return waypoints[currentWaypointIndex];
    }

    private void AdvanceToNextWaypoint()
    {
        switch (movementType)
        {
            case MovementType.StopAtEnd:
                if (currentWaypointIndex < waypoints.Length - 1)
                {
                    currentWaypointIndex++;
                }
                break;

            case MovementType.Loop:
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                break;

            case MovementType.PingPong:
                if (isMovingForward)
                {
                    if (currentWaypointIndex < waypoints.Length - 1)
                    {
                        currentWaypointIndex++;
                    }
                    else
                    {
                        isMovingForward = false;
                        currentWaypointIndex--;
                    }
                }
                else
                {
                    if (currentWaypointIndex > 0)
                    {
                        currentWaypointIndex--;
                    }
                    else
                    {
                        isMovingForward = true;
                        currentWaypointIndex++;
                    }
                }
                break;
        }
    }


    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 worldPosition = transform.TransformPoint(waypoints[i].localPosition);
            Gizmos.DrawSphere(worldPosition, 0.1f);

            if (i < waypoints.Length - 1)
            {
                Vector3 nextWorldPosition = transform.TransformPoint(waypoints[i + 1].localPosition);
                Gizmos.DrawLine(worldPosition, nextWorldPosition);
            }
            else if (movementType == MovementType.Loop)
            {
                Vector3 firstWorldPosition = transform.TransformPoint(waypoints[0].localPosition);
                Gizmos.DrawLine(worldPosition, firstWorldPosition);
            }
        }
    }
}



