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

    turnText.text = humanTurn ? "Your turn" : "Computer's turn";
    energyText.text = $"Energy {current.Energy} / {current.MaxEnergy}";
    endTurnButton.interactable = humanTurn;
        }
}