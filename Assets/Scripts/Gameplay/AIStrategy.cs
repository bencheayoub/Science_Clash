/// <summary>Decides which card the AI plays. Pure logic: no Unity, no timing.</summary>
public class AIStrategy
{
    private const float HurtHealthRatio = 0.6f;
    private const int HealPriorityBonus = 100;

    // Heal only when hurt, otherwise play the most expensive affordable card.
    public CardData ChooseCard(PlayerState me)
    {
        Scientist self = me.ActiveScientist;
        if (self == null) return null;

        bool hurt = self.CurrentHealth <= self.MaxHealth * HurtHealthRatio;
        CardData best = null;
        int bestScore = int.MinValue;

        foreach (CardData card in me.Hand.Cards)
        {
            if (card.Cost > me.Energy) continue;

            bool isHeal = card.TargetType == CardTargetType.Self;
            if (isHeal && !hurt) continue;

            int score = card.Cost + (isHeal ? HealPriorityBonus : 0);   // when hurt, healing comes first
            if (score > bestScore)
            {
                best = card;
                bestScore = score;
            }
        }
        return best;
    }
}
