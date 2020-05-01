public class Status : Command
{
    public override void Run(string[] args)
    {
        if (Server.IsRunning())
        {
            Console.Log("Server is online.");
        }
        else
        {
            Console.Log("Server is offline.");
        }
    }
}