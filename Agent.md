# Agent.md — Science Clash

Rules for any AI agent (and human) working in this repository.
Read this fully before making changes.

## Project

- Turn-based 1v1 card battler: pick a scientist, play ability cards against a computer opponent.
- Engine: Unity 6 (6000.x), Universal Render Pipeline (URP), C#.
- Goals: clean, scalable architecture and a polished look (shaders, particles, juice). No placeholder or "AI slop" visuals in the final build.

## Workflow rules

1. **One feature at a time.** Never implement several features in one go.
2. **Plan first.** Before coding a feature, write a short plan (files to touch, new classes, events involved) and wait for approval.
3. After implementing, stop and report: what changed, how to test it in the editor, anything uncertain.
4. Do not refactor unrelated code while working on a feature. Mention it instead and propose a separate task.
5. If a requirement is unclear, ask. Do not guess.
6. Do not add packages or change project settings (render pipeline, input system, quality) without asking.

## Git rules

- `main` always works and opens in Unity with no console errors. **Never commit directly to `main`.**
- One branch per feature or fix, named `type/short-description`:
  - `feat/event-manager`, `fix/ai-skips-turn`, `refactor/battle-subsystems`, `chore/agent-md`, `vfx/card-dissolve`
- Small, focused commits using conventional prefixes:
  - `feat:` `fix:` `refactor:` `chore:` `docs:` `vfx:` `art:`
  - Example: `feat: add Tesla chain lightning ability`
- Merge to `main` only when the feature is finished and tested in Play mode. Delete the branch afterwards.
- **Always commit `.meta` files** together with their assets. Never delete or hand-edit a `.meta` file.
- Never commit `Library/`, `Temp/`, `Logs/`, `obj/`, `UserSettings/` or build output.
- Do not hand-edit scene or prefab YAML. Make those changes in the Unity editor.
- Keep large binary assets (audio, big textures) small. Ask before adding anything over 10 MB.

## Architecture rules

- **Events:** every game event is declared in one static class, `GameEvents`. No static events anywhere else. Systems raise and subscribe through `GameEvents` only.
  - Subscribe in `OnEnable`, unsubscribe in `OnDisable`.
- **Decoupling:** systems never reference each other's concrete classes directly. Communicate via `GameEvents` or small interfaces.
- **Sub-systems:** each system is split into focused classes with a single responsibility (for example `TurnManager`, `CardPlayService`, `AIController`). If a class needs "and" to describe it, split it.
- **Data:** scientists, cards, abilities and effects are ScriptableObjects. Adding content must not require changing code.
  - Abilities are lists of `EffectData` ScriptableObjects (Damage, Heal, Shield, Stun, ...).
- **Pure logic vs. presentation:** battle state and rules contain no `MonoBehaviour`/UI/VFX code. Presentation listens to events and reacts.
- **Update loops:** avoid logic in `Update`. Prefer events, coroutines or tweens.
- **No magic strings or numbers:** use constants, enums or serialized fields. Shader property IDs via `Shader.PropertyToID`.
- **Inspector fields:** `[SerializeField] private`, not `public`.
- No `Find`/`FindObjectOfType`/`GameObject.Find` at runtime.
- Avoid singletons unless there is no clean alternative; ask before adding one.

## Code style

- C# namespaces mirror folders: `ClashScientist.Core`, `ClashScientist.Battle`, etc.
- `PascalCase` for types and methods, `_camelCase` for private fields, `camelCase` for locals and parameters.
- One class per file; the file name matches the class name.
- Small methods, descriptive names, comments only for the "why".

## Folder structure

Assets/
Scripts/
Core/ GameEvents, EffectContext, ICombatant, shared interfaces
Data/ ScriptableObject definitions (CardData, ScientistData, ...)
Effects/ EffectData base class and concrete effects (Damage, Heal, ...)
Gameplay/ game flow and rules (GameManager, TurnManager, CardPlayService, AIController, Deck, Hand)
UI/ menus, hand, HUD, views
Shaders/ shader graphs and HLSL shaders
VFX/ particle prefabs


Put new scripts in the existing folder that fits. Do not create new top-level script folders without asking.

## Shaders and VFX

- Ported shaders (for example from Shadertoy) must credit the original author and link in a comment at the top of the file, and be listed in the README.
- Drive shader values from scripts via `MaterialPropertyBlock` or tweens. Do not create a new material instance per frame.
- Keep every visual within the project palette defined in the README.

## Definition of done (per feature)

- Works in Play mode with no console errors or warnings introduced.
- Follows the architecture rules above.
- Committed on its own branch with a clear message, then merged to `main`.
- README updated if the feature changes architecture or adds a shader.
