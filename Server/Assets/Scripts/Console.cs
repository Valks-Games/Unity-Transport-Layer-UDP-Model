using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public const int MESSAGE_HEIGHT = 20;
    public const float PADDING = 50.5f;

    public GameObject GoServer;
    public GameObject GoContent;
    public GameObject GoTextPrefab;
    public GameObject GoInput;

    private Server Server;
    private ScrollRect UIScrollRect;
    private InputField UIInputField;
    private RectTransform ContentRectTransform;
    private List<GameObject> ConsoleLines = new List<GameObject>();

    void Awake() 
    {
        Server = GoServer.GetComponent<Server>();
        UIInputField = GoInput.GetComponent<InputField>();
        ContentRectTransform = GoContent.GetComponent<RectTransform>();
    }

    void Print(string message) 
    {
        Print(message, Color.white);
    }

    void Print(string message, Color color) 
    {
        GameObject goText = Instantiate(GoTextPrefab, GoContent.transform);
        goText.transform.Translate(new Vector3(0, -20 * ConsoleLines.Count, 0));
        Text text = goText.GetComponent<Text>();
        text.text = message;
        text.color = color;

        ContentRectTransform.Translate(new Vector3(0, -20 / 2 * (ConsoleLines.Count), 0));
        ContentRectTransform.sizeDelta = new Vector2(0, (20 * (ConsoleLines.Count + 1)));
        
        ConsoleLines.Add(goText);

        ResetInput();
    }

    public void HandleInput() 
    {
        if (!Input.GetKeyDown(KeyCode.Return))
            return;
        
        string input = UIInputField.text;
        string[] args = input.Split();

        switch (args[0])
        {
            case "kick":
            if (args.Length <= 1) 
            {
                Print("Error: Command kick requires <user> to kick", Color.red);
                return;
            }

            Print("Kicked " + args[0]);
            break;
            case "start":

            break;
            case "exit":
            Application.Quit();
            break;
            default:
            if (!args[0].Equals("")) 
            {
                Print("Error: Unknown command \"" + args[0] + "\"", Color.red);
                return;
            }
            break;
        }
    }

    void ResetInput() 
    {
        UIInputField.text = "";
        UIInputField.Select();
        UIInputField.ActivateInputField();

        ContentRectTransform.anchoredPosition = new Vector2(0, -Screen.height + PADDING);
    }
}