using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _interactionDistance = 3f;
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private KeyCode _interactionKey = KeyCode.E;
    [SerializeField] private InteractionUI _interactionUI;

    private IInteractable _currentInteractable;

    private void Update()
    {
        UpdateTarget();

        if (_currentInteractable != null && Input.GetKeyDown(_interactionKey))
        {
            _currentInteractable.Interact();
        }
    }

    private void UpdateTarget()
    {
        IInteractable interactable = FindInteractable();

        if (interactable == _currentInteractable)
        {
            return;
        }

        if (_currentInteractable != null)
        {
            _currentInteractable.SetHighlighted(false);
        }

        _currentInteractable = interactable;

        if (_currentInteractable != null)
        {
            _currentInteractable.SetHighlighted(true);
            _interactionUI.Show(_currentInteractable.PromptText, _interactionKey);
        }
        else
        {
            _interactionUI.Hide();
        }
    }

    private IInteractable FindInteractable()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (!Physics.Raycast(
                ray,
                out RaycastHit hit,
                _interactionDistance,
                _interactableLayer))
        {
            return null;
        }

        return hit.collider.GetComponentInParent<IInteractable>();
    }

    private void OnDisable()
    {
        if (_currentInteractable != null)
        {
            _currentInteractable.SetHighlighted(false);
        }

        _interactionUI.Hide();
        _currentInteractable = null;
    }
}