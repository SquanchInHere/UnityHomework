using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Puzzle : MonoBehaviour
{
    [SerializeField] private Texture2D[] _levelImages;
    [SerializeField] private string loadFromPath = "";

    [Header("Level params")]
    [SerializeField] private int _startLevelIndex = 0;
    [SerializeField] private int _startGridSize = 2;
    [SerializeField] private int _gridStep = 2;

    [SerializeField] private RectTransform board;
    [SerializeField] private RectTransform tray;
    [SerializeField] private RectTransform pieceLayer;


    [Header("Puzzle Settings")]
    [SerializeField] private float snapDistance = 40f;
    [SerializeField] private bool showBorders = true;
    [SerializeField] private float _nextLevelDelay = 1f;

    [Header("Dynamic Image Fit")]
    [SerializeField] private bool fitBoardToImage = true;
    [SerializeField] private float maxBoardWidth = 500f;
    [SerializeField] private float maxBoardHeight = 500f;

    [Header("Win UI")]
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private Text _winText;

    [Header("Sounds")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _piecePlacedSound;
    [SerializeField] private AudioClip _levelCompletedSound;
    [SerializeField] private AudioClip _gameWinSound;

    [Header("Buttons")]
    [SerializeField] private Button _resetGame;
    [SerializeField] private Button _exitGame;

    private Canvas canvas;

    private Texture2D _sourceImage;
    private int _cols;
    private int _rows;
    private int _currentLevelIndex;

    private int _totalPieces;
    private int _placedPieces;
    private bool _levelCompleted;
    private bool _gameCompleted;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        if (pieceLayer == null)
            pieceLayer = (RectTransform)transform;

        pieceLayer.anchorMin = Vector2.zero;
        pieceLayer.anchorMax = Vector2.one;
        pieceLayer.pivot = new Vector2(0.5f, 0.5f);
        pieceLayer.offsetMin = Vector2.zero;
        pieceLayer.offsetMax = Vector2.zero;

        if (!string.IsNullOrEmpty(loadFromPath))
            LoadImageFromDisk(loadFromPath);

        if (board == null || tray == null) return;

        if (_winPanel != null)
            _winPanel.SetActive(false);

        LoadLevel(_startLevelIndex);
    }

    void LoadImageFromDisk(string path)
    {
        if (!File.Exists(path)) return;

        byte[] data = File.ReadAllBytes(path);
        var tex = new Texture2D(2, 2);
        tex.LoadImage(data);
        _sourceImage = tex;
    }

    private void BuildPuzzle()
    {
        float boardW = board.rect.width;
        float boardH = board.rect.height;

        float cellW = boardW / _cols;
        float cellH = boardH / _rows;

        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _cols; j++)
            {
                var go = new GameObject($"Piece_{j}_{i}", typeof(RectTransform));

                var pieceRect = go.GetComponent<RectTransform>();

                pieceRect.SetParent(pieceLayer, false);
                pieceRect.anchorMin = pieceRect.anchorMax = new Vector2(0.5f, 0.5f);
                pieceRect.pivot = new Vector2(0.5f, 0.5f);
                pieceRect.sizeDelta = new Vector2(cellW, cellH);

                var img = go.AddComponent<RawImage>();
                img.texture = _sourceImage;
                img.uvRect = new Rect(
                    (float)j / _cols,
                    (float)i / _rows,
                    1f / _cols,
                    1f / _rows
                );

                if (showBorders)
                {
                    var outline = go.AddComponent<Outline>();
                    outline.effectColor = new Color(0f, 0f, 0f, 0.6f);
                    outline.effectDistance = new Vector2(2f, 2f);
                }

                go.AddComponent<CanvasGroup>();

                var drag = go.AddComponent<PuzzlePieceDrag>();
                drag.SnapDistance = snapDistance;
                drag.Init(this);

                Vector3 cellLocal = new Vector3(
                    board.rect.xMin + cellW * (j + 0.5f),
                    board.rect.yMin + cellH * (i + 0.5f),
                    0f
                );

                Vector3 cellWorld = board.TransformPoint(cellLocal);

                drag.TargetPosition = WorldToLayer(cellWorld);

                pieceRect.anchoredPosition = RandomPointInTray();
            }
        }
    }

    private Vector2 RandomPointInTray()
    {
        float x = Random.Range(tray.rect.xMin, tray.rect.xMax);
        float y = Random.Range(tray.rect.yMin, tray.rect.yMax);

        Vector3 world = tray.TransformPoint(new Vector3(x, y, 0f));

        return WorldToLayer(world);
    }

    private Vector2 WorldToLayer(Vector3 world)
    {
        Camera cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            ? canvas.worldCamera
            : null;

        Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, world);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            pieceLayer,
            screen,
            cam,
            out Vector2 local
        );

        return local;
    }

    private void FitBoardToSourceImage()
    {
        if (_sourceImage == null || board == null)
            return;

        float imageWidth = _sourceImage.width;
        float imageHeight = _sourceImage.height;

        if (imageWidth <= 0 || imageHeight <= 0)
            return;

        float imageAspect = imageWidth / imageHeight;
        float maxAspect = maxBoardWidth / maxBoardHeight;

        float finalWidth;
        float finalHeight;

        if (imageAspect > maxAspect)
        {
            finalWidth = maxBoardWidth;
            finalHeight = finalWidth / imageAspect;
        }
        else
        {
            finalHeight = maxBoardHeight;
            finalWidth = finalHeight * imageAspect;
        }

        board.anchorMin = new Vector2(0.5f, 0.5f);
        board.anchorMax = new Vector2(0.5f, 0.5f);
        board.pivot = new Vector2(0.5f, 0.5f);
        board.sizeDelta = new Vector2(finalWidth, finalHeight);
    }

    private void LoadLevel(int levelIndex)
    {
        if (_levelImages == null || _levelImages.Length == 0)
        {
            Debug.LogError("Level images array is empty.");
            return;
        }

        Texture2D image = _levelImages[levelIndex];
        _currentLevelIndex = levelIndex;
        int gridSize = _startGridSize + levelIndex * _gridStep;

        SetPuzzle(image, gridSize);
    }

    private void LoadNextLevel()
    {
        int nextIndex = _currentLevelIndex + 1;

        if (nextIndex >= _levelImages.Length)
        {
            WinGame();
            return;
        }

        LoadLevel(nextIndex);
    }

    private void SetPuzzle(Texture2D image, int gridSize)
    {
        if (image == null)
            return;

        _sourceImage = image;
        _cols = Mathf.Max(1, gridSize);
        _rows = Mathf.Max(1, gridSize);

        _totalPieces = _cols * _rows;
        _placedPieces = 0;
        _levelCompleted = false;
        _gameCompleted = false;

        if (_winPanel != null)
            _winPanel.SetActive(false);

        ClearOldPieces();

        if (fitBoardToImage)
            FitBoardToSourceImage();

        Canvas.ForceUpdateCanvases();

        BuildPuzzle();

    }

    private void ClearOldPieces()
    {
        if (pieceLayer == null)
            return;

        for (int i = pieceLayer.childCount - 1; i >= 0; i--)
        {
            Destroy(pieceLayer.GetChild(i).gameObject);
        }
    }

    public void OnPiecePlaced()
    {
        if (_levelCompleted || _gameCompleted)
            return;

        _placedPieces++;

        PlaySound(_piecePlacedSound);

        if (_placedPieces >= _totalPieces)
        {
            WinLevel();
        }
    }

    private void WinLevel()
    {
        _levelCompleted = true;

        int nextLevelIndex = _currentLevelIndex + 1;

        if (nextLevelIndex >= _levelImages.Length)
        {
            WinGame();
            return;
        }

        PlaySound(_levelCompletedSound);

        Debug.Log("Level completed. Loading next level...");

        Invoke(nameof(LoadNextLevel), _nextLevelDelay);
    }

    private void WinGame()
    {
        _gameCompleted = true;

        ClearOldPieces();

        if (_winPanel != null)
            _winPanel.SetActive(true);

        if (_winText != null)
            _winText.text = "You win!";

        PlaySound(_gameWinSound);

        Debug.Log("You win!");
    }

    private void PlaySound(AudioClip clip)
    {
        if (_audioSource == null || clip == null)
            return;

        _audioSource.PlayOneShot(clip);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _winPanel.SetActive(!_winPanel.activeSelf);
    }

    private void OnEnable()
    {
        if (_resetGame != null)
            _resetGame.onClick.AddListener(ResetGame);

        if (_exitGame != null)
            _exitGame.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        if (_resetGame != null)
            _resetGame.onClick.RemoveListener(ResetGame);

        if (_exitGame != null)
            _exitGame.onClick.RemoveListener(ExitGame);
    }

    private void ResetGame()
    {
        Time.timeScale = 1f;
        
        LoadLevel(_currentLevelIndex = 0);
    }

    private void ExitGame()
    {
        Time.timeScale = 1f;

        Application.Quit();
    }
}