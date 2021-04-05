using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraToPNG : MonoBehaviour
{
    private RenderTexture rt;
    public Vector2Int size = new Vector2Int(128,128);
    private string filename;
    private Socket_to_py stp = null;

    AI_Controller ai;
    Pickup_Spawner pickupSpawner;

    bool ready = true;

    private int count = 0;
    private float timer = 0;

    public Camera[] cams;
    [Space]
    public float updateRate = 1.0f;

    private void Start()
    {
        ai = FindObjectOfType<AI_Controller>();
        pickupSpawner = FindObjectOfType<Pickup_Spawner>();
        ResetLevel();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (stp != null && !ready && !stp.connected)
        {
            ready = true;
            while (ready)
            {
                ready = !stp.OpenClient();
            }
        }

        if (stp == null && ready)
        {
            stp = gameObject.AddComponent<Socket_to_py>();

            while (ready)
            {
                ready = !stp.OpenClient();
            }
        }

        if (stp != null && stp.connected)
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    // use this to print out the output from the server - don't run on pure update, always have an extra requirement like a keypress
            //    System.IO.File.WriteAllBytes(Application.dataPath + @"\outputs\" + filename + "_Py_" + count + ".png", stp.output);
            //}

            if (stp.recievedData)
            {
                ai.Decay();

                for (int i = 0; i < ai.units.Length; i++)
                {
                    ai.SetAction(i, stp.output[8 * i]);
                }

                stp.recievedData = false;
            }
            else
            {
                for (int i = 0; i < ai.units.Length; i++)
                {
                    ai.SetAction(i, -1);
                }
            }

            ai.Act();

            if (timer > updateRate)
            {
                int reward = ai.CheckScore();
                List<byte> dataOutput = new List<byte>();

                foreach (Camera camera in cams)
                {
                    dataOutput.AddRange(GetPNGBytesFromCamera(camera));
                }

                stp.ExchangeData(dataOutput.ToArray(), reward, ai.dead);

                count++;
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                stp.Close(stp.connected);
                stp = null;
            }
        }

        if (ai.dead && stp == null)
        {
            ResetLevel();
        }
    }

    private void ResetLevel()
    {
        ai.Reset();
        pickupSpawner.CleanSpawnPickups();
        ready = true;
    }

    public byte[] GetPNGBytesFromCamera(Camera cam)
    {
        rt = new RenderTexture(size.x, size.y, 24);
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();

        RenderTexture.active = null;

        //debug code to out the image to a folder
        System.IO.File.WriteAllBytes(Application.dataPath + @"\outputs\ScreenShot_Unity" + cam.transform.parent.name + "_cam.png", bytes);

        return bytes;
    }
}
