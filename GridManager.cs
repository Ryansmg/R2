using System;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject portal;
    public GameObject block;
    public GameObject ironBox;
    public GameObject woodenBox;
    public GameObject laser;
    public GameObject laserX;
    public GameObject laserY;
    public GameObject glassPiece;
    public GameObject conveyor;
    public GameObject mirror;
    public GameObject mirrorLaser;
    public GameObject nxtpzle_up;
    public GameObject nxtpzle_down;
    public GameObject[] tiles;
    public PuzzleGrid grid;
    public Puzzle puzzle;

    bool outsideGridUpdated = false;

    void LateUpdate()
    {
        if (!StartPuzzle.puzzleLoaded) return;
        if (outsideGridUpdated && !Editor.isEditing) return;
        int status = grid.status;
        if(portal.activeSelf) portal.SetActive(false);
        if (block.activeSelf) block.SetActive(false);
        if (ironBox.activeSelf) ironBox.SetActive(false);
        if (woodenBox.activeSelf) woodenBox.SetActive(false);
        if (laser.activeSelf) laser.SetActive(false);
        if (glassPiece.activeSelf)
            glassPiece.SetActive(false);
        if (laserX.activeSelf)
            laserX.SetActive(false);
        if (laserY.activeSelf)
            laserY.SetActive(false);
        if (mirror.activeSelf)
            mirror.SetActive(false);
        if (conveyor.activeSelf)
            conveyor.SetActive(false);
        if(nxtpzle_down.activeSelf)
            nxtpzle_down.SetActive(false);
        if(nxtpzle_up.activeSelf)
            nxtpzle_up.SetActive(false);

        if (status == Constant.GRID_OUTSIDE) outsideGridUpdated = true;

        if (Editor.isEditing)
        {
            if (status == Constant.GRID_START) { portal.SetActive(true); }
            if (status == Constant.GRID_IRON_BOX) { ironBox.SetActive(true); }
            if (status == Constant.GRID_WOODEN_BOX) { woodenBox.SetActive(true); }
            if (status == Constant.GRID_MIRROR_Q)
            {
                mirror.SetActive(true);
                mirror.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            if (status == Constant.GRID_MIRROR_Z)
            {
                mirror.SetActive(true);
                mirror.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            if (status == Constant.GRID_MIRROR_C)
            {
                mirror.SetActive(true);
                mirror.transform.rotation = Quaternion.Euler(0, 0, 270);
            }
            if (status == Constant.GRID_MIRROR_E)
            {
                mirror.SetActive(true);
                mirror.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        if (status == Constant.GRID_END) portal.SetActive(true);
        if (status == Constant.GRID_WALL) block.SetActive(true);
        if (status == Constant.GRID_GLASS_PIECE) glassPiece.SetActive(true);
        if (status == Constant.GRID_NEXTPUZZLE_UP) nxtpzle_up.SetActive(true);
        if (status == Constant.GRID_NEXTPUZZLE_DOWN) nxtpzle_down.SetActive(true);
        if (status == Constant.GRID_CONVEYOR_W)
        {
            conveyor.SetActive(true);
            conveyor.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (status == Constant.GRID_CONVEYOR_A)
        {
            conveyor.SetActive(true);
            conveyor.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        if (status == Constant.GRID_CONVEYOR_S)
        {
            conveyor.SetActive(true);
            conveyor.transform.rotation = Quaternion.Euler(0, 0, 270);
        }
        if (status == Constant.GRID_CONVEYOR_D)
        {
            conveyor.SetActive(true);
            conveyor.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (status == Constant.GRID_LASER_W)
        {
            laser.SetActive(true);
            laser.transform.rotation = Quaternion.Euler(0, 0, 270);
        }
        if (status == Constant.GRID_LASER_A)
        {
            laser.SetActive(true);
            laser.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (status == Constant.GRID_LASER_S)
        {
            laser.SetActive(true);
            laser.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (status == Constant.GRID_LASER_D)
        {
            laser.SetActive(true);
            laser.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        bool noMirror = true;
        try
        {
            bool objExi = puzzle.objects.TryGetValue(new(grid.x, grid.y), out PObj thisObj);
            if (objExi) noMirror = !thisObj.isMirror;
        } catch (NullReferenceException) { }
        if (grid.xLaserExists && noMirror)
        {
            laserX.SetActive(true);
        }
        if(grid.yLaserExists && noMirror)
        {
            laserY.SetActive(true);
        }

        gameObject.transform.position = new Vector3(grid.x, grid.y);
        tiles[grid.tile].SetActive(true);
        for (int i = 0; i < grid.tile; i++) if (tiles[i].activeSelf) tiles[i].SetActive(false);
        for (int i = grid.tile + 1; i < 10; i++) if (tiles[i].activeSelf) tiles[i].SetActive(false);
    }
}
