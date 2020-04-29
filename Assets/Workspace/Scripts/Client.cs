using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;

public class Client : MonoBehaviour
{
    public const int PORT = 9000;

    public NetworkDriver Driver;
    public NetworkConnection Connection;
    public bool Done;

    void Start() 
    {
        Driver = NetworkDriver.Create();
        Connection = default(NetworkConnection);

        var endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = PORT;
        Connection = Driver.Connect(endpoint);
    }

    void OnDestroy() 
    {
        Driver.Dispose();
    }

    void Update() 
    {
        Driver.ScheduleUpdate().Complete();

        if (!Connection.IsCreated) 
        {
            if (!Done)
                Debug.Log("Something went wrong during connect");
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = Connection.PopEvent(Driver, out stream)) != NetworkEvent.Type.Empty) 
        {
            if (cmd == NetworkEvent.Type.Connect) 
            {
                Debug.Log("We are now connected to the server.");

                uint value = 1;
                var writer = Driver.BeginSend(Connection);
                writer.WriteUInt(value);
                Driver.EndSend(writer);
            } else if (cmd == NetworkEvent.Type.Data) 
            {
                uint value = stream.ReadUInt();
                Debug.Log("Got the value = " + value + " back from the server.");
                Done = true;
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
