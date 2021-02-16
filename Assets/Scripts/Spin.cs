using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float speed = 0;
    public Color[] colours;
    MeshRenderer mr;
    internal int colourInt;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        colours[0] = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0));

        colourInt = Random.Range(0, colours.Length);
        mr.material.color = colours[colourInt];
    }
}
