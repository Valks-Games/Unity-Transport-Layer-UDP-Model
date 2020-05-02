public class Stop : Command
{
    public override void Run(string[] args)
    {
        if (!Server.IsRunning())
        {
            Console.Error("Server is not running.");
            return;
        }
        
        Server.StopServer();
    }
}