using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;

public class Client : MonoBehaviour
{
    public const string ADDRESS = "142.161.93.165";
    public const int PORT = 7777;
    public const int DISCONNECT_TIMEOUT = 30000;

    public NetworkDriver Driver;
    public NetworkConnection Connection;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        Driver = NetworkDriver.Create(new NetworkConfigParameter { disconnectTimeoutMS = DISCONNECT_TIMEOUT });
        Connection = default(NetworkConnection);
    }

    public void Connect()
    {
        Debug.Log("Attempting to connect to server.");
        var endpoint = NetworkEndPoint.Parse(ADDRESS, PORT);
        Connection = Driver.Connect(endpoint);
    }

    void OnDestroy()
    {
        Driver.Dispose();
    }

    void Update()
    {
        Driver.ScheduleUpdate().Complete();

        if (!Connection.IsCreated) return;

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = Connection.PopEvent(Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server.");

                SceneManager.LoadScene("Main");

                var writer = Driver.BeginSend(Connection);
                Vector3 pos = new Vector3(100, 500, 30);

                byte[] buffer = new byte[16];
                buffer[0] = 5; // Position Data
                BitConverter.GetBytes(pos.x).CopyTo(buffer, 1);
                BitConverter.GetBytes(pos.y).CopyTo(buffer, 5);
                BitConverter.GetBytes(pos.z).CopyTo(buffer, 9);

                var array = new NativeArray<byte>(buffer, Allocator.Temp);

                writer.WriteBytes(array);
                Driver.EndSend(writer);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                /*uint value = stream.ReadUInt();
                Debug.Log("Got the value = " + value + " back from the server.");
                Connection.Disconnect(Driver);
                Connection = default(NetworkConnection); // Reset connection*/
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server.");
                Connection = default(NetworkConnection);
            }
        }
    }
}
