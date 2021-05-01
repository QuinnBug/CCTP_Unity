using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Action
{
    NULL = -1,
    SHOOT = 0,
    STEP = 1,
    LEFT_TURN = 2,
    RIGHT_TURN = 3
}

public class AI_Controller : MonoBehaviour
{
    public GameObject[] units;
    public List<int>[] trackers = new List<int>[2];
    public GameObject[] trackerObjects = new GameObject[2];
    internal Rigidbody[] rbs = new Rigidbody[4];
    internal float[] rotationSpeed = new float[4];
    internal float[] currentSpeed = new float[4];
    internal bool[] doShoot = new bool[4];
    internal bool dead = false;

    public Collider boundaries;

    int shootCount = 0;

    public float moveSpeed = 1.0f;
    public float rotationMod = 90.0f;
    [Space]
    public int scoreSinceLastCheck = 0;
    public int score = 0;
    public int[] scores = new int[] { 0, 0, 0, 0 };
    public int[] scoresSinceLastCheck = new int[] { 0, 0, 0, 0 };
    [Space]
    public int maxHealth = 100;
    public int health;

    private Vector3[] startPos = new Vector3[4];
    private Vector3[] startRot = new Vector3[4];
    internal Pickup_Spawner spawner;

    private GameObject closestPickup;
    [SerializeField]
    private float distanceToPickup;
    private GameObject prevClosestPickup;
    [SerializeField]
    private float prevDistanceToPickup;

    private int prevAction;

    private void Start()
    {
        for (int i = 0; i < units.Length; i++)
        {
            startPos[i] = units[i].transform.position;
            startRot[i] = units[i].transform.eulerAngles;
            rbs[i] = units[i].GetComponent<Rigidbody>();
        }

        for (int i = 0; i < trackers.Length; i++)
        {
            trackers[i] = new List<int>();
        }

        spawner = FindObjectOfType<Pickup_Spawner>();
        Reset();
    }

    public void Act()
    {
        if (health <= 0)
        {
            dead = true;
            if (shootCount == 0)
            {
                scoreSinceLastCheck -= 100;
            }
        }
        else
        {
            dead = false;
        }

        for (int i = 0; i < units.Length; i++)
        {
            if (rotationSpeed[i] != 0)
            {
                units[i].transform.eulerAngles += new Vector3(0, rotationSpeed[i] * rotationMod, 0);
            }

            if (currentSpeed[i] != 0)
            {                
                if (boundaries.bounds.Contains(units[i].transform.position + units[i].transform.forward * currentSpeed[i] * moveSpeed))
                {
                    units[i].transform.position += units[i].transform.forward * currentSpeed[i] * moveSpeed;
                }
                else
                {
                    Debug.Log(i + " tried to leave the arena");
                    scoresSinceLastCheck[i] += -5;
                }
            }

            //if (doShoot[i])
            //{
            //    Shoot(i);
            //    doShoot[i] = false;
            //}
        }

        CheckTrackers();

        #region commented out code
        //if (closestPickup == null)
        //{
        //    closestPickup = spawner.pickups[0];
        //}

        //prevClosestPickup = closestPickup;
        //prevDistanceToPickup = distanceToPickup;
        //distanceToPickup = (int)Vector3.Distance(closestPickup.transform.position, transform.position);

        //for (int i = 0; i < spawner.pickups.Count; i++)
        //{
        //    if (Vector3.Distance(closestPickup.transform.position, transform.position) < distanceToPickup)
        //    {
        //        closestPickup = spawner.pickups[i];
        //        distanceToPickup = (int)Vector3.Distance(closestPickup.transform.position, transform.position);
        //    }
        //}

        //if (closestPickup != null && prevClosestPickup != null)
        //{
        //    if (distanceToPickup < prevDistanceToPickup)
        //    {
        //        scoreSinceLastCheck += 0;
        //    }
        //    if (distanceToPickup > prevDistanceToPickup)
        //    {
        //        scoreSinceLastCheck -= 0;
        //    }
        //}
        #endregion
    }

    public int[] CheckScore()
    {
        int[] x = new int[4];

        for (int i = 0; i < 4; i++)
        {
            x[i] = scoresSinceLastCheck[i];
            scores[i] += scoresSinceLastCheck[i];
            scoresSinceLastCheck[i] = 0;
        }

        return x;
    }

    public void Reset()
    {
        health = maxHealth;
        for (int i = 0; i < units.Length; i++)
        {
            units[i].transform.position = startPos[i];
            units[i].transform.eulerAngles = startRot[i];

            rotationSpeed[i] = 0;
            currentSpeed[i] = 0;
        }

        CheckTrackers();

        score = 0;
        scoreSinceLastCheck = 0;
        dead = false;
        
        closestPickup = null;
        prevClosestPickup = null;

        shootCount = 0;
    }

    public void Decay()
    {
        TakeDamage(1);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    private void CheckTrackers()
    {
        if (trackers[0].Count == 3)
        {
            Debug.Log(trackers[0] + " " + trackers[1] + " " + trackers[2] + " are all scoring in the zone");
            foreach (var item in trackers[0])
            {
                scoresSinceLastCheck[item] += 150;
            }
        }
        else if (trackers[0].Count == 4)
        {
            Debug.Log("Everyone is here!");
            foreach (var item in trackers[0])
            {
                scoresSinceLastCheck[item] -= 50;
            }
        }

        //foreach (int item in trackers[0])
        //{
        //    scoresSinceLastCheck[item] += 50;
        //}

        //for (int i = 0; i < trackers.Length; i++)
        //{
        //    if (trackers[i].Count >= 2)
        //    {
        //        foreach (int item in trackers[i])
        //        {
        //            scoresSinceLastCheck[item] += 50;
        //        }
        //    }

        //    trackers[i].Clear();
        //}
    }

    public void Shoot(int unitNum)
    {
        //Debug.Log("shoot " + unitNum);

        shootCount++;

        bool x = false;

        RaycastHit hit;
        if (Physics.Raycast(units[unitNum].transform.position, units[unitNum].transform.forward, out hit, 100, 1 << LayerMask.NameToLayer("Raycastable")))
        {
            for (int i = 0; i < trackers.Length; i++)
            {
                if (hit.collider.gameObject == trackerObjects[i])
                {
                    if (trackers[i].Contains(unitNum) == false)
                    {
                        trackers[i].Add(unitNum);
                    }

                    x = true;
                    break;
                }
            }

            if (x == false)
            {
                scoresSinceLastCheck[unitNum] -= 5;
            }
            else
            {
                scoresSinceLastCheck[unitNum] += 5;
            }

        }
        else
        {
            scoresSinceLastCheck[unitNum] -= 10;
        }

        #region pickup shoot code
        /*
        Debug.Log("shoot " + unitNum);

        RaycastHit hit;
        if (Physics.Raycast(units[unitNum].transform.position, units[unitNum].transform.forward, out hit, 100, 1 << LayerMask.NameToLayer("Raycastable")))
        {
            if (spawner.pickups.Contains(hit.collider.gameObject))
            {
                Debug.Log("score " + unitNum);
                scoreSinceLastCheck += 20;
                //health = maxHealth;
                spawner.DestroyPickup(hit.collider.gameObject);
            }
            else
            {
                Debug.Log("hit something wrong " + unitNum);
            }
        }
        else
        {
            scoreSinceLastCheck -= 5;
            Debug.Log("miss " + unitNum);
        }

        if (spawner.pickups.Count == 0)
        {
            scoreSinceLastCheck += 100;
            health = 0;
        }
        */
        #endregion
    }

    public void SetAction(int unitNum, int action)
    {
        //Debug.Log(unitNum + " does " + (Action)action);

        currentSpeed[unitNum] = 0;
        rotationSpeed[unitNum] = 0;
        doShoot[unitNum] = false;

        switch ((Action)action)
        {
            case Action.SHOOT:
                //doShoot[unitNum] = true;
                break;
            case Action.STEP:
                currentSpeed[unitNum] = 1;
                break;
            case Action.LEFT_TURN:
                rotationSpeed[unitNum] = -1;
                break;
            case Action.RIGHT_TURN:
                rotationSpeed[unitNum] = 1;
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        //if (other.tag == "Pickup")
        //{
        //    prevDistanceToPickup = 999;
        //    distanceToPickup = 999;

        //    scoreSinceLastCheck -= 10;
        //    //scoreSinceLastCheck += 10 + health;
        //    //health += maxHealth;
        //    spawner.DestroyPickup(other.gameObject);

        //    if (spawner.pickups.Count == 0)
        //    {
        //        //scoreSinceLastCheck += 50;
        //        health = 0;
        //    }
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.tag == "Pickup")
        //{
        //    prevDistanceToPickup = 999;
        //    distanceToPickup = 999;

        //    scoreSinceLastCheck -= 10;
        //    //scoreSinceLastCheck += 10 + health;
        //    //health += maxHealth;
        //    spawner.DestroyPickup(other.gameObject);

        //    if (spawner.pickups.Count == 0)
        //    {
        //        //scoreSinceLastCheck += 50;
        //        health = 0;
        //    }
        //}
    }
}
