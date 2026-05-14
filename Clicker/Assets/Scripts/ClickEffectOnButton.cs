using UnityEngine;
using UnityEngine.EventSystems;

public class ClickEffectOnButton : MonoBehaviour, IPointerDownHandler
{
    [Header("Effect")]
    [SerializeField] private GameObject _clickEffectPrefab;
    [SerializeField] private RectTransform _effectsLayer;
    [SerializeField] private float _destroyDelay = 2f;

    private Canvas _canvas;
    private Camera _uiCamera;

    private void Awake()
    {
        _canvas = _effectsLayer.GetComponentInParent<Canvas>();

        if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            _uiCamera = null;
        }
        else
        {
            _uiCamera = _canvas.worldCamera;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SpawnEffect(eventData);
    }

    private void SpawnEffect(PointerEventData eventData)
    {
        if (_clickEffectPrefab == null || _effectsLayer == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _effectsLayer,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        GameObject effect = Instantiate(_clickEffectPrefab, _effectsLayer, false);

        RectTransform effectRect = effect.GetComponent<RectTransform>();

        if (effectRect != null)
        {
            effectRect.anchorMin = new Vector2(0.5f, 0.5f);
            effectRect.anchorMax = new Vector2(0.5f, 0.5f);
            effectRect.pivot = new Vector2(0.5f, 0.5f);
            effectRect.anchoredPosition = localPoint;
            effectRect.localScale = Vector3.one;
            effectRect.localRotation = Quaternion.identity;
        }
        else
        {
            effect.transform.localPosition = localPoint;
            effect.transform.localScale = Vector3.one;
            effect.transform.localRotation = Quaternion.identity;
        }

        Destroy(effect, _destroyDelay);
    }
}
