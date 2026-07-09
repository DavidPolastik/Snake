using System;
using Snake.Core;

namespace Snake.ConsoleApp;

/// <summary>
/// Vykreslení hry do textové konzole. Jediné místo, které umí kreslit; jádro
/// na něm nezávisí (komunikuje jen přes rozhraní <see cref="IRenderer"/>).
///
/// Zohledňuje připomínku z diskuze pod originálem: rám se kreslí jen jednou
/// a vnitřek se "pseudomaže" přepsáním, takže obraz nebliká.
/// </summary>
public sealed class ConsoleRenderer : IRenderer
{
    private const char Wall = '■';
    private const char BodyCell = '■';
    private const char HeadCell = '■';
    private const char FoodCell = '■';

    private bool _borderDrawn;

    public void Render(SnakeGame game)
    {
        if (!_borderDrawn)
        {
            DrawBorder(game.Board);
            _borderDrawn = true;
        }

        ClearInterior(game.Board);
        DrawSnake(game.Snake);
        DrawFood(game.Food);
        DrawScore(game);
    }

    public void RenderGameOver(SnakeGame game)
    {
        Board board = game.Board;
        Console.ResetColor();
        SetCursor(board.Width / 5, board.Height / 2);
        Console.Write($"Konec hry! Skore: {game.Score}");
        SetCursor(0, board.Height);
        SetCursorVisible(true);
    }

    private static void DrawBorder(Board board)
    {
        Console.ResetColor();
        SetCursorVisible(false);
        string horizontal = new string(Wall, board.Width);
        SetCursor(0, 0);
        Console.Write(horizontal);
        SetCursor(0, board.Height - 1);
        Console.Write(horizontal);
        for (int y = 0; y < board.Height; y++)
        {
            SetCursor(0, y);
            Console.Write(Wall);
            SetCursor(board.Width - 1, y);
            Console.Write(Wall);
        }
    }

    private static void ClearInterior(Board board)
    {
        string blank = new string(' ', board.Width - 2);
        for (int y = 1; y < board.Height - 1; y++)
        {
            SetCursor(1, y);
            Console.Write(blank);
        }
    }

    private static void DrawSnake(SnakeBody snake)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        foreach (Point segment in snake.Segments)
        {
            DrawCell(segment, BodyCell);
        }
        Console.ForegroundColor = ConsoleColor.Red;
        DrawCell(snake.Head, HeadCell);
    }

    private static void DrawFood(Point food)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        DrawCell(food, FoodCell);
    }

    private static void DrawScore(SnakeGame game)
    {
        Console.ResetColor();
        SetCursor(0, game.Board.Height);
        Console.Write($"Skore: {game.Score}   (sipky = pohyb, Esc = konec)");
    }

    private static void DrawCell(Point cell, char glyph)
    {
        SetCursor(cell.X, cell.Y);
        Console.Write(glyph);
    }

    private static void SetCursor(int x, int y)
    {
        if (!Console.IsOutputRedirected)
        {
            Console.SetCursorPosition(x, y);
        }
    }

    private static void SetCursorVisible(bool visible)
    {
        if (!Console.IsOutputRedirected)
        {
            try
            {
                Console.CursorVisible = visible;
            }
            catch (Exception)
            {
                // Neumí-li terminál viditelnost kurzoru, nevadí – přeskoč.
            }
        }
    }
}
