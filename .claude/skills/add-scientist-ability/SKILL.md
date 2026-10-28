---
name: add-scientist-ability
description: Use when adding a new scientist, card or ability. Content is data (ScriptableObjects), not code, unless a brand new effect type is needed.
---

# Add a scientist, card or ability

## How abilities work in this project
- A card (`CardData`) is a list of `EffectData` assets. Effects do the work.
- A scientist's unique ability is a **signature card**: a card listed in that scientist's `ScientistData.SignatureCards`. `MatchSetup` adds those cards to that scientist's deck, on top of the shared `startingCards`.

## Steps
1. Create the `ScientistData` asset next to the existing scientist assets. Set name, health, portrait.
2. Build the ability from existing effects in `Assets/Scripts/Effects/` (`DamageEffect`, `HealEffect`, `LifeStealEffect`, `MultiHitEffect`, `OverchargeEffect`, `ExecuteEffect`). Reuse before writing new code.
3. Create a `CardData` asset for the ability: set cost, `CardTargetType`, and add the effects.
4. Drag that card into the scientist's **Signature Cards** list.
5. If no existing effect fits, add ONE new subclass of `EffectData` in `Assets/Scripts/Effects/` that implements `Apply(in EffectContext context)`. No special-casing of effect types anywhere else.
6. Effects report what happened through `GameEvents` (`RaiseDamageDealt`, `RaiseHealed`). They never reference UI or VFX directly.

## Rules
- Balance numbers live in the assets, not in code.
- A new scientist or card must work with zero changes to `GameManager`, `TurnManager`, `CardPlayService`, `MatchSetup` or `AIController`.
- Follow Agent.md for architecture and git rules.

## Check before finishing
- The AI opponent can use the new card without errors.
- Play one full match with the new scientist and against them.
