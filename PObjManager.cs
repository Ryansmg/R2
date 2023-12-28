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
    public GameObject childGO;
    public float laserTimer = 0f;
    private void Start()
    {
    }

    void Update()
    {
        try
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(pObj.x, pObj.y, 0), ref velocity, smoothness);

            //destroy by hp
            if (pObj.hp <= 0 && pObj.type == Constant.GRID_WOODEN_BOX)
            {
                pObj.puzzle.objects.Remove(new(pObj.x, pObj.y));
                Destroy(gameObject);
            }
        } catch (NullReferenceException) { }

        //laser behavior
        try
        {
            mirrorLaser.SetActive(pObj.isOnLaser);
        }
        catch (NullReferenceException) { }
        catch (UnassignedReferenceException) { }

        if(pObj.type == Constant.GRID_WOODEN_BOX && pObj.isOnLaser)
        {
            if (laserTimer <= 0f) { 
                pObj.hp -= 0.5f; 
                laserTimer = 0.5f; 
            } else
            {
                laserTimer -= Time.deltaTime;
            }
            childGO.GetComponent<SpriteRenderer>().color = new Color(1, 1 - (210 * laserTimer / 255), 1 - (210 * laserTimer / 255));
        } else if(pObj.type == Constant.GRID_WOODEN_BOX) {
            laserTimer = 0f;
            childGO.GetComponent<SpriteRenderer>().color = Color.white;
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
            if (Player.CheckMoveable_s(pObj.x, pObj.y + 1)
                && !Player.CheckPlayer(StartPuzzle.currentPuzzle.GetGrid(pObj.x, pObj.y + 1)))
            {
                conveyorWaitTimer = conveyorWaitTimerDefault;
                pObj.PushTo(pObj.x, pObj.y + 1);
            }
        }
        else if (StartPuzzle.currentPuzzle.GetGrid(pObj.x, pObj.y).status == Constant.GRID_CONVEYOR_A)
        {
            if (Player.CheckMoveable_s(pObj.x - 1, pObj.y)
                && !Player.CheckPlayer(StartPuzzle.currentPuzzle.GetGrid(pObj.x - 1, pObj.y)))
            {
                conveyorWaitTimer = conveyorWaitTimerDefault;
                pObj.PushTo(pObj.x - 1, pObj.y);
            }
        }
        else if (StartPuzzle.currentPuzzle.GetGrid(pObj.x, pObj.y).status == Constant.GRID_CONVEYOR_S)
        {
            if (Player.CheckMoveable_s(pObj.x, pObj.y - 1)
                && !Player.CheckPlayer(StartPuzzle.currentPuzzle.GetGrid(pObj.x, pObj.y - 1)))
            {
                conveyorWaitTimer = conveyorWaitTimerDefault;
                pObj.PushTo(pObj.x, pObj.y - 1);
            }
        }
        else if (StartPuzzle.currentPuzzle.GetGrid(pObj.x, pObj.y).status == Constant.GRID_CONVEYOR_D)
        {
            if (Player.CheckMoveable_s(pObj.x + 1, pObj.y)
                && !Player.CheckPlayer(StartPuzzle.currentPuzzle.GetGrid(pObj.x + 1, pObj.y)))
            {
                conveyorWaitTimer = conveyorWaitTimerDefault;
                pObj.PushTo(pObj.x + 1, pObj.y);
            }
        }
    }
}
