#pragma once

#include <deque>

#include "snake/Direction.h"
#include "snake/Point.h"

namespace snake {

/// Tělo hada jako sekvence buněk (hlava vpředu, ocas vzadu).
///
/// Zapouzdřuje posun těla, který byl v původním kódu roztroušen v ruční
/// smyčce s pomocnými proměnnými `prev`/`prev2` uprostřed funkce `Logic()`.
class Snake {
public:
    explicit Snake(const Point& start) : body_{start} {}

    const Point& head() const { return body_.front(); }
    std::size_t length() const { return body_.size(); }
    const std::deque<Point>& body() const { return body_; }

    /// Kam by se přesunula hlava po kroku v daném směru (bez změny stavu).
    Point nextHead(Direction direction) const { return head() + delta(direction); }

    /// Posune hada o jednu buňku. Při `grow == false` uvolní poslední článek
    /// ocasu, takže délka zůstane stejná; při `grow == true` had povyroste.
    void moveTo(const Point& newHead, bool grow) {
        body_.push_front(newHead);
        if (!grow) {
            body_.pop_back();
        }
    }

    /// Zabírá had danou buňku některým svým článkem?
    bool occupies(const Point& point) const {
        for (const Point& segment : body_) {
            if (segment == point) {
                return true;
            }
        }
        return false;
    }

private:
    std::deque<Point> body_;
};

}  // namespace snake
