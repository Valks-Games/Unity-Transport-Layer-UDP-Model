using System;
using System.Text;

using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;

public class DataHandler
{
    public static void Data(NetworkDriver Driver, NativeList<NetworkConnection> connections, int i, byte[] recBuffer)
    {
        // Heart Beat
        if (recBuffer[0] == 0) 
        {
            Debug.Log("Received heart beat from " + connections[i].InternalId);

            var writer = Driver.BeginSend(NetworkPipeline.Null, connections[i]);

            byte[] buffer = new byte[8];
            buffer[0] = 0;

            var arr = new NativeArray<byte>(buffer, Allocator.Temp);

            writer.WriteBytes(arr);
            Driver.EndSend(writer);
        }

        // Create Account Information
        if (recBuffer[0] == 1)
        {
            string name = Encoding.ASCII.GetString(recBuffer, 1, recBuffer.Length - 1);

            /*var user = new User{Name = name};
            Save.LoadData().UserList.Count*/

            Save.SaveData(name);
        }

        // Position Data
        if (recBuffer[0] == 5)
        {
            Debug.Log(BitConverter.ToSingle(recBuffer, 1));
            Debug.Log(BitConverter.ToSingle(recBuffer, 5));
            Debug.Log(BitConverter.ToSingle(recBuffer, 9));
        }
    }
}
