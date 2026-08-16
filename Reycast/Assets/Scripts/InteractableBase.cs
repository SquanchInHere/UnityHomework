using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promptText = "Взаємодія";
    [SerializeField] private GameObject _highlightObject;

    public bool CanInteract => enabled && gameObject.activeInHierarchy;
    public string PromptText => _promptText;

    protected virtual void Awake()
    {
        SetHighlighted(false);
    }

    public abstract void Interact();

    public virtual void SetHighlighted(bool highlighted)
    {
        if (_highlightObject != null)
        {
            _highlightObject.SetActive(highlighted);
        }
    }
}