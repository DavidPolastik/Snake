#include "snake/SnakeGame.h"

namespace snake {

SnakeGame::SnakeGame(const Board& board, IFoodPlacer& foodPlacer)
    : board_(board),
      foodPlacer_(foodPlacer),
      snake_(board.center()),
      direction_(Direction::Right) {
    food_ = foodPlacer_.place(board_, snake_);
}

void SnakeGame::setDirection(Direction direction) {
    if (!areOpposite(direction, direction_)) {
        direction_ = direction;
    }
}

void SnakeGame::tick() {
    if (status_ != GameStatus::Running) {
        return;
    }

    const Point newHead = snake_.nextHead(direction_);

    if (hitsWall(newHead) || hitsItself(newHead)) {
        status_ = GameStatus::GameOver;
        return;
    }

    const bool eating = (newHead == food_);
    snake_.moveTo(newHead, eating);
    if (eating) {
        score_ += kScorePerFood;
        food_ = foodPlacer_.place(board_, snake_);
    }
}

bool SnakeGame::hitsWall(const Point& point) const {
    return !board_.contains(point);
}

bool SnakeGame::hitsItself(const Point& newHead) const {
    // Ocasní článek se během kroku uvolní, takže pohyb "za vlastní ocas" je
    // povolený. Kontrolujeme kolizi jen s články, které po kroku zůstanou.
    const Point tail = snake_.body().back();
    return newHead != tail && snake_.occupies(newHead);
}

}  // namespace snake
