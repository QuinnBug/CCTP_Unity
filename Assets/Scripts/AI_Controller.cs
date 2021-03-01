using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour
{
    internal float rotationSpeed = 0;
    internal float currentSpeed = 0;
    internal bool dead = false;

    public float moveSpeed = 1.0f;
    public float rotationMod = 90.0f;
    [Space]
    //public float minDamageRange;
    //public float damageRangeStep;
    [Space]
    public int scoreSinceLastCheck = 0;
    public int score = 0;
    [Space]
    public int maxHealth = 100;
    public int health;

    private Vector3 startPos;
    private Pickup_Spawner spawner;
    //private float distanceFromStart;
    [SerializeField]
    private float distanceFromClosestPickup;
    [SerializeField]
    private float prevDistanceFromClosestPickup;
    private GameObject closestPickup;
    private GameObject prevClosestPickup;

    private int prevAction;

    private void Start()
    {
        startPos = transform.position;
        spawner = FindObjectOfType<Pickup_Spawner>();
        Reset();
    }

    public void Act()
    {
        //Debug.Log("health " + health);

        if (health <= 0)
        {
            dead = true;
        }
        else
        {
            dead = false;
        }

        if (rotationSpeed != 0)
        {
            transform.eulerAngles += new Vector3 (0, rotationSpeed * rotationMod, 0);
        }

        if (currentSpeed != 0)
        {
            transform.position += transform.forward * currentSpeed * moveSpeed;
        }
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
        transform.position = startPos;
        transform.eulerAngles = Vector3.zero;
        score = 0;
        scoreSinceLastCheck = 0;
        dead = false;
        rotationSpeed = 0;
        currentSpeed = 0;
        distanceFromClosestPickup = 999;
        prevDistanceFromClosestPickup = 999;
        closestPickup = null;
        prevClosestPickup = null;
    }

    public void Decay(int action)
    {
        prevDistanceFromClosestPickup = distanceFromClosestPickup;
        distanceFromClosestPickup = 999;

        prevClosestPickup = closestPickup;

        foreach (GameObject obj in spawner.pickups)
        {
            if (distanceFromClosestPickup >= Vector3.Distance(transform.position, obj.transform.position))
            {
                distanceFromClosestPickup = Vector3.Distance(transform.position, obj.transform.position);
                closestPickup = obj;
            }
        }

        TakeDamage(1);

        //if ((int)distanceFromClosestPickup <= 5 && action == 1 && distanceFromClosestPickup < prevDistanceFromClosestPickup)
        //{
        //    scoreSinceLastCheck += 6 - (int)distanceFromClosestPickup;
        //}

        //if (prevAction != 1 && action == prevAction)
        //{
        //    scoreSinceLastCheck -= 1;
        //}
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pickup")
        {
            Debug.Log("gained score");
            scoreSinceLastCheck += 10 + health;
            //health += maxHealth;
            health = 0;
            distanceFromClosestPickup = 999;
            prevDistanceFromClosestPickup = 999;
            spawner.DestroyPickup(other.gameObject);
        }
    }
}
