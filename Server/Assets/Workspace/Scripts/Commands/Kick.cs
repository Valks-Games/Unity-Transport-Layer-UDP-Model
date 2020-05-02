using System;
using UnityEngine;

public class Kick : Command
{
    public override void Run(string[] args) 
    {
        if (args.Length <= 1)
        {
            Console.Error("Error: Command kick requires <user> to kick");
            return;
        }

        if (Server.ConnectionCount() == 0) 
        {
            Console.Error("There are no connections online to kick.");
            return;
        }

        int ID;
        bool isParsable = Int32.TryParse(args[1], out ID);
        if (!isParsable || (ID > Server.ConnectionCount() - 1 && ID < 0)) 
        {
            Console.Error("Please specify a valid online connection ID");
            return;
        }

        Server.Kick(ID);
        Console.Log("Kicked " + args[1]);
    }
}