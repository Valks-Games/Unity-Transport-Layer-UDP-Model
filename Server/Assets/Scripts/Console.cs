using System;
using System.IO;
using System.Reflection;
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
    private Dictionary<GameObject, int> ConsoleMessages = new Dictionary<GameObject, int>();

    /// <summary>
    /// Gets the current command manager for this console.
    /// </summary>
    public CommandManager CommandManager { get; } = new CommandManager();

    void Awake()
    {
        Server = GoServer.GetComponent<Server>();
        UIInputField = GoInput.GetComponent<InputField>();
        ContentRectTransform = GoContent.GetComponent<RectTransform>();

        // keep note: reflection is very slow!
        // if you implement dozens and dozens of command, this particular method call will get very expensive.
        // there is nothing that can be done about this. while reflection is very powerful, it's also very inefficient.
        // I recommend you research some pros/cons about reflection in general, and decide for yourself if this is
        // something you want to do
        this.CommandManager.RegisterAssemblyCommands();
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

        if (!string.IsNullOrWhiteSpace(cmd) && !this.CommandManager.TryRunCommand(cmd, args))
        {
            Log("Error: Unknown command \"" + args[0] + "\"", new Color(1f, 0.75f, 0.75f, 1f));
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