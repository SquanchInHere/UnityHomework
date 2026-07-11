using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI linesText;
    public TextMeshProUGUI gameOverScoreText;
    
    public GameObject gameOverPanel;
    public Button restartButton;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());
    }

    public void UpdateScore(int score, int linesCleared)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
        if (linesText != null)
            linesText.text = $"Lines: {linesCleared}";
    }

    public void ShowGameOver(int score, int linesCleared)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverScoreText != null)
            gameOverScoreText.text = $"Game Over!\n\nScore: {score}\nLines: {linesCleared}";
    }
}
