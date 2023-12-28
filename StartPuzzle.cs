using Newtonsoft.Json;
using System.Collections.Generic;
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
    public bool editMode = false;
    public static bool isFirstPuzzle = true;
    public static Dictionary<string, Puzzle> puzzleDictionary;

    //Debug
    public string dbg_startPuzzleName;
    public static bool isDebugStart = true;
    public string changePuzzleName;
    void Update()
    {
        if(Player.currentPlayer != null)
            if(!Player.currentPlayer.isGoingToNextPuzzle) puzzleName = changePuzzleName;
    }
    void Start()
    {
        PObj.woodenBoxPrefab = woodenBoxPrefab;
        PObj.ironBoxPrefab = ironBoxPrefab;
        PObj.mirrorPrefab = mirrorPrefab;
        if (isDebugStart) puzzleName = dbg_startPuzzleName;
        changePuzzleName = puzzleName;

        scripts.GetComponent<SaveFile>().Load();

        PuzzleGrid[] testGrids = { new(0,0,Constant.GRID_START, 4), new(1,0,Constant.GRID_END, 4) };
        Puzzle testpuzzle = new("test", 1000, 11, 500.0f, 500.0f, testGrids);
        testpuzzle.SavePuzzle();

        Debug.Log("Loading Puzzle: " + puzzleName);
        LoadPuzzle(puzzleName, isFirstPuzzle);

        isDebugStart = false;
    }
    public void LoadPuzzle(string name, bool isFirstPuzzle)
    {
        if(!isFirstPuzzle)
        {
            if(puzzleDictionary.TryGetValue(name, out Puzzle outPuzzle))
            {
                LoadPuzzle(outPuzzle, false);
            }
            else LoadPuzzle(GetPuzzle(name), isFirstPuzzle);
        }
        else LoadPuzzle(GetPuzzle(name), isFirstPuzzle);
    }
    public void LoadPuzzle(Puzzle puzzle, bool isFirstPuzzle)
    {
        puzzleLoaded = false;

        Transform gridT = GameObject.Find("Grids").transform;
        currentPuzzle = puzzle;

        if (isFirstPuzzle) puzzleDictionary = new Dictionary<string, Puzzle>();
        bool isFirstEntry = !puzzleDictionary.ContainsKey(puzzle.puzzleName);

        Camera.main.transform.position = new Vector3(puzzle.cameraPosX, puzzle.cameraPosY, -10);
        Camera.main.orthographicSize = puzzle.cameraSize;
        int startX = -10, startY = -10;
        foreach (PuzzleGrid grid in puzzle.grids)
        {
            if (grid.status == Constant.GRID_OUTSIDE) continue;
            GameObject gridObj = Instantiate(puzzleGridPrefab, gridT);
            gridObj.GetComponent<GridManager>().grid = grid;
            gridObj.GetComponent<GridManager>().puzzle = puzzle;
            if (grid.status == Constant.GRID_START) { startX = grid.x; startY = grid.y; }
            if(!editMode && isFirstEntry) new PObj(grid.x, grid.y, grid.status).Generate();
        }

        Dictionary<KeyValuePair<int, int>, PObj> pobjects = new(puzzle.objects);
        if(!isFirstEntry)
        {
            foreach (var v in pobjects) v.Value.Generate();
        }


        //instantiate player
        Player prevPlayer = Player.currentPlayer;
        player = Instantiate(playerPrefab);
        player.GetComponent<Player>().puzzle = puzzle;
        if (isFirstPuzzle)
        {
            if (!puzzle.hasStartGrid)
            {
                if (!isDebugStart && !editMode)
                {
                    Debug.LogError("error: firstPuzzle without startGrid.");
                    return;
                }
            }
            else
            {
                player.GetComponent<Player>().x = startX;
                player.GetComponent<Player>().y = startY;
                player.transform.position = new Vector3(startX, startY);
            }
            player.GetComponent<Player>().oxygen = puzzle.oxygen;
            player.GetComponent<Player>().hp = puzzle.hp;
        }
        else
        {
            player.GetComponent<Player>().x = prevPlayer.x;
            player.GetComponent<Player>().y = prevPlayer.y;
            player.transform.position = new Vector3(prevPlayer.x, prevPlayer.y);
            player.GetComponent<Player>().oxygen = prevPlayer.oxygen;
            player.GetComponent<Player>().hp = prevPlayer.hp;
        }

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
                GameObject backGridGO = Instantiate(puzzleGridPrefab, GameObject.Find("Grids").transform);
                backGridGO.GetComponent<GridManager>().grid = backGrid;
            }
        }

        if (!puzzleDictionary.ContainsKey(puzzle.puzzleName))
            puzzleDictionary.Add(puzzle.puzzleName, puzzle);

        puzzleLoaded = true;
    }
    public static Puzzle GetPuzzle(string name)
    {
        TextAsset puzzle = (TextAsset)Resources.Load($"puzzle\\{name}");
        return JsonConvert.DeserializeObject<Puzzle>(puzzle.text);
    }
}