using UnityEngine;

public class PlayerMovementTracker : MonoBehaviour
{
    [Header("Movement Detection")]
    [SerializeField] private float movementThreshold = 0.5f;

    private float stationaryTimer;
    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        CheckMovement();
    }

    private void CheckMovement()
    {
        Vector3 currentPosition = transform.position;

        Vector3 currentXZ = new Vector3(
            currentPosition.x,
            0f,
            currentPosition.z
        );

        Vector3 lastXZ = new Vector3(
            lastPosition.x,
            0f,
            lastPosition.z
        );

        float distance = Vector3.Distance(
            currentXZ,
            lastXZ
        );

        if (distance > movementThreshold)
        {
            stationaryTimer = 0f;
        }
        else
        {
            stationaryTimer += Time.deltaTime;
        }

        lastPosition = currentPosition;
    }

    public void ResetMovementTracking()
    {
        stationaryTimer = 0f;
        lastPosition = transform.position;
    }

    public float GetStationaryTimer()
    {
        return stationaryTimer;
    }
}
