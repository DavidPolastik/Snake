cmake_minimum_required(VERSION 3.16)
project(SnakeCleanCode LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# --- snake_core: čisté herní jádro bez jakéhokoli I/O -----------------------
# Nezávisí na ničem konzolovém. Právě to, že testy níže linkují JEN tuto
# knihovnu, je fyzickým důkazem oddělení logiky od GUI.
add_library(snake_core
    src/SnakeGame.cpp
    src/RandomFoodPlacer.cpp)
target_include_directories(snake_core PUBLIC include)

# --- snake: spustitelná hra (jádro + konzolové adaptéry + kompoziční kořen) --
add_executable(snake
    src/main.cpp
    src/console/ConsoleRenderer.cpp
    src/console/ConsoleInput.cpp)
target_include_directories(snake PRIVATE src)
target_link_libraries(snake PRIVATE snake_core)

# --- testy jádra ------------------------------------------------------------
enable_testing()
add_executable(snake_tests tests/core_tests.cpp)
target_link_libraries(snake_tests PRIVATE snake_core)
add_test(NAME core_tests COMMAND snake_tests)
