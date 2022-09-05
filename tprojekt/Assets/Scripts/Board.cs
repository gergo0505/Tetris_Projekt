using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviourPun
{
    //public static Board instance;
    public Tilemap tilemap { get; private set; }
    public TetrominoData[] tetrominoes;
    public Piece activePiece { get; private set; }
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public List<TetrominoData> activebag = new List<TetrominoData>();
    public List<TetrominoData> fillerbag = new List<TetrominoData>();
    public List<TetrominoData> holdbag = new List<TetrominoData>();
    public Text cleared;
    public Text scoretxt;
    public Text next;
    public Text hold;
    public Text GameOverScore;
    public Text level;
    public GameObject gameoverui;

    public bool canhold = true;

    public int single = 40;
    public int dbl = 100;
    public int triple = 300;
    public int quad = 1200;

    public int lines;
    public int clearedlines;
    public int score = 0;
    
    private PhotonView view;
    //pontok számolásáért felelős script
    public void addscore()
    {
        
        if (lines == 1)
        {
            score += single;
        }
        else if (lines == 2)
        {
            score += dbl;
        }
        else if (lines == 3)
        {
            score += triple;
        }
        else if (lines == 4)
        {
            score += quad;
        }
        lines = 0;
        scoretxt.text = score.ToString();
    }
    //eltűntetett sorok kiírása
    public void linescleared()
    {
        cleared.text = clearedlines.ToString();
    }

    public RectInt bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }

    private void Start()
    {
        string Scene = SceneManager.GetActiveScene().name.ToString();
        if (Scene == "MP")
        {
            this.view = this.GetComponent<PhotonView>();
            if (this.view.IsMine)
            {
                this.GetComponent<Piece>().enabled = true;
                this.transform.GetChild(1).GetComponent<Ghost>().enabled = true;
            }
            else
            {
                this.GetComponent<Piece>().enabled = false;
                this.transform.GetChild(1).GetComponent<Ghost>().enabled = false;
            }
            this.tilemap = this.GetComponentInChildren<Tilemap>();
            this.activePiece = this.GetComponentInChildren<Piece>();
            for (int index = 0; index < this.tetrominoes.Length; ++index)
                this.tetrominoes[index].Initialize();
        }

        else if (Scene == "Tetris")
        {
            // instance = this;
        }
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();
        for (int i = 0; i < this.tetrominoes.Length; i++)
        {
            this.tetrominoes[i].Initialize();
        }

        //adatok kiírása
        cleared.text = clearedlines.ToString();
        scoretxt.text = score.ToString();
        level.text = "easy";
        SpawnPiece();

    }
    public void Update()
    {
              //szint változtatása
        if (clearedlines > 100)
       {
           level.text = "medium";
       }
        if (clearedlines>200)
        {
            level.text = "hard";
        }
        //hold button funkciója
        if (CrossPlatformInputManager.GetButtonDown("Hold"))
        {
            Debug.Log("pressed");
            HoldPiece();
        }
        //reset gomb
        if (CrossPlatformInputManager.GetButtonDown("Reset"))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Tetris");

        }
    }
    //tetromino félrerakásának funkciója
    public void HoldPiece()
    {
        TetrominoData data = activebag[0];
        if (holdbag.Count == 0)
        {
            holdbag.Add(this.activePiece.data);
            Clear(activePiece);
            this.activePiece.Initialize(this, this.spawnPosition, data);
            activebag.RemoveAt(0);
            Set(this.activePiece);
            next.text = activebag[0].tetromino.ToString();
            canhold = false;
            Debug.Log(this.activePiece.data.tetromino.ToString());

        }
        else if (holdbag.Count != 0 && canhold == true)
        {
            TetrominoData hdata = holdbag[0];
            holdbag.RemoveAt(0);
            holdbag.Add(this.activePiece.data);
            Clear(activePiece);
            this.activePiece.Initialize(this, this.spawnPosition, hdata);
            Set(this.activePiece);
            next.text = activebag[0].tetromino.ToString();
            canhold = false;
            Debug.Log(this.activePiece.data.tetromino.ToString());
            
        }

        hold.text = holdbag[0].tetromino.ToString();
    }

    //tetromino lehívása, sor feltöltése
    public void SpawnPiece()
    {


        if (fillerbag.Count == 0)
        {
            fillerbag.AddRange(tetrominoes);
        }
       if (activebag.Count == 0)
        {
            for (int i = 0; i < 6; i++)
            {
               int index = Random.Range(0, fillerbag.Count);
              activebag.Add(fillerbag[index]);
              fillerbag.RemoveAt(index);
            }
        }
     
        TetrominoData data = activebag[0]; 
        activebag.RemoveAt(0);
        int index2 = Random.Range(0, fillerbag.Count);
        activebag.Add(fillerbag[index2]);
        fillerbag.RemoveAt(index2);

        next.text = activebag[0].tetromino.ToString();
       
        this.activePiece.Initialize(this, this.spawnPosition, data);
        Debug.Log(this.activePiece.data.tetromino.ToString());
 
        if (IsValidPosition(this.activePiece, this.spawnPosition))
        {
            Set(this.activePiece);
        }
        else
        {
            GameOver();
        }
        Set(this.activePiece);
        canhold = true;
        addscore();
    }

    //a játékból való kiesés után valósul meg
    private void GameOver()
    {
        Time.timeScale = 0f;
        this.tilemap.ClearAllTiles();
        gameoverui.SetActive(true);
        GameOverScore.text = score.ToString();
        this.tilemap.ClearAllTiles();
        clearedlines = 0;
        score = 0;
        holdbag.Clear();
        activebag.Clear();
        fillerbag.Clear();
        Clear(activePiece);

    }

    //tetromino megjelenítése a pályán
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    //tetromino eltüntetése
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }


    public bool IsValidPosition(Piece piece, Vector3Int position) // ellernőrzi, hogy üres helyre kerül-e a kocka
    {
        RectInt bounds = this.bounds;
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }
        return true;
    }


    public void ClearLines() //sorok eltűntetése
    {
        RectInt bounds = this.bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                clearedlines++;
                LineClear(row);
                linescleared();
            }
            else
            {
                row++;
            }
        }     
    }

    //ellenőrzi, hogy teli van-e egy sor
    private bool IsLineFull(int row)
    {
        RectInt bounds = this.bounds;
        
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);

                if (!this.tilemap.HasTile(position))
                {
                    return false;
                }
            }
            row++;
            lines++;
            
        
            
        return true;
    }
    //egy sor eltüntetése 
    private void LineClear(int row)
    {
        RectInt bounds = this.bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(position, above);
            } 
            row++;
        }
    }

}