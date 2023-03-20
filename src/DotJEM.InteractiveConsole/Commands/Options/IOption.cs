namespace DotJEM.InteractiveConsole.Commands.Options;

public interface IOption<out TValue>
{
    TValue Value { get; }
    string Label { get; }
}