Science Clash Card Game: Technical Report
Challenge #2 (Card Game) · Author: Atom
1. Overview
Science Clash is a turn-based 1v1 card duel built in Unity. The player picks one of six scientists, the computer receives a random different one, and both draw from a deck of eight data-defined cards. Each turn a player refills their energy to 4, draws a card, and plays cards (damage or healing) until they end the turn. The first scientist to reach 0 health loses.
The project was built around three goals: scalable (new cards, effects and scientists without touching existing code), decoupled (no system knows about another), and data-driven (content lives in ScriptableObject assets, not in code).
2. Architecture: sub-systems and one event manager
Feedback on my previous project asked for two things: a single owner for all events, and a division into sub-systems rather than only into systems. Both are applied here.
• Core holds what everyone shares: GameEvents, ICombatant, EffectContext.
• Data holds the ScriptableObject definitions: CardData, ScientistData, CardTargetType.
• Effects holds the card logic: EffectData, DamageEffect, HealEffect.
• Gameplay holds the rules and runtime state: GameManager, TurnManager, CardPlayService, PlayerState, Deck, Hand, Scientist, AIController.
• UI holds the views and input: CardView, HandView, ScientistPanelView, TurnHUDView, GameOverView, ScientistOptionView, ScientistSelectionView, CardInputController.
The UI never calls gameplay classes to change state, and gameplay never references the UI. They communicate only through events.
3. Why each script exists
Core
• GameEvents is the single static class that owns every event (12 in total: game start and over, turn start and end, card drawn, played and clicked, damage, heal, scientist defeated and chosen, end turn clicked). Events can only be raised through its Raise... methods, so there is one place to see everything that can happen in the game. A ResetAll method clears them on startup, which avoids duplicate subscriptions when Domain Reload is disabled in the editor.
• ICombatant is the interface effects work against (name, health, TakeDamage, Heal). Effects depend on this abstraction rather than on the concrete scientist class (dependency inversion), so anything can become a target later.
• EffectContext bundles source, target and player index into one struct. Adding information for future effects does not change every effect's method signature.
Data
• CardData is a ScriptableObject holding a card's name, cost, art, target type and a list of effects. Resolve() applies them in order. A card contains no logic of its own.
• ScientistData is the fixed definition of a scientist (name, max health, portrait). It is never modified at runtime.
• CardTargetType states whether a card targets the enemy or the player's own scientist.
Effects
• EffectData is an abstract ScriptableObject with one method, Apply(context). This is the Strategy pattern: a card delegates to interchangeable behaviours.
• DamageEffect and HealEffect are the two concrete behaviours. Each has a configurable amount and raises an event after applying itself.
Gameplay
• Scientist is the runtime instance of a ScientistData. Keeping data and runtime state separate means playing never changes the assets on disk.
• Deck, Hand and PlayerState are plain C# classes with no scene dependency. The deck shuffles with Fisher-Yates and reshuffles its discard pile when empty. PlayerState holds a list of scientists, so going to three per player is a data change.
• TurnManager owns turn order, energy refill and drawing.
• CardPlayService owns the rules for playing a card: affordability, the card being in hand, target selection, resolving effects, discarding, and raising OnCardPlayed.
• GameManager is the composition root. It builds the players, wires the services together, listens for the scientist choice and detects the winner by reacting to the defeat event.
• AIController plays for the computer. It listens for OnTurnStarted and calls the same TryPlayCard and EndTurn as the human, with a simple heuristic: heal when below 60% health, otherwise spend energy on the most expensive affordable card.
UI
• CardView and ScientistOptionView are reusable views that bind to any asset, so one prefab renders every card and every scientist.
• HandView, ScientistPanelView and TurnHUDView are read-only: on each event they re-read the current state and redraw, so the UI cannot drift out of sync.
• CardInputController is the only bridge from UI clicks to game rules. Clicks become events, and this controller translates them into method calls.
• ScientistSelectionView builds the selection screen from the roster, and GameOverView shows the result and restarts the scene.
4. Example: what happens when a card is played
1. The player clicks a card. CardView raises OnCardClicked.
2. CardInputController receives it and calls GameManager.TryPlayCard.
3. CardPlayService validates the play, spends energy, removes the card from the hand, and calls CardData.Resolve.
4. Each EffectData changes the target through ICombatant and raises OnDamageDealt or OnHealed.
5. If the target reaches 0 health, Scientist raises OnScientistDefeated. GameManager hears it and raises OnGameOver.
6. The views react to the events they care about and redraw. None of them was called directly.
5. Patterns used
• Observer: the central GameEvents class.
• Strategy: EffectData and its subclasses.
• Data-driven design / ScriptableObjects: cards, effects and scientists are assets.
• Composition over inheritance: a card is a list of effects, so complex cards need no new code.
• Dependency inversion: effects depend on ICombatant.
• Single responsibility: turn flow, card rules and composition are in separate classes.
• Composition root: only GameManager creates and connects the runtime objects.
6. How to extend the game
• New card: create a CardData asset and drag existing effect assets into its list. Zero code. The eight cards in the game were all made this way, including combinations such as 9 damage from two effects.
• New effect (for example shield or poison): add one EffectData subclass. No existing class changes.
• New scientist: create a ScientistData asset and add it to the roster list on GameManager.
• Different opponent: replace or disable AIController. The game began as hot-seat for two humans and switching to the computer required no change to the rules or the UI.
• More scientists per player: PlayerState already stores a list.
7. Technical decisions and problems solved
• Static events survive between Play sessions when Domain Reload is off, which causes duplicate listeners. Solved with ResetAll using RuntimeInitializeOnLoadMethod.
• Effects originally needed to know about concrete scientists. Introducing ICombatant and EffectContext removed that dependency.
• The UI first updated by tracking individual changes. Rebuilding views from state on each event proved simpler and removed synchronisation bugs.
• The same TryPlayCard method validates turn order, energy and hand contents, so neither the UI nor the AI can break the rules.
8. From the original idea to a three-day build, and next steps
In three days I built a complete, playable core of my original Science Clash idea: a turn-based scientist duel against a computer opponent. The full vision is larger, and the architecture was chosen so the missing parts can be added without rewriting what exists.
The original idea
• Science Clash was designed as turn-based 1v1 battles between scientists, with three scientists per player, each using abilities built from effects.
What was realised in three days
• A full turn loop with energy, drawing, discarding and a win condition.
• Eight cards defined entirely as ScriptableObject assets, built from reusable damage and heal effects.
• Six selectable scientists (Curie, Newton, Tesla, Turing, Lovelace and Einstein) with portraits, a selection screen, and a random opponent for the computer.
• A computer opponent that plays as player 2.
• A complete interface: card hand, health panels, turn display, game over screen with restart, a lab background and card art.
• The structure requested in feedback on my previous project: one central event manager and a division into sub-systems.
Not yet realised
• Scientists differ only in health and portrait. None has abilities of its own yet.
• Each player fields one scientist instead of three.
• There is no way to buy or unlock new scientists and abilities.
Next steps
1. Give every scientist its own ability. A ScientistData would hold a list of ability effects that reuse EffectData, and the battle screen would get an ability button. The existing damage and heal effects are reused as they are.
2. Add more abilities by writing new EffectData subclasses (for example shield, poison, draw a card, gain energy). No existing class changes.
3. Bring back three scientists per player. PlayerState already stores a list, so the work is switching the active scientist and showing a team panel.
4. Let the player buy or unlock more characters and abilities. The roster is already a list of assets, so a shop only decides which assets are in it.
5. Improve the computer opponent so it uses abilities and looks ahead.
6. Add automated tests for the plain C# classes (Deck, Hand, PlayerState), which do not depend on the scene.
