using DotJEM.InteractiveConsole.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotJEM.InteractiveConsole.Commands.Options;


public class OptionsConsoleCommand<T> : IConsoleCommand<IOption<T>>
{
    private readonly IConsole console;
    private readonly string message;
    private readonly IOption<T>[] options;

    public OptionsConsoleCommand(IConsole console, string message, IOption<T>[] options)
    {
        this.console = console;
        this.message = message;
        this.options = options;
    }

    public IOption<T>? Execute()
    {
        RenderedOptionCollection<T> rendered = new RenderedOptionCollection<T>(options);


        string search = string.Empty;
        (int left, int top) = console.GetCursorPosition();
        while (true)
        {
            console.SetCursorPosition(left, top);
            console.WriteLine(message);

            if (!string.IsNullOrEmpty(search))
                console.WriteLine("filter: " + search + " ");

            rendered.Render(console);

            ConsoleKeyInfo input = console.ReadKey(false);
            switch (input)
            {
                case { Key: ConsoleKey.UpArrow }:
                    rendered.SelectPrevious();
                    break;

                case { Key: ConsoleKey.DownArrow }:
                    rendered.SelectNext();
                    break;

                case { Key: ConsoleKey.Enter }:
                    IOption<T>? selected = rendered.Selected?.Item;
                    console.SetCursorPosition(left, top);
                    if(selected != null)
                        console.WriteLine($"{message} > \u001b[36m{selected.Label}\u001b[0m");
                    else
                        console.WriteLine(message);

                    return selected; 

                case { Key: ConsoleKey.Escape }:
                    return null;

                case { Key: ConsoleKey.Backspace }:
                    if (!string.IsNullOrEmpty(search))
                        search = search[..^1];
                    rendered.Filter(search);
                    break;

                default:
                    if (input.KeyChar is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9')
                        search += input.KeyChar;
                    rendered.Filter(search);
                    break;
            }

        }

    }
}


public class RenderedOptionCollection<T>
{
    private readonly string emptyLine;
    private readonly RenderedOption<T>[] options;
    private RenderedOption<T>[] filteredOptions;

    public RenderedOption<T>? Selected => filteredOptions.FirstOrDefault(item => item.IsSelected);

    public RenderedOptionCollection(IEnumerable<IOption<T>> options)
    {
        int width = options.Max(item => item.Label.Length) + 2;
        this.emptyLine = new string(' ', width);
        this.options = options
            .Select(item => new RenderedOption<T>(item, width))
            .ToArray();
        this.filteredOptions = this.options;
    }

    public void SelectPrevious()
    {
        int selectedIndex = this.filteredOptions.CountWhile(option => !option.IsSelected);
        foreach (var option in options)
            option.IsSelected = false;
        this.filteredOptions[(selectedIndex + filteredOptions.Length - 1) % filteredOptions.Length].IsSelected = true;
    }

    public void SelectNext()
    {
        int selectedIndex = this.filteredOptions.CountWhile(option => !option.IsSelected);
        foreach (var option in options)
            option.IsSelected = false;
        this.filteredOptions[(selectedIndex + filteredOptions.Length + 1) % filteredOptions.Length].IsSelected = true;
    }

    public void Render(IConsole console)
    {
        foreach (RenderedOption<T> option in filteredOptions)
            option.Render(console);

        (int left, int top) = console.GetCursorPosition();
        for (int i = 0; i < options.Length; i++)
            console.WriteLine(emptyLine);
        console.SetCursorPosition(left, top);
    }

    public void Filter(string search)
    {
        this.filteredOptions = options.Where(item => item.IsMatch(search)).ToArray();
    }
}

public class RenderedOption<T>
{
    private readonly string text;
    private readonly string selectedText;

    public IOption<T> Item { get; }
    public bool IsSelected { get; set; }
    public RenderedOption(IOption<T> item, int width)
    {
        this.Item = item;
        this.text = ($"  {item.Label}").PadRight(width);
        this.selectedText = ($"\u001b[36m> {item.Label}\u001b[0m").PadRight(width);
    }

    public void Render(IConsole console)
    {
        if (IsSelected) console.WriteLine(selectedText);
        else console.WriteLine(text);
    }

    public bool IsMatch(string search)
    {
        return Item.Label.StartsWith(search);
    }
}

public static class Extensions
{
    public static int CountWhile<T>(this IEnumerable<T> self, Predicate<T> predicate)
    {
        int cnt = 0;
        foreach (T item in self)
        {
            if (!predicate(item))
                return cnt;
            cnt++;
        }
        return -1;
    }
}