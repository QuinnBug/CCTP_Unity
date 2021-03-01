using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Spawner : MonoBehaviour
{
    public int pickupCount = 10;
    public GameObject pickupPrefab;
    public float spawnRange;
    public float noSpawnRange;

    public List<GameObject> pickups = new List<GameObject>();

    public void CleanSpawnPickups()
    {
        ClearPickups();

        for (int i = 0; i < pickupCount; i++)
        {
            Vector3 position = Vector3.zero;

            while (Mathf.Abs(position.x) <= noSpawnRange && Mathf.Abs(position.z) <= noSpawnRange)
            {
                //position = new Vector3(transform.position.x + (int)Random.Range(-spawnRange, spawnRange),
                //                           transform.position.y + 1,
                //                           transform.position.z + (int)Random.Range(-spawnRange, spawnRange));
                int x = Random.Range(0, 3);
                if (x == 1)
                {
                    position = new Vector3(-4, 0, 0);

                }
                else if (x == 2)
                {
                    position = new Vector3(4, 0, 0);

                }
                else
                {
                    position = new Vector3(0, 0, 5);
                }
            }

            GameObject newObj = Instantiate(pickupPrefab, position, Quaternion.identity);
            pickups.Add(newObj);
        }
    }

    private void ClearPickups()
    {
        for (int i = 0; i < pickups.Count; i++)
        {
            Destroy(pickups[i]);
        }

        pickups.Clear();
    }

    public void DestroyPickup(GameObject pickup)
    {
        if (pickups.Contains(pickup))
        {
            pickups.Remove(pickup);
            Destroy(pickup);
        }
    }

    
}
