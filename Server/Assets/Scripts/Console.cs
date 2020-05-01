using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public Dictionary<string, Command> Commands = new Dictionary<string, Command>();
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
    private Dictionary<GameObject, int> ConsoleMessages = new Dictionary<GameObject, int>();

    void Awake()
    {
        Server = GoServer.GetComponent<Server>();
        UIInputField = GoInput.GetComponent<InputField>();
        ContentRectTransform = GoContent.GetComponent<RectTransform>();

        /*DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Scripts/Commands");
        FileInfo[] files = dir.GetFiles("*.cs");
        foreach (FileInfo file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file.Name).ToLower();
            if (!name.Equals("command")) {
                Type t = Type.GetType(name);
                Commands.Add(name, new Activator.CreateInstance(t));
            }
        }*/

        Commands.Add("broadcast", new Broadcast());
        Commands.Add("exit", new Exit());
        Commands.Add("help", new Help());
        Commands.Add("kick", new Kick());
        Commands.Add("list", new List());
        Commands.Add("restart", new Restart());
        Commands.Add("start", new Start());
        Commands.Add("status", new Status());
        Commands.Add("stop", new Stop());
    }

    void Start()
    {
        FocusInput();
    }

    public void Log(string message)
    {
        Log(message, new Color(0.6f, 0.6f, 0.6f, 1.0f));
    }

    public void Log(string message, Color color)
    {
        GameObject goText = Instantiate(GoTextPrefab, GoContent.transform);
        RectTransform goTextRect = goText.GetComponent<RectTransform>();
        Text text = goText.GetComponent<Text>();

        text.text = message;
        text.color = color;

        int lines = CalcLines(text.preferredWidth, goTextRect.rect.width);

        ConsoleMessages.Add(goText, lines); // Add message to list, this will effect ConsoleMessages.Count

        // Resize content box
        ContentRectTransform.sizeDelta = new Vector2(0, MESSAGE_HEIGHT * AllLinesCount() + (MESSAGE_HEIGHT / 2) * lines);

        ResetInput();
        FocusInput();
    }

    int CalcLines(float contentWidth, float lineWidth)
    {
        int lines = 1;
        while (contentWidth > lineWidth)
        {
            contentWidth -= lineWidth;
            lines++;
        }
        return lines;
    }

    int AllLinesCount()
    {
        int height = 0;
        foreach (var item in ConsoleMessages)
        {
            height += item.Value;
        }
        return height;
    }

    public void HandleInput()
    {
        if (!Input.GetKeyDown(KeyCode.Return))
            return;

        string[] args = UIInputField.text.ToLower().Split();
        string cmd = args[0];

        if (Commands.ContainsKey(cmd))
        {
            Commands[cmd].Run(args);
        }
        else
        {
            if (!cmd.Equals(""))
            {
                Log("Error: Unknown command \"" + args[0] + "\"", new Color(1f, 0.75f, 0.75f, 1f));
            }
        }
    }

    void ResetInput()
    {
        UIInputField.text = "";
        ContentRectTransform.anchoredPosition = new Vector2(10, -Screen.height + PADDING);
    }

    void FocusInput()
    {
        UIInputField.Select();
        UIInputField.ActivateInputField();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
        {
            FocusInput();
        }
    }
}