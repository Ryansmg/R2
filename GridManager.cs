using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject portal;
    public GameObject block;
    public GameObject ironBox;
    public GameObject woodenBox;
    public GameObject laser;
    public GameObject laser2;
    public GameObject glassPiece;
    public GameObject conveyor;
    public GameObject mirror;
    public GameObject mirrorLaser;
    public GameObject[] tiles;
    public PuzzleGrid grid;
    public Puzzle puzzle;
    // Update is called once per frame
    void LateUpdate()
    {
        if (!StartPuzzle.puzzleLoaded) return;
        int status = grid.status;
        portal.SetActive(false);
        block.SetActive(false);
        ironBox.SetActive(false);
        woodenBox.SetActive(false);
        laser.SetActive(false);
        glassPiece.SetActive(false);
        laser2.SetActive(false);

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
        if (status == Constant.GRID_LASER) laser2.SetActive(true);
        if (status == Constant.GRID_GLASS_PIECE) glassPiece.SetActive(true);
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
        if (grid.xLaserExists)
        {
            laser2.SetActive(true);
            laser2.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if(grid.yLaserExists)
        {
            laser2.SetActive(true);
            laser2.transform.rotation = Quaternion.Euler(0, 0, 90);
        }

        //deprecated status
        if (status == Constant.GRID_LASER_START)
        {
            laser.SetActive(true); laser2.SetActive(true);
        }
        if (status == Constant.GRID_LASER_END) laser2.SetActive(true);
        gameObject.transform.position = new Vector3(grid.x, grid.y);
        tiles[grid.tile].SetActive(true);
        for (int i = 0; i < grid.tile; i++) tiles[i].SetActive(false);
        for (int i = grid.tile + 1; i < 10; i++) tiles[i].SetActive(false);
    }
}
