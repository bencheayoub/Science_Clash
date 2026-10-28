using System.Collections.Generic;
using UnityEngine;

/// <summary>Builds everything a new match needs: opponent choice, decks, players, starting hands.</summary>
public class MatchSetup
{
    private readonly IReadOnlyList<ScientistData> roster;
    private readonly IReadOnlyList<CardData> startingCards;
    private readonly int copiesPerCard;
    private readonly int startingHandSize;

    public MatchSetup(
        IReadOnlyList<ScientistData> roster,
        IReadOnlyList<CardData> startingCards,
        int copiesPerCard,
        int startingHandSize)
    {
        this.roster = roster;
        this.startingCards = startingCards;
        this.copiesPerCard = copiesPerCard;
        this.startingHandSize = startingHandSize;
    }

    public bool IsValid(ScientistData human, ScientistData computer) =>
        human != null && computer != null && startingCards.Count > 0;

    // The computer gets a random scientist different from the player's.
    public ScientistData PickOpponent(ScientistData chosen)
    {
        var options = new List<ScientistData>(roster);
        options.Remove(chosen);
        if (options.Count == 0) return chosen;
        return options[Random.Range(0, options.Count)];
    }

    public PlayerState[] CreatePlayers(ScientistData human, ScientistData computer)
    {
        var players = new PlayerState[]
        {
            new PlayerState(0, new[] { human }, BuildDeck()),
            new PlayerState(1, new[] { computer }, BuildDeck())
        };

        foreach (PlayerState p in players)
            p.DrawCards(startingHandSize);

        return players;
    }

    private List<CardData> BuildDeck()
    {
        var deck = new List<CardData>();
        foreach (CardData card in startingCards)
            for (int i = 0; i < copiesPerCard; i++)
                deck.Add(card);
        return deck;
    }
}