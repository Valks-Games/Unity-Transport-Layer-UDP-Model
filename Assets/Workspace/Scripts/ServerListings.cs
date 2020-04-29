using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerListings : MonoBehaviour
{
    public GameObject client;

    public void Connect() 
    {
        client.GetComponent<Client>().Connect();
    }
}
