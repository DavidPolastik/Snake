namespace Snake.Core;

/// <summary>
/// Rozhraní pro vykreslení stavu hry. Odděluje logiku od GUI – jádro o žádné
/// konkrétní implementaci neví. Konzole je jen jeden adaptér.
/// </summary>
public interface IRenderer
{
    void Render(SnakeGame game);
    void RenderGameOver(SnakeGame game);
}
