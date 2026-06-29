using UnityEngine;
using UnityEngine.UI;

public class SudokuGridLines : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _boardSize = 720f;
    [SerializeField] private float _thinLine = 2f;
    [SerializeField] private float _thickLine = 6f;

    private RectTransform _root;

    private void Awake()
    {
        _root = GetComponent<RectTransform>();
    }

    private void Start()
    {
        ClearOldLines();
        CreateLines();
    }

    private void ClearOldLines()
    {
        for (int i = _root.childCount - 1; i >= 0; i--)
        {
            Destroy(_root.GetChild(i).gameObject);
        }
    }

    private void CreateLines()
    {
        float cellSize = _boardSize / 9f;

        for (int i = 0; i <= 9; i++)
        {
            float thickness = i % 3 == 0 ? _thickLine : _thinLine;
            float position = -_boardSize / 2f + i * cellSize;

            CreateVerticalLine(position, thickness);
            CreateHorizontalLine(position, thickness);
        }
    }

    private void CreateVerticalLine(float x, float thickness)
    {
        GameObject line = new GameObject("VerticalLine", typeof(RectTransform), typeof(Image));
        line.transform.SetParent(_root, false);

        RectTransform rect = line.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(thickness, _boardSize);
        rect.anchoredPosition = new Vector2(x, 0);

        Image image = line.GetComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = false;
    }

    private void CreateHorizontalLine(float y, float thickness)
    {
        GameObject line = new GameObject("HorizontalLine", typeof(RectTransform), typeof(Image));
        line.transform.SetParent(_root, false);

        RectTransform rect = line.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(_boardSize, thickness);
        rect.anchoredPosition = new Vector2(0, y);

        Image image = line.GetComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = false;
    }
}