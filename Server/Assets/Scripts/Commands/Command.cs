using UnityEngine;

public class Command
{
    protected Console Console { get; }
    protected Server Server { get; }

    public Command() 
    {
        Console = GameObject.Find("Console").GetComponent<Console>();
        Server = GameObject.Find("Server").GetComponent<Server>();
    }

    public virtual void Run(string[] args) 
    {

    }
}
