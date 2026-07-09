using Snake.Core;

namespace Snake.Core.Tests;

/// <summary>
/// Jednotkové testy herního jádra. Projekt referencuje POUZE Snake.Core (žádnou
/// konzoli) – to prakticky dokazuje, že logika je oddělená od GUI a samostatně
/// testovatelná.
/// </summary>
internal static class Program
{
    // Střed plochy 20x20 = (10,10); výchozí směr = doprava; startLength=1.
    private const int W = 20;
    private const int H = 20;
    private static readonly Point Start = new Point(W / 2, H / 2);

    private static int Main()
    {
        var runner = new TestRunner();

        runner.Run(nameof(MovesInChosenDirection), () => MovesInChosenDirection(runner));
        runner.Run(nameof(OppositeDirectionIsIgnored), () => OppositeDirectionIsIgnored(runner));
        runner.Run(nameof(MultipleTurnsPerTickCannotReverse), () => MultipleTurnsPerTickCannotReverse(runner));
        runner.Run(nameof(EatingFoodGrowsAndScores), () => EatingFoodGrowsAndScores(runner));
        runner.Run(nameof(HittingWallEndsGame), () => HittingWallEndsGame(runner));
        runner.Run(nameof(HittingSelfEndsGame), () => HittingSelfEndsGame(runner));

        return runner.Summarize();
    }

    private static SnakeGame NewGame(IFoodPlacer food) =>
        new SnakeGame(new Board(W, H), food, startLength: 1);

    private static void MovesInChosenDirection(TestRunner r)
    {
        var game = NewGame(new ScriptedFoodPlacer(new Point(1, 1)));  // jídlo daleko

        r.Check(game.Snake.Head == Start, "start ve stredu");
        game.Tick();  // vychozi smer doprava
        r.Check(game.Snake.Head == new Point(Start.X + 1, Start.Y), "posun doprava");

        game.SetDirection(Direction.Up);
        game.Tick();
        r.Check(game.Snake.Head == new Point(Start.X + 1, Start.Y - 1), "posun nahoru");
    }

    private static void OppositeDirectionIsIgnored(TestRunner r)
    {
        var game = NewGame(new ScriptedFoodPlacer(new Point(1, 1)));

        game.SetDirection(Direction.Left);  // opak doprava -> ignorovat
        game.Tick();
        r.Check(game.Snake.Head == new Point(Start.X + 1, Start.Y), "opacny smer ignorovan");
    }

    private static void MultipleTurnsPerTickCannotReverse(TestRunner r)
    {
        // Had délky 2 mířící doprava; hlava (11,10), krk (10,10).
        var game = new SnakeGame(new Board(W, H), new ScriptedFoodPlacer(new Point(1, 1)), startLength: 2);
        game.Tick();  // dorostl na délku 2, míří doprava
        r.Check(game.Snake.Length == 2, "delka 2");

        // Během jednoho tiku: nahoru a hned doleva. Otočka o 180° (doleva na krk)
        // musí být odmítnuta, jinak by had vjel sám do sebe.
        game.SetDirection(Direction.Up);
        game.SetDirection(Direction.Left);
        game.Tick();

        r.Check(game.IsRunning, "vice otacek za tik neobrati hada do sebe");
    }

    private static void EatingFoodGrowsAndScores(TestRunner r)
    {
        var ahead = new Point(Start.X + 1, Start.Y);
        var relocated = new Point(5, 5);
        var game = NewGame(new ScriptedFoodPlacer(ahead, relocated));

        r.Check(game.Food == ahead, "jidlo pred hlavou");
        r.Check(game.Snake.Length == 1, "delka 1 na startu");
        r.Check(game.Score == 0, "skore 0 na startu");

        game.Tick();  // snez jidlo primo pred sebou

        r.Check(game.Snake.Length == 2, "had povyrostl");
        r.Check(game.Score == SnakeGame.ScorePerFood, "skore +10");
        r.Check(game.Food == relocated, "jidlo premisteno");
        r.Check(game.IsRunning, "hra bezi dal");
    }

    private static void HittingWallEndsGame(TestRunner r)
    {
        var game = NewGame(new ScriptedFoodPlacer(new Point(1, 1)));

        for (int i = 0; i < W + 1 && game.IsRunning; i++)
        {
            game.Tick();  // porad doprava, az narazi do prave zdi
        }
        r.Check(game.Status == GameStatus.GameOver, "naraz do zdi konci hru");
    }

    private static void HittingSelfEndsGame(TestRunner r)
    {
        // Nakrm hada na delku 5 v prime rade, pak ho zatoc do vlastniho tela.
        var game = NewGame(new ScriptedFoodPlacer(
            new Point(Start.X + 1, Start.Y),
            new Point(Start.X + 2, Start.Y),
            new Point(Start.X + 3, Start.Y),
            new Point(Start.X + 4, Start.Y),
            new Point(1, 1)));

        for (int i = 0; i < 4; i++)
        {
            game.Tick();  // snez 4 jidla -> delka 5
        }
        r.Check(game.Snake.Length == 5, "delka 5 po jezeni");
        r.Check(game.IsRunning, "hra zatim bezi");

        game.SetDirection(Direction.Up);
        game.Tick();
        game.SetDirection(Direction.Left);
        game.Tick();
        game.SetDirection(Direction.Down);
        game.Tick();  // vjede do vlastniho tela

        r.Check(game.Status == GameStatus.GameOver, "naraz do sebe konci hru");
    }
}
