using System;

namespace Snake.Core;

/// <summary>
/// Pravidla hry Snake – bez jakéhokoli vstupu/výstupu.
///
/// Nahrazuje obří <c>Main</c> z původního kódu. Nezná konzoli ani klávesnici;
/// stav pouze poskytuje přes vlastnosti, které si přečte renderer. Jeden krok
/// simulace = <see cref="Tick"/>.
/// </summary>
public sealed class SnakeGame
{
    public const int ScorePerFood = 10;
    public const int DefaultStartLength = 3;

    private readonly Board _board;
    private readonly IFoodPlacer _foodPlacer;
    private Direction _direction = Direction.Right;
    private int _pendingGrowth;

    public SnakeGame(Board board, IFoodPlacer foodPlacer, int startLength = DefaultStartLength)
    {
        if (startLength < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(startLength));
        }

        _board = board;
        _foodPlacer = foodPlacer;
        Snake = new SnakeBody(board.Center());
        _pendingGrowth = startLength - 1;  // dorůst na počáteční délku
        Food = foodPlacer.Place(board, Snake);
    }

    public SnakeBody Snake { get; }
    public Point Food { get; private set; }
    public GameStatus Status { get; private set; } = GameStatus.Running;
    public int Score { get; private set; }
    public Board Board => _board;
    public bool IsRunning => Status == GameStatus.Running;

    /// <summary>Změní směr; požadavek na otočení přímo do sebe je ignorován.</summary>
    public void SetDirection(Direction direction)
    {
        if (!direction.IsOppositeTo(_direction))
        {
            _direction = direction;
        }
    }

    /// <summary>Jeden krok hry: posun hada, kolize, případné snězení jídla.</summary>
    public void Tick()
    {
        if (Status != GameStatus.Running)
        {
            return;
        }

        Point newHead = Snake.NextHead(_direction);

        if (_board.HitsWall(newHead) || HitsItself(newHead))
        {
            Status = GameStatus.GameOver;
            return;
        }

        bool eating = newHead == Food;
        bool grow = eating || _pendingGrowth > 0;
        if (!eating && _pendingGrowth > 0)
        {
            _pendingGrowth--;
        }

        Snake.MoveTo(newHead, grow);

        if (eating)
        {
            Score += ScorePerFood;
            Food = _foodPlacer.Place(_board, Snake);
        }
    }

    private bool HitsItself(Point newHead)
    {
        // Ocasní článek se během kroku uvolní, takže pohyb "za vlastní ocas" je
        // povolený – kontrolujeme jen články, které po kroku zůstanou.
        bool tailWillMove = _pendingGrowth == 0 && newHead != Food;
        if (tailWillMove && newHead == Snake.Tail)
        {
            return false;
        }
        return Snake.Occupies(newHead);
    }
}
