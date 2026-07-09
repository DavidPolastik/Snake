namespace Snake.Core;

/// <summary>
/// Strategie umístění jídla. Produkčně náhodná, v testech skriptovaná – díky
/// tomu je jádro deterministicky testovatelné bez závislosti na globálním RNG.
/// </summary>
public interface IFoodPlacer
{
    /// <summary>Vrátí volnou buňku pro jídlo (uvnitř zdí, mimo tělo hada).</summary>
    Point Place(Board board, SnakeBody snake);
}
