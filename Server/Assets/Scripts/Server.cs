using System;

using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;

public class Server : MonoBehaviour
{
    public const string ADDRESS = "142.161.93.165";
    public const int PORT = 7777;
    public const int MAX_CONNECTIONS = 16;
    public const int DISCONNECT_TIMEOUT = 30000;

    public NetworkDriver Driver;
    private NativeList<NetworkConnection> connections;

    void Start()
    {
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
    }

    void InitializeNetwork() 
    {
        // Creating Driver without any params
        Driver = NetworkDriver.Create(new NetworkConfigParameter{disconnectTimeoutMS=DISCONNECT_TIMEOUT});
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = PORT;
        if (Driver.Bind(endpoint) != 0)
            Debug.Log("Failed to bind to port " + PORT);
        else
            Driver.Listen(); // Sets the NetworkDriver in the Listen state
        
        connections = new NativeList<NetworkConnection>(MAX_CONNECTIONS, Allocator.Persistent);
    }

    void OnDestroy() 
    {
        // Clear up unmanaged memory on destroy
        Driver.Dispose();
        connections.Dispose();
    }

    void Update()
    {
        Driver.ScheduleUpdate().Complete();

        // Clean up old connections
        for (int i = 0; i < connections.Length; i++) 
        {
            if (!connections[i].IsCreated) 
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        // Accept new connections
        NetworkConnection c;
        while ((c = Driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
            Debug.Log("Accepted a connection");
        }

        // Query Driver for events that might have happened since last update
        DataStreamReader streamReader;
        for (int i = 0; i < connections.Length; i++) {
            if (!connections[i].IsCreated)
                continue;
            
            NetworkEvent.Type cmd;
            while ((cmd = Driver.PopEventForConnection(connections[i], out streamReader)) != NetworkEvent.Type.Empty) 
            {
                if (cmd == NetworkEvent.Type.Data) 
                {
                    byte[] recBuffer = new byte[streamReader.Length];
                    var array = new NativeArray<byte>(recBuffer, Allocator.Temp);

                    streamReader.ReadBytes(array);
                    Debug.Log("ID: " + connections[0].InternalId);
                    recBuffer = array.ToArray();

                    if (recBuffer[0] == 5) // Position Data
                    {
                        Debug.Log(BitConverter.ToSingle(recBuffer, 1));
                        Debug.Log(BitConverter.ToSingle(recBuffer, 5));
                        Debug.Log(BitConverter.ToSingle(recBuffer, 9));
                    }

                    /*var writer = Driver.BeginSend(NetworkPipeline.Null, connections[i]);
                    writer.WriteUInt(number);
                    Driver.EndSend(writer);*/
                } else if (cmd == NetworkEvent.Type.Disconnect) 
                {
                    Debug.Log("Client diconnected from the server.");
                    connections[i] = default(NetworkConnection); // Reset connection
                }
            }
        }
    }
}
