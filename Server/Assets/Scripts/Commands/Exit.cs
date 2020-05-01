using UnityEngine;

public class Exit : Command
{
    public override void Run(string[] args) 
    {
        Application.Quit();
    }
}