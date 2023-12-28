using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PObj
{
    public int x, y;
    public readonly int type;
    public static GameObject woodenBoxPrefab;
    public static GameObject ironBoxPrefab;
    public static GameObject mirrorPrefab;
    public GameObject gObject;
    public Puzzle puzzle;
    public bool isPlayerMoveable;
    public bool isBreakable;
    public bool isPushable;
    public float hp;
    public bool isOnLaser = false;
    public int laserDir = ld_none;
    public const int ld_none = -1;
    public const int ld_w = 0;
    public const int ld_a = 1;
    public const int ld_s = 2;
    public const int ld_d = 3;

    public bool isMirror = false;

    public PObj(int x, int y, int type)
    {
        this.x = x; this.y = y; this.type = type;
    }

    /// <summary>
    /// Call when puzzle is generated.
    /// </summary>
    public void Generate()
    {
        puzzle = StartPuzzle.currentPuzzle;
        switch (type)
        {
            case Constant.GRID_WOODEN_BOX:
                gObject = Object.Instantiate(woodenBoxPrefab, GameObject.Find("Objects").transform);
                isPlayerMoveable = false;
                isBreakable = true;
                isPushable = true;
                hp = 3f;
                break;
            case Constant.GRID_IRON_BOX:
                gObject = Object.Instantiate(ironBoxPrefab, GameObject.Find("Objects").transform);
                isPlayerMoveable = false;
                isBreakable = false;
                isPushable = true;
                break;
            case Constant.GRID_MIRROR_Q:
                gObject = Object.Instantiate(mirrorPrefab, GameObject.Find("Objects").transform);
                gObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                isPlayerMoveable = false;
                isBreakable = false;
                isPushable = true;
                isMirror = true;
                break;
            case Constant.GRID_MIRROR_Z:
                gObject = Object.Instantiate(mirrorPrefab, GameObject.Find("Objects").transform);
                gObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                isPlayerMoveable = false;
                isBreakable = false;
                isPushable = true;
                isMirror = true;
                break;
            case Constant.GRID_MIRROR_C:
                gObject = Object.Instantiate(mirrorPrefab, GameObject.Find("Objects").transform);
                gObject.transform.rotation = Quaternion.Euler(0, 0, 270);
                isPlayerMoveable = false;
                isBreakable = false;
                isPushable = true;
                isMirror = true;
                break;
            case Constant.GRID_MIRROR_E:
                gObject = Object.Instantiate(mirrorPrefab, GameObject.Find("Objects").transform);
                gObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                isPlayerMoveable = false;
                isBreakable = false;
                isPushable = true;
                isMirror = true;
                break;
            default: return;
        }
        gObject.GetComponent<PObjManager>().pObj = this;
        gObject.transform.position = new Vector3(x, y, 0);
        puzzle.objects.TryAdd(new(x, y), this);
    }

    public void PushTo(int nx, int ny)
    {
        if(Mathf.Abs(x-nx) > 1 || Mathf.Abs(y-ny) > 1 || !isPushable) { return; }
        bool objCheck = true;
        if (puzzle.objects.TryGetValue(new(nx, ny), out PObj pObj))
            if (!pObj.isPlayerMoveable) objCheck = false;

        if(puzzle.GetGrid(nx, ny).isMoveable && objCheck)
        {
            puzzle.objects.Remove(new(x, y));
            puzzle.objects.Add(new(nx, ny), this);
            x = nx;
            y = ny;
            isOnLaser = false;
        }
    }

    public void Destroy()
    {
        puzzle.objects.Remove(new(x, y));
        Object.Destroy(gObject);
    }
}
