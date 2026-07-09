#pragma once

#include "snake/Point.h"

namespace snake {

/// Směr pohybu hada. Nahrazuje původní volný enum `eDirecton` s hodnotou STOP.
enum class Direction { Up, Down, Left, Right };

/// Posun o jednu buňku v daném směru. Osa Y roste směrem dolů (jako řádky konzole).
inline Point delta(Direction direction) {
    switch (direction) {
        case Direction::Up:    return Point{0, -1};
        case Direction::Down:  return Point{0, 1};
        case Direction::Left:  return Point{-1, 0};
        case Direction::Right: return Point{1, 0};
    }
    return Point{0, 0};
}

/// Jsou dva směry navzájem opačné? Slouží k zákazu otočení hada přímo do sebe.
inline bool areOpposite(Direction a, Direction b) {
    return (a == Direction::Up && b == Direction::Down) ||
           (a == Direction::Down && b == Direction::Up) ||
           (a == Direction::Left && b == Direction::Right) ||
           (a == Direction::Right && b == Direction::Left);
}

}  // namespace snake
