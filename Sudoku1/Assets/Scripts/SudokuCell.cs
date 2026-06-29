using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class SudokuCell : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _background;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;

    [Header("Colors")]
    [SerializeField] private Color _emptyColor = Color.white;
    [SerializeField] private Color _lockedColor = new Color(0.82f, 0.82f, 0.82f);
    [SerializeField] private Color _selectedColor = new Color(0.55f, 0.8f, 1f);
    [SerializeField] private Color _wrongColor = new Color(1f, 0.45f, 0.45f);
    [SerializeField] private Color _lockedTextColor = Color.black;
    [SerializeField] private Color _userTextColor = new Color(0.05f, 0.2f, 0.85f);

    private SudokuGame _game;

    private int _row;
    private int _col;
    private int _value;

    private bool _isLocked;
    private bool _isSelected;
    private bool _isWrong;

    public int Row => _row;
    public int Col => _col;
    public int Value => _value;
    public bool IsLocked => _isLocked;

    public void Init(SudokuGame game, int row, int col)
    {
        _game = game;
        _row = row;
        _col = col;

        if (_background == null)
            _background = GetComponent<Image>();

        if (_button == null)
            _button = GetComponent<Button>();

        if (_text == null)
            _text = GetComponentInChildren<TextMeshProUGUI>();

        if (_game == null)
        {
            Debug.LogError($"{name}: SudokuGame reference is missing.");
            return;
        }

        if (_background == null)
        {
            Debug.LogError($"{name}: Image component is missing.");
            return;
        }

        if (_button == null)
        {
            Debug.LogError($"{name}: Button component is missing.");
            return;
        }

        if (_text == null)
        {
            Debug.LogError($"{name}: TextMeshProUGUI component is missing.");
            return;
        }

        _button.transition = Selectable.Transition.None;
        _button.targetGraphic = _background;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnCellClick);

        _background.raycastTarget = true;

        _text.raycastTarget = false;
        _text.alignment = TextAlignmentOptions.Center;
        _text.fontSize = 36;

        SetValue(0, false);
        SetSelected(false);
        SetWrong(false);
    }

    public void SetValue(int value, bool locked)
    {
        _value = value;
        _isLocked = locked;
        _isWrong = false;

        _text.text = value == 0 ? "" : value.ToString();
        _text.color = _isLocked ? _lockedTextColor : _userTextColor;

        RefreshColor();
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        RefreshColor();
    }

    public void SetWrong(bool wrong)
    {
        _isWrong = wrong;
        RefreshColor();
    }

    private void RefreshColor()
    {
        if (_background == null)
            return;

        if (_isWrong)
        {
            _background.color = _wrongColor;
            return;
        }

        if (_isSelected)
        {
            _background.color = _selectedColor;
            return;
        }

        if (_isLocked)
        {
            _background.color = _lockedColor;
            return;
        }

        _background.color = _emptyColor;
    }

    private void OnCellClick()
    {
        if (_game == null)
        {
            Debug.LogError($"{name}: cell was not initialized. SudokuGame reference is null.");
            return;
        }

        _game.SelectCell(this);
    }
}