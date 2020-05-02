using UnityEngine;

[Command("start")]
public class Start : Command
{
    public override void Run(string[] args)
    {
        if (!Server.IsRunning())
        {
            Server.StartServer();
        }
        else
        {
            Console.Log("Server is already running.", new Color(1f, 0.75f, 0.75f, 1f));
        }
    }
}