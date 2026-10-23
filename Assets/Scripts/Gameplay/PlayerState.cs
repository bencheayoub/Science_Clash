using System.Collections.Generic;

public class PlayerState
{
    public int Index { get; }
    public Deck Deck { get; }
    public Hand Hand { get; }
    public int Energy { get; private set; }
    public int MaxEnergy { get; }

    private readonly List<Scientist> scientists = new List<Scientist>();
    public IReadOnlyList<Scientist> Scientists => scientists;

    /// <summary>First scientist that is still alive, or null if all are defeated.</summary>
    public Scientist ActiveScientist
    {
        get
        {
            foreach (var s in scientists)
                if (s.IsAlive) return s;
            return null;
        }
    }

    public PlayerState(int index, IEnumerable<ScientistData> team, IEnumerable<CardData> startingDeck,
                       int handSize = 5, int maxEnergy = 4)
    {
        Index = index;
        foreach (var data in team) scientists.Add(new Scientist(data));
        Deck = new Deck(startingDeck);
        Hand = new Hand(handSize);
        MaxEnergy = maxEnergy;
    }

    public void DrawCards(int count)
    {
        for (int i = 0; i < count && !Hand.IsFull; i++)
        {
            CardData card = Deck.Draw();
            if (card == null) return;

            Hand.Add(card);
            GameEvents.RaiseCardDrawn(Index, card);
        }
    }

    public void RefillEnergy() => Energy = MaxEnergy;

    public bool TrySpendEnergy(int cost)
    {
        if (cost > Energy) return false;
        Energy -= cost;
        return true;
    }
}