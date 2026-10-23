using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnHUDView : MonoBehaviour
{
    [SerializeField] private GameManager game;
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private Button endTurnButton;

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
        if (game == null || !game.IsStarted) return;

        if (game.IsGameOver)
        {
            turnText.text = "Game Over";
            energyText.text = "";
            endTurnButton.interactable = false;
            return;
        }

        PlayerState current = game.GetPlayer(game.CurrentPlayerIndex);
        turnText.text = $"Player {game.CurrentPlayerIndex + 1}'s turn";
        energyText.text = $"Energy {current.Energy} / {current.MaxEnergy}";
        endTurnButton.interactable = true;
    }
}