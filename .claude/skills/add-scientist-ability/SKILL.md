---
name: add-scientist-ability
description: Use when adding a new scientist or giving a scientist a new ability. Content is data (ScriptableObjects), not code, unless a brand new effect type is needed.
---

# Add a scientist or ability

## Steps
1. Create the scientist asset in `Assets/Data/Scientists/` (Create > ScienceClash > Scientist). Fill stats, portrait, color.
2. Build the ability from existing `AbilityEffect` assets in `Assets/Data/Effects/` (Damage, Heal, Shield, Stun...). Reuse before writing new code.
3. If no existing effect fits, add ONE new subclass of `AbilityEffect` in `Assets/Scripts/Cards/Effects/` implementing `Apply(context)`. No switch statements on effect type anywhere else.
4. Assign the effect list to the scientist's ability field.
5. Effects report what happened by raising events through `GameEvents` (damage dealt, status applied). They never reference UI or VFX directly.

## Rules
- Balance numbers live in the assets, not in code.
- A new scientist must work with zero changes to battle code.

## Check before finishing
- The AI opponent can use the ability without errors.
- Play one full match with the scientist.
