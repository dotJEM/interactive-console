// See https://aka.ms/new-console-template for more information

using DotJEM.InteractiveConsole;
using DotJEM.InteractiveConsole.Commands.Options;

Console.WriteLine("Hello, World!");

IInteractiveConsole console = new InteractiveConsole();

var selected = console.Options<string>("Select an Option:", 
    new Option<string>("dk", "Denmark"),
    new Option<string>("no", "Norway"),
    new Option<string>("se", "Sweden"),
    new Option<string>("uk", "Great britain."),
    new Option<string>("de", "Germany"),
    new Option<string>("fr", "France"),
    new Option<string>("es", "Spain")
);

Console.WriteLine(selected);