using UnityEngine;

[Command("kick")]
public class Kick : Command
{
    public override void Run(string[] args) 
    {
        if (args.Length <= 1)
        {
            Console.Log("Error: Command kick requires <user> to kick", new Color(1f, 0.75f, 0.75f, 1f));
            return;
        }

        Console.Log("Kicked " + args[0]);
    }
}