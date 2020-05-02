using UnityEngine;

[Command("exit")]
public class Exit : Command
{
    public override void Run(string[] args) 
    {
        Application.Quit();
    }
}