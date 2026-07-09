# Snake – refaktoring podle zásad čistého kódu (C#)

Refaktoring známé konzolové hry **Snake** napsané v C#. Cílem úlohy nebylo hru
vymyslet, ale vzít **legacy kód** a přepsat ho podle pravidel *čistého kódu*
(Clean Code) – se zvláštním důrazem na **oddělení herní logiky od GUI**.

- Původní kód: <https://codereview.stackexchange.com/questions/127515/first-c-program-snake-game>
  (autor *Wagacca*) — ponechán beze změny v [`legacy/Original.cs`](legacy/Original.cs) pro srovnání „před vs. po".
- Jazyk: **C# / .NET 8**, řešení pro Visual Studio (`Snake.sln`).
- Jde o **samostatnou práci** (jednotlivec).

## Co bylo na původním kódu špatně

Původní verze byla jedna metoda `Main`, kde bylo všechno propletené dohromady:

- **Vše v jednom `Main`** – deklarace, kreslení rámu, herní logika, čtení kláves
  i výpis skóre v jedné dlouhé metodě s vnořenými smyčkami.
- **Míchání kompetencí a úrovní abstrakce** – `Console.Clear()`, kreslení,
  pohyb hada, detekce kolizí a vstup přímo vedle sebe.
- **Řízení přes „stringly-typed" hodnoty** – směr jako `"UP"/"DOWN"/...`,
  `gameover` jako `int` (0/1), `buttonpressed` jako `"yes"/"no"`.
- **Dvě paralelní pole** `xposlijf`/`yposlijf` pro tělo hada + ruční
  `RemoveAt(0)`.
- **Nejasná jména** (navíc nizozemská): `hoofd`, `schermkleur`, `randomnummer`,
  `tijd`, `berryx`…
- **Netestovatelnost** – logiku nešlo spustit bez konzole a klávesnice.
- **Blikání** – `Console.Clear()` každý snímek (zmíněno i v diskuzi pod postem).

## Jak vypadá refaktorovaná verze

Kód je rozdělený do tří projektů, jejichž oddělení je **fyzicky vynucené**
(jádro nereferencuje nic z konzole):

```
Snake.sln
legacy/Original.cs               původní kód (pro srovnání)
src/
  Snake.Core/                    ČISTÉ JÁDRO – žádné using System.Console
    Point, Direction, Board      value/logic typy
    SnakeBody, GameStatus
    SnakeGame                    pravidla hry (Tick, Score, Status)
    IFoodPlacer, RandomFoodPlacer
    IRenderer, IInput            rozhraní (porty – inverze závislostí)
  Snake.ConsoleApp/              GUI adaptér + kompoziční kořen
    ConsoleRenderer              vykreslení do konzole
    ConsoleInput                 čtení kláves
    Program                      složení jádra + adaptérů, herní smyčka
tests/
  Snake.Core.Tests/             testy jádra (referencují JEN Snake.Core)
```

### Oddělení logiky od GUI (jádro úlohy)

- Třída `SnakeGame` obsahuje **jen pravidla** – neví o konzoli ani klávesnici.
  Krok hry je `Tick()`, stav si okolí přečte přes vlastnosti (`Snake`, `Food`,
  `Score`, `Status`).
- Vykreslování a vstup jsou schované za rozhraními **`IRenderer`** a
  **`IInput`**, která definuje jádro. `Program` do hry vstříkne konkrétní
  konzolové adaptéry (*dependency injection*). Vyměnit konzoli za jiné GUI
  (WinForms, WPF, web…) = napsat nový adaptér, jádra se to nedotkne.
- Umístění jídla je za rozhraním **`IFoodPlacer`**: produkčně náhodné
  (`RandomFoodPlacer` s vlastním `Random`, žádný sdílený globální stav),
  v testech deterministické (`ScriptedFoodPlacer`).
- Že `Snake.Core.Tests` referencuje **pouze** `Snake.Core`, je praktickým
  důkazem, že logika je testovatelná bez GUI.

### Další úklid v duchu Clean Code

| Původně | Nově |
|---|---|
| `string movement = "RIGHT"` | `enum Direction` + `ToDelta()`, `IsOppositeTo()` |
| `int gameover = 0/1` | `enum GameStatus` |
| `xposlijf` + `yposlijf` + `RemoveAt(0)` | `SnakeBody` nad `LinkedList<Point>` |
| nizozemská/nejasná jména | smysluplná anglická jména |
| jeden obří `Main` | malé metody a třídy s jednou odpovědností |
| `Console.Clear()` každý snímek | rám se kreslí jednou, vnitřek se přepisuje (bez blikání) |
| chybějící pravidlo | zákaz otočení přímo do sebe (`IsOppositeTo`) |

## Sestavení a spuštění

Potřebuješ **.NET SDK 8** (<https://dotnet.microsoft.com/download>).

```bash
dotnet build Snake.sln -c Release
dotnet run --project src/Snake.ConsoleApp -c Release
```

Nebo otevři `Snake.sln` ve Visual Studiu 2022 a stiskni **F5**.

**Ovládání:** šipky = pohyb, **Esc** = konec. Had roste snězením jídla, konec
při nárazu do zdi nebo do sebe.

## Testy

Testy jádra jsou samostatný konzolový běh (bez externí závislosti, funguje
offline) – vrací nenulový kód při selhání:

```bash
dotnet run --project tests/Snake.Core.Tests -c Release
```

Pokrývají: pohyb zvoleným směrem, ignorování otočení do sebe, snězení jídla
(růst + skóre + přemístění), náraz do zdi a náraz do sebe.

## Poznámka k vývoji

Vývojový stroj neměl nainstalovaný .NET SDK (jen runtime), proto byl kód lokálně
ověřen přímo kompilátorem Roslyn (`csc.exe`) – zkompiloval se a testy i hra
proběhly. Na běžném stroji s SDK stačí příkazy `dotnet` výše.
