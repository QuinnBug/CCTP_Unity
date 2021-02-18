using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Socket_to_py : MonoBehaviour
{
    TcpClient skt;
    string host = "127.0.0.1";
    int port = 65432;
    //int size = 56000;
    public byte[] output;
    public bool connected;
    public bool recievedData;

    public bool OpenClient()
    {
        try
        {
            skt = new TcpClient();
            skt.Connect(host, port);
            if (skt.Connected)
            {
                connected = true;
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception making the client -> " + e.ToString());
        }
        return false;
    }

    internal void Close(bool connected)
    {
        //sends the message telling python to stop looping and start listening for a connection again
        if (connected)
        {
            byte[] bytes = new byte[1] { 0 };
            ExchangeData(bytes, 0, false);
        }

        skt.Close();
        skt = null;
        Destroy(this);
    }

    public void ExchangeData(byte[] bytes, int reward, bool game_over)
    {
        int stateInt = 0;
        if (game_over)
        {
            stateInt = 1;
            Debug.Log("game over");
        }

        List<byte> byteList = new List<byte>();
        byteList.Add((byte)stateInt);
        if (reward > 0)
        {
            byteList.Add(0);
        }
        else
        {
            byteList.Add(1);
        }
        byteList.Add((byte)Mathf.Abs(reward));
        byteList.AddRange(bytes);
        byte[] data = byteList.ToArray();

        //Debug.Log("data " + data[0] + " / " + data[1] + " / " + data[2] + " / ");

        NetworkStream stream = skt.GetStream();
        if (stream.DataAvailable == false)
        {
            //Debug.Log("Sending Data");
            stream.Write(data, 0, data.Length);
        }
        else
        {
            Debug.Log("Data stream error");
        }

        data = new byte[8];

        int dataSize = stream.Read(data, 0, 8);

        recievedData = true;
        output = data;

        if (game_over)
        {
            Close(false);
        }
    }
}
