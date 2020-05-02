using UnityEngine;

public class Start : Command
{
    public override void Run(string[] args)
    {
        if (Server.IsRunning())
        {
            Console.Error("Server is already running.");
            return;
        }

        Server.StartServer();
    }
}