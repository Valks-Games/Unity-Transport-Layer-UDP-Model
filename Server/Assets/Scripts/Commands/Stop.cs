[Command("stop")]
public class Stop : Command
{
    public override void Run(string[] args)
    {
        if (Server.IsRunning())
        {
            Server.StopServer();
        }
        else
        {
            Console.Log("Server is not running.");
        }
    }
}