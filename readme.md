# Snake – refaktoring podle zásad čistého kódu

Refaktoring známé konzolové hry **Snake** napsané v C++. Cílem úlohy nebylo hru
vymyslet, ale vzít **legacy kód** a přepsat ho podle pravidel *čistého kódu*
(Clean Code) – se zvláštním důrazem na **oddělení herní logiky od GUI**.

- Původní kód: <https://codereview.stackexchange.com/questions/127515/first-c-program-snake-game>
- Jazyk: C++17, build přes CMake (přenositelné, ne jen Windows).
- Jde o **samostatnou práci** (jednotlivec).

## Co bylo na původním kódu špatně

Původní verze byla jeden soubor, kde bylo všechno propletené dohromady:

- **Globální stav** – `x, y, fruitX, fruitY, tailX[100], tailY[100], nTail,
  dir, gameOver` jako globální proměnné. Pole ocasu mělo napevno 100 prvků
  (magické číslo, skrytý strop délky).
- **Míchání úrovní abstrakce a kompetencí** – funkce `Draw()` zároveň mazala
  obrazovku (`system("cls")`), procházela herní logiku (hledání článků ocasu)
  a tiskla znaky. Logika, vstup i výstup byly slité v `Setup/Draw/Input/Logic`.
- **Nečitelný posun ocasu** – ruční smyčka s pomocnými `prev`/`prev2`
  uprostřed `Logic()`.
- **Nepřenositelnost** – přímé volání `conio.h`/`windows.h` (`_kbhit`,
  `_getch`, `Sleep`) rozeseté v kódu; hru nešlo přeložit ani otestovat jinde.
- **Netestovatelnost** – logiku nešlo spustit bez konzole a klávesnice.
- **Chybějící pravidlo** – šlo se otočit přímo do sebe (okamžitý konec).

## Jak vypadá refaktorovaná verze

Kód je rozdělený do tří vrstev, jejichž oddělení je **fyzicky vynucené**
strukturou CMake (jádro nelinkuje nic z konzole):

```
include/snake/   Veřejné API čistého jádra – ŽÁDNÝ <iostream>/<conio.h>
  Point, Direction, Board, Snake, GameStatus   value/logic typy
  SnakeGame                                     pravidla hry (tick, skóre, stav)
  IFoodPlacer, IRenderer, IInput                rozhraní (inverze závislostí)
src/
  SnakeGame.cpp, RandomFoodPlacer               implementace jádra
  console/ConsoleRenderer                       vykreslení do konzole (jen zde <iostream>)
  console/ConsoleInput                          čtení kláves (jediné místo s conio/termios)
  main.cpp                                      kompoziční kořen + herní smyčka
tests/core_tests.cpp                            testy jádra (linkují jen jádro)
```

### Oddělení logiky od GUI (jádro úlohy)

- Třída `SnakeGame` obsahuje **jen pravidla** – neví o konzoli ani klávesnici.
  Krok hry je `tick()`, stav si okolí přečte přes gettery.
- Vykreslování a vstup jsou schované za rozhraními **`IRenderer`** a
  **`IInput`**. `main.cpp` do jádra vstříkne konkrétní konzolové adaptéry
  (*dependency injection*). Vyměnit textovou konzoli za jiné GUI = napsat nový
  adaptér, jádra se to nedotkne.
- Umístění jídla je za rozhraním **`IFoodPlacer`**: produkčně náhodné
  (`RandomFoodPlacer` s vlastním `std::mt19937`, žádný globální `rand()`),
  v testech deterministické.

### Další úklid v duchu Clean Code

- Globální proměnné → zapouzdřený stav v malých třídách s jasnou odpovědností.
- Ošklivý posun ocasu → `std::deque<Point>` a metoda `Snake::moveTo`.
- Magická čísla → pojmenované konstanty (`kScorePerFood`, rozměry plochy…).
- Blikající `system("cls")` → přenositelné ANSI přesuny kurzoru, kreslí se
  jeden sestavený rámec.
- Pojmenované typy: `enum class Direction`, `GameStatus`, `Command`.

## Sestavení a spuštění

Potřebuješ CMake (3.16+) a překladač C++17 (MSVC, MinGW/g++ nebo clang).

```bash
cmake -S snake -B snake/build
cmake --build snake/build
```

Spuštění hry:

```bash
# Windows
snake\build\Debug\snake.exe   # nebo snake\build\snake.exe dle generátoru
# Linux/macOS
./snake/build/snake
```

**Ovládání:** `W A S D` = pohyb, `X` = konec. Had roste sněžením jídla (`F`),
konec při nárazu do zdi nebo do sebe.

## Testy

Testy jádra jsou registrované v CTestu:

```bash
ctest --test-dir snake/build --output-on-failure
```

Pokrývají: pohyb zvoleným směrem, ignorování otočení do sebe, snězení jídla
(růst + skóre + přemístění), náraz do zdi a náraz do sebe.
