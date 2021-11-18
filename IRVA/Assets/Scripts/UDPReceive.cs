using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;

/* TODO 2 Complete coordinate system for camera representation in scene (add OY and OZ axis to hierarchy for camera representation) */
public class UDPReceive : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    private int port;

    Vector3 position;
    Quaternion orientation;
   
    string positionTransform;

    public Text ARCorePosition;
    public Text ARCoreRotation;

    public GameObject arcore;


    public void Start()
    {
        /* TODO 1.3 Set a port to listen to (same port from the phone app) */
        port = 0;

        /* Setup UDP for receiving data */
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    /* Receive messages via UDP */
    private void ReceiveData()
    {
        client = new UdpClient(port);

        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                string text = Encoding.UTF8.GetString(data);
                ParseText(text);

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    void OnDisable()
    {
        if (receiveThread != null)
            receiveThread.Abort();

        client.Close();
    }

    /* Parse data received from UDP */
    public void ParseText(string text)
    {
        positionTransform = text.Split(':')[0];

        switch (positionTransform)
        {
            case "ARCore":
                orientation = StringToQuaternion(text.Split(':')[1].Split(';')[0]);
                position = StringToVector3(text.Split(':')[1].Split(';')[1]);
                break;
            default:
                break;
        }
    }

    /* Convert string to Vector3 */
    public static Vector3 StringToVector3(string sVector)
    {
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        string[] sArray = sVector.Split(',');

        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    /* TODO 4 Convert string to Quaternion */
    public static Quaternion StringToQuaternion(string sVector)
    {
        return new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
    }

    public void Update()
    {
        /* TODO 5.1 Set the camera representation position and rotation to the values received via UDP */
        arcore.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
        arcore.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

        /* TODO 5.2 Display the values received via UDP on screen
         * (convert the quaternion to Euler angles so that we can easily undestand the data on screen)
         */
        ARCorePosition.text = "Position: " + new Vector3(0.0f, 0.0f, 0.0f).ToString();
        ARCoreRotation.text = "Orientation: " + new Quaternion(0.0f, 0.0f, 0.0f, 1.0f).eulerAngles.ToString();
    }
}