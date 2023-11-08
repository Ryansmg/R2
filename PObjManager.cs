using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PObjManager : MonoBehaviour
{
    public PObj pObj;
    Vector3 velocity = Vector3.zero;
    public float smoothness;
    public float conveyorWaitTimer = 0f;
    public float conveyorWaitTimerDefault;
    public GameObject mirrorLaser;
    private void Start()
    {
    }

    void Update()
    {
        try
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(pObj.x, pObj.y, 0), ref velocity, smoothness);
        } catch (NullReferenceException) { }

        //laser behavior
        if(pObj.isOnLaser)
        {
            mirrorLaser.SetActive(true);
        }

        //conveyor movement
        if (Player.IsGridConveyor(StartPuzzle.currentPuzzle.GetGrid(pObj.x, pObj.y)) && conveyorWaitTimer > 0)
        {
            conveyorWaitTimer -= Time.deltaTime;
            return;
        }
        else if (!Player.IsGridConveyor(StartPuzzle.currentPuzzle.GetGrid(pObj.x, pObj.y)))
            conveyorWaitTimer = conveyorWaitTimerDefault;

        if (StartPuzzle.currentPuzzle.GetGrid(pObj.x, pObj.y).status == Constant.GRID_CONVEYOR_W)
        {
            if (Player.CheckMoveableStatic(pObj.x, pObj.y + 1)
                && !Player.CheckPlayer(StartPuzzle.currentPuzzle.GetGrid(pObj.x, pObj.y + 1)))
            {
                conveyorWaitTimer = conveyorWaitTimerDefault;
                pObj.PushTo(pObj.x, pObj.y + 1);
            }
        }
        else if (StartPuzzle.currentPuzzle.GetGrid(pObj.x, pObj.y).status == Constant.GRID_CONVEYOR_A)
        {
            if (Player.CheckMoveableStatic(pObj.x - 1, pObj.y)
                && !Player.CheckPlayer(StartPuzzle.currentPuzzle.GetGrid(pObj.x - 1, pObj.y)))
            {
                conveyorWaitTimer = conveyorWaitTimerDefault;
                pObj.PushTo(pObj.x - 1, pObj.y);
            }
        }
        else if (StartPuzzle.currentPuzzle.GetGrid(pObj.x, pObj.y).status == Constant.GRID_CONVEYOR_S)
        {
            if (Player.CheckMoveableStatic(pObj.x, pObj.y - 1)
                && !Player.CheckPlayer(StartPuzzle.currentPuzzle.GetGrid(pObj.x, pObj.y - 1)))
            {
                conveyorWaitTimer = conveyorWaitTimerDefault;
                pObj.PushTo(pObj.x, pObj.y - 1);
            }
        }
        else if (StartPuzzle.currentPuzzle.GetGrid(pObj.x, pObj.y).status == Constant.GRID_CONVEYOR_D)
        {
            if (Player.CheckMoveableStatic(pObj.x + 1, pObj.y)
                && !Player.CheckPlayer(StartPuzzle.currentPuzzle.GetGrid(pObj.x + 1, pObj.y)))
            {
                conveyorWaitTimer = conveyorWaitTimerDefault;
                pObj.PushTo(pObj.x + 1, pObj.y);
            }
        }
    }
}
