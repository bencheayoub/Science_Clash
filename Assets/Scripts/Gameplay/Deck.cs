using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private readonly List<CardData> drawPile;
    private readonly List<CardData> discardPile = new List<CardData>();

    public int DrawCount => drawPile.Count;
    public int DiscardCount => discardPile.Count;

    public Deck(IEnumerable<CardData> cards)
    {
        drawPile = new List<CardData>(cards);
        Shuffle(drawPile);
    }

    /// <summary>Draws the top card. Reshuffles the discard pile if the deck is empty. Returns null if nothing is left.</summary>
    public CardData Draw()
    {
        if (drawPile.Count == 0)
        {
            if (discardPile.Count == 0) return null;

            drawPile.AddRange(discardPile);
            discardPile.Clear();
            Shuffle(drawPile);
        }

        int last = drawPile.Count - 1;
        CardData card = drawPile[last];
        drawPile.RemoveAt(last);
        return card;
    }

    public void Discard(CardData card) => discardPile.Add(card);

    // Fisher-Yates shuffle
    private static void Shuffle(List<CardData> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}