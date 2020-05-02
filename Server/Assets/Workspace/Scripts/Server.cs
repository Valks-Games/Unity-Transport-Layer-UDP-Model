using System;

using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;

public class Server : MonoBehaviour
{
    public const int PORT = 7777;
    public const int MAX_CONNECTIONS = 16;
    public const int DISCONNECT_TIMEOUT = 3000;
    public const int CONNECT_TIMEOUT = 3000;
    public const int MAX_CONNECTION_ATTEMPTS = 3;

    public GameObject GoConsole;
    private Console Console;

    public static NetworkDriver Driver;
    private static NativeList<NetworkConnection> connections;

    void Awake() 
    {
        Console = GoConsole.GetComponent<Console>();
    }

    void Start()
    {
        Application.targetFrameRate = 30;
        DontDestroyOnLoad(gameObject);
        StartServer();
    }

    public static void StartServer() 
    {
        Console.Log("Starting server..");

        var config = new NetworkConfigParameter {
            connectTimeoutMS = CONNECT_TIMEOUT,
            disconnectTimeoutMS = DISCONNECT_TIMEOUT,
            maxConnectAttempts = MAX_CONNECTION_ATTEMPTS
        };

        Driver = NetworkDriver.Create(config);
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = PORT;
        if (Driver.Bind(endpoint) != 0) 
        {
            Debug.Log("Failed to bind to port " + PORT);
        }
        else 
        {
            Driver.Listen(); // Sets the NetworkDriver in the Listen state
        }
        
        connections = new NativeList<NetworkConnection>(MAX_CONNECTIONS, Allocator.Persistent);

        Console.Log("Server is up and running!");
    }

    public static void StopServer() 
    {
        Driver.Dispose();
        Console.Log("Stopped server.");
    }

    public static int ConnectionCount() 
    {
        return connections.Length;
    }

    public static void Kick(int connectionID) 
    {
        connections[connectionID].Disconnect(Driver);
    }

    public static bool IsRunning()
    {
        return Driver.IsCreated;
    }

    void OnDestroy() 
    {
        if (Driver.IsCreated) 
        {
            // Clear up unmanaged memory on destroy
            Driver.Dispose();
            connections.Dispose();
        }
    }

    void Update()
    {
        if (!Driver.IsCreated)
            return;

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
            Console.Log(c.InternalId + " connected");
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
                    recBuffer = array.ToArray();

                    if (recBuffer[0] == 5) // Position Data
                    {
                        Debug.Log(BitConverter.ToSingle(recBuffer, 1));
                        Debug.Log(BitConverter.ToSingle(recBuffer, 5));
                        Debug.Log(BitConverter.ToSingle(recBuffer, 9));
                    }

                    var writer = Driver.BeginSend(NetworkPipeline.Null, connections[i]);
                    writer.WriteUInt(1);
                    Driver.EndSend(writer);
                } else if (cmd == NetworkEvent.Type.Disconnect) 
                {
                    Debug.Log("Client diconnected from the server.");
                    Console.Log(c.InternalId + " disconnected");
                    connections[i] = default(NetworkConnection); // Reset connection
                }
            }
        }
    }
}
