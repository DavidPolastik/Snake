#pragma once

#include <optional>

#include "snake/Direction.h"

namespace snake {

/// Povel od hráče v jednom kroku hry.
enum class Command { Turn, Quit };

/// Vyžádaný povel spolu s případným směrem (u `Command::Turn`).
struct InputEvent {
    Command command;
    Direction direction = Direction::Up;
};

/// Rozhraní pro zdroj vstupu. Odděluje logiku od vstupního HW/API –
/// jádro nezná klávesnici. Neblokující: vrací prázdno, když se nic nestisklo.
class IInput {
public:
    virtual ~IInput() = default;

    virtual std::optional<InputEvent> poll() = 0;
};

}  // namespace snake
