using UnityEngine;
using UnityEngine.Events;

public class ButtonInteractable : InteractableBase
{
    [SerializeField] private UnityEvent _onActivated;
    [SerializeField] private UnityEvent _onDicativated;
    [SerializeField] private bool _oneShot;

    private bool _pressed;

    public override void Interact()
    {
        if (!CanInteract)
        {
            Debug.Log("Is Dicativate pupupu 3");
            return;
        }

        if (_oneShot && _pressed)
        {
            Debug.Log("Is Dicativate pupupu 1");
            return;
        }

        _pressed = !_pressed;

        if (_pressed) {

            Debug.Log("Is Dicativate pupupu");
            _onActivated?.Invoke(); 
        }
        else
        {
            Debug.Log("Is Dicativate button 2");
            _onDicativated?.Invoke();
        }
    }
}