using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnHUDView : MonoBehaviour
{
    [SerializeField] private GameManager game;
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private int humanPlayerIndex = 0;

    private int lastEnergy = -1;
    private string lastTurnLabel;

    private void Awake() => endTurnButton.onClick.AddListener(GameEvents.RaiseEndTurnClicked);

    private void OnEnable()
    {
        GameEvents.OnGameStarted += Refresh;
        GameEvents.OnTurnStarted += HandleTurnStarted;
        GameEvents.OnCardPlayed += HandleCardPlayed;
        GameEvents.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStarted -= Refresh;
        GameEvents.OnTurnStarted -= HandleTurnStarted;
        GameEvents.OnCardPlayed -= HandleCardPlayed;
        GameEvents.OnGameOver -= HandleGameOver;
    }

    private void HandleTurnStarted(int player) => Refresh();
    private void HandleCardPlayed(int player, CardData card) => Refresh();
    private void HandleGameOver(int winner) => Refresh();

    private void Refresh()
    {
        bool humanTurn = game.CurrentPlayerIndex == humanPlayerIndex;
        PlayerState current = game.GetPlayer(game.CurrentPlayerIndex);

        string turnLabel = humanTurn ? "Your turn" : "Computer's turn";
        turnText.text = turnLabel;
        energyText.text = $"Energy {current.Energy} / {current.MaxEnergy}";
        endTurnButton.interactable = humanTurn;

        // Juice: pop the text that just changed.
        if (turnLabel != lastTurnLabel) Punch(turnText, 0.2f);
        if (current.Energy != lastEnergy) Punch(energyText, 0.3f);
        lastTurnLabel = turnLabel;
        lastEnergy = current.Energy;
    }

    private static void Punch(TMP_Text text, float strength)
    {
        if (!text.TryGetComponent(out UiPunch punch))
            punch = text.gameObject.AddComponent<UiPunch>();
        punch.Play(strength);
    }
}
