using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int unitNum;

    public AI_Controller controller;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Zone")
        {
            if (!controller.trackers[0].Contains(unitNum))
            {
                Debug.Log(unitNum + " entered the zone");
                controller.trackers[0].Add(unitNum);
                controller.scoresSinceLastCheck[unitNum] += 10;
            }
        }

        if (other.tag == "Pickup")
        {
            Debug.Log(unitNum + " got a pickup");
            controller.scoresSinceLastCheck[unitNum] += 10;
            controller.spawner.DestroyPickup(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Zone")
        {
            if(controller.trackers[0].Contains(unitNum))
            {
                Debug.Log(unitNum + " left the zone");
                controller.trackers[0].Remove(unitNum);
                controller.scoresSinceLastCheck[unitNum] -= 10;
            }
        }
    }
}
