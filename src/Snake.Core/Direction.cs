namespace Snake.Core;

/// <summary>Směr pohybu hada. Nahrazuje řízení přes stringy "UP"/"DOWN"...</summary>
public enum Direction
{
    Up,
    Down,
    Left,
    Right,
}

/// <summary>Pomocné operace nad směrem – drží znalost geometrie pohromadě.</summary>
public static class DirectionExtensions
{
    /// <summary>Posun o jednu buňku. Osa Y roste dolů (jako řádky konzole).</summary>
    public static Point ToDelta(this Direction direction) => direction switch
    {
        Direction.Up => new Point(0, -1),
        Direction.Down => new Point(0, 1),
        Direction.Left => new Point(-1, 0),
        Direction.Right => new Point(1, 0),
        _ => new Point(0, 0),
    };

    /// <summary>Jsou dva směry navzájem opačné? (Zákaz otočení přímo do sebe.)</summary>
    public static bool IsOppositeTo(this Direction a, Direction b) => (a, b) switch
    {
        (Direction.Up, Direction.Down) => true,
        (Direction.Down, Direction.Up) => true,
        (Direction.Left, Direction.Right) => true,
        (Direction.Right, Direction.Left) => true,
        _ => false,
    };
}
