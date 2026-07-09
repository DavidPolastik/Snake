namespace Snake.Core;

/// <summary>Souřadnice buňky na hrací ploše. Ryze hodnotový typ.</summary>
public readonly struct Point
{
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }

    public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);

    public bool Equals(Point other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is Point other && Equals(other);
    public override int GetHashCode() => (X * 397) ^ Y;

    public static bool operator ==(Point a, Point b) => a.Equals(b);
    public static bool operator !=(Point a, Point b) => !a.Equals(b);

    public override string ToString() => $"({X}, {Y})";
}
