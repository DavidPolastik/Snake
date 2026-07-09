using System;

namespace Snake.Core;

/// <summary>
/// Náhodné umístění jídla na volnou buňku uvnitř zdí. RNG je vlastní (žádný
/// globální stav), seed lze předat pro reprodukovatelnost.
/// </summary>
public sealed class RandomFoodPlacer : IFoodPlacer
{
    private readonly Random _random;

    public RandomFoodPlacer(int seed) => _random = new Random(seed);

    public RandomFoodPlacer() => _random = new Random();

    public Point Place(Board board, SnakeBody snake)
    {
        // Hrací pole je vnitřek 1 .. rozměr-2 (vnější prstenec je zeď).
        Point candidate;
        do
        {
            int x = _random.Next(1, board.Width - 1);
            int y = _random.Next(1, board.Height - 1);
            candidate = new Point(x, y);
        }
        while (snake.Occupies(candidate));
        return candidate;
    }
}
