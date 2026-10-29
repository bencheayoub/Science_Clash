using System;
using UnityEngine;

/// <summary>
/// Single owner of every game event.
/// Systems never reference each other: they only raise or listen to events here.
/// </summary>
public static class GameEvents
{
    // ---- Game flow ----
    public static event Action OnGameStarted;
    public static event Action<int> OnTurnStarted;   // player index (0 or 1)
    public static event Action<int> OnTurnEnded;     // player index
    public static event Action<int> OnGameOver;      // winning player index
    public static event Action<ICombatant, int> OnDamageDealt;
    public static event Action<ICombatant, int> OnHealed;
    public static event Action<int, CardData> OnCardPlayed;   // player index, card
    public static event Action<int, CardData> OnCardDrawn;       // player index, card
    public static event Action<ICombatant> OnScientistDefeated;
    public static event Action<CardData> OnCardClicked;
    public static event Action OnEndTurnClicked;
    public static event Action<ScientistData> OnScientistChosen;
    public static event Action<int, CardData> OnCardPlayStarted;

    // ---- Raise methods (events can only be invoked from inside this class) ----
    public static void RaiseGameStarted() => OnGameStarted?.Invoke();
    public static void RaiseTurnStarted(int player) => OnTurnStarted?.Invoke(player);
    public static void RaiseTurnEnded(int player) => OnTurnEnded?.Invoke(player);
    public static void RaiseGameOver(int winner) => OnGameOver?.Invoke(winner);
    public static void RaiseDamageDealt(ICombatant target, int amount) => OnDamageDealt?.Invoke(target, amount);
    public static void RaiseHealed(ICombatant target, int amount) => OnHealed?.Invoke(target, amount);
    public static void RaiseCardPlayed(int player, CardData card) => OnCardPlayed?.Invoke(player, card);
    public static void RaiseCardDrawn(int player, CardData card) => OnCardDrawn?.Invoke(player, card);
    public static void RaiseScientistDefeated(ICombatant who) => OnScientistDefeated?.Invoke(who);
    public static void RaiseCardClicked(CardData card) => OnCardClicked?.Invoke(card);
    public static void RaiseEndTurnClicked() => OnEndTurnClicked?.Invoke();
    public static void RaiseScientistChosen(ScientistData data) => OnScientistChosen?.Invoke(data);
    public static void RaiseCardPlayStarted(int player, CardData card) => OnCardPlayStarted?.Invoke(player, card);
    // Static events survive between Play sessions when Domain Reload is off,
    // so we clear them on startup to avoid duplicate subscriptions.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetAll()
    {
        OnGameStarted = null;
        OnTurnStarted = null;
        OnTurnEnded = null;
        OnGameOver = null;
        OnDamageDealt = null;
        OnHealed = null;
        OnCardPlayed = null;
        OnCardDrawn = null;
        OnScientistDefeated = null;
        OnCardClicked = null;
        OnEndTurnClicked = null;
        OnScientistChosen = null;
        OnScientistChosen = null;
        OnCardPlayStarted = null;
    }
}
