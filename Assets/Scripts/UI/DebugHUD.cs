using System.Collections.Generic;
using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    [SerializeField] private GameManager game;

    private void OnGUI()
    {
        if (game == null || !game.IsStarted) return;

        GUILayout.BeginArea(new Rect(10, 10, 600, 700));

        for (int p = 0; p < 2; p++)
        {
            PlayerState player = game.GetPlayer(p);
            Scientist sci = player.ActiveScientist;
            string hp = sci != null ? $"{sci.DisplayName} {sci.CurrentHealth}/{sci.MaxHealth}" : "DEFEATED";
            GUILayout.Label($"Player {p + 1}: {hp} | Energy {player.Energy}/{player.MaxEnergy} | Deck {player.Deck.DrawCount}");
        }

        if (game.IsGameOver)
        {
            GUILayout.Label($"GAME OVER - Player {game.WinnerIndex + 1} wins!");
        }
        else
        {
            int current = game.CurrentPlayerIndex;
            GUILayout.Label($"--- Turn: Player {current + 1} ---");

            var hand = new List<CardData>(game.GetPlayer(current).Hand.Cards);
            foreach (CardData card in hand)
            {
                if (GUILayout.Button($"{card.CardName} (cost {card.Cost}): {card.Description}"))
                {
                    game.TryPlayCard(current, card);
                    break;
                }
            }

            if (GUILayout.Button("END TURN"))
                game.EndTurn();
        }

        GUILayout.EndArea();
    }
}