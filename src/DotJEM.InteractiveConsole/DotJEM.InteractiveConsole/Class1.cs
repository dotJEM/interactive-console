using System;
using System.Runtime.CompilerServices;
using System.Text;
using DotJEM.InteractiveConsole.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotJEM.InteractiveConsole;


public interface IInteractiveConsole
{
    IOption<T> Options<T>(string message, params IOption<T>[] options);
}


public class InteractiveConsole : IInteractiveConsole
{
    public InteractiveConsole()
    {
        //TODO: Via options.
        Console.OutputEncoding = Encoding.UTF8;

    }

    public IOption<T> Options<T>(string message, params IOption<T>[] options)
    {
        Console.WriteLine(message);

        int selected = 0;
        (int left, int top) = Console.GetCursorPosition();

        while (true)
        {
            Console.SetCursorPosition(left, top);
            for (int i = 0; i < options.Length; i++)
            {
                if (selected == i)
                {
                    Console.WriteLine($" → \u001b[32m{options[i].Label}\u001b[0m");
                }
                else
                {
                    Console.WriteLine($"   {options[i].Label}");
                }

            }

            switch (Console.ReadKey(false).Key)
            {
                case ConsoleKey.UpArrow:
                    selected = (selected + options.Length - 1) % options.Length;
                    break;

                case ConsoleKey.DownArrow:
                    selected = (selected + options.Length + 1) % options.Length;
                    break;

                case ConsoleKey.Enter:
                    return options[selected];
            }

        }
    }
}

public interface IOption<out TValue>
{
    TValue Value { get; }
    string Label { get; }
}
public record struct Option<T>(T Value, string Label) : IOption<T>;

public interface IInteractiveConsoleBuilder { }

public class InteractiveConsoleBuilder : IInteractiveConsoleBuilder { }

public static class InteractiveConsoleServiceCollectionExtensions
{
    /// <summary>
    /// Adds data protection services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    public static IInteractiveConsoleBuilder AddInteractiveConsole(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        //services.TryAddSingleton<IActivator, TypeForwardingActivator>();
        //services.AddOptions();
        //AddDataProtectionServices(services);

        services.TryAddSingleton<IInteractiveConsole, InteractiveConsole>();

        //return new DataProtectionBuilder(services);
        return new InteractiveConsoleBuilder();
    }
}

public interface IConsoleProvider
{
    IConsole ConsoleImpl { get; }
}