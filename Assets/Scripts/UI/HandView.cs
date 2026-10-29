using UnityEngine;

public class HandView : MonoBehaviour
{
    [SerializeField] private GameManager game;
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private int viewedPlayer = 0;

    [Header("Play effect")]
    [SerializeField] private Material dissolveMaterial;
    [SerializeField, Min(0.1f)] private float dissolveDuration = 0.6f;

    private void OnEnable()
    {
        GameEvents.OnGameStarted += Refresh;
        GameEvents.OnTurnStarted += HandleTurnStarted;
        GameEvents.OnCardDrawn += HandleCardDrawn;
        GameEvents.OnCardPlayStarted += HandleCardPlayStarted;
        GameEvents.OnCardPlayed += HandleCardPlayed;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStarted -= Refresh;
        GameEvents.OnTurnStarted -= HandleTurnStarted;
        GameEvents.OnCardDrawn -= HandleCardDrawn;
        GameEvents.OnCardPlayStarted -= HandleCardPlayStarted;
        GameEvents.OnCardPlayed -= HandleCardPlayed;
    }

    private void HandleTurnStarted(int player) => Refresh();
    private void HandleCardDrawn(int player, CardData card) => Refresh();
    private void HandleCardPlayed(int player, CardData card) => Refresh();

    // The card is still on screen at this moment: keep it and let it dissolve.
    private void HandleCardPlayStarted(int player, CardData card)
    {
        if (player != viewedPlayer || dissolveMaterial == null) return;

        foreach (Transform child in container)
        {
            if (child.TryGetComponent(out UiDissolve _)) continue;   // already dissolving
            if (!child.TryGetComponent(out CardView view) || view.Data != card) continue;

            view.PlayDissolve(dissolveMaterial, AccentColorOf(player), dissolveDuration);
            return;   // only one copy of the card is played
        }
    }

    // Rebuilds the visible hand of whoever's turn it is (hot-seat)
    private void Refresh()
    {
        if (game == null || !game.IsStarted) return;

        foreach (Transform child in container)
        {
            if (child.TryGetComponent(out UiDissolve _)) continue;   // let dissolving cards finish
            Destroy(child.gameObject);
        }

        Hand hand = game.GetPlayer(viewedPlayer).Hand;
        foreach (CardData card in hand.Cards)
        {
            CardView view = Instantiate(cardPrefab, container);
            view.Bind(card);
        }
    }

    private Color AccentColorOf(int player)
    {
        Scientist scientist = game.GetPlayer(player).ActiveScientist;
        return scientist != null ? scientist.Data.AccentColor : Color.white;
    }
}
