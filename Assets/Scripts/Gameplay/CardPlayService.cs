public class CardPlayService
{
    private readonly PlayerState[] players;

    public CardPlayService(PlayerState[] players) => this.players = players;

    public bool TryPlay(int playerIndex, CardData card)
    {
        PlayerState player = players[playerIndex];
        PlayerState opponent = players[1 - playerIndex];

        Scientist source = player.ActiveScientist;
        Scientist target = card.TargetType == CardTargetType.Self ? source : opponent.ActiveScientist;
        if (source == null || target == null) return false;

        if (card.Cost > player.Energy) return false;
        if (!player.Hand.Remove(card)) return false;   // card must really be in hand
        player.TrySpendEnergy(card.Cost);

        GameEvents.RaiseCardPlayStarted(playerIndex, card);   // before effects, so presentation can lead the hit

        card.Resolve(new EffectContext(playerIndex, source, target));

        player.Deck.Discard(card);
        GameEvents.RaiseCardPlayed(playerIndex, card);
        return true;
    }
}
