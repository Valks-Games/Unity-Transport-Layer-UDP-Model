using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

using Unity.Networking.Transport;

public class UIConnecting : MonoBehaviour
{
    public GameObject GoText;
    private TextMeshProUGUI Text;

    string message = "Attempting to Connect to Servers";
    string[] dots = new string[] { "", ".", "..", "..." };

    void Start()
    {
        Text = GoText.GetComponent<TextMeshProUGUI>();
        StartCoroutine(CheckConnection());
    }

    IEnumerator CheckConnection()
    {
        int i = 0;
        while (Client.GetState() == NetworkConnection.State.Connecting)
        {
            Text.text = message + dots[i % dots.Length];
            i++;
            yield return new WaitForSeconds(0.5f);
        }

        if (Client.GetState() == NetworkConnection.State.Disconnected)
            SceneManager.LoadScene("Main Menu");

        if (Client.GetState() == NetworkConnection.State.Connected)
            SceneManager.LoadScene("Create Account");
    }
}
