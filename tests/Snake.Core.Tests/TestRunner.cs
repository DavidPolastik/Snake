using System;
using System.Collections.Generic;

namespace Snake.Core.Tests;

/// <summary>
/// Minimální běhový runner testů bez externí závislosti (žádný NuGet, funguje
/// offline). Každý test je akce; selhání se sbírají a promítnou do exit kódu.
/// </summary>
public sealed class TestRunner
{
    private int _failures;

    public void Check(bool condition, string message)
    {
        if (!condition)
        {
            _failures++;
            Console.WriteLine($"  FAIL: {message}");
        }
    }

    public void Run(string name, Action test)
    {
        Console.WriteLine(name);
        int before = _failures;
        try
        {
            test();
        }
        catch (Exception ex)
        {
            _failures++;
            Console.WriteLine($"  VYJIMKA: {ex.GetType().Name}: {ex.Message}");
        }
        if (_failures == before)
        {
            Console.WriteLine("  OK");
        }
    }

    public int Summarize()
    {
        Console.WriteLine();
        if (_failures == 0)
        {
            Console.WriteLine("Vsechny testy proshly.");
            return 0;
        }
        Console.WriteLine($"Selhalo kontrol: {_failures}");
        return 1;
    }
}

/// <summary>
/// Deterministický "placer" jídla pro testy: vrací předem dané body v pořadí,
/// po vyčerpání pak roh, který had běžně nepotká.
/// </summary>
public sealed class ScriptedFoodPlacer : IFoodPlacer
{
    private readonly Queue<Point> _positions;

    public ScriptedFoodPlacer(params Point[] positions) => _positions = new Queue<Point>(positions);

    public Point Place(Board board, SnakeBody snake) =>
        _positions.Count > 0 ? _positions.Dequeue() : new Point(1, 1);
}
