#pragma once

#include "snake/Point.h"

namespace snake {

/// Obdélníková hrací plocha. Zná jen své rozměry a to, co je uvnitř.
class Board {
public:
    Board(int width, int height) : width_(width), height_(height) {}

    int width() const { return width_; }
    int height() const { return height_; }

    /// Leží bod uvnitř plochy (tj. nenarazil had do zdi)?
    bool contains(const Point& point) const {
        return point.x >= 0 && point.x < width_ &&
               point.y >= 0 && point.y < height_;
    }

    Point center() const { return Point{width_ / 2, height_ / 2}; }

private:
    int width_;
    int height_;
};

}  // namespace snake
