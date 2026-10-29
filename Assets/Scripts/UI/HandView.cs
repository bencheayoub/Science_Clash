using UnityEngine;

public class HandView : MonoBehaviour
{
    private const float StaggerDelay = 0.07f;   // seconds between cards when the whole hand pops in

    private enum PopIn { None, WholeHand, NewestCard }

    [SerializeField] private GameManager game;
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private int viewedPlayer = 0;

    [Header("Play effect")]
    [SerializeField] private Material dissolveMaterial;
    [SerializeField, Min(0.1f)] private float dissolveDuration = 0.6f;

    [Header("Hover effect")]
    [SerializeField] private Material holoMaterial;

    private void OnEnable()
    {
        GameEvents.OnGameStarted += HandleGameStarted;
        GameEvents.OnTurnStarted += HandleTurnStarted;
        GameEvents.OnCardDrawn += HandleCardDrawn;
        GameEvents.OnCardPlayStarted += HandleCardPlayStarted;
        GameEvents.OnCardPlayed += HandleCardPlayed;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStarted -= HandleGameStarted;
        GameEvents.OnTurnStarted -= HandleTurnStarted;
        GameEvents.OnCardDrawn -= HandleCardDrawn;
        GameEvents.OnCardPlayStarted -= HandleCardPlayStarted;
        GameEvents.OnCardPlayed -= HandleCardPlayed;
    }

    private void HandleGameStarted() => Refresh(PopIn.WholeHand);

    private void HandleTurnStarted(int player) =>
        Refresh(player == viewedPlayer ? PopIn.WholeHand : PopIn.None);

    private void HandleCardDrawn(int player, CardData card) =>
        Refresh(player == viewedPlayer ? PopIn.NewestCard : PopIn.None);

    // After a play the hand just rebuilds quietly: the played card already dissolved.
    private void HandleCardPlayed(int player, CardData card) => Refresh(PopIn.None);

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
    private void Refresh(PopIn popIn)
    {
        if (game == null || !game.IsStarted) return;

        foreach (Transform child in container)
        {
            if (child.TryGetComponent(out UiDissolve _)) continue;   // let dissolving cards finish
            Destroy(child.gameObject);
        }

        Color accent = AccentColorOf(viewedPlayer);

        CardView lastView = null;
        int index = 0;

        Hand hand = game.GetPlayer(viewedPlayer).Hand;
        foreach (CardData card in hand.Cards)
        {
            CardView view = Instantiate(cardPrefab, container);
            view.Bind(card);
            view.EnableHolo(holoMaterial, accent);   // foil in the scientist's accent color

            if (popIn == PopIn.WholeHand)
                view.PlayAppear(index * StaggerDelay);

            lastView = view;
            index++;
        }

        // A single draw: only the card that just arrived pops in.
        if (popIn == PopIn.NewestCard && lastView != null)
            lastView.PlayAppear(0f);
    }

    private Color AccentColorOf(int player)
    {
        Scientist scientist = game.GetPlayer(player).ActiveScientist;
        return scientist != null ? scientist.Data.AccentColor : Color.white;
    }
}
