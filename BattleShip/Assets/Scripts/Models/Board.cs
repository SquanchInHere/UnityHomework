using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Board
{
    public string[,] Field;
    public bool[,] Shots;
    public Text[,] Texts;
    public Image[,] Images;
    public Button[,] Buttons;

    public Board(int rows, int cols)
    {
        Field = new string[rows, cols];
        Shots = new bool[rows, cols];
        Texts = new Text[rows, cols];
        Images = new Image[rows, cols];
        Buttons = new Button[rows, cols];
    }
}
