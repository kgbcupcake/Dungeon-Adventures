---
description: Fixes ImGui stack assertions and API versioning for 1.91.6.1
---

You are an expert C# ImGui developer. Your task is to analyze the provided code for "Stack Leaks."

**Rules:**

1. Ensure every `ImGui.BeginChild()` has a matching `ImGui.EndChild()`.
2. Replace boolean borders with `ImGuiChildFlags.Borders` to satisfy version 1.91.6.1.
3. Move `ImGui.End()` outside of any conditional blocks to prevent "Missing End" crashes.
4. Do not re-declare `isWindowOpen` if it already exists in the scope.

Please refactor the code provided in the context.
