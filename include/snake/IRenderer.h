#pragma once

namespace snake {

class SnakeGame;

/// Rozhraní pro vykreslení stavu hry. Odděluje logiku od GUI –
/// jádro o žádné konkrétní implementaci neví. Konzole je jen jeden adaptér.
class IRenderer {
public:
    virtual ~IRenderer() = default;

    virtual void render(const SnakeGame& game) = 0;
    virtual void renderGameOver(const SnakeGame& game) = 0;
};

}  // namespace snake
