using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _cellPrefab;

    [Header("Grid Parents")]
    [SerializeField] private Transform _leftGrid;
    [SerializeField] private Transform _rightGrid;

    [Header("UI")]
    [SerializeField] private Button _startBattleButton;
    [SerializeField] private Text _setupInfoText;

    [Header("Ships")]
    [SerializeField]
    private List<ShipConfig> _ships = new List<ShipConfig>
    {
        new ShipConfig { Size = 4, Count = 1 },
        new ShipConfig { Size = 3, Count = 2 },
        new ShipConfig { Size = 2, Count = 3 },
        new ShipConfig { Size = 1, Count = 4 }
    };

    [Header("Symbols")]
    [SerializeField] private string _emptySymbol = "";
    [SerializeField] private string _shipSymbol = "#";
    [SerializeField] private string _missSymbol = "O";
    [SerializeField] private string _hitSymbol = "X";

    [Header("Battle Colors")]
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _shipDamage = Color.orange;
    [SerializeField] private Color _shipDead = Color.red;
    [SerializeField] private Color _missColor = Color.grey;

    [Header("Setup Colors")]
    [SerializeField] private Color _setupShipColor = Color.cyan;
    [SerializeField] private Color _setupInvalidColor = Color.red;

    private const int Rows = 10;
    private const int Cols = 10;

    private Board _playerBoard;
    private Board _enemyBoard;

    private readonly List<Vector2Int> _aiTargets = new List<Vector2Int>();

    private bool _setupMode = true;
    private bool _gameOver;

    private void Start()
    {
        _playerBoard = new Board(Rows, Cols);
        _enemyBoard = new Board(Rows, Cols);

        InitBoard(_playerBoard);
        InitBoard(_enemyBoard);

        PlaceAllShips(_enemyBoard);

        CreateGrid(_leftGrid, _playerBoard, true, true);
        CreateGrid(_rightGrid, _enemyBoard, false, false);

        if (_startBattleButton != null)
        {
            _startBattleButton.onClick.AddListener(StartBattle);
            _startBattleButton.interactable = false;
        }

        SetBoardButtons(_enemyBoard, false);
        UpdateSetupInfo();
    }

    private void InitBoard(Board board)
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                board.Field[row, col] = _emptySymbol;
                board.Shots[row, col] = false;
            }
        }
    }

    private void CreateGrid(
        Transform grid,
        Board board,
        bool isPlayerBoard,
        bool showShips
    )
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                int row = i;
                int col = j;

                GameObject cell = Instantiate(_cellPrefab, grid);

                Text text = cell.GetComponentInChildren<Text>();
                Button button = cell.GetComponent<Button>();
                Image image = cell.GetComponent<Image>();

                if (text == null || button == null || image == null)
                {
                    Debug.LogError("Cell prefab must have Text, Button and Image components.");
                    return;
                }

                button.transition = Selectable.Transition.None;

                board.Texts[row, col] = text;
                board.Images[row, col] = image;
                board.Buttons[row, col] = button;

                image.color = _defaultColor;
                text.text = showShips ? board.Field[row, col] : _emptySymbol;

                if (isPlayerBoard)
                {
                    button.onClick.AddListener(() => PlacePlayerShipCell(row, col));
                }
                else
                {
                    button.onClick.AddListener(() => PlayerShoot(row, col));
                }
            }
        }
    }

    private void PlacePlayerShipCell(int row, int col)
    {
        if (!_setupMode)
            return;

        if (_playerBoard.Field[row, col] == _shipSymbol)
        {
            _playerBoard.Field[row, col] = _emptySymbol;
            _playerBoard.Texts[row, col].text = _emptySymbol;
            _playerBoard.Images[row, col].color = _defaultColor;

            UpdateSetupInfo();
            return;
        }

        if (_playerBoard.Field[row, col] != _emptySymbol)
            return;

        if (!CanPlacePlayerCell(row, col, out string error))
        {
            if (_setupInfoText != null)
                _setupInfoText.text = error;

            FlashCell(_playerBoard, row, col, _setupInvalidColor);
            return;
        }

        _playerBoard.Field[row, col] = _shipSymbol;
        _playerBoard.Texts[row, col].text = _shipSymbol;

        UpdateSetupInfo();
    }

    private bool CanPlacePlayerCell(int row, int col, out string error)
    {
        error = "";

        if (CountShipCells(_playerBoard) >= GetRequiredShipCells())
        {
            error = "All ship cells are already placed.";
            return false;
        }

        if (WouldTouchDiagonally(row, col))
        {
            error = "Invalid move. Ships cannot touch diagonally.";
            return false;
        }

        _playerBoard.Field[row, col] = _shipSymbol;

        List<List<Vector2Int>> ships = GetShips(_playerBoard);

        foreach (List<Vector2Int> ship in ships)
        {
            if (!IsStraightShip(ship))
            {
                _playerBoard.Field[row, col] = _emptySymbol;
                error = "Invalid move. Ships must be straight.";
                return false;
            }

            if (ship.Count > GetMaxShipSize())
            {
                _playerBoard.Field[row, col] = _emptySymbol;
                error = $"Invalid move. Ship cannot be longer than {GetMaxShipSize()} cells.";
                return false;
            }
        }

        if (!CanStillMatchRequiredShips(ships))
        {
            _playerBoard.Field[row, col] = _emptySymbol;
            error = "Invalid move. This ship set cannot match the required fleet.";
            return false;
        }

        _playerBoard.Field[row, col] = _emptySymbol;
        return true;
    }

    private void UpdateSetupInfo()
    {
        RefreshSetupView();

        bool valid = GetSetupStatus(out string message);

        if (_setupInfoText != null)
            _setupInfoText.text = message;

        if (_startBattleButton != null)
            _startBattleButton.interactable = valid;
    }

    private bool GetSetupStatus(out string message)
    {
        int placedCells = CountShipCells(_playerBoard);
        int requiredCells = GetRequiredShipCells();

        List<int> currentShips = GetShipSizes(_playerBoard);
        List<int> requiredShips = GetRequiredShipSizes();

        currentShips.Sort();
        requiredShips.Sort();

        string currentText = FormatShips(currentShips);
        string requiredText = FormatShips(requiredShips);

        if (placedCells < requiredCells)
        {
            message =
                "Ship setup\n" +
                $"Cells: {placedCells}/{requiredCells}\n" +
                $"Required: {requiredText}\n" +
                $"Current: {currentText}\n" +
                $"Place {requiredCells - placedCells} more cells.";

            return false;
        }

        if (placedCells > requiredCells)
        {
            message =
                "Too many ship cells\n" +
                $"Cells: {placedCells}/{requiredCells}\n" +
                $"Remove {placedCells - requiredCells} cells.";

            return false;
        }

        if (HasDiagonalTouch(_playerBoard))
        {
            message =
                "Invalid ship setup\n" +
                "Ships cannot touch diagonally.";

            return false;
        }

        if (HasInvalidShape(_playerBoard))
        {
            message =
                "Invalid ship setup\n" +
                "Ships must be straight: horizontal or vertical only.";

            return false;
        }

        if (!SameShipSet(currentShips, requiredShips))
        {
            message =
                "Invalid ship set\n" +
                $"Required: {requiredText}\n" +
                $"Current: {currentText}";

            return false;
        }

        message =
            "Ship setup is ready\n" +
            $"Ships: {currentText}\n" +
            "You can start the battle.";

        return true;
    }

    private void RefreshSetupView()
    {
        if (!_setupMode)
            return;

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                if (_playerBoard.Field[row, col] == _shipSymbol)
                    _playerBoard.Images[row, col].color = _setupShipColor;
                else
                    _playerBoard.Images[row, col].color = _defaultColor;
            }
        }

        List<List<Vector2Int>> ships = GetShips(_playerBoard);

        foreach (List<Vector2Int> ship in ships)
        {
            bool invalid = false;

            if (!IsStraightShip(ship))
                invalid = true;

            if (ship.Count > GetMaxShipSize())
                invalid = true;

            if (HasShipDiagonalTouch(_playerBoard, ship))
                invalid = true;

            if (!invalid)
                continue;

            foreach (Vector2Int cell in ship)
            {
                _playerBoard.Images[cell.x, cell.y].color = _setupInvalidColor;
            }
        }
    }

    private void FlashCell(Board board, int row, int col, Color color)
    {
        if (board.Images[row, col] == null)
            return;

        board.Images[row, col].color = color;
    }

    private void StartBattle()
    {
        RefreshSetupView();

        if (!GetSetupStatus(out string message))
        {
            if (_setupInfoText != null)
                _setupInfoText.text = message;

            return;
        }

        _setupMode = false;

        SetBoardButtons(_playerBoard, false);
        SetBoardButtons(_enemyBoard, true);

        if (_setupInfoText != null)
            _setupInfoText.text = "Battle started. Shoot the enemy grid.";

        Debug.Log("Battle started.");
    }

    private void PlayerShoot(int row, int col)
    {
        if (_setupMode || _gameOver)
            return;

        bool shotDone = Shoot(_enemyBoard, row, col);

        if (!shotDone)
            return;

        if (AllShipsDestroyed(_enemyBoard))
        {
            _gameOver = true;
            SetBoardButtons(_enemyBoard, false);

            if (_setupInfoText != null)
                _setupInfoText.text = "Player wins.";

            Debug.Log("Player wins.");
            return;
        }

        AiShoot();

        if (AllShipsDestroyed(_playerBoard))
        {
            _gameOver = true;
            SetBoardButtons(_enemyBoard, false);

            if (_setupInfoText != null)
                _setupInfoText.text = "AI wins.";

            Debug.Log("AI wins.");
        }
    }

    private void AiShoot()
    {
        Vector2Int cell = GetAiCell();

        bool shotDone = Shoot(_playerBoard, cell.x, cell.y);

        if (!shotDone)
            return;

        if (_playerBoard.Field[cell.x, cell.y] == _hitSymbol)
        {
            AddAiTargets(cell.x, cell.y);
        }
    }

    private bool Shoot(Board board, int row, int col)
    {
        if (board.Shots[row, col])
            return false;

        board.Shots[row, col] = true;

        if (board.Buttons[row, col] != null)
            board.Buttons[row, col].interactable = false;

        if (board.Field[row, col] == _shipSymbol)
        {
            board.Field[row, col] = _hitSymbol;
            board.Texts[row, col].text = _hitSymbol;

            List<Vector2Int> ship = GetShipAfterHit(board, row, col);

            if (IsShipDead(board, ship))
            {
                PaintShip(board, ship, _shipDead);
                Debug.Log("Ship destroyed.");
            }
            else
            {
                board.Images[row, col].color = _shipDamage;
                Debug.Log("Ship damaged.");
            }
        }
        else
        {
            board.Field[row, col] = _missSymbol;
            board.Texts[row, col].text = _missSymbol;
            board.Images[row, col].color = _missColor;

            Debug.Log("Miss.");
        }

        return true;
    }

    private Vector2Int GetAiCell()
    {
        while (_aiTargets.Count > 0)
        {
            int index = Random.Range(0, _aiTargets.Count);
            Vector2Int cell = _aiTargets[index];

            _aiTargets.RemoveAt(index);

            if (IsInsideField(cell.x, cell.y) && !_playerBoard.Shots[cell.x, cell.y])
                return cell;
        }

        int row;
        int col;

        do
        {
            row = Random.Range(0, Rows);
            col = Random.Range(0, Cols);
        }
        while (_playerBoard.Shots[row, col]);

        return new Vector2Int(row, col);
    }

    private void AddAiTargets(int row, int col)
    {
        AddAiTarget(row - 1, col);
        AddAiTarget(row + 1, col);
        AddAiTarget(row, col - 1);
        AddAiTarget(row, col + 1);
    }

    private void AddAiTarget(int row, int col)
    {
        if (!IsInsideField(row, col))
            return;

        if (_playerBoard.Shots[row, col])
            return;

        Vector2Int cell = new Vector2Int(row, col);

        if (_aiTargets.Contains(cell))
            return;

        _aiTargets.Add(cell);
    }

    private int CountShipCells(Board board)
    {
        int count = 0;

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                if (board.Field[row, col] == _shipSymbol)
                    count++;
            }
        }

        return count;
    }

    private int GetRequiredShipCells()
    {
        int count = 0;

        foreach (ShipConfig ship in _ships)
        {
            count += ship.Size * ship.Count;
        }

        return count;
    }

    private List<int> GetRequiredShipSizes()
    {
        List<int> sizes = new List<int>();

        foreach (ShipConfig ship in _ships)
        {
            for (int i = 0; i < ship.Count; i++)
            {
                sizes.Add(ship.Size);
            }
        }

        return sizes;
    }

    private Dictionary<int, int> GetRequiredShipCountMap()
    {
        Dictionary<int, int> result = new Dictionary<int, int>();

        foreach (ShipConfig ship in _ships)
        {
            if (!result.ContainsKey(ship.Size))
                result[ship.Size] = 0;

            result[ship.Size] += ship.Count;
        }

        return result;
    }

    private int GetMaxShipSize()
    {
        int max = 0;

        foreach (ShipConfig ship in _ships)
        {
            if (ship.Size > max)
                max = ship.Size;
        }

        return max;
    }

    private bool CanStillMatchRequiredShips(List<List<Vector2Int>> ships)
    {
        Dictionary<int, int> required = GetRequiredShipCountMap();

        foreach (List<Vector2Int> ship in ships)
        {
            if (!IsStraightShip(ship))
                return false;

            int size = ship.Count;

            if (size > GetMaxShipSize())
                return false;

            bool canBeSomeRequiredShip = false;

            foreach (int requiredSize in required.Keys)
            {
                if (size <= requiredSize)
                {
                    canBeSomeRequiredShip = true;
                    break;
                }
            }

            if (!canBeSomeRequiredShip)
                return false;
        }

        return true;
    }

    private List<int> GetShipSizes(Board board)
    {
        List<int> sizes = new List<int>();
        List<List<Vector2Int>> ships = GetShips(board);

        foreach (List<Vector2Int> ship in ships)
        {
            if (!IsStraightShip(ship))
                sizes.Add(-1);
            else
                sizes.Add(ship.Count);
        }

        return sizes;
    }

    private List<List<Vector2Int>> GetShips(Board board)
    {
        List<List<Vector2Int>> ships = new List<List<Vector2Int>>();
        bool[,] visited = new bool[Rows, Cols];

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                if (visited[row, col])
                    continue;

                if (board.Field[row, col] != _shipSymbol)
                    continue;

                List<Vector2Int> ship = new List<Vector2Int>();

                CollectShipCells(board, row, col, visited, ship);

                ships.Add(ship);
            }
        }

        return ships;
    }

    private void CollectShipCells(
        Board board,
        int row,
        int col,
        bool[,] visited,
        List<Vector2Int> ship
    )
    {
        if (!IsInsideField(row, col))
            return;

        if (visited[row, col])
            return;

        if (board.Field[row, col] != _shipSymbol)
            return;

        visited[row, col] = true;
        ship.Add(new Vector2Int(row, col));

        CollectShipCells(board, row - 1, col, visited, ship);
        CollectShipCells(board, row + 1, col, visited, ship);
        CollectShipCells(board, row, col - 1, visited, ship);
        CollectShipCells(board, row, col + 1, visited, ship);
    }

    private List<Vector2Int> GetShipAfterHit(Board board, int row, int col)
    {
        List<Vector2Int> ship = new List<Vector2Int>();
        bool[,] visited = new bool[Rows, Cols];

        CollectShipAfterHit(board, row, col, visited, ship);

        return ship;
    }

    private void CollectShipAfterHit(
        Board board,
        int row,
        int col,
        bool[,] visited,
        List<Vector2Int> ship
    )
    {
        if (!IsInsideField(row, col))
            return;

        if (visited[row, col])
            return;

        if (board.Field[row, col] != _shipSymbol && board.Field[row, col] != _hitSymbol)
            return;

        visited[row, col] = true;
        ship.Add(new Vector2Int(row, col));

        CollectShipAfterHit(board, row - 1, col, visited, ship);
        CollectShipAfterHit(board, row + 1, col, visited, ship);
        CollectShipAfterHit(board, row, col - 1, visited, ship);
        CollectShipAfterHit(board, row, col + 1, visited, ship);
    }

    private bool IsStraightShip(List<Vector2Int> ship)
    {
        if (ship.Count <= 1)
            return true;

        bool sameRow = true;
        bool sameCol = true;

        int row = ship[0].x;
        int col = ship[0].y;

        foreach (Vector2Int cell in ship)
        {
            if (cell.x != row)
                sameRow = false;

            if (cell.y != col)
                sameCol = false;
        }

        return sameRow || sameCol;
    }

    private bool HasInvalidShape(Board board)
    {
        List<List<Vector2Int>> ships = GetShips(board);

        foreach (List<Vector2Int> ship in ships)
        {
            if (!IsStraightShip(ship))
                return true;
        }

        return false;
    }

    private bool WouldTouchDiagonally(int row, int col)
    {
        return
            IsShipCell(_playerBoard, row - 1, col - 1) ||
            IsShipCell(_playerBoard, row - 1, col + 1) ||
            IsShipCell(_playerBoard, row + 1, col - 1) ||
            IsShipCell(_playerBoard, row + 1, col + 1);
    }

    private bool HasDiagonalTouch(Board board)
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                if (board.Field[row, col] != _shipSymbol)
                    continue;

                if (IsShipCell(board, row - 1, col - 1))
                    return true;

                if (IsShipCell(board, row - 1, col + 1))
                    return true;

                if (IsShipCell(board, row + 1, col - 1))
                    return true;

                if (IsShipCell(board, row + 1, col + 1))
                    return true;
            }
        }

        return false;
    }

    private bool HasShipDiagonalTouch(Board board, List<Vector2Int> ship)
    {
        foreach (Vector2Int cell in ship)
        {
            if (IsShipCell(board, cell.x - 1, cell.y - 1))
                return true;

            if (IsShipCell(board, cell.x - 1, cell.y + 1))
                return true;

            if (IsShipCell(board, cell.x + 1, cell.y - 1))
                return true;

            if (IsShipCell(board, cell.x + 1, cell.y + 1))
                return true;
        }

        return false;
    }

    private bool IsShipCell(Board board, int row, int col)
    {
        if (!IsInsideField(row, col))
            return false;

        return board.Field[row, col] == _shipSymbol;
    }

    private bool SameShipSet(List<int> currentShips, List<int> requiredShips)
    {
        if (currentShips.Count != requiredShips.Count)
            return false;

        for (int i = 0; i < currentShips.Count; i++)
        {
            if (currentShips[i] != requiredShips[i])
                return false;
        }

        return true;
    }

    private string FormatShips(List<int> ships)
    {
        if (ships.Count == 0)
            return "none";

        Dictionary<int, int> countBySize = new Dictionary<int, int>();

        foreach (int size in ships)
        {
            if (!countBySize.ContainsKey(size))
                countBySize[size] = 0;

            countBySize[size]++;
        }

        List<int> sizes = new List<int>(countBySize.Keys);
        sizes.Sort();
        sizes.Reverse();

        List<string> parts = new List<string>();

        foreach (int size in sizes)
        {
            if (size <= 0)
                parts.Add("invalid shape");
            else
                parts.Add($"{size}-deck x{countBySize[size]}");
        }

        return string.Join(", ", parts);
    }

    private bool IsShipDead(Board board, List<Vector2Int> ship)
    {
        foreach (Vector2Int cell in ship)
        {
            if (board.Field[cell.x, cell.y] == _shipSymbol)
                return false;
        }

        return true;
    }

    private void PaintShip(Board board, List<Vector2Int> ship, Color color)
    {
        foreach (Vector2Int cell in ship)
        {
            board.Images[cell.x, cell.y].color = color;
        }
    }

    private bool AllShipsDestroyed(Board board)
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                if (board.Field[row, col] == _shipSymbol)
                    return false;
            }
        }

        return true;
    }

    private void PlaceAllShips(Board board)
    {
        foreach (ShipConfig ship in _ships)
        {
            PlaceShip(board, ship.Size, ship.Count);
        }
    }

    private void PlaceShip(Board board, int sizeShip, int count)
    {
        for (int i = 0; i < count; i++)
        {
            bool placed = false;
            int attempts = 0;

            while (!placed && attempts < 1000)
            {
                attempts++;

                bool horizontal = Random.Range(0, 2) == 0;

                int row = Random.Range(0, Rows);
                int col = Random.Range(0, Cols);

                if (!CanPlaceShip(board, sizeShip, row, col, horizontal))
                    continue;

                for (int part = 0; part < sizeShip; part++)
                {
                    int targetRow = row;
                    int targetCol = col;

                    if (horizontal)
                        targetCol += part;
                    else
                        targetRow += part;

                    board.Field[targetRow, targetCol] = _shipSymbol;
                }

                placed = true;
            }

            if (!placed)
            {
                Debug.LogError($"Failed to place ship. Size: {sizeShip}, Index: {i}");
            }
        }
    }

    private bool CanPlaceShip(Board board, int sizeShip, int row, int col, bool horizontal)
    {
        int endRow = row;
        int endCol = col;

        if (horizontal)
            endCol += sizeShip - 1;
        else
            endRow += sizeShip - 1;

        if (endRow >= Rows || endCol >= Cols)
            return false;

        for (int part = 0; part < sizeShip; part++)
        {
            int checkRow = row;
            int checkCol = col;

            if (horizontal)
                checkCol += part;
            else
                checkRow += part;

            if (board.Field[checkRow, checkCol] != _emptySymbol)
                return false;

            if (HasShipAround(board, checkRow, checkCol))
                return false;
        }

        return true;
    }

    private bool HasShipAround(Board board, int row, int col)
    {
        for (int checkRow = row - 1; checkRow <= row + 1; checkRow++)
        {
            for (int checkCol = col - 1; checkCol <= col + 1; checkCol++)
            {
                if (!IsInsideField(checkRow, checkCol))
                    continue;

                if (board.Field[checkRow, checkCol] == _shipSymbol)
                    return true;
            }
        }

        return false;
    }

    private void SetBoardButtons(Board board, bool state)
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                if (board.Buttons[row, col] != null)
                    board.Buttons[row, col].interactable = state;
            }
        }
    }

    private bool IsInsideField(int row, int col)
    {
        return row >= 0 && row < Rows && col >= 0 && col < Cols;
    }
}
