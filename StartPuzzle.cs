using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class StartPuzzle : MonoBehaviour
{
    //PObj prefabs
    public GameObject woodenBoxPrefab;
    public GameObject ironBoxPrefab;
    public GameObject laserBoxPrefab;

    public GameObject scripts;
    public GameObject puzzleGridPrefab;
    public GameObject playerPrefab;
    public GameObject player;
    public static string puzzleName = "test";
    public static Puzzle currentPuzzle;
    public static bool puzzleLoaded = false;

    //Debug
    public string changePuzzleName;
    void Update()
    {
        puzzleName = changePuzzleName;
    }
    void Start()
    {
        PObj.woodenBoxPrefab = woodenBoxPrefab;
        PObj.ironBoxPrefab = ironBoxPrefab;
        changePuzzleName = puzzleName;
        if (!Editor.pnstatic.Equals("placeholder")) puzzleName = Editor.pnstatic;

        scripts.GetComponent<SaveFile>().Load();
        PuzzleGrid[] testGrids = {
            new(0,3,0), new(1,3,1), new(2,3,1), new(3,3,1), new(4,3,1), new(5,3,1), new(6,3,1), new(7,3,1), new(8,3,1), new(9,3,2),
            new(0,2,3), new(1,2,4), new(2,2,4), new(3,2,Constant.GRID_GLASS_PIECE,4), new(4,2,Constant.GRID_GLASS_PIECE, 4), new(5,2,4), new(6,2,Constant.GRID_IRON_BOX,4), new(7,2,Constant.GRID_LASER_A,4), new(8,2,4), new(9,2,5),
            new(0, 1, Constant.GRID_START, 3), new(1, 1, Constant.GRID_WALL, 4), new(2, 1, Constant.GRID_WALL, 4), new(3,1,4), new(4,1,Constant.GRID_WOODEN_BOX,4), new(5,1,Constant.GRID_LASER_D,4), new(6,1,4), new(7,1,4), new(8,1,4), new(9, 1, Constant.GRID_END, 5), 
            new(0,0,3), new(1,0,4), new(2,0,4), new(3,0,4), new(4,0,4), new(5,0,4), new(6,0,4), new(7,0,4), new(8,0,4), new(9,0,5),
            new(0,-1,6), new(1,-1,7), new(2,-1,7), new(3,-1,7), new(4,-1,7), new(5,-1,7), new(6,-1,7), new(7,-1,7), new(8,-1,7), new(9,-1,8)};
        Puzzle puzzle = new("test", 1000, 11, 500.0f, 500.0f, testGrids);
        puzzle.SavePuzzle();
        //LoadPuzzle(puzzle);
        LoadPuzzle(puzzleName);
        //LoadPuzzle("test");
    }
    public void LoadPuzzle(string name)
    {
        LoadPuzzle(GetPuzzle(name));
    }
    public void LoadPuzzle(Puzzle puzzle)
    {
        currentPuzzle = puzzle;

        Camera.main.transform.position = new Vector3(puzzle.cameraPosX, puzzle.cameraPosY, -10);
        Camera.main.orthographicSize = puzzle.cameraSize;
        int startX = -10, startY = -10;
        foreach (PuzzleGrid grid in puzzle.grids)
        {
            GameObject gridObj = Instantiate(puzzleGridPrefab);
            gridObj.GetComponent<GridManager>().grid = grid;
            gridObj.GetComponent<GridManager>().puzzle = puzzle;
            if (grid.status == Constant.GRID_START) { startX = grid.x; startY = grid.y; }
            new PObj(grid.x, grid.y, grid.status).Generate();
        }


        //instantiate player
        player = Instantiate(playerPrefab);
        player.transform.position = new Vector3(startX, startY);
        player.GetComponent<Player>().x = startX;
        player.GetComponent<Player>().y = startY;
        player.GetComponent<Player>().puzzle = puzzle;
        player.GetComponent<Player>().oxygen = puzzle.oxygen;
        player.GetComponent<Player>().hp = puzzle.hp;

        //set cameraPos, mapLength, etc
        int mapEndX, mapEndY, mapStartX, mapStartY;
        if (!puzzle.isXLong)
        {
            mapStartY = puzzle.maxY + 1;
            mapEndY = puzzle.minY - 1;
            mapStartX = (int)(puzzle.cameraPosX - ((mapStartY - mapEndY) * 8f / 9f) - 2f);
            mapEndX = (int)(puzzle.cameraPosX + ((mapStartY - mapEndY) * 8f / 9f) + 2f);
        } else
        {
            mapStartX = puzzle.maxX + 1;
            mapEndX = puzzle.minX - 1;
            mapStartY = (int)(puzzle.cameraPosY - ((mapStartX - mapEndX) * 9f / 32f) - 2f);
            mapEndY = (int)(puzzle.cameraPosY + ((mapStartX - mapEndX) * 9f / 32f) + 2f);
            (mapEndX, mapStartX) = (mapStartX, mapEndX);
            (mapEndY, mapStartY) = (mapStartY, mapEndY);
        }
        //Debug.Log($"{mapStartX}, {mapStartY}, {mapEndX}, {mapEndY}");
        
        //generate background grids
        for(int x = mapStartX; x <= mapEndX; x++) { 
            for(int y = mapStartY; y >= mapEndY; y--)
            {
                PuzzleGrid backGrid = new(x, y, Constant.GRID_OUTSIDE, 9);
                GameObject backGridGO = Instantiate(puzzleGridPrefab);
                backGridGO.GetComponent<GridManager>().grid = backGrid;
            }
        }

        puzzleLoaded = true;
    }
    public Puzzle GetPuzzle(string name)
    {
        TextAsset puzzle = (TextAsset)Resources.Load($"puzzle\\{name}");
        return JsonConvert.DeserializeObject<Puzzle>(puzzle.text);
    }
}