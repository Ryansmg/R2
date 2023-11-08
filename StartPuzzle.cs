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
    public GameObject mirrorPrefab;

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
        PObj.mirrorPrefab = mirrorPrefab;
        if (!Editor.pnstatic.Equals("placeholder")) puzzleName = Editor.pnstatic;
        changePuzzleName = puzzleName;

        scripts.GetComponent<SaveFile>().Load();
        PuzzleGrid[] testGrids = {
            new(0,0,Constant.GRID_START, 4), new(1,0,Constant.GRID_END, 4)};
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
            if (grid.status == Constant.GRID_OUTSIDE) continue;
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