using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>Owns match lifecycle and exposes it to controllers. Details live in MatchSetup, WinChecker, TurnManager, CardPlayService.</summary>
public class GameManager : MonoBehaviour, IMatchActions
{
    [Header("Roster")]
    [SerializeField] private List<ScientistData> roster = new List<ScientistData>();

    [Header("Deck")]
    [SerializeField] private List<CardData> startingCards = new List<CardData>();
    [SerializeField, Min(1)] private int copiesPerCard = 2;

    [Header("Rules")]
    [Tooltip("Cards dealt at the start. The hand is refilled to this size at the start of every turn.")]
    [FormerlySerializedAs("startingHandSize")]
    [SerializeField, Min(1)] private int handSize = 3;
    [Tooltip("ON: unplayed cards are discarded and you get a fresh hand every turn. OFF: unplayed cards stay and the hand is refilled.")]
    [SerializeField] private bool discardHandEachTurn = true;

    private PlayerState[] players;
    private MatchSetup setup;
    private TurnManager turnManager;
    private CardPlayService cardPlayService;

    public IReadOnlyList<ScientistData> Roster => roster;
    public bool IsStarted { get; private set; }
    public bool IsGameOver { get; private set; }
    public int WinnerIndex { get; private set; } = -1;
    public int CurrentPlayerIndex => turnManager.CurrentPlayerIndex;

    public PlayerState GetPlayer(int index) => players[index];

    private void Awake()
    {
        setup = new MatchSetup(roster, startingCards, copiesPerCard, handSize);
    }

    private void OnEnable()
    {
        GameEvents.OnScientistDefeated += HandleScientistDefeated;
        GameEvents.OnScientistChosen += HandleScientistChosen;
    }

    private void OnDisable()
    {
        GameEvents.OnScientistDefeated -= HandleScientistDefeated;
        GameEvents.OnScientistChosen -= HandleScientistChosen;
    }

    private void HandleScientistChosen(ScientistData chosen)
    {
        if (IsStarted) return;
        StartGame(chosen, setup.PickOpponent(chosen));
    }

    public void StartGame(ScientistData human, ScientistData computer)
    {
        if (!setup.IsValid(human, computer))
        {
            Debug.LogError("GameManager: assign the roster and starting cards in the Inspector.");
            return;
        }

        players = setup.CreatePlayers(human, computer);
        turnManager = new TurnManager(players, handSize, discardHandEachTurn);
        cardPlayService = new CardPlayService(players);

        IsStarted = true;
        GameEvents.RaiseGameStarted();
        turnManager.StartFirstTurn(0);
    }

    public bool TryPlayCard(int playerIndex, CardData card)
    {
        if (!IsStarted || IsGameOver) return false;
        if (playerIndex != turnManager.CurrentPlayerIndex) return false;
        return cardPlayService.TryPlay(playerIndex, card);
    }

    public void EndTurn()
    {
        if (!IsStarted || IsGameOver) return;
        turnManager.EndTurn();
    }

    private void HandleScientistDefeated(ICombatant defeated)
    {
        if (IsGameOver) return;
        if (!WinChecker.TryGetWinner(players, out int winner)) return;

        IsGameOver = true;
        WinnerIndex = winner;
        GameEvents.RaiseGameOver(WinnerIndex);
    }
}
