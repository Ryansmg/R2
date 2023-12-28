using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player currentPlayer;
    public int x;
    public int y;
    public int moveDirection = DIR_NONE;
    public int oxygen;
    public float hp;

    float preHp;

    bool goBackAtNextUpdate;
    bool logErrorNullRefUpdate0 = true;

    public Puzzle puzzle;

    public const int DIR_NONE = -1;
    public const int DIR_W = 0;
    public const int DIR_A = 1;
    public const int DIR_S = 2;
    public const int DIR_D = 3;

    Vector3 velocity = Vector3.zero;
    public float defaultSmoothness;
    public float conveyorSmoothness;
    float smoothness;
    public float laserDamageTimer = 0f;
    public int currentLaserStartX;
    public int currentLaserStartY;

    public int currentGlassPieceX;
    public int currentGlassPieceY;
    public bool wasGlassPiece = false;

    public float conveyorWaitTimerDefault;
    float conveyorWaitTimer = 0f;

    public bool isGoingToNextPuzzle = false;

    // Debug Variables
    public bool allowFreeMovement = false;
    void Start()
    {
        currentPlayer = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartPuzzle.puzzleLoaded) return;

        preHp = hp;

        bool allowInput = true;
        if (goBackAtNextUpdate)
        {
            GoBack();
            allowInput = false;
            goBackAtNextUpdate = false;
        }
        PuzzleGrid currentGrid;

        //move to current position grid.
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(x, y, 1), ref velocity, smoothness);

        //editor mode
        if(Editor.isEditing)
        {
            Editor editor = GameObject.Find("Scripts").GetComponent<Editor>();
            if (editor.endEdit) return;

            if (Input.GetKeyDown(KeyCode.UpArrow)) { y++; moveDirection = DIR_W; }
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { x--; moveDirection = DIR_A; }
            if (Input.GetKeyDown(KeyCode.DownArrow)) { y--; moveDirection = DIR_S; }
            if (Input.GetKeyDown(KeyCode.RightArrow)) { x++; moveDirection = DIR_D; }
            if (!Editor.selectedOther)
            {
                if (Input.GetKeyDown(KeyCode.W)) { y++; moveDirection = DIR_W; }
                if (Input.GetKeyDown(KeyCode.A)) { x--; moveDirection = DIR_A; }
                if (Input.GetKeyDown(KeyCode.S)) { y--; moveDirection = DIR_S; }
                if (Input.GetKeyDown(KeyCode.D)) { x++; moveDirection = DIR_D; }
                if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    if (Input.GetKeyDown(KeyCode.W)) { y++; moveDirection = DIR_W; }
                    if (Input.GetKeyDown(KeyCode.A)) { x--; moveDirection = DIR_A; }
                    if (Input.GetKeyDown(KeyCode.S)) { y--; moveDirection = DIR_S; }
                    if (Input.GetKeyDown(KeyCode.D)) { x++; moveDirection = DIR_D; }
                }
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    if (Input.GetKeyDown(KeyCode.W)) { y++; moveDirection = DIR_W; }
                    if (Input.GetKeyDown(KeyCode.A)) { x--; moveDirection = DIR_A; }
                    if (Input.GetKeyDown(KeyCode.S)) { y--; moveDirection = DIR_S; }
                    if (Input.GetKeyDown(KeyCode.D)) { x++; moveDirection = DIR_D; }
                }
            }

            //GenerateLasers();

            editor.selectedX = x; editor.selectedY = y;
            if(puzzle.GetGrid(x,y) == Puzzle.nonExistentGrid)
            {
                PuzzleGrid newGrid = new(x, y, Constant.GRID_OUTSIDE, 9);
                puzzle.grids.Add(newGrid);
                puzzle.gridsDict.Add($"{x},{y}", puzzle.grids.Count - 1);

                GameObject puzzleGridPrefab = GameObject.Find("Scripts").GetComponent<StartPuzzle>().puzzleGridPrefab;

                GameObject gridObj = Instantiate(puzzleGridPrefab, GameObject.Find("Grids").transform);
                gridObj.GetComponent<GridManager>().grid = newGrid;
                gridObj.GetComponent<GridManager>().puzzle = puzzle;
                //new PObj(newGrid.x, newGrid.y, newGrid.status).Generate();
            }

            return;
        }

        //nextpuzzle
        currentGrid = puzzle.GetGrid(x, y);
        if (isGoingToNextPuzzle) return;
        if (currentGrid.status == Constant.GRID_NEXTPUZZLE_UP || currentGrid.status == Constant.GRID_NEXTPUZZLE_DOWN)
        {
            if (StartPuzzle.puzzleDictionary.TryGetValue(currentGrid.nextPuzzleName, out Puzzle vp))
            {
                if (CheckMoveable(x, y, vp))
                {
                    GameObject.Find("Scripts").GetComponent<ChangeScene>().ChangeToPuzzle(currentGrid.nextPuzzleName, false);
                    isGoingToNextPuzzle = true;
                    allowInput = false;
                }
                else Debug.Log("다른 퍼즐의 해당 그리드가 막혀 있어 이동할 수 없습니다!");
            }
            else
            {
                if (CheckMoveable(x, y, StartPuzzle.GetPuzzle(currentGrid.nextPuzzleName))) {
                    GameObject.Find("Scripts").GetComponent<ChangeScene>().ChangeToPuzzle(currentGrid.nextPuzzleName, false);
                    isGoingToNextPuzzle = true;
                    allowInput = false;
                }
                else Debug.Log("다른 퍼즐의 해당 그리드가 막혀 있어 이동할 수 없습니다!");
            }
        }

        try
        {
            GenerateLasers();

            //glass piece damage
            if(currentGrid.status == Constant.GRID_GLASS_PIECE)
            {
                if(currentGlassPieceX != x || currentGlassPieceY != y || !wasGlassPiece)
                {
                    currentGlassPieceX = x;
                    currentGlassPieceY = y;
                    hp -= 1;
                    wasGlassPiece=true;
                }
            } else
            {
                wasGlassPiece = false;
            }

            //laser damage
            if (laserDamageTimer > 0f)
            {
                laserDamageTimer -= Time.deltaTime;
                if (!currentGrid.laserExists) laserDamageTimer = 0f;
                if ((currentGrid.laserStartX != currentLaserStartX
                    || currentGrid.laserStartY != currentLaserStartY) && currentGrid.laserExists)
                    laserDamageTimer = 0f;
            }
            else
            {
                bool hasObject = puzzle.objects.TryGetValue(new(x,y), out PObj obj);
                bool checkObj = true;
                if (hasObject) checkObj = obj.isPlayerMoveable;
                if (currentGrid.laserExists && checkObj)
                {
                    currentLaserStartX = currentGrid.laserStartX;
                    currentLaserStartY = currentGrid.laserStartY;
                    laserDamageTimer = 0.5f;
                    hp -= 0.5f;
                }
            }

            if (hp < 0) hp = 0;
            if(hp == 0) { Debug.Log("Dead"); allowInput = false; }
            else if (currentGrid.status == Constant.GRID_END) { Debug.Log("win"); allowInput = false; }
            else if (oxygen == 0)
            {
                if (goBackAtNextUpdate) { allowInput = false; }
                else
                {
                    Debug.Log("Dead");
                    allowInput = false;
                }
            }
        }
        catch (NullReferenceException)
        {
            if (logErrorNullRefUpdate0)
            {
                Debug.LogError("Waiting for restart...");
                logErrorNullRefUpdate0 = false;
            }
        }

        if (hp < preHp) { UIManager.wasDamaged = true; }

        //conveyor movement
        currentGrid = puzzle.GetGrid(x, y);
        if (IsGridConveyor(currentGrid) && conveyorWaitTimer > 0)
        {
            conveyorWaitTimer -= Time.deltaTime;
            return;
        }
        else if (!IsGridConveyor(currentGrid))
        {
            conveyorWaitTimer = conveyorWaitTimerDefault;
            smoothness = defaultSmoothness;
        }

        if (currentGrid.status == Constant.GRID_CONVEYOR_W)
        {
            if (CheckMoveable(x, y + 1))
            {
                smoothness = conveyorSmoothness;
                y++;
                moveDirection = DIR_W;
                conveyorWaitTimer = conveyorWaitTimerDefault;
                return;
            }
        }
        else if (currentGrid.status == Constant.GRID_CONVEYOR_A)
        {
            if (CheckMoveable(x - 1, y))
            {
                smoothness = conveyorSmoothness;
                x--;
                moveDirection = DIR_A;
                conveyorWaitTimer = conveyorWaitTimerDefault;
                return;
            }
        }
        else if (currentGrid.status == Constant.GRID_CONVEYOR_S)
        {
            if (CheckMoveable(x, y - 1))
            {
                smoothness = conveyorSmoothness;
                y--;
                moveDirection = DIR_S;
                conveyorWaitTimer = conveyorWaitTimerDefault;
                return;
            }
        }
        else if (currentGrid.status == Constant.GRID_CONVEYOR_D)
        {
            if (CheckMoveable(x + 1, y))
            {
                smoothness = conveyorSmoothness;
                x++;
                moveDirection = DIR_D;
                conveyorWaitTimer = conveyorWaitTimerDefault;
                return;
            }
        }

        if (!allowInput) { return; }

        //detect inputs and move player
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        { y++; moveDirection = DIR_W; oxygen--; }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) 
        { x--; moveDirection = DIR_A; oxygen--; }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) 
        { y--; moveDirection = DIR_S; oxygen--; }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) 
        { x++; moveDirection = DIR_D; oxygen--; }

        try
        {
            int dx = moveDirection switch
            {
                DIR_A => -1,
                DIR_D => 1,
                _ => 0
            };
            int dy = moveDirection switch
            {
                DIR_W => 1,
                DIR_S => -1,
                _ => 0
            };
            if (puzzle.objects.TryGetValue(new(x, y), out PObj pObj))
            {
                if (!pObj.isPlayerMoveable)
                {
                    SetPosToBack(0.4f);
                    //goBack = true;
                    pObj.PushTo(x + dx, y + dy);
                    GoBack();
                }
            }

            else if (!puzzle.GetGrid(x, y).isMoveable && !allowFreeMovement) {
                SetPosToBack(0.4f);
                GoBack();
                //goBack = true;
            }

            GenerateLasers();

        }
        catch (NullReferenceException)
        {
            if (logErrorNullRefUpdate0)
            {
                Debug.LogError("Waiting for restart...");
                logErrorNullRefUpdate0 = false;
            }
        }
    }

    public static bool IsGridConveyor(PuzzleGrid grid)
    {
        return grid.status == Constant.GRID_CONVEYOR_W || grid.status == Constant.GRID_CONVEYOR_A
            || grid.status == Constant.GRID_CONVEYOR_S || grid.status == Constant.GRID_CONVEYOR_D;
    }

    public static bool CheckPlayer(PuzzleGrid grid)
    {
        return currentPlayer.x == grid.x && currentPlayer.y == grid.y;
    }

    public bool CheckMoveable(int nx, int ny)
    {
        bool isMoveable = true;
        if (!puzzle.GetGrid(nx, ny).isMoveable) isMoveable = false;
        if (puzzle.objects.TryGetValue(new(nx, ny), out PObj pObj))
            if (!pObj.isPlayerMoveable) isMoveable = false;
        return isMoveable;
    }
    public static bool CheckMoveable(int nx, int ny, Puzzle puzzle_)
    {
        bool isMoveable = true;
        if (!puzzle_.GetGrid(nx, ny).isMoveable) isMoveable = false;
        if (puzzle_.objects.TryGetValue(new(nx, ny), out PObj pObj))
            if (!pObj.isPlayerMoveable) isMoveable = false;
        return isMoveable;
    }
    public static bool CheckMoveable_s(int nx, int ny)
    {
        bool isMoveable = true;
        if (!StartPuzzle.currentPuzzle.GetGrid(nx, ny).isMoveable) isMoveable = false;
        if (StartPuzzle.currentPuzzle.objects.TryGetValue(new(nx, ny), out PObj pObj))
            if (!pObj.isPlayerMoveable) isMoveable = false;
        return isMoveable;
    }
    public void GoBack()
    {
        switch (moveDirection)
        {
            case DIR_W: y--; break;
            case DIR_A: x++; break;
            case DIR_S: y++; break;
            case DIR_D: x--; break;
            default: break;
        }
    }

    public void SetPosToBack(float f)
    {
        switch(moveDirection)
        {
            case DIR_W: transform.position = new Vector3(x, y - f, 1); break;
            case DIR_A: transform.position = new Vector3(x + f, y, 1); break;
            case DIR_S: transform.position = new Vector3(x, y + f, 1); break;
            case DIR_D: transform.position = new Vector3(x - f, y, 1); break;
            default: break;
        }
    }

    public void GenerateLasers()
    {
        foreach (KeyValuePair<KeyValuePair<int, int>, PObj> pair in currentPlayer.puzzle.objects)
            pair.Value.isOnLaser = false;
        int currentX, currentY;
        foreach (PuzzleGrid grid in puzzle.grids)
        {
            grid.laserExists = false;
            grid.xLaserExists = false;
            grid.yLaserExists = false;
        }
        foreach (PuzzleGrid grid in puzzle.laserStartGrids)
        {
            PObj currentPObject;
            currentX = grid.x; currentY = grid.y;
            int dir = grid.status;
            changeDir:
            switch (dir)
            {
                case Constant.GRID_LASER_W:
                    puzzle.GetGrid(currentX, currentY).laserExists = true;
                    puzzle.GetGrid(currentX, currentY).yLaserExists = true;
                    puzzle.GetGrid(currentX, currentY).laserStartX = grid.x;
                    puzzle.GetGrid(currentX, currentY).laserStartY = grid.y;
                    currentY++;
                    while (puzzle.GetGrid(currentX, currentY).isMoveable)
                    {
                        if (puzzle.objects.TryGetValue(new(currentX, currentY), out currentPObject))
                        {
                            if (currentPObject.isPlayerMoveable)
                            {
                                puzzle.GetGrid(currentX, currentY).laserExists = true;
                                puzzle.GetGrid(currentX, currentY).yLaserExists = true;
                                puzzle.GetGrid(currentX, currentY).laserStartX = grid.x;
                                puzzle.GetGrid(currentX, currentY).laserStartY = grid.y;
                                currentY++;
                            }
                            else
                            {
                                currentPObject.laserDir = PObj.ld_w;
                                if (currentPObject.type == Constant.GRID_MIRROR_C)
                                {
                                    dir = Constant.GRID_LASER_D;
                                    currentPObject.isOnLaser = true;
                                    goto changeDir;
                                }
                                else if (currentPObject.type == Constant.GRID_MIRROR_Z)
                                {
                                    dir = Constant.GRID_LASER_A;
                                    currentPObject.isOnLaser = true;
                                    goto changeDir;
                                } else if(!currentPObject.isMirror)
                                    currentPObject.isOnLaser = true;
                                break;
                            }
                        }
                        else
                        {
                            puzzle.GetGrid(currentX, currentY).laserExists = true;
                            puzzle.GetGrid(currentX, currentY).yLaserExists = true;
                            puzzle.GetGrid(currentX, currentY).laserStartX = grid.x;
                            puzzle.GetGrid(currentX, currentY).laserStartY = grid.y;
                            currentY++;
                        }
                    }
                    break;
                case Constant.GRID_LASER_A:
                    puzzle.GetGrid(currentX, currentY).laserExists = true;
                    puzzle.GetGrid(currentX, currentY).xLaserExists = true;
                    puzzle.GetGrid(currentX, currentY).laserStartX = grid.x;
                    puzzle.GetGrid(currentX, currentY).laserStartY = grid.y;
                    currentX--;
                    while (puzzle.GetGrid(currentX, currentY).isMoveable)
                    {
                        if (puzzle.objects.TryGetValue(new(currentX, currentY), out currentPObject))
                        {
                            if (currentPObject.isPlayerMoveable)
                            {
                                puzzle.GetGrid(currentX, currentY).laserExists = true;
                                puzzle.GetGrid(currentX, currentY).xLaserExists = true;
                                puzzle.GetGrid(currentX, currentY).laserStartX = grid.x;
                                puzzle.GetGrid(currentX, currentY).laserStartY = grid.y;
                                currentX--;
                            }
                            else
                            {
                                currentPObject.laserDir = PObj.ld_a;
                                if (currentPObject.type == Constant.GRID_MIRROR_E)
                                {
                                    dir = Constant.GRID_LASER_W;
                                    currentPObject.isOnLaser = true;
                                    goto changeDir;
                                } else if (currentPObject.type == Constant.GRID_MIRROR_C)
                                {
                                    dir = Constant.GRID_LASER_S;
                                    currentPObject.isOnLaser = true;
                                    goto changeDir;
                                } else if (!currentPObject.isMirror)
                                currentPObject.isOnLaser = true;
                            break;
                            }
                        }
                        else
                        {
                            puzzle.GetGrid(currentX, currentY).laserExists = true;
                            puzzle.GetGrid(currentX, currentY).xLaserExists = true;
                            puzzle.GetGrid(currentX, currentY).laserStartX = grid.x;
                            puzzle.GetGrid(currentX, currentY).laserStartY = grid.y;
                            currentX--;
                        }
                    }
                    break;
                case Constant.GRID_LASER_S:
                    puzzle.GetGrid(currentX, currentY).laserExists = true;
                    puzzle.GetGrid(currentX, currentY).yLaserExists = true;
                    puzzle.GetGrid(currentX, currentY).laserStartX = grid.x;
                    puzzle.GetGrid(currentX, currentY).laserStartY = grid.y;
                    currentY--;
                    while (puzzle.GetGrid(currentX, currentY).isMoveable)
                    {
                        if (puzzle.objects.TryGetValue(new(currentX, currentY), out currentPObject))
                        {
                            if (currentPObject.isPlayerMoveable)
                            {
                                puzzle.GetGrid(currentX, currentY).laserExists = true;
                                puzzle.GetGrid(currentX, currentY).yLaserExists = true;
                                puzzle.GetGrid(currentX, currentY).laserStartX = grid.x;
                                puzzle.GetGrid(currentX, currentY).laserStartY = grid.y;
                                currentY--;
                            }
                            else
                            {
                                currentPObject.laserDir = PObj.ld_s;
                                if (currentPObject.type == Constant.GRID_MIRROR_Q)
                                {
                                    dir = Constant.GRID_LASER_A;
                                    currentPObject.isOnLaser = true;
                                    goto changeDir;
                                }
                                else if (currentPObject.type == Constant.GRID_MIRROR_E)
                                {
                                    dir = Constant.GRID_LASER_D;
                                    currentPObject.isOnLaser = true;
                                    goto changeDir;
                                }
                                else if (!currentPObject.isMirror)
                                    currentPObject.isOnLaser = true;
                                break;
                            }
                        }
                        else
                        {
                            puzzle.GetGrid(currentX, currentY).laserExists = true;
                            puzzle.GetGrid(currentX, currentY).yLaserExists = true;
                            puzzle.GetGrid(currentX, currentY).laserStartX = grid.x;
                            puzzle.GetGrid(currentX, currentY).laserStartY = grid.y;
                            currentY--;
                        }
                    }
                    break;
                case Constant.GRID_LASER_D:
                    puzzle.GetGrid(currentX, currentY).laserExists = true;
                    puzzle.GetGrid(currentX, currentY).xLaserExists = true;
                    puzzle.GetGrid(currentX, currentY).laserStartX = grid.x;
                    puzzle.GetGrid(currentX, currentY).laserStartY = grid.y;
                    currentX++;
                    while (puzzle.GetGrid(currentX, currentY).isMoveable)
                    {
                        if (puzzle.objects.TryGetValue(new(currentX, currentY), out currentPObject))
                        {
                            if (currentPObject.isPlayerMoveable)
                            {
                                puzzle.GetGrid(currentX, currentY).laserExists = true;
                                puzzle.GetGrid(currentX, currentY).xLaserExists = true;
                                puzzle.GetGrid(currentX, currentY).laserStartX = grid.x;
                                puzzle.GetGrid(currentX, currentY).laserStartY = grid.y;
                                currentX++;
                            }
                            else
                            {
                                currentPObject.laserDir = PObj.ld_d;
                                if (currentPObject.type == Constant.GRID_MIRROR_Q)
                                {
                                    dir = Constant.GRID_LASER_W;
                                    currentPObject.isOnLaser = true;
                                    goto changeDir;
                                }
                                else if (currentPObject.type == Constant.GRID_MIRROR_Z)
                                {
                                    dir = Constant.GRID_LASER_S;
                                    currentPObject.isOnLaser = true;
                                    goto changeDir;
                                }
                                else if (!currentPObject.isMirror)
                                    currentPObject.isOnLaser = true;
                                break;
                            }
                        }
                        else
                        {
                            puzzle.GetGrid(currentX, currentY).laserExists = true;
                            puzzle.GetGrid(currentX, currentY).xLaserExists = true;
                            puzzle.GetGrid(currentX, currentY).laserStartX = grid.x;
                            puzzle.GetGrid(currentX, currentY).laserStartY = grid.y;
                            currentX++;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

}
