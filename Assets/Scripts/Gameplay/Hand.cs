using System.Collections.Generic;

public class Hand
{
    private readonly List<CardData> cards = new List<CardData>();

    public int MaxSize { get; }
    public int Count => cards.Count;
    public bool IsFull => cards.Count >= MaxSize;
    public IReadOnlyList<CardData> Cards => cards;

    public Hand(int maxSize = 5) => MaxSize = maxSize;

    public bool Add(CardData card)
    {
        if (IsFull) return false;
        cards.Add(card);
        return true;
    }

    public bool Remove(CardData card) => cards.Remove(card);
}