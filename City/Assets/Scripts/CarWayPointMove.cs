using UnityEngine;

public class CarWaypointMover : MonoBehaviour
{
    public Transform[] points;

    public float speed = 5f;
    public float rotationSpeed = 5f;
    public float reachDistance = 0.5f;
    public bool loop = true;

    private int currentPointIndex = 0;

    void Update()
    {
        if (points == null || points.Length == 0)
            return;

        Transform targetPoint = points[currentPointIndex];

        Vector3 direction = targetPoint.position - transform.position;
        direction.y = 0f;

        if (direction.magnitude <= reachDistance)
        {
            currentPointIndex++;

            if (currentPointIndex >= points.Length)
            {
                if (loop)
                    currentPointIndex = 0;
                else
                    enabled = false;
            }

            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
