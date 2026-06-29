using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SudokuGame : MonoBehaviour
{
    [Header("Roots")]
    [SerializeField] private RectTransform _cellsRoot;
    [SerializeField] private GridLayoutGroup _gridLayout;

    [Header("Prefab")]
    [SerializeField] private SudokuCell _cellPrefab;

    [Header("Number Buttons")]
    [SerializeField] private Button[] _numberButtons;
    [SerializeField] private Button _clearButton;

    [Header("Generated Number Panel")]
    [SerializeField] private bool _autoCreateNumberPanel = true;
    [SerializeField] private RectTransform _numberPanelRoot;
    [SerializeField] private float _numberPanelWidth = 300f;
    [SerializeField] private float _numberPanelHeight = 400f;
    [SerializeField] private float _numberPanelOffsetX = -570f;
    [SerializeField] private float _numberPanelOffsetY = 165f;
    [SerializeField] private int _columnCounts = 3;

    [Header("Settings")]
    [SerializeField] private int _size = 9;
    [SerializeField] private int _startNumbersCount = 30;
    [SerializeField] private float _cellSize = 80f;

    private SudokuCell[,] _cells;
    private SudokuCell _selectedCell;

    private int[,] _solution;
    private int[,] _puzzle;

    private bool _boardCreated;

    private void Awake()
    {
        InitComponents();

        if (!enabled)
            return;

        CreateBoard();

        if (_autoCreateNumberPanel)
            CreateNumberPanelIfNeeded();

        InitNumberButtons();
        StartNewGame();
    }

    private void Update()
    {
        HandleKeyboardInput();
    }

    private void CreateNumberPanelIfNeeded()
    {
        if (_numberButtons != null && _numberButtons.Length == 9 && _clearButton != null)
            return;

        RectTransform parent = transform as RectTransform;

        if (parent == null)
        {
            Debug.LogError("SudokuGame: this object must have RectTransform.");
            return;
        }

        GameObject panelObject = new GameObject("NumberPanel", typeof(RectTransform), typeof(Image), typeof(GridLayoutGroup));
        panelObject.transform.SetParent(transform, false);

        _numberPanelRoot = panelObject.GetComponent<RectTransform>();
        _numberPanelRoot.anchorMin = new Vector2(0.5f, 0.5f);
        _numberPanelRoot.anchorMax = new Vector2(0.5f, 0.5f);
        _numberPanelRoot.pivot = new Vector2(0.5f, 0.5f);
        _numberPanelRoot.sizeDelta = new Vector2(_numberPanelWidth, _numberPanelHeight);
        _numberPanelRoot.anchoredPosition = new Vector2(_numberPanelOffsetX, _numberPanelOffsetY);

        Image panelImage = panelObject.GetComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0f);
        panelImage.raycastTarget = false;

        GridLayoutGroup layout = panelObject.GetComponent<GridLayoutGroup>();
        layout.padding = new RectOffset(0, 0, 0, 0);
        layout.cellSize = new Vector2(80f, 80f);
        layout.spacing = new Vector2(0f, 0f);
        layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        layout.startAxis = GridLayoutGroup.Axis.Horizontal;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = _columnCounts;
        layout.childAlignment = TextAnchor.MiddleCenter;

        _numberButtons = new Button[9];

        for (int number = 1; number <= 9; number++)
        {
            _numberButtons[number - 1] = CreateNumberButton(number.ToString(), _numberPanelRoot);
        }

        _clearButton = CreateNumberButton("Clear", _numberPanelRoot);
    }

    private Button CreateNumberButton(string label, RectTransform parent)
    {
        GameObject buttonObject = new GameObject($"Button_{label}", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.9f, 0.9f, 0.9f);
        image.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;

        LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.minWidth = 55f;
        layoutElement.minHeight = 55f;
        layoutElement.preferredHeight = 55f;

        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(buttonObject.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 24;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.black;
        text.raycastTarget = false;

        return button;
    }

    private void HandleKeyboardInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        if (_selectedCell == null)
            return;

        if (_selectedCell.IsLocked)
            return;

        if (keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame)
        {
            SetNumberToSelectedCell(1);
            return;
        }

        if (keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame)
        {
            SetNumberToSelectedCell(2);
            return;
        }

        if (keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame)
        {
            SetNumberToSelectedCell(3);
            return;
        }

        if (keyboard.digit4Key.wasPressedThisFrame || keyboard.numpad4Key.wasPressedThisFrame)
        {
            SetNumberToSelectedCell(4);
            return;
        }

        if (keyboard.digit5Key.wasPressedThisFrame || keyboard.numpad5Key.wasPressedThisFrame)
        {
            SetNumberToSelectedCell(5);
            return;
        }

        if (keyboard.digit6Key.wasPressedThisFrame || keyboard.numpad6Key.wasPressedThisFrame)
        {
            SetNumberToSelectedCell(6);
            return;
        }

        if (keyboard.digit7Key.wasPressedThisFrame || keyboard.numpad7Key.wasPressedThisFrame)
        {
            SetNumberToSelectedCell(7);
            return;
        }

        if (keyboard.digit8Key.wasPressedThisFrame || keyboard.numpad8Key.wasPressedThisFrame)
        {
            SetNumberToSelectedCell(8);
            return;
        }

        if (keyboard.digit9Key.wasPressedThisFrame || keyboard.numpad9Key.wasPressedThisFrame)
        {
            SetNumberToSelectedCell(9);
            return;
        }

        if (
            keyboard.digit0Key.wasPressedThisFrame ||
            keyboard.numpad0Key.wasPressedThisFrame ||
            keyboard.backspaceKey.wasPressedThisFrame ||
            keyboard.deleteKey.wasPressedThisFrame
        )
        {
            SetNumberToSelectedCell(0);
        }
    }

    private void InitComponents()
    {
        if (_cellsRoot == null)
        {
            Debug.LogError("SudokuGame: не указан CellsRoot.");
            enabled = false;
            return;
        }

        if (_gridLayout == null)
            _gridLayout = _cellsRoot.GetComponent<GridLayoutGroup>();

        if (_gridLayout == null)
        {
            Debug.LogError("SudokuGame: на CellsRoot нет GridLayoutGroup.");
            enabled = false;
            return;
        }

        if (_cellPrefab == null)
        {
            Debug.LogError("SudokuGame: не указан SudokuCell prefab.");
            enabled = false;
            return;
        }

        _startNumbersCount = Mathf.Clamp(_startNumbersCount, 1, 81);
    }

    private void CreateBoard()
    {
        if (_boardCreated)
            return;

        _cells = new SudokuCell[_size, _size];

        _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayout.constraintCount = _size;
        _gridLayout.cellSize = new Vector2(_cellSize, _cellSize);
        _gridLayout.spacing = Vector2.zero;
        _gridLayout.childAlignment = TextAnchor.MiddleCenter;
        _gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        _gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;

        for (int row = 0; row < _size; row++)
        {
            for (int col = 0; col < _size; col++)
            {
                SudokuCell cell = Instantiate(_cellPrefab, _cellsRoot);

                cell.name = $"Cell_{row}_{col}";
                cell.Init(this, row, col);

                _cells[row, col] = cell;
            }
        }

        _boardCreated = true;
    }

    private void InitNumberButtons()
    {
        if (_numberButtons != null)
        {
            for (int i = 0; i < _numberButtons.Length; i++)
            {
                int number = i + 1;

                if (_numberButtons[i] == null)
                    continue;

                _numberButtons[i].onClick.RemoveAllListeners();
                _numberButtons[i].onClick.AddListener(() => SetNumberToSelectedCell(number));
            }
        }

        if (_clearButton != null)
        {
            _clearButton.onClick.RemoveAllListeners();
            _clearButton.onClick.AddListener(() => SetNumberToSelectedCell(0));
        }
    }

    public void StartNewGame()
    {
        if (!_boardCreated)
            CreateBoard();

        _selectedCell = null;

        _solution = GenerateSolvedSudoku();
        _puzzle = CreatePuzzleFromSolution(_solution, _startNumbersCount);

        LoadPuzzle();
    }

    private void LoadPuzzle()
    {
        for (int row = 0; row < _size; row++)
        {
            for (int col = 0; col < _size; col++)
            {
                int value = _puzzle[row, col];
                bool locked = value != 0;

                _cells[row, col].SetValue(value, locked);
                _cells[row, col].SetSelected(false);
                _cells[row, col].SetWrong(false);
            }
        }
    }

    public void SelectCell(SudokuCell cell)
    {
        if (_selectedCell != null)
            _selectedCell.SetSelected(false);

        _selectedCell = cell;
        _selectedCell.SetSelected(true);
    }

    public void SetNumberToSelectedCell(int number)
    {
        if (_selectedCell == null)
        {
            Debug.Log("Сначала выбери клетку.");
            return;
        }

        if (_selectedCell.IsLocked)
        {
            Debug.Log("Эта клетка заблокирована.");
            return;
        }

        int row = _selectedCell.Row;
        int col = _selectedCell.Col;

        _puzzle[row, col] = number;
        _selectedCell.SetValue(number, false);

        if (number != 0 && number != _solution[row, col])
        {
            _selectedCell.SetWrong(true);
        }
        else
        {
            _selectedCell.SetWrong(false);
            _selectedCell.SetSelected(true);
        }

        CheckWin();
    }

    private void CheckWin()
    {
        for (int row = 0; row < _size; row++)
        {
            for (int col = 0; col < _size; col++)
            {
                if (_puzzle[row, col] != _solution[row, col])
                    return;
            }
        }

        Debug.Log("Sudoku solved!");
    }

    private int[,] GenerateSolvedSudoku()
    {
        int[,] board = new int[9, 9];
        FillBoard(board);
        return board;
    }

    private bool FillBoard(int[,] board)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board[row, col] != 0)
                    continue;

                int[] numbers = GetShuffledNumbers();

                foreach (int number in numbers)
                {
                    if (IsValidNumber(board, row, col, number))
                    {
                        board[row, col] = number;

                        if (FillBoard(board))
                            return true;

                        board[row, col] = 0;
                    }
                }

                return false;
            }
        }

        return true;
    }

    private int[] GetShuffledNumbers()
    {
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        for (int i = 0; i < numbers.Length; i++)
        {
            int randomIndex = Random.Range(i, numbers.Length);

            int temp = numbers[i];
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        return numbers;
    }

    private bool IsValidNumber(int[,] board, int row, int col, int number)
    {
        for (int i = 0; i < 9; i++)
        {
            if (board[row, i] == number)
                return false;

            if (board[i, col] == number)
                return false;
        }

        int blockRow = row / 3 * 3;
        int blockCol = col / 3 * 3;

        for (int r = blockRow; r < blockRow + 3; r++)
        {
            for (int c = blockCol; c < blockCol + 3; c++)
            {
                if (board[r, c] == number)
                    return false;
            }
        }

        return true;
    }

    private int[,] CreatePuzzleFromSolution(int[,] solution, int startNumbersCount)
    {
        int[,] puzzle = new int[9, 9];

        int placedNumbers = 0;

        while (placedNumbers < startNumbersCount)
        {
            int row = Random.Range(0, 9);
            int col = Random.Range(0, 9);

            if (puzzle[row, col] != 0)
                continue;

            puzzle[row, col] = solution[row, col];
            placedNumbers++;
        }

        return puzzle;
    }
}