namespace DotJEM.InteractiveConsole.Commands.Options;

public record struct Option<T>(T Value, string Label) : IOption<T>;