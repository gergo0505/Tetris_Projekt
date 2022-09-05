using UnityEngine;
using UnityEngine.Tilemaps;


//tetromino l�trehoz�sa, ennek �s a dat�nak a seg�ts�g�vel �p�lnek fel a j�t�kelemek
public enum Tetromino
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z,
}

[System.Serializable]
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] cells { get; private set; }
    public Vector2Int[,] wallKicks { get; private set; }


    public void Initialize()
    {
        this.cells = Data.Cells[this.tetromino];
        this.wallKicks = Data.WallKicks[this.tetromino];
    }  
}