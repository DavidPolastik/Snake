#pragma once

#include "snake/Point.h"

namespace snake {

class Board;
class Snake;

/// Strategie umístění nového jídla. Produkčně náhodná, v testech skriptovaná –
/// díky tomu je jádro deterministicky testovatelné bez závislosti na `rand()`.
class IFoodPlacer {
public:
    virtual ~IFoodPlacer() = default;

    /// Vrátí volnou buňku pro jídlo (mimo tělo hada, uvnitř plochy).
    virtual Point place(const Board& board, const Snake& snake) = 0;
};

}  // namespace snake
