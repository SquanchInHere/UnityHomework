using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;

    [Header("Standing Time")]
    [SerializeField] private float minimumStandingTime = 0f;
    [SerializeField] private float maximumStandingTime = 5f;

    [Header("Shot Force")]
    [SerializeField] private float minimumShotForce = 5f;
    [SerializeField] private float maximumShotForce = 20f;

    [Header("Input")]
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;

    private PlayerMovementTracker playerMovementTracker;

    private void Awake()
    {
        playerMovementTracker =
            GetComponent<PlayerMovementTracker>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(shootKey))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning(
                "Projectile Prefab is not assigned!"
            );

            return;
        }

        if (shootPoint == null)
        {
            Debug.LogWarning(
                "Shoot Point is not assigned!"
            );

            return;
        }

        float shotForce = CalculateShotForce();

        GameObject projectile = Instantiate(
            projectilePrefab,
            shootPoint.position,
            shootPoint.rotation
        );

        Rigidbody rb =
            projectile.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogWarning(
                "Projectile does not have a Rigidbody!",
                projectile
            );

            Destroy(projectile);

            return;
        }

        rb.AddForce(
            shootPoint.forward * shotForce,
            ForceMode.Impulse
        );

        playerMovementTracker
            .ResetMovementTracking();

        Debug.Log(
            "Projectile shot force: " +
            shotForce
        );
    }

    private float CalculateShotForce()
    {
        float stationaryTimer =
            playerMovementTracker
                .GetStationaryTimer();

        float shotCharge = Mathf.InverseLerp(
            minimumStandingTime,
            maximumStandingTime,
            stationaryTimer
        );

        float shotForce = Mathf.Lerp(
            minimumShotForce,
            maximumShotForce,
            shotCharge
        );

        return shotForce;
    }

    private void OnValidate()
    {
        minimumStandingTime =
            Mathf.Max(0f, minimumStandingTime);

        maximumStandingTime = Mathf.Max(
            minimumStandingTime + 0.01f,
            maximumStandingTime
        );

        minimumShotForce =
            Mathf.Max(0f, minimumShotForce);

        maximumShotForce = Mathf.Max(
            minimumShotForce,
            maximumShotForce
        );
    }
}
