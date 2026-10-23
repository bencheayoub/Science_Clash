using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Roster")]
    [SerializeField] private List<ScientistData> roster = new List<ScientistData>();

    [Header("Deck")]
    [SerializeField] private List<CardData> startingCards = new List<CardData>();
    [SerializeField, Min(1)] private int copiesPerCard = 2;

    [Header("Rules")]
    [SerializeField, Min(1)] private int startingHandSize = 3;
    [SerializeField, Min(1)] private int cardsPerTurn = 1;

    private PlayerState[] players;
    private TurnManager turnManager;
    private CardPlayService cardPlayService;

    public IReadOnlyList<ScientistData> Roster => roster;
    public bool IsStarted { get; private set; }
    public bool IsGameOver { get; private set; }
    public int WinnerIndex { get; private set; } = -1;
    public int CurrentPlayerIndex => turnManager.CurrentPlayerIndex;

    public PlayerState GetPlayer(int index) => players[index];

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
        StartGame(chosen, PickOpponent(chosen));
    }

    // The computer gets a random scientist different from the player's.
    private ScientistData PickOpponent(ScientistData chosen)
    {
        var options = new List<ScientistData>(roster);
        options.Remove(chosen);
        if (options.Count == 0) return chosen;
        return options[Random.Range(0, options.Count)];
    }

    public void StartGame(ScientistData human, ScientistData computer)
    {
        if (human == null || computer == null || startingCards.Count == 0)
        {
            Debug.LogError("GameManager: assign the roster and starting cards in the Inspector.");
            return;
        }

        players = new PlayerState[]
        {
            new PlayerState(0, new[] { human }, BuildDeck()),
            new PlayerState(1, new[] { computer }, BuildDeck())
        };

        turnManager = new TurnManager(players, cardsPerTurn);
        cardPlayService = new CardPlayService(players);

        foreach (PlayerState p in players)
            p.DrawCards(startingHandSize);

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

    private List<CardData> BuildDeck()
    {
        var deck = new List<CardData>();
        foreach (CardData card in startingCards)
            for (int i = 0; i < copiesPerCard; i++)
                deck.Add(card);
        return deck;
    }

    private void HandleScientistDefeated(ICombatant defeated)
    {
        if (IsGameOver) return;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].ActiveScientist == null)
            {
                IsGameOver = true;
                WinnerIndex = 1 - i;
                GameEvents.RaiseGameOver(WinnerIndex);
                return;
            }
        }
    }
}