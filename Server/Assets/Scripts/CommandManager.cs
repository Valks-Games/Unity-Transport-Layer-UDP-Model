using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class CommandManager
{
    private Dictionary<string, Command> commands = new Dictionary<string, Command>();

    /// <summary>
    /// Gets a dictionary which contains the commands, such that the key represents the command name, and the value
    /// represents the description.
    /// </summary>
    public Dictionary<string, string> GetCommandsWithDescriptions()
    {
        var result = new Dictionary<string, string>();

        foreach (Command command in this.commands.Values)
        {
            var type = command.GetType();
            var attribute = type.GetCustomAttribute<CommandAttribute>();
            if (attribute is null)
            {
                continue;
            }

            result.Add(attribute.Name, attribute.Description);
        }

        return result;
    }

    /// <summary>
    /// Registers a command to be recognised by this manager.
    /// </summary>
    /// <param name="command">The command instance to register.</param>
    public void RegisterCommand(Command command)
    {
        if (!(command.GetType().GetCustomAttribute<CommandAttribute>() is CommandAttribute attribute))
        {
            throw new ArgumentException(
                $"{nameof(command)} does not have {nameof(CommandAttribute)}",
                nameof(command));
        }

        this.commands.Add(attribute.Name, command);
    }

    /// <summary>
    /// Registers all <see cref="Command"/> types found in the current assembly.
    /// </summary>
    public void RegisterAssemblyCommands()
    {
        var baseType = typeof(Command);
        var assembly = Assembly.GetAssembly(baseType);
        var allTypes = assembly.GetTypes();
        var commandTypes = allTypes.Where(t => t.IsSubclassOf(baseType));

        foreach (var type in commandTypes)
        {
            var instance = Activator.CreateInstance(type) as Command;
            this.RegisterCommand(instance);
        }
    }

    /// <summary>
    /// Attempts to call <see cref="Command.Run(string[])"/> on the command whose name matches <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The command name.</param>
    /// <param name="args">The command arguments.</param>
    /// <returns><see langword="true"/> if the execution was successful, <see langword="false"/> otherwise.</returns>
    public bool TryRunCommand(string name, string[] args)
    {
        if (!commands.TryGetValue(name, out Command command))
        {
            // no such command
            return false;
        }

        command.Run(args);
        return true;
    }
}