public class Restart : Command
{
    public override void Run(string[] args)
    {
        if (!Server.IsRunning())
        {
            Console.Error("Server needs to be running to restart.");
            return;
        }

        Server.StopServer();
        Server.StartServer();
        Console.Log("Restarted server successfully.");
    }
}