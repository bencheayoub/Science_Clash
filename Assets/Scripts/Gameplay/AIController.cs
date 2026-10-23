using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private GameManager game;
    [SerializeField] private int aiPlayerIndex = 1;
    [SerializeField, Min(0.1f)] private float thinkDelay = 1f;

    private void OnEnable() => GameEvents.OnTurnStarted += HandleTurnStarted;
    private void OnDisable() => GameEvents.OnTurnStarted -= HandleTurnStarted;

    private void HandleTurnStarted(int player)
    {
        if (player == aiPlayerIndex)
            StartCoroutine(PlayTurn());
    }

    private IEnumerator PlayTurn()
    {
        yield return new WaitForSeconds(thinkDelay);

        while (!game.IsGameOver)
        {
            CardData card = ChooseCard();
            if (card == null) break;                                    // nothing useful left
            if (!game.TryPlayCard(aiPlayerIndex, card)) break;          // safety: never loop forever
            yield return new WaitForSeconds(thinkDelay);
        }

        if (!game.IsGameOver)
            game.EndTurn();
    }

    // Simple heuristic: heal only when hurt, otherwise play the most expensive affordable card.
    private CardData ChooseCard()
    {
        PlayerState me = game.GetPlayer(aiPlayerIndex);
        Scientist self = me.ActiveScientist;
        if (self == null) return null;

        bool hurt = self.CurrentHealth <= self.MaxHealth * 0.6f;
        CardData best = null;
        int bestScore = int.MinValue;

        foreach (CardData card in me.Hand.Cards)
        {
            if (card.Cost > me.Energy) continue;

            bool isHeal = card.TargetType == CardTargetType.Self;
            if (isHeal && !hurt) continue;

            int score = card.Cost + (isHeal ? 100 : 0);   // when hurt, healing comes first
            if (score > bestScore)
            {
                best = card;
                bestScore = score;
            }
        }
        return best;
    }
}