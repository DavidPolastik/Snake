#pragma once

#include "snake/Board.h"
#include "snake/Direction.h"
#include "snake/GameStatus.h"
#include "snake/IFoodPlacer.h"
#include "snake/Point.h"
#include "snake/Snake.h"

namespace snake {

/// Pravidla hry Snake bez jakéhokoli vstupu/výstupu.
///
/// Nahrazuje globální stav a funkce `Setup()`/`Logic()` z původního kódu.
/// Nezná konzoli ani klávesnici – stav pouze poskytuje přes gettery, které
/// si přečte renderer. Jeden krok simulace = `tick()`.
class SnakeGame {
public:
    static constexpr int kScorePerFood = 10;

    SnakeGame(const Board& board, IFoodPlacer& foodPlacer);

    /// Změní směr pohybu; požadavek na otočení přímo do sebe je ignorován.
    void setDirection(Direction direction);

    /// Provede jeden krok hry: posun hada, kolize, případné snězení jídla.
    void tick();

    GameStatus status() const { return status_; }
    bool isRunning() const { return status_ == GameStatus::Running; }
    int score() const { return score_; }

    const Board& board() const { return board_; }
    const Snake& snake() const { return snake_; }
    const Point& food() const { return food_; }

private:
    bool hitsWall(const Point& point) const;
    bool hitsItself(const Point& newHead) const;

    Board board_;
    IFoodPlacer& foodPlacer_;
    Snake snake_;
    Point food_;
    Direction direction_;
    GameStatus status_ = GameStatus::Running;
    int score_ = 0;
};

}  // namespace snake
