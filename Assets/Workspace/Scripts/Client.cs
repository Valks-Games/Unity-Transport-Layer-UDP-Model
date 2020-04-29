using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;

public class Client : MonoBehaviour
{
    public const string ADDRESS = "142.161.93.165";
    public const int PORT = 7777;

    public NetworkDriver Driver;
    public NetworkConnection Connection;

    void Start() 
    {
        Driver = NetworkDriver.Create();
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

                uint value = 1;
                var writer = Driver.BeginSend(Connection);

                byte[] type = BitConverter.GetBytes(0);
                byte[] x = BitConverter.GetBytes(100);
                byte[] y = BitConverter.GetBytes(500);
                byte[] z = BitConverter.GetBytes(30);

                List<byte> bytes = new List<byte>();
                bytes.AddRange(type);
                bytes.AddRange(x);
                bytes.AddRange(y);
                bytes.AddRange(z);

                var array = new NativeArray<byte>(bytes.ToArray(), Allocator.Persistent);

                writer.WriteBytes(array);

                writer.WriteUInt(value);
                Driver.EndSend(writer);
            } else if (cmd == NetworkEvent.Type.Data) 
            {
                uint value = stream.ReadUInt();
                Debug.Log("Got the value = " + value + " back from the server.");
                Connection.Disconnect(Driver);
                Connection = default(NetworkConnection); // Reset connection
            } else if (cmd == NetworkEvent.Type.Disconnect) 
            {
                Debug.Log("Client got disconnected from server.");
                Connection = default(NetworkConnection);
            }
        }
    }
}
