using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{
    private static string savePath = savePath = Application.persistentDataPath + "/users.save";

    public static void SaveData(string name)
    {
        var users = LoadData();
        if (users == null) 
        {
            users = new Users();
            users.UserList.Add(name, new User{Name = name});
        } else {
            users.UserList.Add(name, new User{Name = name});
        }
        
        var binaryFormatter = new BinaryFormatter();
        using (var fileStream = File.Create(savePath))
        {
            binaryFormatter.Serialize(fileStream, users);
        }

        Debug.Log("User saved.");
    }

    public static Users LoadData()
    {
        Users users;

        var binaryFormatter = new BinaryFormatter();

        try
        {
            using (var fileStream = File.Open(savePath, FileMode.Open))
            {
                users = (Users)binaryFormatter.Deserialize(fileStream);
            }

            return users;
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }
}
