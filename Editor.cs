using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Editor : MonoBehaviour
{
    public static bool isEditing = false;

    public bool incCamSize = false;
    public bool decCamSize = false;
    public Camera cam;
    public bool loadNew = false;
    public bool indentSave = true;
    public bool save = false;
    public bool endEdit = false;

    public int status;
    public int tile;
    public string nextPuzzleName;
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

    public TMP_InputField statusIPF;
    public TMP_InputField tileIPF;
    public TMP_InputField npnIPF;
    public Toggle swipeToggle;
    public static Dictionary<string, int> strToStatus;
    public static Dictionary<int, string> statusToStr;
    public static bool selectedOther = false;

    // Start is called before the first frame update
    void Start()
    {
        selectedOther = false;
        isEditing = true;
        StartPuzzle.isDebugStart = false;
        strToStatus = new Dictionary<string, int>
            {
                { "outsidewall", Constant.GRID_OUTSIDEWALL },
                { "outside", Constant.GRID_OUTSIDE },
                { "empty", Constant.GRID_EMPTY },
                { "", Constant.GRID_EMPTY },
                { "start", Constant.GRID_START },
                { "end", Constant.GRID_END },
                { "wall", Constant.GRID_WALL },
                { "glasspiece", Constant.GRID_GLASS_PIECE },
                { "piece", Constant.GRID_GLASS_PIECE },
                { "glass", Constant.GRID_GLASS_PIECE },
                { "ironbox", Constant.GRID_IRON_BOX },
                { "woodenbox", Constant.GRID_WOODEN_BOX },
                { "conveyorw", Constant.GRID_CONVEYOR_W },
                { "conveyora", Constant.GRID_CONVEYOR_A },
                { "conveyors", Constant.GRID_CONVEYOR_S },
                { "conveyord", Constant.GRID_CONVEYOR_D },
                { "mirrorq", Constant.GRID_MIRROR_Q },
                { "mirrorz", Constant.GRID_MIRROR_Z },
                { "mirrorc", Constant.GRID_MIRROR_C },
                { "mirrore", Constant.GRID_MIRROR_E },
                { "npup", Constant.GRID_NEXTPUZZLE_UP },
                { "nextpuzzleup", Constant.GRID_NEXTPUZZLE_UP },
                { "npdown", Constant.GRID_NEXTPUZZLE_DOWN },
                { "nextpuzzledown", Constant.GRID_NEXTPUZZLE_DOWN }
            };
        statusToStr = new Dictionary<int, string>
        {
                { Constant.GRID_OUTSIDEWALL, "outsidewall" },
                {  Constant.GRID_OUTSIDE, "outside" },
                {  Constant.GRID_EMPTY, "empty" },
                { Constant.GRID_START,"start" },
                { Constant.GRID_END, "end" },
                {  Constant.GRID_WALL, "wall" },
                {  Constant.GRID_GLASS_PIECE, "piece" },
                { Constant.GRID_IRON_BOX, "ironbox"  },
                {  Constant.GRID_WOODEN_BOX, "woodenbox" },
                { Constant.GRID_CONVEYOR_W, "conveyorw"  },
                {  Constant.GRID_CONVEYOR_A, "conveyora" },
                {  Constant.GRID_CONVEYOR_S, "conveyors" },
                {  Constant.GRID_CONVEYOR_D, "conveyord" },
                {  Constant.GRID_MIRROR_Q, "mirrorq" },
                {  Constant.GRID_MIRROR_Z, "mirrorz" },
                {  Constant.GRID_MIRROR_C, "mirrorc" },
                {  Constant.GRID_MIRROR_E, "mirrore" },
                { Constant.GRID_NEXTPUZZLE_UP,  "npup" },
                {  Constant.GRID_NEXTPUZZLE_DOWN, "npdown" }
        };

    }

    // Update is called once per frame
    void Update()
    {
        pnstatic = puzzleName;
        try
        {
            if (endEdit)
            {
                List<PuzzleGrid> removeList = new();
                foreach (PuzzleGrid g in StartPuzzle.currentPuzzle.grids)
                    if (g.status == Constant.GRID_OUTSIDE) removeList.Add(g);
                foreach (PuzzleGrid g in removeList) StartPuzzle.currentPuzzle.grids.Remove(g);
                StartPuzzle.currentPuzzle.ResetGridsDict();

                Puzzle newPuzzle = new(puzzleName, oxygen, minMove, hp, perfectHp, StartPuzzle.currentPuzzle.grids.ToArray());
                newPuzzle.SavePuzzle(indentSave);
                StartPuzzle.puzzleName = puzzleName;
                SceneManager.LoadScene("PuzzleScene");
                pnstatic = "placeholder";
                isEditing = false;
                return;
            }
            if (save)
            {
                List<PuzzleGrid> removeList = new();
                foreach(PuzzleGrid g in StartPuzzle.currentPuzzle.grids)
                    if (g.status == Constant.GRID_OUTSIDE) removeList.Add(g);
                foreach (PuzzleGrid g in removeList) StartPuzzle.currentPuzzle.grids.Remove(g);
                StartPuzzle.currentPuzzle.ResetGridsDict();

                Puzzle newPuzzle = new(puzzleName, oxygen, minMove, hp, perfectHp, StartPuzzle.currentPuzzle.grids.ToArray());
                newPuzzle.SavePuzzle(indentSave);
                save = false;
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
                grid.nextPuzzleName = nextPuzzleName;

                if (grid.status == Constant.GRID_LASER_W || grid.status == Constant.GRID_LASER_A ||
                    grid.status == Constant.GRID_LASER_S || grid.status == Constant.GRID_LASER_D)
                {
                    StartPuzzle.currentPuzzle.laserStartGrids.Add(grid);
                } else
                {
                    foreach(PuzzleGrid lgrid in StartPuzzle.currentPuzzle.laserStartGrids)
                    {
                        if(lgrid.x == grid.x && lgrid.y == grid.y)
                        {
                            StartPuzzle.currentPuzzle.laserStartGrids.Remove(lgrid);
                            break;
                        }
                    }
                }
                confirmChange = false;
            }
            if (preSelX != selectedX || preSelY != selectedY)
            {
                preSelX = selectedX;
                preSelY = selectedY;
                PuzzleGrid grid = StartPuzzle.currentPuzzle.GetGrid(selectedX, selectedY);
                if (!swipeToggle.isOn)
                {
                    status = grid.status;
                    tile = grid.tile;
                    nextPuzzleName = grid.nextPuzzleName;
                    statusIPF.text = statusToStr[grid.status];
                    tileIPF.text = grid.tile + "";
                    npnIPF.text = grid.nextPuzzleName;
                } else
                {
                    StatusIPFUpdated();
                    TileIPFUpdated();
                    NPNIPFUpdated();
                }
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
    public void StatusIPFUpdated()
    {
        string s = statusIPF.text.Replace(" ", "").ToLower();
        if (int.TryParse(s, out int r))
        {
            status = r;
            confirmChange = true;
        }
        else if (strToStatus.ContainsKey(s)) { 
            status = strToStatus[s]; 
            confirmChange = true;
        }
    }
    public void TileIPFUpdated() {
        if (int.TryParse(tileIPF.text, out int i))
        {
            tile = i;
            confirmChange = true;
        }
    }
    public void NPNIPFUpdated() { nextPuzzleName = npnIPF.text; confirmChange = true; }
    public void SaveButtonPress() { save = true; }
    public void EndEditButtonPress() { endEdit = true; }
    public void ZoomOutButtonPress() { incCamSize = true; }
    public void ZoomInButtonPress() { decCamSize = true; }
    public void IPFSelected() { selectedOther = true; }
    public void IPFDeselected() { selectedOther = false; }
}
