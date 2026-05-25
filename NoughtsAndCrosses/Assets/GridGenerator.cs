using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    public enum CellState
    {
        Empty,
        X,
        O
    }

    private GameObject[,] _buttons;
    private CellState[,] _cells;
    private Text[,] _cellText;
    private CellState _currentTurn = CellState.X;
    private bool _gameOver = false;
    private int _rows;
    private int _cols;
    private float _spacing;
    private readonly List<Vector2Int> _winCells = new List<Vector2Int>();

    [SerializeField] private GameObject _btnPref;
    [SerializeField] private Transform _parent;
    [SerializeField] private Text _stateMessage;

    [Header("Button style")]
    [SerializeField] private Color _XColor = Color.green;
    [SerializeField] private Color _0Color = Color.red;
    [SerializeField] private Color _pressedButton = Color.lightGray;
    [SerializeField] private Color _textButton = Color.aliceBlue;


    [Header("Grid Sithe")]
    [SerializeField] private float _maxGridWidth = 1200f;
    [SerializeField] private float _maxGridHeight = 1200f;
    [SerializeField] private float _minButtonSize = 18f;
    [SerializeField] private float _maxButtonSize = 100f;
    [SerializeField] private float _gap = 4f;
    [SerializeField] private int _winLength = 3;

    public void StartGame(int rows, int cols)
    {
        _rows = rows;
        _cols = cols;

        if (rows > 10 && _winLength < 5)
            _winLength = 5;

        _gameOver = false;
        _currentTurn = CellState.X;
        _stateMessage.text = "";

        ClearGrid();
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        float buttonSize = CalculateButtonSize();
        _spacing = buttonSize + _gap;

        _buttons = new GameObject[_rows, _cols];
        _cells = new CellState[_rows, _cols];
        _cellText = new Text[_rows, _cols];

        float offsetX = (_cols - 1) * _spacing / 2f;
        float offsetY = (_rows - 1) * _spacing /2f;

        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _cols; j++)
            {
                GameObject button = Instantiate(_btnPref, _parent);
                button.SetActive(true);

                RectTransform rect = button.GetComponent<RectTransform>();

                rect.sizeDelta = new Vector2(buttonSize, buttonSize);

                rect.anchoredPosition = new Vector2(
                    j * _spacing - offsetX,
                    -i * _spacing + offsetY
                );

                _buttons[i, j] = button;
                _cells[i, j] = CellState.Empty;


                Text cellText = button.GetComponentInChildren<Text>();
                _cellText[i, j] = cellText;

                int captureRow = i;
                int captureColl = j;

                button.GetComponent<Button>().onClick.AddListener(() => SetCell(captureRow, captureColl));
            }
        }

    }

    private void SetCell(int row, int col)
    {
        if (_gameOver)
        {
            return;
        }


        if (_cells[row, col] != CellState.Empty)
            return;

        _cells[row, col] = _currentTurn;

        _cellText[row, col].text = _currentTurn == CellState.X ? "X" : "O";
        _cellText[row, col].color = _textButton;
        _buttons[row, col].GetComponent<Image>().color = _pressedButton;


        if (CheckWin(_currentTurn))
        {
            PaintWinCells();

            _stateMessage.text = $"{_currentTurn} win";
            _gameOver = true;
            return;
        }

        if (IsDraw())
        {
            _stateMessage.text = "Draw";
            _gameOver = true;
            return;
        }

        SwitchTurn();

        if (_currentTurn == CellState.O)
        {
            MakeAiMove();
        }
    }

    private void SwitchTurn()
    {
        _currentTurn = _currentTurn == CellState.X
            ? CellState.O
            : CellState.X;
    }

    private bool CheckWin(CellState state)
    {
        _winCells.Clear();

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                if (_cells[row, col] != state)
                    continue;

                if (CheckDirection(row, col, 1, 0, state))
                    return true;

                if (CheckDirection(row, col, 0, 1, state))
                    return true;

                if (CheckDirection(row, col, 1, 1, state))
                    return true;

                if (CheckDirection(row, col, 1, -1, state))
                    return true;
            }
        }

        return false;
    }

    private bool IsDraw()
    {
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                if (_cells[row, col] == CellState.Empty)
                    return false;
            }
        }

        return true;
    }

    private void MakeAiMove()
    {
        if (_gameOver)
            return;

        if (TryGetAiMove(out int row, out int col))
            SetCell(row, col);
    }


    private bool TryFindBestMove(CellState state, out int bestRow, out int bestCol)
    {
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                if (_cells[row, col] != CellState.Empty)
                    continue;

                _cells[row, col] = state;

                bool isWin = CheckWin(state);

                _cells[row, col] = CellState.Empty;

                if (isWin)
                {
                    bestRow = row;
                    bestCol = col;
                    return true;
                }
            }
        }

        bestRow = -1;
        bestCol = -1;
        return false;
    }

    private void ClearGrid()
    {
        foreach (Transform child in _parent)
        {
            Destroy(child.gameObject);
        }
    }

    private float CalculateButtonSize()
    {
        float sizeByWidth = _maxGridWidth / _cols;
        float sizeByHeight = _maxGridHeight / _rows;

        float size = Mathf.Min(sizeByWidth, sizeByHeight) - _gap;

        return Mathf.Clamp(size, _minButtonSize, _maxButtonSize);
    }

    private bool CheckDirection(int startRow, int startCol, int rowDir, int colDir, CellState state)
    {
        List<Vector2Int> tempWinCells = new List<Vector2Int>();

        for (int i = 0; i < _winLength; i++)
        {
            int row = startRow + rowDir * i;
            int col = startCol + colDir * i;

            if (row < 0 || row >= _rows || col < 0 || col >= _cols)
                return false;

            if (_cells[row, col] != state)
                return false;

            tempWinCells.Add(new Vector2Int(row, col));
        }

        _winCells.Clear();
        _winCells.AddRange(tempWinCells);

        return true;
    }

    private void PaintWinCells()
    {
        foreach (Vector2Int cell in _winCells)
        {
            Image image = _buttons[cell.x, cell.y].GetComponent<Image>();

            if (image != null)
            {
                image.color = _currentTurn == CellState.X ? _XColor : _0Color;
                _cellText[cell.x, cell.y].color = _currentTurn == CellState.X ? _0Color : _XColor;
            }
        }
    }

    public void ResetGame(GameObject objectToHide)
    {
        if (objectToHide != null)
            objectToHide.SetActive(false);

        StartGame(_rows, _cols);
    }

    private bool TryGetAiMove(out int row, out int col)
    {
        if (TryFindBestMove(CellState.O, out row, out col))
            return true;

        if (TryFindBestMove(CellState.X, out row, out col))
            return true;

        int centerRow = _rows / 2;
        int centerCol = _cols / 2;

        if (_cells[centerRow, centerCol] == CellState.Empty)
        {
            row = centerRow;
            col = centerCol;
            return true;
        }

        Vector2Int[] corners =
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, _cols - 1),
            new Vector2Int(_rows - 1, 0),
            new Vector2Int(_rows - 1, _cols - 1)
        };

        foreach (Vector2Int corner in corners)
        {
            if (_cells[corner.x, corner.y] == CellState.Empty)
            {
                row = corner.x;
                col = corner.y;
                return true;
            }
        }

        return TryFindFirstEmptyCell(out row, out col);
    }

    private bool TryFindFirstEmptyCell(out int row, out int col)
    {
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _cols; c++)
            {
                if (_cells[r, c] == CellState.Empty)
                {
                    row = r;
                    col = c;
                    return true;
                }
            }
        }

        row = -1;
        col = -1;
        return false;
    }
}
