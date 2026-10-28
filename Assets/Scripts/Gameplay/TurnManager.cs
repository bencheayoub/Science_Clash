using System.Collections.Generic;

public class TurnManager
{
    private readonly PlayerState[] players;
    private readonly int handSize;
    private readonly bool discardHandEachTurn;

    public int CurrentPlayerIndex { get; private set; }
    public PlayerState CurrentPlayer => players[CurrentPlayerIndex];

    public TurnManager(PlayerState[] players, int handSize, bool discardHandEachTurn)
    {
        this.players = players;
        this.handSize = handSize;
        this.discardHandEachTurn = discardHandEachTurn;
    }

    public void StartFirstTurn(int firstPlayer = 0)
    {
        CurrentPlayerIndex = firstPlayer;
        BeginTurn(drawCards: false);   // both hands were already dealt at match start
    }

    public void EndTurn()
    {
        GameEvents.RaiseTurnEnded(CurrentPlayerIndex);
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % players.Length;
        BeginTurn(drawCards: true);
    }

    private void BeginTurn(bool drawCards)
    {
        CurrentPlayer.RefillEnergy();

        if (drawCards)
        {
            if (discardHandEachTurn)
                DiscardHand(CurrentPlayer);   // unplayed cards go to the discard pile

            int missing = CardsMissing(CurrentPlayer);
            if (missing > 0)
                CurrentPlayer.DrawCards(missing);   // refill the hand up to handSize
        }

        GameEvents.RaiseTurnStarted(CurrentPlayerIndex);
    }

    private void DiscardHand(PlayerState player)
    {
        // Copy first: we can't remove cards from a list while looping over it.
        var leftovers = new List<CardData>();
        foreach (CardData card in player.Hand.Cards)
            leftovers.Add(card);

        foreach (CardData card in leftovers)
        {
            if (player.Hand.Remove(card))
                player.Deck.Discard(card);
        }
    }

    private int CardsMissing(PlayerState player)
    {
        int inHand = 0;
        foreach (CardData card in player.Hand.Cards)
            inHand++;

        return handSize > inHand ? handSize - inHand : 0;
    }
}
