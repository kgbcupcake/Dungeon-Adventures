# Spec: Hostile Terminal UI Implementation

This document outlines the technical specifications for refactoring the UI Engine and Game Loading sequence to align with the "Hostile Terminal" master directive.

## 1. Core Engine Refactoring
- **Objective:** Rip out existing UI rendering and replace it with a pure, high-performance console rendering engine.
- **Requirements:**
    - **Deprecate GUI Libraries:** All code related to `ImGui.NET` and `ClickableTransparentOverlay` must be located and marked as deprecated. Its execution path should be disabled.
    - **Console Control:** The application window must be configured to a static size of 85 columns by 25 rows.
    - **Rendering Engine:** A new rendering service must be created that uses `System.Text.StringBuilder` to construct each frame before writing it to the console with `Console.SetCursorPosition` and `Console.Write`.
    - **Total Frame Reset:** The rendering loop must implement the "Total Frame Reset" protocol. `Console.Clear()` is forbidden.

## 2. Game Loading Sequence: "Hardware Agony"
- **Objective:** Transform the initial game loading screen into a simulation of a dying, sentient terminal.
- **Requirements:**
    - **Immutable Seed:** The `DUMGERAGN` logo and existing icons (💀, ⸸, ☣, ☢) must be the first elements rendered to the buffer on every frame.
    - **ASCII Corruption:** A 50% density ASCII "rain" effect must be layered on top of the immutable seed. The characters used must be from a predefined "corrupted" set.
    - **Hardware Agony Loop:** The corruption animation must loop 20 times. Each iteration must have a mandatory `Thread.Sleep(800)` to simulate system struggle.
    - **Color Palette:** The entire sequence must use only "Emergency Red" (`#FF0000`) and "Toxic Green" (`#00FF00`) via the `Pastel` library.

## 3. Global UI Rules
- **The Reality Anchor:** All UI elements, text, and effects must be strictly contained within the `84x24` safe zone.
- **Zero-Deletions Policy:** No existing game logic or variables should be deleted. The new UI is an "infection" layered on top of the existing structure.

