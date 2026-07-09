// Jednotkové testy herního jádra. Linkují POUZE knihovnu snake_core (žádnou
// konzoli), což prakticky dokazuje, že logika je oddělená od GUI a je
// samostatně testovatelná.
//
// Bez externího frameworku – stačí malé makro CHECK nad <iostream>/<cstdlib>.

#include <cstdlib>
#include <iostream>
#include <vector>

#include "snake/Board.h"
#include "snake/Direction.h"
#include "snake/IFoodPlacer.h"
#include "snake/Point.h"
#include "snake/Snake.h"
#include "snake/SnakeGame.h"

namespace {

int g_failures = 0;

void check(bool condition, const char* expr, int line) {
    if (!condition) {
        std::cerr << "  FAIL (radek " << line << "): " << expr << "\n";
        ++g_failures;
    }
}

#define CHECK(cond) check((cond), #cond, __LINE__)

/// Deterministický "placer" jídla pro testy: vrací předem dané body v pořadí,
/// po vyčerpání pak vzdálený roh, který had běžně nepotká.
class ScriptedFoodPlacer : public snake::IFoodPlacer {
public:
    explicit ScriptedFoodPlacer(std::vector<snake::Point> positions)
        : positions_(std::move(positions)) {}

    snake::Point place(const snake::Board&, const snake::Snake&) override {
        if (index_ < positions_.size()) {
            return positions_[index_++];
        }
        return snake::Point{0, 0};
    }

private:
    std::vector<snake::Point> positions_;
    std::size_t index_ = 0;
};

using snake::Board;
using snake::Direction;
using snake::GameStatus;
using snake::Point;
using snake::SnakeGame;

// Startovní hlava = střed plochy, výchozí směr = doprava.
constexpr int kW = 20;
constexpr int kH = 20;
const Point kStart{kW / 2, kH / 2};  // {10, 10}

void test_moves_in_chosen_direction() {
    std::cout << "test_moves_in_chosen_direction\n";
    ScriptedFoodPlacer food({Point{0, 0}});  // jídlo daleko
    SnakeGame game(Board(kW, kH), food);

    CHECK(game.snake().head() == kStart);
    game.tick();  // výchozí směr doprava
    CHECK(game.snake().head() == (Point{kStart.x + 1, kStart.y}));

    game.setDirection(Direction::Up);
    game.tick();
    CHECK(game.snake().head() == (Point{kStart.x + 1, kStart.y - 1}));
}

void test_opposite_direction_is_ignored() {
    std::cout << "test_opposite_direction_is_ignored\n";
    ScriptedFoodPlacer food({Point{0, 0}});
    SnakeGame game(Board(kW, kH), food);

    game.setDirection(Direction::Left);  // opak doprava -> ignorovat
    game.tick();
    CHECK(game.snake().head() == (Point{kStart.x + 1, kStart.y}));
}

void test_eating_food_grows_and_scores() {
    std::cout << "test_eating_food_grows_and_scores\n";
    const Point ahead{kStart.x + 1, kStart.y};
    const Point relocated{3, 3};
    ScriptedFoodPlacer food({ahead, relocated});
    SnakeGame game(Board(kW, kH), food);

    CHECK(game.food() == ahead);
    CHECK(game.snake().length() == 1);
    CHECK(game.score() == 0);

    game.tick();  // sní jídlo přímo před sebou

    CHECK(game.snake().length() == 2);
    CHECK(game.score() == SnakeGame::kScorePerFood);
    CHECK(game.food() == relocated);
    CHECK(game.isRunning());
}

void test_hitting_wall_ends_game() {
    std::cout << "test_hitting_wall_ends_game\n";
    ScriptedFoodPlacer food({Point{0, 0}});
    SnakeGame game(Board(kW, kH), food);

    for (int i = 0; i < kW + 1 && game.isRunning(); ++i) {
        game.tick();  // pořád doprava, až narazí do pravé zdi
    }
    CHECK(game.status() == GameStatus::GameOver);
}

void test_hitting_self_ends_game() {
    std::cout << "test_hitting_self_ends_game\n";
    // Nakrm hada na délku 5 v přímé řadě, pak ho zatoč do vlastního těla.
    ScriptedFoodPlacer food({
        Point{kStart.x + 1, kStart.y},
        Point{kStart.x + 2, kStart.y},
        Point{kStart.x + 3, kStart.y},
        Point{kStart.x + 4, kStart.y},
        Point{0, 0},  // po dorůstu daleko
    });
    SnakeGame game(Board(kW, kH), food);

    for (int i = 0; i < 4; ++i) {
        game.tick();  // sní 4 jídla -> délka 5
    }
    CHECK(game.snake().length() == 5);
    CHECK(game.isRunning());

    game.setDirection(Direction::Up);
    game.tick();
    game.setDirection(Direction::Left);
    game.tick();
    game.setDirection(Direction::Down);
    game.tick();  // vjede do vlastního těla

    CHECK(game.status() == GameStatus::GameOver);
}

}  // namespace

int main() {
    test_moves_in_chosen_direction();
    test_opposite_direction_is_ignored();
    test_eating_food_grows_and_scores();
    test_hitting_wall_ends_game();
    test_hitting_self_ends_game();

    if (g_failures == 0) {
        std::cout << "\nVsechny testy proshly.\n";
        return EXIT_SUCCESS;
    }
    std::cerr << "\nSelhalo kontrol: " << g_failures << "\n";
    return EXIT_FAILURE;
}
