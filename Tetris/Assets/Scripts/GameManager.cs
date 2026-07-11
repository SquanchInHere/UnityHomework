using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public GridManager gridManager;
    public UIManager uiManager;
    
    public float fallInterval = 1f;
    public float lineSpawnInterval = 10f;
    
    public Sprite blockSprite;

    private Tetromino currentPiece;
    private int score;
    private int totalLinesCleared;

    private float fallTimer;
    private float lineTimer;

    private bool gameOver;
    private bool isHardDropping;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (blockSprite == null)
            blockSprite = GenerateBlockSprite();
    }

    private void Start()
    {
        DrawGridBorder();
        CenterCamera();

        score = 0;
        totalLinesCleared = 0;
        fallTimer = 0f;
        lineTimer = lineSpawnInterval;
        gameOver = false;
        isHardDropping = false;

        uiManager?.UpdateScore(0, 0);
        SpawnPiece();
    }

    private void Update()
    {
        if (gameOver) return;

        HandleInput();

        if (!isHardDropping)
        {
            fallTimer += Time.deltaTime;
            if (fallTimer >= fallInterval)
            {
                fallTimer = 0f;
                MovePieceDown();
            }
        }

        lineTimer -= Time.deltaTime;
        if (lineTimer <= 0f)
        {
            lineTimer = lineSpawnInterval;
            AddGarbageLine();
        }
    }

    private void HandleInput()
    {
        if (currentPiece == null) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentPiece.TryMove(Vector2.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            currentPiece.TryMove(Vector2.right);
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentPiece.TryRotate();
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!currentPiece.TryMove(Vector2.down))
                currentPiece.Land();
            else
                score += 1;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isHardDropping)
            StartHardDrop();

        if (Input.GetKeyDown(KeyCode.R))
            RestartGame();
    }

    private void SpawnPiece()
    {
        if (gameOver) return;

        int index = Random.Range(0, 7);
        Vector2 spawnPos = new Vector2(width / 2f, height - 2f);

        GameObject pieceObj = new GameObject("Tetromino");
        currentPiece = pieceObj.AddComponent<Tetromino>();
        currentPiece.Initialize(index, spawnPos, gridManager, blockSprite);

        if (!currentPiece.CanFitAtPosition(spawnPos))
            GameOver();
    }

    private void MovePieceDown()
    {
        if (currentPiece == null) return;
        if (!currentPiece.TryMove(Vector2.down))
            currentPiece.Land();
    }

    private void StartHardDrop()
    {
        if (currentPiece == null || gameOver) return;
        isHardDropping = true;
        currentPiece.HardDrop();
        score += 2;
        isHardDropping = false;
        if (currentPiece != null)
            currentPiece.Land();
    }

    public void OnPieceLanded()
    {
        currentPiece = null;

        int lines = gridManager.ClearFullRows();
        if (lines > 0)
        {
            totalLinesCleared += lines;
            score += GetLineScore(lines);
        }

        uiManager?.UpdateScore(score, totalLinesCleared);
        SpawnPiece();
    }

    private void AddGarbageLine()
    {
        if (gameOver) return;

        Vector3 nextPosition = (Vector3)currentPiece.transform.position + Vector3.up;

        if (!currentPiece.CanFitAtPosition(nextPosition))
        {
            GameOver();
            return;
        }

        if (currentPiece != null)
            currentPiece.transform.position = nextPosition;

        bool ok = gridManager.SpawnGarbageLine(blockSprite);
        if (!ok)
        {
            GameOver();
            return;
        }

        if (currentPiece != null && !currentPiece.CanFitAtPosition(currentPiece.transform.position))
            GameOver();
    }

    int GetLineScore(int lines)
    {
        switch (lines)
        {
            case 1: return 100;
            case 2: return 300;
            case 3: return 500;
            case 4: return 800;
            default: return 0;
        }
    }

    private void GameOver()
    {
        gameOver = true;
        currentPiece = null;
        uiManager?.ShowGameOver(score, totalLinesCleared);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager
            .LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void DrawGridBorder()
    {
        GameObject borderObj = new GameObject("GridBorder");
        LineRenderer lr = borderObj.AddComponent<LineRenderer>();

        lr.positionCount = 5;
        lr.loop = false;
        lr.useWorldSpace = true;

        float left = -0.5f;
        float right = width - 0.5f;
        float bottom = -0.5f;
        float top = height - 0.5f;

        lr.SetPositions(new Vector3[]
        {
            new Vector3(left, bottom, 0),
            new Vector3(left, top, 0),
            new Vector3(right, top, 0),
            new Vector3(right, bottom, 0),
            new Vector3(left, bottom, 0),
        });

        lr.startWidth = 0.06f;
        lr.endWidth = 0.06f;

        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = Color.white;
        lr.material = mat;
    }

    private void CenterCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        cam.transform.position = new Vector3(
            (width - 1f) / 2f,
            (height - 1f) / 2f,
            -10f
        );
        cam.orthographicSize = height / 2f + 2f;
        cam.orthographic = true;
        cam.backgroundColor = Color.black;
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    Sprite GenerateBlockSprite()
    {
        int size = 16;
        Texture2D tex = new Texture2D(size, size);

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                tex.SetPixel(x, y, Color.white);

        for (int x = 0; x < size; x++)
        {
            tex.SetPixel(x, 0, Color.black);
            tex.SetPixel(x, size - 1, Color.black);
        }
        for (int y = 0; y < size; y++)
        {
            tex.SetPixel(0, y, Color.black);
            tex.SetPixel(size - 1, y, Color.black);
        }

        tex.Apply();
        tex.filterMode = FilterMode.Point;

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    int width => gridManager != null ? gridManager.width : 10;
    int height => gridManager != null ? gridManager.height : 20;

    public bool IsGameOver() => gameOver;
}
