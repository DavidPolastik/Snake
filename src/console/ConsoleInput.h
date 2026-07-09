#pragma once

#include "snake/IInput.h"

namespace snake {

/// Neblokující čtení kláves z terminálu. Veškerý platform-specifický kód
/// (Windows conio vs. POSIX termios) je uzavřený zde – jádro o něm neví.
///
/// Ovládání: W/A/S/D = směr, X = ukončení.
class ConsoleInput : public IInput {
public:
    ConsoleInput();
    ~ConsoleInput() override;

    std::optional<InputEvent> poll() override;

private:
    std::optional<InputEvent> mapKey(int key) const;
};

}  // namespace snake
