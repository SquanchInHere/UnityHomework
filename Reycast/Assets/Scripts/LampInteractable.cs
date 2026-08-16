using UnityEngine;

public class LampInteractable : InteractableBase
{
    [SerializeField] private Light[] _lights;
    [SerializeField] private bool _isOn;

    private void Start()
    {
        UpdateLights();
    }

    public override void Interact()
    {
        if (!CanInteract)
        {
            return;
        }

        _isOn = !_isOn;
        UpdateLights();
    }

    private void UpdateLights()
    {
        foreach (Light lightSource in _lights)
        {
            if (lightSource != null)
            {
                lightSource.enabled = _isOn;
            }
        }
    }
}