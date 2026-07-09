namespace Snake.Core;

/// <summary>
/// Obdélníková hrací plocha. Vnější prstenec buněk je zeď – jako v originále,
/// kde had umíral při dotyku okraje (x == 0, x == width-1, ...).
/// </summary>
public sealed class Board
{
    public Board(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public int Width { get; }
    public int Height { get; }

    /// <summary>Narazil by bod do zdi (na okraji nebo mimo plochu)?</summary>
    public bool HitsWall(Point point) =>
        point.X <= 0 || point.X >= Width - 1 ||
        point.Y <= 0 || point.Y >= Height - 1;

    public Point Center() => new Point(Width / 2, Height / 2);
}
