#include "console/ConsoleRenderer.h"

#include <iostream>
#include <string>

#include "snake/Board.h"
#include "snake/Point.h"
#include "snake/SnakeGame.h"

namespace snake {
namespace {

constexpr char kWall = '#';
constexpr char kHead = 'O';
constexpr char kBody = 'o';
constexpr char kFood = 'F';
constexpr char kEmpty = ' ';

/// Přesun kurzoru do levého horního rohu ANSI sekvencí. Na rozdíl od původního
/// `system("cls")` nemaže obrazovku (nebliká) a je přenositelné.
void moveCursorHome() { std::cout << "\x1b[H"; }

void clearScreen() { std::cout << "\x1b[2J\x1b[H"; }

}  // namespace

char ConsoleRenderer::glyphAt(const SnakeGame& game, const Point& cell) const {
    if (cell == game.snake().head()) {
        return kHead;
    }
    if (cell == game.food()) {
        return kFood;
    }
    if (game.snake().occupies(cell)) {
        return kBody;
    }
    return kEmpty;
}

void ConsoleRenderer::drawFrame(const SnakeGame& game) const {
    const Board& board = game.board();
    const std::string horizontalWall(board.width() + 2, kWall);

    std::string frame;
    frame.reserve(horizontalWall.size() * (board.height() + 2));

    frame += horizontalWall + '\n';
    for (int y = 0; y < board.height(); ++y) {
        frame += kWall;
        for (int x = 0; x < board.width(); ++x) {
            frame += glyphAt(game, Point{x, y});
        }
        frame += kWall;
        frame += '\n';
    }
    frame += horizontalWall + '\n';

    std::cout << frame;
    std::cout << "Skore: " << game.score() << "    (WASD = pohyb, X = konec)\n";
}

void ConsoleRenderer::render(const SnakeGame& game) {
    moveCursorHome();
    drawFrame(game);
    std::cout.flush();
}

void ConsoleRenderer::renderGameOver(const SnakeGame& game) {
    clearScreen();
    drawFrame(game);
    std::cout << "\n=== KONEC HRY ===  Finalni skore: " << game.score() << "\n";
    std::cout.flush();
}

}  // namespace snake
