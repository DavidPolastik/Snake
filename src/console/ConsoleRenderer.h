#pragma once

#include "snake/IRenderer.h"
#include "snake/Point.h"

namespace snake {

class SnakeGame;


/// Vykreslení hry do textové konzole. Jediné místo v projektu, které umí
/// kreslit; jádro na něm nezávisí (komunikuje jen přes rozhraní IRenderer).
class ConsoleRenderer : public IRenderer {
public:
    void render(const SnakeGame& game) override;
    void renderGameOver(const SnakeGame& game) override;

private:
    void drawFrame(const SnakeGame& game) const;
    char glyphAt(const SnakeGame& game, const Point& cell) const;
};

}  // namespace snake
