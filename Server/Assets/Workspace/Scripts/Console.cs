using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public Dictionary<string, Command> Commands = new Dictionary<string, Command>();
    public const int MESSAGE_HEIGHT = 20;
    public const float PADDING = 50.5f;

    public GameObject GoServer;
    public static GameObject GoContent;
    public static GameObject GoTextPrefab;
    public GameObject GoInput;

    private Server Server;
    private ScrollRect UIScrollRect;
    private static InputField UIInputField;
    private static RectTransform ContentRectTransform;
    private static Dictionary<GameObject, int> ConsoleMessages = new Dictionary<GameObject, int>();

    void Awake()
    {
        Commands = typeof(Command).Assembly.GetTypes().Where(x => typeof(Command).IsAssignableFrom(x) && !x.IsAbstract).Select(Activator.CreateInstance).Cast<Command>().ToDictionary(x => x.GetType().Name.ToLower(), x => x);
        GoContent = GameObject.Find("Content");
        GoTextPrefab = Resources.Load("Text") as GameObject;
        Server = GoServer.GetComponent<Server>();
        UIInputField = GoInput.GetComponent<InputField>();
        ContentRectTransform = GoContent.GetComponent<RectTransform>();
    }

    void Start()
    {
        FocusInput();
    }

    public static void Error(string message)
    {
        Log(message, new Color(1f, 0.75f, 0.75f, 1f));
    }

    public static void Log(string message)
    {
        Log(message, new Color(0.6f, 0.6f, 0.6f, 1.0f));
    }

    public static void Log(string message, Color color)
    {
        GameObject goText = Instantiate(GoTextPrefab, GoContent.transform);
        RectTransform goTextRect = goText.GetComponent<RectTransform>();
        Text text = goText.GetComponent<Text>();

        text.text = message;
        text.color = color;

        int lines = CalcLines(text.preferredWidth, goTextRect.rect.width);
        ConsoleMessages.Add(goText, lines);

        goText.name = "Message " + ConsoleMessages.Count;

        ContentRectTransform.sizeDelta = new Vector2(0, MESSAGE_HEIGHT * AllLinesCount() + (MESSAGE_HEIGHT / 2) * lines);

        ResetInput();
        FocusInput();
    }

    static int CalcLines(float contentWidth, float lineWidth)
    {
        int lines = 1;
        while (contentWidth > lineWidth)
        {
            contentWidth -= lineWidth;
            lines++;
        }
        return lines;
    }

    static int AllLinesCount()
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
                Error("Error: Unknown command \"" + args[0] + "\"");
            }
        }
    }

    static void ResetInput()
    {
        UIInputField.text = "";
        ContentRectTransform.anchoredPosition = new Vector2(10, -Screen.height + PADDING);
    }

    static void FocusInput()
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