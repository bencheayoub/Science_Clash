using UnityEngine;

public class HandView : MonoBehaviour
{
    [SerializeField] private GameManager game;
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private Transform container;

    private void OnEnable()
    {
        GameEvents.OnGameStarted += Refresh;
        GameEvents.OnTurnStarted += HandleTurnStarted;
        GameEvents.OnCardDrawn += HandleCardDrawn;
        GameEvents.OnCardPlayed += HandleCardPlayed;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStarted -= Refresh;
        GameEvents.OnTurnStarted -= HandleTurnStarted;
        GameEvents.OnCardDrawn -= HandleCardDrawn;
        GameEvents.OnCardPlayed -= HandleCardPlayed;
    }

    private void HandleTurnStarted(int player) => Refresh();
    private void HandleCardDrawn(int player, CardData card) => Refresh();
    private void HandleCardPlayed(int player, CardData card) => Refresh();

    // Rebuilds the visible hand of whoever's turn it is (hot-seat)
    private void Refresh()
    {
        if (game == null || !game.IsStarted) return;

        foreach (Transform child in container)
            Destroy(child.gameObject);

        Hand hand = game.GetPlayer(game.CurrentPlayerIndex).Hand;
        foreach (CardData card in hand.Cards)
        {
            CardView view = Instantiate(cardPrefab, container);
            view.Bind(card);
        }
    }
}