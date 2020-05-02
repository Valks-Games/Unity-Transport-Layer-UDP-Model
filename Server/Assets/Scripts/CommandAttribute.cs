using System;

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <remarks>Defaults to the command having an empty description.</remarks>
    public CommandAttribute(string name)
        : this(name, string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <param name="description">A brief description of the command.</param>
    public CommandAttribute(string name, string description)
    {
        this.Name = name;
        this.Description = description;
    }

    /// <summary>
    /// Gets the description of this command.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the name of this command.
    /// </summary>
    public string Name { get; }
}