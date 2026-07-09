#pragma once

namespace snake {

/// Souřadnice buňky na hrací ploše. Ryze hodnotový typ bez chování GUI.
struct Point {
    int x = 0;
    int y = 0;

    friend bool operator==(const Point& a, const Point& b) {
        return a.x == b.x && a.y == b.y;
    }
    friend bool operator!=(const Point& a, const Point& b) {
        return !(a == b);
    }
    friend Point operator+(const Point& a, const Point& b) {
        return Point{a.x + b.x, a.y + b.y};
    }
};

}  // namespace snake
