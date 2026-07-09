namespace Snake.Core;

/// <summary>Typ povelu od hráče.</summary>
public enum CommandType
{
    Turn,
    Quit,
}

/// <summary>Povel v jednom kroku hry (u <see cref="CommandType.Turn"/> nese směr).</summary>
public readonly struct InputCommand
{
    private InputCommand(CommandType type, Direction direction)
    {
        Type = type;
        Direction = direction;
    }

    public CommandType Type { get; }
    public Direction Direction { get; }

    public static InputCommand Turn(Direction direction) => new InputCommand(CommandType.Turn, direction);
    public static InputCommand Quit() => new InputCommand(CommandType.Quit, default);
}

/// <summary>
/// Rozhraní pro zdroj vstupu. Odděluje logiku od vstupního HW/API – jádro nezná
/// klávesnici. Neblokující: vrací <c>null</c>, když se nic nestisklo.
/// </summary>
public interface IInput
{
    InputCommand? Poll();
}
