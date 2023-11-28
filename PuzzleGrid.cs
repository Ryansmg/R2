using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PuzzleGrid { 
    public int x;
    public int y;
    public int status;
    public int tile;
    public bool isMoveable;
    public bool laserExists;
    //for enabling laser obj
    public bool xLaserExists;
    public bool yLaserExists;
    public int laserStartX;
    public int laserStartY;

    /// <summary>
    /// Default constructor for JSON deserialization.
    /// </summary>
    public PuzzleGrid() { }

    public PuzzleGrid(int x, int y, int status, int tile)
    {
        this.x = x;
        this.y = y;
        this.status = status;
        this.tile = tile;
        isMoveable = status switch
        {
            Constant.GRID_WALL => false,
            Constant.GRID_OUTSIDE => false,
            Constant.GRID_OUTSIDEWALL => false,
            Constant.GRID_LASER_W => false,
            Constant.GRID_LASER_A => false,
            Constant.GRID_LASER_S => false,
            Constant.GRID_LASER_D => false,
            _ => true,
        };
        laserExists = false;
    }

    public PuzzleGrid(int x, int y) : this(x, y, Constant.GRID_EMPTY, 4) { }
    public PuzzleGrid(int x, int y, int tile) : this(x, y, Constant.GRID_EMPTY, tile) { }
}