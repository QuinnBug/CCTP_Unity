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
    internal Rigidbody[] rbs = new Rigidbody[4];
    internal float[] rotationSpeed = new float[4];
    internal float[] currentSpeed = new float[4];
    internal bool[] doShoot = new bool[4];
    internal bool dead = false;

    public float moveSpeed = 1.0f;
    public float rotationMod = 90.0f;
    [Space]
    public int scoreSinceLastCheck = 0;
    public int score = 0;
    [Space]
    public int maxHealth = 100;
    public int health;

    private Vector3[] startPos = new Vector3[4];
    private Vector3[] startRot = new Vector3[4];
    private Pickup_Spawner spawner;

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

        spawner = FindObjectOfType<Pickup_Spawner>();
        Reset();
    }

    public void Act()
    {
        if (health <= 0)
        {
            dead = true;
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
                rbs[i].AddForce(units[i].transform.forward * currentSpeed[i] * moveSpeed);
            }

            if (doShoot[i])
            {
                Shoot(i);
                doShoot[i] = false;
            }
        }

        

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

    public int CheckScore()
    {
        int x = scoreSinceLastCheck;
        score += scoreSinceLastCheck;
        scoreSinceLastCheck = 0;

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
        
        score = 0;
        scoreSinceLastCheck = 0;
        dead = false;
        
        closestPickup = null;
        prevClosestPickup = null;
    }

    public void Decay()
    {
        TakeDamage(1);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void Shoot(int unitNum)
    {
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
                doShoot[unitNum] = true;
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
}
