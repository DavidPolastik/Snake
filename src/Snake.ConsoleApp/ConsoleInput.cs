using System;
using Snake.Core;

namespace Snake.ConsoleApp;

/// <summary>
/// Čtení kláves z konzole. Jediné místo, které zná klávesnici; převádí stisky
/// na povely jádra (<see cref="InputCommand"/>). Neblokující.
///
/// Ovládání: šipky = směr, Esc = konec.
/// </summary>
public sealed class ConsoleInput : IInput
{
    public InputCommand? Poll()
    {
        if (Console.IsInputRedirected)
        {
            return null;  // přesměrovaný vstup nemá klávesnici (KeyAvailable by házel)
        }

        InputCommand? command = null;
        // Vyprázdni buffer a ponech si poslední smysluplný povel v jednom kroku.
        while (Console.KeyAvailable)
        {
            ConsoleKey key = Console.ReadKey(intercept: true).Key;
            InputCommand? mapped = Map(key);
            if (mapped.HasValue)
            {
                command = mapped;
            }
        }
        return command;
    }

    private static InputCommand? Map(ConsoleKey key) => key switch
    {
        ConsoleKey.UpArrow => InputCommand.Turn(Direction.Up),
        ConsoleKey.DownArrow => InputCommand.Turn(Direction.Down),
        ConsoleKey.LeftArrow => InputCommand.Turn(Direction.Left),
        ConsoleKey.RightArrow => InputCommand.Turn(Direction.Right),
        ConsoleKey.Escape => InputCommand.Quit(),
        _ => null,
    };
}
