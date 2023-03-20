namespace DotJEM.InteractiveConsole.Commands;

public interface IConsoleCommand<out T>
{
    T Execute();
}