#pragma once

#include <random>

#include "snake/IFoodPlacer.h"

namespace snake {

/// Náhodné umístění jídla na volnou buňku pomocí `std::mt19937`.
/// RNG je součástí této třídy (jádro `rand()` nikde nepoužívá), seed lze
/// předat pro reprodukovatelnost.
class RandomFoodPlacer : public IFoodPlacer {
public:
    explicit RandomFoodPlacer(unsigned int seed);

    Point place(const Board& board, const Snake& snake) override;

private:
    std::mt19937 rng_;
};

}  // namespace snake
