using UnityEngine;

public class Portals : MonoBehaviour
{
    [Header("Portals")]
    [SerializeField] private Transform FirstPortal;
    [SerializeField] private Transform SecondPortal;
    [SerializeField] private Transform LastPortal;

    [Header("Target")]
    [SerializeField] private Transform _plank;

    private static Transform _lastCorrectPortal;
    private static Transform _lastEnteredPortal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        Transform currentPortal = transform;

        if (_lastEnteredPortal == currentPortal)
            return;

        _lastEnteredPortal = currentPortal;

        if (currentPortal == FirstPortal)
        {
            _lastCorrectPortal = FirstPortal;
            Debug.Log("Correct: FirstPortal");
            return;
        }

        if (currentPortal == SecondPortal)
        {
            if (_lastCorrectPortal == FirstPortal)
            {
                _lastCorrectPortal = SecondPortal;
                Debug.Log("Correct: SecondPortal");
            }
            else
            {
                ResetSequence();
            }

            return;
        }

        if (currentPortal == LastPortal)
        {
            if (_lastCorrectPortal == SecondPortal)
            {
                TeleportToPlank(collision);
                ResetSequence();
            }
            else
            {
                ResetSequence();
            }

            return;
        }

        ResetSequence();
    }

    private void TeleportToPlank(Collider2D player)
    {
        player.transform.position = _plank.position;
        Debug.Log("Player moved to Plank");
    }

    private void ResetSequence()
    {
        _lastCorrectPortal = null;
        _lastEnteredPortal = null;
        Debug.Log("Portal sequence reset");
    }
}
