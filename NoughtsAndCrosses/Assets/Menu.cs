using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private GridGenerator _gameManager;

    [SerializeField] private Text _stateMessage;

    [Header("Menu buttons")]
    [SerializeField] private Button _button3x3;
    [SerializeField] private Button _button5x5;
    [SerializeField] private Button _button10x10;
    [SerializeField] private Button _submitButton;
    [SerializeField] private InputField _rowsInput;
    [SerializeField] private InputField _colsInput;
    [SerializeField] private Button _exitButton;

    private const int MinGridSize = 3;
    private const int MaxGridSize = 25;

    public void Start()
    {
        _button3x3.onClick.AddListener(() => StartGame(3, 3));
        _button5x5.onClick.AddListener(() => StartGame(5, 5));
        _button10x10.onClick.AddListener(() => StartGame(10, 10));
        _submitButton.onClick.AddListener(StartCustomGame);
        _exitButton.onClick.AddListener(() => ExitGame());

        _stateMessage.text = "Choose grid size";
    }

    private void StartGame(int rows, int cols)
    {
        gameObject.SetActive(false);
        _stateMessage.text = "";
        _gameManager.StartGame(rows, cols);
    }

    private void StartCustomGame()
    {
        if (!int.TryParse(_rowsInput.text, out int rows))
        {
            _stateMessage.text = "Wrong rows";
            return;
        }

        if (!int.TryParse(_colsInput.text, out int cols))
        {
            _stateMessage.text = "Wrong cols";
            return;
        }

        if (rows < MinGridSize || cols < MinGridSize)
        {
            _stateMessage.text = $"Minimum size is {MinGridSize}x{MinGridSize}";
            return;
        }

        if (rows > MaxGridSize || cols > MaxGridSize)
        {
            _stateMessage.text = $"Maximum size is {MaxGridSize}x{MaxGridSize}";
            return;
        }

        StartGame(rows, cols);
    }

    private void ExitGame()
    {
        Debug.Log("Exit Game!");
        Application.Quit();
    }
}
