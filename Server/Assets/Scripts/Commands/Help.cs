public class Help : Command
{
    public override void Run(string[] args) 
    {
        Console.Log("Commands: broadcast, list, kick, status, start, stop, restart, exit");
    }
}
