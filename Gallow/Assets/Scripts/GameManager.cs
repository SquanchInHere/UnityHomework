using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UIBuilder ui;

    [Header("Game")]
    [SerializeField] private int maxMistakes = 6;

    [Header("3D Body Parts")]
    [SerializeField] private GameObject[] bodyParts;

    private readonly string[] words =
    {
        "UNITY",
        "GAME",
        "PLAYER",
        "SCRIPT",
        "GALLOWS",
        "HANGMAN"
    };

    private string currentWord;
    private HashSet<char> guessedLetters = new();
    private int mistakes;
    private bool gameOver;

    private void Start()
    {
        RestartGame();
    }

    private void Update()
    {
        if (gameOver)
            return;

        foreach (char c in Input.inputString)
        {
            if (char.IsLetter(c))
            {
                CheckLetter(char.ToUpper(c));
            }
        }
    }

    public void RestartGame()
    {
        currentWord = words[Random.Range(0, words.Length)];

        guessedLetters.Clear();
        mistakes = 0;
        gameOver = false;

        if (bodyParts != null)
        {
            foreach (GameObject part in bodyParts)
            {
                if (part != null)
                    part.SetActive(false);
            }
        }

        ui.HideResult();
        ui.ResetKeyboard();
        ui.ClearFullWordInput();

        UpdateUI();
    }

    public void CheckLetter(char letter)
    {
        if (gameOver)
            return;

        letter = char.ToUpper(letter);

        if (guessedLetters.Contains(letter))
            return;

        guessedLetters.Add(letter);
        ui.DisableLetter(letter);

        if (!currentWord.Contains(letter))
        {
            AddMistake();
        }

        UpdateUI();
        CheckGameEnd();
    }

    public void CheckFullWord(string input)
    {
        if (gameOver)
            return;

        if (string.IsNullOrWhiteSpace(input))
            return;

        string guess = input.Trim().ToUpper();

        if (guess == currentWord)
        {
            WinGame();
        }
        else
        {
            AddMistake();
            ui.ClearFullWordInput();
            UpdateUI();
            CheckGameEnd();
        }
    }

    private void AddMistake()
    {
        mistakes++;

        int index = mistakes - 1;

        if (bodyParts != null && index >= 0 && index < bodyParts.Length)
        {
            if (bodyParts[index] != null)
                bodyParts[index].SetActive(true);
        }
    }

    private void UpdateUI()
    {
        ui.SetWordText(GetHiddenWord());
        ui.SetMistakeText(mistakes, maxMistakes);
    }

    private string GetHiddenWord()
    {
        StringBuilder builder = new StringBuilder();

        foreach (char letter in currentWord)
        {
            if (guessedLetters.Contains(letter))
                builder.Append(letter);
            else
                builder.Append("_");

            builder.Append(" ");
        }

        return builder.ToString();
    }

    private void CheckGameEnd()
    {
        if (mistakes >= maxMistakes)
        {
            LoseGame();
            return;
        }

        foreach (char letter in currentWord)
        {
            if (!guessedLetters.Contains(letter))
                return;
        }

        WinGame();
    }

    private void WinGame()
    {
        gameOver = true;
        ui.ShowResult("YOU WIN");
    }

    private void LoseGame()
    {
        gameOver = true;
        ui.ShowResult("YOU LOSE\nWORD: " + currentWord);
    }

    public void SetBodyParts(GameObject[] parts)
    {
        bodyParts = parts;

        if (bodyParts == null)
            return;

        foreach (GameObject part in bodyParts)
        {
            if (part != null)
                part.SetActive(false);
        }
    }
}
