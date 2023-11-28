using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Puzzle
{
    public string puzzleName;
    public int puzzleTheme;

    public int oxygen;
    public int minMove;
    public float hp;
    public float perfectHp;

    public int xLength;
    public int yLength;

    public int maxX;
    public int maxY;
    public int minX;
    public int minY;

    public int gridCount;

    public float cameraPosX;
    public float cameraPosY;
    public float cameraSize;

    public bool isXLong;

    public List<PuzzleGrid> grids;
    //x,y -> index of grids
    public Dictionary<string, int> gridsDict;
    public Dictionary<KeyValuePair<int, int>, PObj> objects = new();
    public List<PuzzleGrid> laserStartGrids;

    public static PuzzleGrid nonExistentGrid = new(int.MinValue, int.MinValue, Constant.GRID_OUTSIDE, -1);
    Puzzle() { }
    public Puzzle(string name, int oxygen, int minMove, float hp, float perfectHp, PuzzleGrid[] grids)
    {
        puzzleName = name;
        this.grids = grids.ToList();
        gridsDict = new();
        maxX = int.MinValue;
        maxY = int.MinValue;
        minX = int.MaxValue;
        minY = int.MaxValue;
        gridCount = grids.Length;
        this.oxygen = oxygen;
        this.minMove = minMove;
        this.hp = hp;
        this.perfectHp = perfectHp;
        bool hasStart = false, hasEnd = false;

        List<string> duplicationCheck = new();
        List<PuzzleGrid> laserStartGrids = new();

        int gridIndex = -1;
        foreach ( PuzzleGrid grid in grids )
        {
            gridIndex++;
            if (grid.status == Constant.GRID_OUTSIDE) continue;

            if (grid.x > maxX) maxX = grid.x;
            if (grid.y > maxY) maxY = grid.y;
            if (grid.x < minX) minX = grid.x;
            if (grid.y < minY) minY = grid.y;

            if(duplicationCheck.Contains(grid.x + "," + grid.y)) throw new InvalidDataException("This puzzle has duplicate grids. Delete one grid.");
            duplicationCheck.Add(grid.x + "," + grid.y);
            gridsDict.Add($"{grid.x},{grid.y}", gridIndex);

            if ((grid.status == Constant.GRID_START) && hasStart) throw new InvalidDataException("This puzzle has multiple start grids.");
            if (grid.status == Constant.GRID_START) hasStart = true;
            if (grid.status == Constant.GRID_END) hasEnd = true;
            if (IsGridLaserStart(grid.status)) { laserStartGrids.Add(grid); }
        }
        if (!hasStart) throw new InvalidDataException("This puzzle doesn't have a start grid.");
        if (!hasEnd) throw new InvalidDataException("This puzzle doesn't have a end grid.");

        this.laserStartGrids = laserStartGrids;

        xLength = maxX - minX + 1;
        yLength = maxY - minY + 1;
        cameraPosX = (maxX + minX) / 2f;
        cameraPosY = (maxY + minY) / 2f;
        if (yLength * 16 >= xLength * 9) { cameraSize = maxY - cameraPosY + 1.5f; isXLong = false; }
        else { cameraSize = ((maxX - cameraPosX) * 9f / 16f) + 0.5f; isXLong = true; }

        puzzleTheme = Constant.THEME_TEST;
    }
    public void SavePuzzle(bool indented)
    {
        if (indented) SavePuzzle();
        else SavePuzzleNotIndented();
    }
    public void SavePuzzle()
    {
        string filePath = $"Assets\\Resources\\puzzle\\{puzzleName}.txt";
#if UNITY_EDITOR
        File.WriteAllText(filePath, JsonConvert.SerializeObject(this, Formatting.Indented));
#else
        Debug.LogError("Tried SavePuzzle() outside unity editor.");
#endif
    }
    public void SavePuzzleNotIndented()
    {
        string filePath = $"Assets\\Resources\\puzzle\\{puzzleName}.txt";
#if UNITY_EDITOR
        File.WriteAllText(filePath, JsonConvert.SerializeObject(this));
#else
        Debug.LogError("Tried SavePuzzle() outside unity editor.");
#endif
    }
    public PuzzleGrid GetGrid(int x, int y)
    {
        if (gridsDict.TryGetValue($"{x},{y}", out int value))
        {
            return grids[value];
        }
        else return nonExistentGrid;
    }
    bool IsGridLaserStart(int status)
    {
        bool b = status switch
        {
            Constant.GRID_LASER_START => true,
            Constant.GRID_LASER_W => true,
            Constant.GRID_LASER_A => true,
            Constant.GRID_LASER_S => true,
            Constant.GRID_LASER_D => true,
            _ => false
        };
        return b;
    }
}
