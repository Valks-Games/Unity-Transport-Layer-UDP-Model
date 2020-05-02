using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;

public class Client : MonoBehaviour
{
    public const string ADDRESS = "142.160.69.144";
    public const int PORT = 7777;
    public const int DISCONNECT_TIMEOUT = 3000;
    public const int CONNECT_TIMEOUT = 3000;
    public const int MAX_CONNECTION_ATTEMPTS = 3;

    public static NetworkDriver Driver;
    public static NetworkConnection Connection;

    void Start()
    {
        Application.targetFrameRate = 30;
        Application.runInBackground = true;
        DontDestroyOnLoad(gameObject);

        var config = new NetworkConfigParameter
        {
            connectTimeoutMS = CONNECT_TIMEOUT,
            disconnectTimeoutMS = DISCONNECT_TIMEOUT,
            maxConnectAttempts = MAX_CONNECTION_ATTEMPTS
        };

        Driver = NetworkDriver.Create(config);
        Connection = default(NetworkConnection);
    }

    public static NetworkConnection.State GetState()
    {
        return Connection.GetState(Driver);
    }

    public static bool IsConnected()
    {
        return Connection.GetState(Driver) == NetworkConnection.State.Connected;
    }

    public static void Connect()
    {
        Debug.Log("Attempting to connect to server.");
        var endpoint = NetworkEndPoint.Parse(ADDRESS, PORT);
        Connection = Driver.Connect(endpoint);
    }

    public static void Login(string name)
    {
        var writer = Driver.BeginSend(Connection);

        byte[] buffer = new byte[1 + name.Length];
        buffer[0] = 1;

        Encoding.ASCII.GetBytes(name).CopyTo(buffer, 1);

        var array = new NativeArray<byte>(buffer, Allocator.Temp);

        writer.WriteBytes(array);
        Driver.EndSend(writer);
    }

    void OnDestroy()
    {
        Driver.Dispose();
    }

    void Update()
    {
        Driver.ScheduleUpdate().Complete();

        if (!Connection.IsCreated) return;

        DataStreamReader streamReader;
        NetworkEvent.Type cmd;
        while ((cmd = Connection.PopEvent(Driver, out streamReader)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server.");

                StartCoroutine(DataPump());
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                byte[] recBuffer = new byte[streamReader.Length];
                var array = new NativeArray<byte>(recBuffer, Allocator.Temp);

                streamReader.ReadBytes(array);
                recBuffer = array.ToArray();

                if (recBuffer[0] == 0)
                {
                    Debug.Log("Recieved heart beat from server.");
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client disconnect event fired.");
                Connection = default(NetworkConnection);
            }
        }
    }

    IEnumerator DataPump()
    {
        while (Connection.IsCreated)
        {
            var writer = Driver.BeginSend(Connection);

            byte[] buffer = new byte[8];
            buffer[0] = 0; // Heart Beat (Hey server! I'm still alive! Don't kick me!)

            var array = new NativeArray<byte>(buffer, Allocator.Temp);

            writer.WriteBytes(array);
            Driver.EndSend(writer);

            yield return new WaitForSeconds(1f);

            /*var writer = Driver.BeginSend(Connection);
            Vector3 pos = new Vector3(100, 500, 30);

            byte[] buffer = new byte[16];
            buffer[0] = 5; // Position Data
            BitConverter.GetBytes(pos.x).CopyTo(buffer, 1);
            BitConverter.GetBytes(pos.y).CopyTo(buffer, 5);
            BitConverter.GetBytes(pos.z).CopyTo(buffer, 9);

            var array = new NativeArray<byte>(buffer, Allocator.Temp);

            writer.WriteBytes(array);
            Driver.EndSend(writer);

            yield return new WaitForSeconds(1f);*/
        }
    }
}
