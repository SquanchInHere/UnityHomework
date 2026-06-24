using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [Header("UI Settings")]
    [SerializeField] private Font defaultFont;
    [SerializeField] private int keyboardButtonSize = 55;
    [SerializeField] private int keyboardSpacing = 8;
    [SerializeField] private int wordInputSize = 60;
    [SerializeField] private float wordSpacing = 15f;

    private Canvas canvas;
    private TextMeshProUGUI wordText;
    private TextMeshProUGUI mistakeText;
    private TMP_InputField fullWordInput;
    private TextMeshProUGUI resultText;
    private GameObject resultPanel;

    private readonly List<Button> letterButtons = new();

    private readonly char[] letters =
    {
        'A','B','C','D','E','F','G','H','I',
        'J','K','L','M','N','O','P','Q','R',
        'S','T','U','V','W','X','Y','Z'
    };

    private void Awake()
    {
        BuildUI();
    }

    public void SetWordText(string text)
    {
        wordText.text = text;
    }

    public void SetMistakeText(int mistakes, int maxMistakes)
    {
        mistakeText.text = $"Mistakes: {mistakes} / {maxMistakes}";
    }

    public void ShowResult(string message)
    {
        resultText.text = message;
        resultPanel.SetActive(true);
    }

    public void HideResult()
    {
        resultPanel.SetActive(false);
    }

    public void ClearFullWordInput()
    {
        fullWordInput.text = "";
    }

    public string GetFullWordInput()
    {
        return fullWordInput.text;
    }

    public void DisableLetter(char letter)
    {
        foreach (Button button in letterButtons)
        {
            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

            if (text != null && text.text == letter.ToString())
            {
                button.interactable = false;
                break;
            }
        }
    }

    public void ResetKeyboard()
    {
        foreach (Button button in letterButtons)
        {
            button.interactable = true;
        }
    }

    private void BuildUI()
    {
        CreateCanvas();

        CreateMistakeText();
        CreateWordText();
        CreateKeyboardPanel();
        CreateFullWordInputPanel();
        CreateResultPanel();
    }

    private void CreateCanvas()
    {
        GameObject canvasObject = new GameObject("Canvas");

        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
    }

    private void CreateMistakeText()
    {
        GameObject obj = CreateTextObject("MistakeText", canvas.transform);

        mistakeText = obj.GetComponent<TextMeshProUGUI>();
        mistakeText.text = "Mistakes: 0 / 6";
        mistakeText.fontSize = 34;
        mistakeText.alignment = TextAlignmentOptions.Center;

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -25f);
        rect.sizeDelta = new Vector2(500f, 60f);
    }

    private void CreateWordText()
    {
        GameObject obj = CreateTextObject("WordText", canvas.transform);

        wordText = obj.GetComponent<TextMeshProUGUI>();
        wordText.text = "_ _ _ _ _";
        wordText.fontSize = wordInputSize;
        wordText.alignment = TextAlignmentOptions.Center;
        wordText.characterSpacing = wordSpacing;

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.8f);
        rect.anchorMax = new Vector2(0.5f, 0.8f);
        rect.pivot = new Vector2(0.5f, 0.8f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(1200f, 100f);
    }

    private void CreateKeyboardPanel()
    {
        GameObject panel = new GameObject("KeyboardPanel");
        panel.transform.SetParent(canvas.transform, false);

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.35f);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 120f);
        rect.sizeDelta = new Vector2(780f, 260f);

        GridLayoutGroup grid = panel.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(keyboardButtonSize, keyboardButtonSize);
        grid.spacing = new Vector2(keyboardSpacing, keyboardSpacing);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 9;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.padding = new RectOffset(20, 20, 20, 20);

        foreach (char letter in letters)
        {
            Button button = CreateButton(letter.ToString(), panel.transform);

            char cachedLetter = letter;
            button.onClick.AddListener(() =>
            {
                if (gameManager != null)
                {
                    gameManager.CheckLetter(cachedLetter);
                }
            });

            letterButtons.Add(button);
        }
    }

    private void CreateFullWordInputPanel()
    {
        GameObject inputObj = new GameObject("FullWordInput");
        inputObj.transform.SetParent(canvas.transform, false);

        Image inputBg = inputObj.AddComponent<Image>();
        inputBg.color = Color.white;

        fullWordInput = inputObj.AddComponent<TMP_InputField>();

        RectTransform inputRect = inputObj.GetComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(0.5f, 0f);
        inputRect.anchorMax = new Vector2(0.5f, 0f);
        inputRect.pivot = new Vector2(0.5f, 0f);
        inputRect.anchoredPosition = new Vector2(-110f, 45f);
        inputRect.sizeDelta = new Vector2(380f, 55f);

        GameObject textObj = CreateTextObject("Text", inputObj.transform);
        TextMeshProUGUI inputText = textObj.GetComponent<TextMeshProUGUI>();
        inputText.text = "";
        inputText.fontSize = 28;
        inputText.color = Color.black;
        inputText.alignment = TextAlignmentOptions.MidlineLeft;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(15f, 0f);
        textRect.offsetMax = new Vector2(-15f, 0f);

        GameObject placeholderObj = CreateTextObject("Placeholder", inputObj.transform);
        TextMeshProUGUI placeholderText = placeholderObj.GetComponent<TextMeshProUGUI>();
        placeholderText.text = "Enter full word...";
        placeholderText.fontSize = 24;
        placeholderText.color = new Color(0f, 0f, 0f, 0.45f);
        placeholderText.alignment = TextAlignmentOptions.MidlineLeft;

        RectTransform placeholderRect = placeholderObj.GetComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = new Vector2(15f, 0f);
        placeholderRect.offsetMax = new Vector2(-15f, 0f);

        fullWordInput.textComponent = inputText;
        fullWordInput.placeholder = placeholderText;

        Button guessButton = CreateButton("CHECK", canvas.transform);

        RectTransform buttonRect = guessButton.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0f);
        buttonRect.anchorMax = new Vector2(0.5f, 0f);
        buttonRect.pivot = new Vector2(0.5f, 0f);
        buttonRect.anchoredPosition = new Vector2(225f, 45f);
        buttonRect.sizeDelta = new Vector2(160f, 55f);

        guessButton.onClick.AddListener(() =>
        {
            if (gameManager != null)
            {
                gameManager.CheckFullWord(fullWordInput.text);
            }
        });
    }

    private void CreateResultPanel()
    {
        resultPanel = new GameObject("ResultPanel");
        resultPanel.transform.SetParent(canvas.transform, false);

        Image image = resultPanel.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.8f);

        RectTransform rect = resultPanel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(600f, 300f);

        GameObject resultTextObj = CreateTextObject("ResultText", resultPanel.transform);
        resultText = resultTextObj.GetComponent<TextMeshProUGUI>();
        resultText.text = "YOU WIN";
        resultText.fontSize = 48;
        resultText.alignment = TextAlignmentOptions.Center;

        RectTransform resultTextRect = resultTextObj.GetComponent<RectTransform>();
        resultTextRect.anchorMin = new Vector2(0.5f, 0.65f);
        resultTextRect.anchorMax = new Vector2(0.5f, 0.65f);
        resultTextRect.pivot = new Vector2(0.5f, 0.5f);
        resultTextRect.anchoredPosition = Vector2.zero;
        resultTextRect.sizeDelta = new Vector2(550f, 100f);

        Button restartButton = CreateButton("RESTART", resultPanel.transform);

        RectTransform restartRect = restartButton.GetComponent<RectTransform>();
        restartRect.anchorMin = new Vector2(0.5f, 0.25f);
        restartRect.anchorMax = new Vector2(0.5f, 0.25f);
        restartRect.pivot = new Vector2(0.5f, 0.5f);
        restartRect.anchoredPosition = Vector2.zero;
        restartRect.sizeDelta = new Vector2(220f, 65f);

        restartButton.onClick.AddListener(() =>
        {
            if (gameManager != null)
            {
                gameManager.RestartGame();
            }
        });

        resultPanel.SetActive(false);
    }

    private Button CreateButton(string label, Transform parent)
    {
        GameObject obj = new GameObject("Button_" + label);
        obj.transform.SetParent(parent, false);

        Image image = obj.AddComponent<Image>();
        image.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        Button button = obj.AddComponent<Button>();

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.highlightedColor = Color.white;
        colors.pressedColor = new Color(0.65f, 0.65f, 0.65f, 1f);
        colors.disabledColor = new Color(0.35f, 0.35f, 0.35f, 0.6f);
        button.colors = colors;

        GameObject textObj = CreateTextObject("Text", obj.transform);
        TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 26;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return button;
    }

    private GameObject CreateTextObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.color = Color.white;
        text.raycastTarget = false;

        return obj;
    }
}
