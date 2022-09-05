using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using UnityEngine.EventSystems;

 
public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int position { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public int rotationIndex { get; private set; }
    public Text lol;
    public Text delay;

    private float stepDelay = 0.8f;
    public float moveDelay = 0.1f;
    private float lockDelay = 0.3f;

    private float stepTime;
    private float lockTime;
    private float moveTime;

    public float repeatrateleftright = 0.1f;
    public float repeatratedown = 0.1f;
    private float nextmove = 0.0f;
    public bool falls = true;

    public bool spun = false;

    PhotonView view;



    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.data = data;
        this.position = position;
        stepTime = Time.time + stepDelay;
        moveTime = moveDelay;
        lockTime = 0f;
        Scene Current = SceneManager.GetActiveScene();
        string Name = Current.name;

        if (this.cells==null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }
        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }
    private void Start()
    {
        view = GetComponent<PhotonView>();
    }
    private void Update()
    {

        this.board.Clear(this);

        this.lockTime += Time.deltaTime;

        //nehézségi szint változtatása
          if (board.clearedlines>100)
          {
              stepDelay = 0.5f;
          }
        if (board.clearedlines>200)
        {
            stepDelay = 0.2f;
        }

        //az irányítás
        if (view.IsMine)
        {
            if (CrossPlatformInputManager.GetButtonDown("CCW"))
            {
                Rotate(-1);
            }

            else if (CrossPlatformInputManager.GetButtonDown("CW"))
            {
                Rotate(1);
            }
            else if (CrossPlatformInputManager.GetButtonDown("180"))
            {
                Rotate(1);
                Rotate(1);
            }

            else if (CrossPlatformInputManager.GetButton("Left") && Time.time > nextmove)
            {
                nextmove = Time.time + repeatrateleftright;
                Move(Vector2Int.left);
            }

            else if (CrossPlatformInputManager.GetButton("Right") && Time.time > nextmove)
            {
                nextmove = Time.time + repeatrateleftright;
                Move(Vector2Int.right);
            }
            else if (CrossPlatformInputManager.GetButton("Down") && Time.time > nextmove)
            {
                falls = false;
                nextmove = Time.time + repeatratedown;
                Move(Vector2Int.down);
            }
            else if (CrossPlatformInputManager.GetButtonUp("Down"))
            {
                falls = true;
            }
            else if (CrossPlatformInputManager.GetButtonDown("HardDrop"))
            {
                HardDrop();


            }

            if (Time.time >= this.stepTime)
            {
                Step();
            }
        }
        

        this.board.Set(this);
    }

        //a tetromino lefelé mozgása
    private void Step()
    {
        if (falls)
        {
            this.stepTime = Time.time + this.stepDelay;

            Move(Vector2Int.down);

            if (this.lockTime >= this.lockDelay)
            {
                Lock();
            }
        }
    }
   //harddrop - ilyenkor a tetromino egyből leesik
   private void HardDrop()
   {
       while (Move(Vector2Int.down))
       {
           continue;
       }
   
       Lock();
   }



    private void Lock()
    {
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
    }

    //a jobbra-balra mozgatást megvalósító metódus

    private bool Move(Vector2Int translation)
    {  
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;
        bool valid = this.board.IsValidPosition(this, newPosition);
        if (valid)
        {
            this.position = newPosition;
            this.lockTime = 0f;
        }
        return valid;
    }

    // az ellenőrző és a forgató mátrixok egyesítése
    private void Rotate(int direction) //alakzatok forgatása
    {
        int originalRotation = this.rotationIndex;
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0 , 4);

        ApplyRotationMatrix(direction);
      
        if (!TestWallKicks(this.rotationIndex, direction))
        {
            this.rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
            spun = true;
        }

    }
    

    // a tetrominok forgatása mátrixokkal
    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];
            int x, y;

            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));


                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));

                    break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    // az srs forgatást ellenőrző script
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    //az srs forgatást ellenőrző script
    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
    }


    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        
        else
        {
            return min + (input - min) % (max - min);
        }
    }
}


