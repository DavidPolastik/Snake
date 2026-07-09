#include "RandomFoodPlacer.h"

#include "snake/Board.h"
#include "snake/Snake.h"

namespace snake {

RandomFoodPlacer::RandomFoodPlacer(unsigned int seed) : rng_(seed) {}

Point RandomFoodPlacer::place(const Board& board, const Snake& snake) {
    std::uniform_int_distribution<int> xDist(0, board.width() - 1);
    std::uniform_int_distribution<int> yDist(0, board.height() - 1);

    // Losuj, dokud netrefíš buňku mimo tělo hada. Pro rozumné velikosti plochy
    // je počet obsazených buněk malý oproti celku, takže je to rychlé.
    Point candidate;
    do {
        candidate = Point{xDist(rng_), yDist(rng_)};
    } while (snake.occupies(candidate));
    return candidate;
}

}  // namespace snake
