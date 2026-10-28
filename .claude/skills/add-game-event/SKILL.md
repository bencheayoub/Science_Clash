---
name: add-game-event
description: Use when adding a new gameplay event (something that happened that other systems may react to). Adds it to GameEvents and wires raisers and listeners.
---

# Add a game event

## Steps
1. Open `Assets/Scripts/Core/GameEvents.cs` and declare the event:
   `public static event Action<...> OnSomethingHappened;`
2. Add a raise method:
   `public static void RaiseSomethingHappened(...) => OnSomethingHappened?.Invoke(...);`
3. Raise it from exactly one place: the system that owns that fact.
4. In every listener, subscribe in `OnEnable` and unsubscribe in `OnDisable`.
5. Pass data via parameters (ScriptableObject references or ids), never concrete scripts.

## Rules
- No static events outside `GameEvents`.
- Name events in past tense (`OnCardPlayed`, not `OnPlayCard`).
- Every subscribe has a matching unsubscribe.

## Check before finishing
- Search the project for the event name: one raise site, balanced subscribe/unsubscribe.
- Play mode shows no new errors.
