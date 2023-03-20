using System;
using System.Runtime.CompilerServices;
using System.Text;
using DotJEM.InteractiveConsole.Abstractions;
using DotJEM.InteractiveConsole.Commands;
using DotJEM.InteractiveConsole.Commands.Options;
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
        IConsoleCommand<IOption<T>> consoleCommand = new OptionsConsoleCommand<T>(new SystemConsoleProxy(), message, options);
        return consoleCommand.Execute();
    }
}


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