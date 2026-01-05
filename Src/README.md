# Dungeon Adventures ⚔️

Welcome to **Dungeon Adventures**, a C# .NET 8.0 console-based adventure game with a unique "Hostile Terminal" aesthetic. This project prioritizes an immersive, unsettling, and corrupted user experience through a strictly text-based interface, intentional visual artifacts, and flicker-free rendering.

## Overview

"Dungeon Adventures" is not your typical console RPG. It's designed to feel like a maligned, sentient system struggling to run a fantasy game. The UI is intentionally corrupted, the system messages are cryptic, and the overall atmosphere is one of a dying machine.

## Technology Stack

The project is built on the .NET 8.0 framework and leverages several key libraries to achieve its signature "Hostile Terminal" experience:

*   **Programming Language:** C#
*   **Framework:** .NET 8.0
*   **Key Libraries:**
    *   **`Spectre.Console`**: For advanced, structured console output and argument parsing, tailored to the game's aesthetic.
    *   **`Pastel`**: Provides precise 24-bit console coloring, restricted to "Emergency Red" (`#FF0000`) and "Toxic Green" (`#00FF00`) to enhance the corrupted theme.
    *   **`System.Text.StringBuilder`**: Used for high-performance, flicker-free rendering of ASCII effects and UI elements.
    *   **`NUnit`**: The framework used for all unit testing to ensure game logic is robust.
    *   **`ImGui.NET`**: Utilized for extensive developer and debug GUI overlays.

## Features

*   **Character Creation & Progression:** Create your character and develop them through adventures.
*   **Quests & Adventures:** Embark on quests, like the "Duke's Quest," and face perilous encounters.
*   **Challenging Encounters:** Battle formidable bosses with unique attributes and traits.
*   **In-Game Economy:** Visit the Town Square, interact with a global shop, and manage your items.
*   **Loot System:** Discover and manage items, weapons, and powerful gems.
*   **The Forge:** Upgrade and craft your equipment.
*   **Save/Load System:** Persist your character's progress.

## Getting Started

To build and run the project, use the standard .NET CLI commands.

### Build the Project
```bash
dotnet build
```

### Run the Application
```bash
dotnet run --project DungeonAdventures.csproj
```

### Run Tests
```bash
dotnet test DungeonAdventures.Tests.csproj
```

## Development Conventions

The project follows a strict set of "Hostile C# Style Guide" conventions to maintain its unique aesthetic and architectural integrity.

*   **Buffer-Safe Rendering (84x24 Reality Anchor):** All rendering logic must be confined within an `84x24` character grid to prevent scrolling or buffer overflows.
*   **Static Output (No Scrolling, No Shifting):** `Console.WriteLine()` is strictly forbidden. All screen output is managed using `Console.SetCursorPosition()` and `Console.Write()` for a static display.
*   **Zero-Deletions Policy (Additive Infection):** New development is strictly additive. Existing logic, variables, and especially corrupted atmospheric text must not be removed or refactored, fostering a sense of malignant growth.
*   **Visual Intensity & Atmospheric Priority:** Visual effects are built with `System.Text.StringBuilder` for high-speed, stable rendering. Coloring is restricted to "Emergency Red" and "Toxic Green." Comments are encouraged to be "glitch-comments" to enhance the atmosphere.
*   **Persona Alignment (Sentient System Malignancy):** Code should intentionally reflect a malevolent, dying machine. Clarity can be sacrificed for aesthetic impact, and error handling should manifest as further visual corruption rather than user-friendly messages.
