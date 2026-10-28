using System.Collections;
using UnityEngine;

/// <summary>Timing only: waits, asks AIStrategy what to do, and acts through IMatchActions.</summary>
public class AIController : MonoBehaviour
{
    [Tooltip("Any component implementing IMatchActions (the GameManager).")]
    [SerializeField] private MonoBehaviour game;
    [SerializeField] private int aiPlayerIndex = 1;
    [SerializeField, Min(0.1f)] private float thinkDelay = 1f;

    private IMatchActions match;
    private readonly AIStrategy strategy = new AIStrategy();

    private void Awake()
    {
        match = game as IMatchActions;
        if (match == null)
            Debug.LogError("AIController: 'game' must reference a component that implements IMatchActions.", this);
    }

    private void OnEnable() => GameEvents.OnTurnStarted += HandleTurnStarted;
    private void OnDisable() => GameEvents.OnTurnStarted -= HandleTurnStarted;

    private void HandleTurnStarted(int player)
    {
        if (match != null && player == aiPlayerIndex)
            StartCoroutine(PlayTurn());
    }

    private IEnumerator PlayTurn()
    {
        yield return new WaitForSeconds(thinkDelay);

        while (!match.IsGameOver)
        {
            CardData card = strategy.ChooseCard(match.GetPlayer(aiPlayerIndex));
            if (card == null) break;                                    // nothing useful left
            if (!match.TryPlayCard(aiPlayerIndex, card)) break;         // safety: never loop forever
            yield return new WaitForSeconds(thinkDelay);
        }

        if (!match.IsGameOver)
            match.EndTurn();
    }
}
