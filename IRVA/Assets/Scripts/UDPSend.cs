using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSend : MonoBehaviour
{
    public Camera FirstPersonCamera;
    private string IP;
    private int port;


    IPEndPoint remoteEndPoint;
    UdpClient client;


    public void Start()
    {
        /* TODO 1.1 Set your PC IP */
        IP = "127.0.0.1";
        /* TODO 1.2 Set a port to send messages to. You can use 1098 */
        port = 0;

        /* Setup UDP connection for sending messages */
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();
    }

    /* Send data via UDP */
    private void sendMessage(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    public void Update()
    {
        /* TODO 3 Send the camera orientation and position to the tracking app */
        sendMessage("ARCore:" + new Quaternion(0.0f, 0.0f, 0.0f, 1.0f).ToString() + ";" + new Vector3(0.0f, 0.0f, 0.0f).ToString());
    }
}

