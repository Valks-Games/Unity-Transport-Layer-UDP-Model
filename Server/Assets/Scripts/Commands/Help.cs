using System.Collections.Generic;

[Command("help", "Lists available commands")]
public class Help : Command
{
    public override void Run(string[] args)
    {
        // list all registered commands by the manager
        // while also printing the command description - if there is one

        var commandManager = this.Console.CommandManager;
        var commands = commandManager.GetCommandsWithDescriptions();

        foreach (var pair in commands)
        {
            var output = pair.Key;

            if (!string.IsNullOrWhiteSpace(pair.Value))
            {
                output += $"- {pair.Value}";
            }

            this.Console.Log(output);
        }
    }
}
