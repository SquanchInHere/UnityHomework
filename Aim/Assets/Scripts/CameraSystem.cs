using System;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Camera _topCamera;

    [SerializeField] private float _normalFov = 60f;
    [SerializeField] private float _zoomFov = 20f;
    [SerializeField] private float _zoomSpeed = 10f;

    [SerializeField] private KeyCode _zoomKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode _topCameraKey = KeyCode.G;

    [SerializeField] private bool _showCrosshair = true;
    [SerializeField] private bool _showOnlyWhenZooming = false;
    [SerializeField] private bool _isTopView = true;

    [SerializeField] private string _crosshairText = "+";

    [SerializeField] private int _crosshairFontSize = 40;

    [SerializeField] private Color _crosshairColor = Color.red;

    [SerializeField] private Vector3 _topCameraRotation = new Vector3(90f, 0f, 0f);

    void Start()
    {
        if (_playerCamera == null)
        {
            Debug.Log("No player camera");
            return;
        }

        _playerCamera.fieldOfView = _normalFov;

        if (_topCamera != null)
        {
            _topCamera.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        HandleZoom();
        HandleTopView();
    }

    private void HandleZoom()
    {
        bool isZooming = Input.GetMouseButton(1);

        float targetFov = isZooming ? _zoomFov : _normalFov;

        _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, targetFov, _zoomSpeed * Time.deltaTime);
    }

    private void HandleTopView()
    {
        if (!Input.GetKeyDown(_topCameraKey))
        {
            return;
        }

        _isTopView = !_isTopView;

        _playerCamera.gameObject.SetActive(!_isTopView);

        if (_topCamera != null)
        {
            _topCamera.gameObject.SetActive(_topCamera);
        }
    }

    private void LateUpdate()
    {
        if (!_isTopView)
        {
            return;
        }

        if (_topCamera == null)
        {
            return;
        }

        _topCamera.transform.rotation = Quaternion.Euler(_topCameraRotation);
    }

    private void OnGUI()
    {
        if (!_showCrosshair)
        {
            return;
        }

        if (_showOnlyWhenZooming && Input.GetMouseButtonDown(1))
        {
            return;
        }

        GUIStyle style = new GUIStyle(GUI.skin.label);

        style.fontSize = _crosshairFontSize;
        style.normal.textColor = _crosshairColor;

        Vector2 textSize = style.CalcSize(new GUIContent(_crosshairText));

        float centerX = (Screen.width - textSize.x) / 2f;
        float centerY = (Screen.height - textSize.y) / 2f;

        GUI.Label(new Rect(centerX, centerY, textSize.x, textSize.y), _crosshairText, style);
    }
}