using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Editor : MonoBehaviour
{
    public static bool isEditing = false;

    public bool incCamSize = false;
    public bool decCamSize = false;
    public Camera cam;
    public bool loadNew = false;
    public bool endEdit = false;

    public int status;
    public int tile;
    public bool confirmChange;

    public int selectedX=0;
    public int selectedY=0;
    int preSelX=0;
    int preSelY=0;

    public static string pnstatic = "placeholder";
    public string puzzleName;
    public int oxygen;
    public float hp;
    public int minMove;
    public float perfectHp;
    public int sizeX;
    public int sizeY;

    bool puzzleInfoUpdated = false;

    // Start is called before the first frame update
    void Start()
    {
        isEditing = true;
    }

    // Update is called once per frame
    void Update()
    {
        pnstatic = puzzleName;
        try
        {
            if (endEdit)
            {
                Puzzle newPuzzle = new(puzzleName, oxygen, minMove, hp, perfectHp, StartPuzzle.currentPuzzle.grids);
                newPuzzle.SavePuzzle();
                StartPuzzle.puzzleName = puzzleName;
                SceneManager.LoadScene("PuzzleScene");
                pnstatic = "placeholder";
                isEditing = false;
                return;
            }
            if (confirmChange)
            {
                PuzzleGrid grid = StartPuzzle.currentPuzzle.GetGrid(selectedX, selectedY);
                grid.status = status;
                grid.tile = tile;
                grid.isMoveable = status switch
                {
                    Constant.GRID_WALL => false,
                    Constant.GRID_OUTSIDE => false,
                    Constant.GRID_LASER_W => false,
                    Constant.GRID_LASER_A => false,
                    Constant.GRID_LASER_S => false,
                    Constant.GRID_LASER_D => false,
                    _ => true,
                };
                grid.laserExists = false;

                if (grid.status == Constant.GRID_LASER_W || grid.status == Constant.GRID_LASER_A ||
                    grid.status == Constant.GRID_LASER_S || grid.status == Constant.GRID_LASER_D)
                {
                    PuzzleGrid[] newLSG = new PuzzleGrid[StartPuzzle.currentPuzzle.laserStartGrids.Length + 1];
                    int length = StartPuzzle.currentPuzzle.laserStartGrids.Length;
                    for (int i = 0; i < length; i++) newLSG[i] = StartPuzzle.currentPuzzle.laserStartGrids[i];
                    newLSG[length] = grid;

                    StartPuzzle.currentPuzzle.laserStartGrids = newLSG;
                }
                confirmChange = false;
            }
            if (preSelX != selectedX || preSelY != selectedY)
            {
                preSelX = selectedX;
                preSelY = selectedY;
                PuzzleGrid grid = StartPuzzle.currentPuzzle.GetGrid(selectedX, selectedY);
                status = grid.status;
                tile = grid.tile;
            }
            if(incCamSize)
            {
                cam.orthographicSize += 1;
                incCamSize = false;
            }
            if(decCamSize)
            {
                cam.orthographicSize -= 1;
                decCamSize = false;
            }
            if(!puzzleInfoUpdated)
            {
                puzzleInfoUpdated = true;
                puzzleName = StartPuzzle.currentPuzzle.puzzleName;
                oxygen = StartPuzzle.currentPuzzle.oxygen;
                hp = StartPuzzle.currentPuzzle.hp;
                minMove = StartPuzzle.currentPuzzle.minMove;
                perfectHp = StartPuzzle.currentPuzzle.perfectHp;
            }
        } catch (NullReferenceException) { }
    }
}
