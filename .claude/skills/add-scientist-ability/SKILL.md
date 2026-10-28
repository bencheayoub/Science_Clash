---
name: add-scientist-ability
description: Use when adding a new scientist, card or ability. Content is data (ScriptableObjects), not code, unless a brand new effect type is needed.
---

# Add a scientist, card or ability

## Steps
1. Create the `ScientistData` asset (see `Assets/Scripts/Data/ScientistData.cs`). Put it next to the existing ScientistData assets. Check where they live before creating a new one.
2. Create or edit the `CardData` assets (see `Assets/Scripts/Data/CardData.cs`) and set the correct `CardTargetType`.
3. Build the card's behavior from existing `EffectData` assets (`DamageEffect`, `HealEffect`). Reuse before writing new code.
4. If no existing effect fits, add ONE new subclass of `EffectData` in `Assets/Scripts/Effects/` that implements its apply logic using `EffectContext` and `ICombatant`. Do not special-case effect types anywhere else (no switch statements on effect type).
5. Effects report what happened by raising events through `GameEvents` (for example `OnDamageDealt`, `OnHealed`). They never reference UI or VFX directly.

## Rules
- Balance numbers live in the assets, not in code.
- A new scientist or card must work with zero changes to `GameManager`, `TurnManager`, `CardPlayService` or `AIController`.
- Follow Agent.md for architecture and git rules.

## Check before finishing
- The AI opponent can use the new card without errors.
- Play one full match with the new content.
