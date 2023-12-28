using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Constant
{
    //puzzle status
    public const int PUZZLE_HIDDEN = -2;
    public const int PUZZLE_NOT_OPENED = -1;
    public const int PUZZLE_NEW = 0;
    public const int PUZZLE_PLAYED = 1;
    public const int PUZZLE_CLEARED = 2;
    public const int PUZZLE_SEMI_PERFECT = 3;
    public const int PUZZLE_PERFECT = 4;

    //theme
    public const int THEME_TEST = 0;

    //grid status
    public const int GRID_OUTSIDEWALL = -2;
    public const int GRID_OUTSIDE = -1;

    public const int GRID_EMPTY = 0;
    public const int GRID_START = 1;
    public const int GRID_END = 2;
    public const int GRID_WALL = 3;
    public const int GRID_GLASS_PIECE = 4;

    public const int GRID_IRON_BOX = 5;
    public const int GRID_WOODEN_BOX = 6;

    public const int GRID_LASER_W = 10;
    public const int GRID_LASER_A = 11;
    public const int GRID_LASER_S = 12;
    public const int GRID_LASER_D = 13;

    public const int GRID_CONVEYOR_W = 14;
    public const int GRID_CONVEYOR_A = 15;
    public const int GRID_CONVEYOR_S = 16;
    public const int GRID_CONVEYOR_D = 17;

    public const int GRID_MIRROR_Q = 18;
    public const int GRID_MIRROR_Z = 19;
    public const int GRID_MIRROR_C = 20;
    public const int GRID_MIRROR_E = 21;

    public const int GRID_NEXTPUZZLE_UP = 22;
    public const int GRID_NEXTPUZZLE_DOWN = 23;

    public const int GRID_ITEM_DASH = 10000;

}
