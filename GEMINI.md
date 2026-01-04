# Gemini Project Context: Dungeon Adventures

This document provides a comprehensive overview of the "Dungeon Adventures" project, designed to serve as instructional context for future interactions with the Gemini CLI agent.

## Project Overview

"Dungeon Adventures" is a C# .NET 8.0 console-based adventure game. Its core design principle revolves around a unique "Hostile Terminal" aesthetic, prioritizing an unsettling and corrupted user experience. The application enforces a strictly text-based user interface, employing intentional visual artifacts, flicker-free rendering, and specific display constraints to create an immersive, maligned atmosphere.

## Technology Stack

The project is built upon the .NET 8.0 framework with C# as the primary programming language. The following key libraries are integral to achieving the intended "Hostile Terminal" experience:

*   **Programming Language:** C#
*   **Framework:** .NET 8.0
*   **Key Libraries:**
    *   `Pastel`: Utilized for precise 24-bit console coloring (specifically "Emergency Red" `#FF0000` and "Toxic Green" `#00FF00`) to enhance the corrupted visual theme.
    *   `System.Text.StringBuilder`: Essential for high-performance, flicker-free rendering of dense ASCII corruption effects and dynamic UI elements, ensuring visual stability.
    *   `System.Threading`: Employed to simulate system struggle and intentional delays (e.g., `Thread.Sleep`) that contribute to the game's atmospheric hostility.
    *   `System.Console`: Used for granular control over the console output, including `SetCursorPosition` and `Write`, to manage a static, non-scrolling display.
    *   `Spectre.Console` and `Spectre.Console.Cli`: Provides advanced capabilities for structured console output and command-line argument parsing, integrated within the constraints of the "Hostile Terminal" aesthetic.
    *   `NUnit`: The chosen framework for unit testing, ensuring the robustness and correctness of game logic and components.

## Building, Running, and Testing

The project uses standard .NET CLI commands for development workflows:

*   **Build the Project:**
    ```bash
    dotnet build
    ```
*   **Run the Application:**
    ```bash
    dotnet run --project DungeonAdventures.csproj
    ```
    (Note: This should be executed from the solution root or the `DungeonAdventures` project directory.)
*   **Run Tests:**
    ```bash
    dotnet test DungeonAdventures.Tests.csproj
    ```
    (Note: This should be executed from the solution root or the `DungeonAdventures.Tests` project directory.)

## Development Conventions

The project adheres to a strict set of development conventions outlined in the `conductor` documentation, primarily driven by the "Hostile C# Style Guide." These conventions are critical for maintaining the intended aesthetic and architectural integrity:

*   **Buffer-Safe Rendering (84x24 Reality Anchor):** All rendering logic *must* be confined within an `84x24` character grid. This ensures a 1-character safe zone from the console edges, preventing unwanted scrolling or buffer overflows. Rendering loops should iterate up to `width - 1` and `height - 1`.
*   **Static Output (No Scrolling, No Shifting):** The use of `Console.WriteLine()` is strictly forbidden. All screen output *must* be managed using `Console.SetCursorPosition(x, y)` followed by `Console.Write(character)`. This mandate is crucial for maintaining a static, non-shifting display.
*   **Zero-Deletions Policy (Additive Infection):** Existing logic, variables, functions, and strings (especially corrupted atmospheric text) *must not* be removed or modified. New development is strictly additive, aiming to layer new corruptions onto the existing codebase, fostering a sense of malignant growth rather than refactoring.
*   **Visual Intensity & Atmospheric Priority:** Frame-by-frame visual effects, such as "Digital Decay," must be constructed using `System.Text.StringBuilder` before writing to the console to ensure high-speed, stable rendering. Coloring is restricted to "Emergency Red" (`#FF0000`) and "Toxic Green" (`#00FF00`). Comments are encouraged to be "glitch-comments," enhancing the atmosphere with cryptic phrases and hex fragments rather than typical logical explanations.
*   **Persona Alignment (Sentient System Malignancy):** Code should intentionally reflect a malevolent, dying machine. Clarity can be sacrificed for aesthetic impact, variable names can be cryptic, and logic can be complex if it contributes to chaotic visual output. Error handling should manifest as further corruption or system judgments, not user-friendly messages, as the application is designed to become more unstable rather than crash.
