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

        Vector3 position = new Vector3(Random.Range(0, -5), 1, -5);

        GameObject newObj = Instantiate(pickupPrefab, position, Quaternion.identity);
        pickups.Add(newObj);

        position = new Vector3(Random.Range(0, 5), 1, 5);

        newObj = Instantiate(pickupPrefab, position, Quaternion.identity);
        pickups.Add(newObj);

        position = new Vector3(-5, 1, Random.Range(0, 5));

        newObj = Instantiate(pickupPrefab, position, Quaternion.identity);
        pickups.Add(newObj);

        position = new Vector3(5, 1, Random.Range(0, -5));

        newObj = Instantiate(pickupPrefab, position, Quaternion.identity);
        pickups.Add(newObj);

        //for (int i = 0; i < pickupCount; i++)
        //{
        //    position = Vector3.zero;

        //    while (Mathf.Abs(position.x) <= noSpawnRange && Mathf.Abs(position.z) <= noSpawnRange)
        //    {
        //        //true random pick spawning

        //        position = new Vector3(transform.position.x + (int)Random.Range(-spawnRange, spawnRange),
        //                                   1,
        //                                   transform.position.z + (int)Random.Range(-spawnRange, spawnRange));
        //    /*
        //        int x = Random.Range(0, 3);

        //        //training on only front place pickups
        //        //x = 0;

        //        if (x == 1)
        //        {
        //            position = new Vector3(-5, 0, 0);

        //        }
        //        else if (x == 2)
        //        {
        //            position = new Vector3(5, 0, 0);

        //        }
        //        else
        //        {
        //            position = new Vector3(0, 0, 5);
        //        }

        //    */
        //    }
        //    newObj = Instantiate(pickupPrefab, position, Quaternion.identity);
        //    pickups.Add(newObj);
        //}
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
