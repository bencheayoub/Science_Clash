using UnityEngine;

/// <summary>Bridge between UI clicks (events) and the game rules.</summary>
public class CardInputController : MonoBehaviour
{
    [SerializeField] private GameManager game;
    [SerializeField] private int humanPlayerIndex = 0; 
    
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

private void HandleCardClicked(CardData card) => game.TryPlayCard(humanPlayerIndex, card);

private void HandleEndTurnClicked()
{
    if (game.CurrentPlayerIndex == humanPlayerIndex)
        game.EndTurn();
}
}