using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Snake.Core;

namespace Snake.ConsoleApp;

/// <summary>
/// Kompoziční kořen aplikace: složí čisté jádro s konzolovými adaptéry a otočí
/// herní smyčku. Sám neobsahuje žádná pravidla hry.
/// </summary>
internal static class Program
{
    private const int BoardWidth = 32;
    private const int BoardHeight = 16;
    private const int TickIntervalMs = 120;
    private const int PollIntervalMs = 10;
    private const int Seed = 12345;

    private static int Main()
    {
        var board = new Board(BoardWidth, BoardHeight);
        var game = new SnakeGame(board, new RandomFoodPlacer(Seed));

        IRenderer renderer = new ConsoleRenderer();
        IInput input = new ConsoleInput();

        TryConfigureWindow(board);
        renderer.Render(game);

        while (game.IsRunning)
        {
            if (PlayerWantsToQuit(input, game))
            {
                break;
            }
            game.Tick();
            renderer.Render(game);
        }

        renderer.RenderGameOver(game);
        return 0;
    }

    /// <summary>
    /// Průběžně čte vstup po dobu jednoho tiku (aby ovládání reagovalo hned)
    /// a nastavuje směr. Vrací true, když hráč požádal o konec.
    /// </summary>
    private static bool PlayerWantsToQuit(IInput input, SnakeGame game)
    {
        var elapsed = Stopwatch.StartNew();
        do
        {
            InputCommand? command = input.Poll();
            if (command.HasValue)
            {
                if (command.Value.Type == CommandType.Quit)
                {
                    return true;
                }
                game.SetDirection(command.Value.Direction);
            }
            Thread.Sleep(PollIntervalMs);
        }
        while (elapsed.ElapsedMilliseconds < TickIntervalMs);
        return false;
    }

    /// <summary>Nastaví velikost okna dle plochy – jen best-effort (Windows).</summary>
    private static void TryConfigureWindow(Board board)
    {
        try
        {
            if (!Console.IsOutputRedirected && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WindowHeight = board.Height + 2;  // +řádek pro skóre
                Console.WindowWidth = board.Width + 1;
            }
        }
        catch (Exception)
        {
            // Nepodstatné pro běh hry – přeskoč, když prostředí okno neumí.
        }
    }
}
