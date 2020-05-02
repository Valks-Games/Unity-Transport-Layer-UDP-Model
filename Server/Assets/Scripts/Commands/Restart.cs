[Command("restart")]
public class Restart : Command
{
    public override void Run(string[] args)
    {
        if (!Server.IsRunning())
        {
            Console.Log("Server needs to be running to restart.");
        }
        else
        {
            Server.StopServer();
            Server.StartServer();
            Console.Log("Restarted server successfully.");
        }
    }
}