public interface IInteractable
{
    bool CanInteract { get; }
    string PromptText { get; }

    void Interact();
    void SetHighlighted(bool highlighted);
}