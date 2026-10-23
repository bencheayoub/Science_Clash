using UnityEngine;

/// <summary>Bridge between UI clicks (events) and the game rules.</summary>
public class CardInputController : MonoBehaviour
{
    [SerializeField] private GameManager game;

    private void OnEnable()
    {
        GameEvents.OnCardClicked += HandleCardClicked;
        GameEvents.OnEndTurnClicked += HandleEndTurnClicked;
    }

    private void OnDisable()
    {
        GameEvents.OnCardClicked -= HandleCardClicked;
        GameEvents.OnEndTurnClicked -= HandleEndTurnClicked;
    }

    private void HandleCardClicked(CardData card) => game.TryPlayCard(game.CurrentPlayerIndex, card);
    private void HandleEndTurnClicked() => game.EndTurn();
}