// Kompoziční kořen aplikace: složí čisté jádro s konzolovými adaptéry
// a otočí herní smyčku. Sám neobsahuje žádná pravidla hry.

#include <chrono>
#include <thread>

#include "console/ConsoleInput.h"
#include "console/ConsoleRenderer.h"
#include "RandomFoodPlacer.h"

#include "snake/Board.h"
#include "snake/IInput.h"
#include "snake/IRenderer.h"
#include "snake/SnakeGame.h"

namespace {

constexpr int kBoardWidth = 30;
constexpr int kBoardHeight = 18;
constexpr auto kTickInterval = std::chrono::milliseconds(120);
constexpr unsigned int kSeed = 12345u;

/// Zpracuje vstup hráče do jednoho kroku. Vrací false, pokud si přeje skončit.
bool applyInput(snake::IInput& input, snake::SnakeGame& game) {
    if (auto event = input.poll()) {
        if (event->command == snake::Command::Quit) {
            return false;
        }
        game.setDirection(event->direction);
    }
    return true;
}

}  // namespace

int main() {
    snake::Board board(kBoardWidth, kBoardHeight);
    snake::RandomFoodPlacer foodPlacer(kSeed);
    snake::SnakeGame game(board, foodPlacer);

    snake::ConsoleRenderer renderer;
    snake::ConsoleInput input;

    while (game.isRunning()) {
        if (!applyInput(input, game)) {
            break;
        }
        game.tick();
        renderer.render(game);
        std::this_thread::sleep_for(kTickInterval);
    }

    renderer.renderGameOver(game);
    return 0;
}
