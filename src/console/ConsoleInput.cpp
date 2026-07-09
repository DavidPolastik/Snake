#include "console/ConsoleInput.h"

#include <cctype>

#if defined(_WIN32)
#include <conio.h>
namespace {
bool keyPressed() { return _kbhit() != 0; }
int readKey() { return _getch(); }
void enableRawMode() {}
void disableRawMode() {}
}  // namespace
#else
#include <termios.h>
#include <unistd.h>
#include <fcntl.h>
namespace {
termios g_originalTermios;

void enableRawMode() {
    tcgetattr(STDIN_FILENO, &g_originalTermios);
    termios raw = g_originalTermios;
    raw.c_lflag &= ~(ICANON | ECHO);  // vypni řádkový buffer a echo
    tcsetattr(STDIN_FILENO, TCSANOW, &raw);
    fcntl(STDIN_FILENO, F_SETFL, fcntl(STDIN_FILENO, F_GETFL) | O_NONBLOCK);
}

void disableRawMode() {
    tcsetattr(STDIN_FILENO, TCSANOW, &g_originalTermios);
}

bool keyPressed() {
    unsigned char c;
    if (read(STDIN_FILENO, &c, 1) == 1) {
        ungetc(c, stdin);
        return true;
    }
    return false;
}

int readKey() { return getchar(); }
}  // namespace
#endif

namespace snake {

ConsoleInput::ConsoleInput() { enableRawMode(); }

ConsoleInput::~ConsoleInput() { disableRawMode(); }

std::optional<InputEvent> ConsoleInput::mapKey(int key) const {
    switch (std::tolower(key)) {
        case 'w': return InputEvent{Command::Turn, Direction::Up};
        case 's': return InputEvent{Command::Turn, Direction::Down};
        case 'a': return InputEvent{Command::Turn, Direction::Left};
        case 'd': return InputEvent{Command::Turn, Direction::Right};
        case 'x': return InputEvent{Command::Quit};
        default:  return std::nullopt;
    }
}

std::optional<InputEvent> ConsoleInput::poll() {
    std::optional<InputEvent> event;
    // Vyprázdni buffer a ponech si poslední smysluplný povel v jednom kroku.
    while (keyPressed()) {
        if (auto mapped = mapKey(readKey())) {
            event = mapped;
        }
    }
    return event;
}

}  // namespace snake
