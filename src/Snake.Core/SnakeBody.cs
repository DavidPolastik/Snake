using System.Collections.Generic;

namespace Snake.Core;

/// <summary>
/// Tělo hada jako sekvence buněk (hlava vpředu, ocas vzadu).
///
/// Nahrazuje dvě paralelní pole <c>xposlijf</c>/<c>yposlijf</c> a ruční
/// <c>RemoveAt(0)</c> z původního kódu jednou zapouzdřenou strukturou.
/// </summary>
public sealed class SnakeBody
{
    private readonly LinkedList<Point> _segments;

    public SnakeBody(Point start)
    {
        _segments = new LinkedList<Point>();
        _segments.AddFirst(start);
    }

    public Point Head => _segments.First!.Value;
    public Point Tail => _segments.Last!.Value;
    public int Length => _segments.Count;
    public IEnumerable<Point> Segments => _segments;

    /// <summary>Kam by se dostala hlava po kroku v daném směru (bez změny stavu).</summary>
    public Point NextHead(Direction direction) => Head + direction.ToDelta();

    /// <summary>
    /// Posune hada na novou hlavu. Když <paramref name="grow"/> je false, uvolní
    /// poslední článek ocasu (délka zůstává); jinak had povyroste.
    /// </summary>
    public void MoveTo(Point newHead, bool grow)
    {
        _segments.AddFirst(newHead);
        if (!grow)
        {
            _segments.RemoveLast();
        }
    }

    /// <summary>Zabírá had danou buňku některým svým článkem?</summary>
    public bool Occupies(Point point)
    {
        foreach (Point segment in _segments)
        {
            if (segment == point)
            {
                return true;
            }
        }
        return false;
    }
}
