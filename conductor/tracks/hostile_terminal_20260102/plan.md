# Plan: Hostile Terminal UI Implementation

This plan details the phases and tasks required to implement the "Hostile Terminal" UI aesthetic.

## Phase 1: Engine Transplant & Console Setup

- [x] Task: Identify and isolate all code related to `ImGui.NET` and `ClickableTransparentOverlay`. Add comments marking them as deprecated and disable their execution paths.
- [x] Task: Create a new `ConsoleManager` or similar static class to handle console setup. This class will set the window size to 85x25, buffer size, and hide the cursor.
- [x] Task: Implement a new `RenderService` that uses a `StringBuilder` for frame construction. It should have a method to `Present()` the final `StringBuilder` content to the console using `Console.SetCursorPosition` and `Console.Write`.
- [x] Task: Conductor - User Manual Verification 'Phase 1: Engine Transplant & Console Setup' (Protocol in workflow.md)

## Phase 2: Implementing "Hardware Agony" Loading Sequence

- [x] Task: Create a new `LoadingSequence` class that will be called when the game starts.
- [x] Task: Implement the "Immutable Seed" logic within the `LoadingSequence` to draw the centered `DUMGERAGN` logo and icons to the `StringBuilder` on every frame.
- [x] Task: Develop the "ASCII Corruption" generator, which creates a randomized string of "corrupted" characters matching 50% of the screen area.
- [ ] Task: Implement the main "Hardware Agony" loop in `LoadingSequence`. This loop will run 20 times, calling the Immutable Seed and ASCII Corruption rendering, presenting the frame, and then executing a `Thread.Sleep(800)`.
- [ ] Task: Integrate the `Pastel` library to colorize the loading sequence with "Emergency Red" and "Toxic Green".
- [ ] Task: Conductor - User Manual Verification 'Phase 2: Implementing "Hardware Agony" Loading Sequence' (Protocol in workflow.md)

## Phase 3: Integration and Finalization

- [ ] Task: Replace the old `StartGameLoading` methods body with a call to the new `LoadingSequence` class.
- [ ] Task: Perform a full review of the implementation to ensure all constraints from `tech-stack.md` and `Hostile-C-Sharp.md` have been met (e.g., no `Console.WriteLine`, adherence to the 84x24 safe zone).
- [ ] Task: Conductor - User Manual Verification 'Phase 3: Integration and Finalization' (Protocol in workflow.md)

