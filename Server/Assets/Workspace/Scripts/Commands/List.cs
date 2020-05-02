public class List : Command
{
    public override void Run(string[] args) 
    {
        if (!Server.IsRunning()) {
            Console.Error("Server needs to be running.");
            return;
        }

        Console.Log("There are " + Server.ConnectionCount() + " connections.");
    }
}