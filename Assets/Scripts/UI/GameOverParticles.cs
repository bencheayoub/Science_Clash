using UnityEngine;

// Confetti when the player wins, falling embers when the computer wins.
public class GameOverParticles : MonoBehaviour
{
    [SerializeField] private UiParticles particles;
    [SerializeField] private UiPalette palette;

    private void OnEnable() => GameEvents.OnGameOver += HandleGameOver;
    private void OnDisable() => GameEvents.OnGameOver -= HandleGameOver;

    private void HandleGameOver(int winner)
    {
        if (particles == null || palette == null) return;

        if (winner == 0)
        {
            particles.Confetti(new[] { palette.Primary, palette.Secondary, palette.Heal, Color.white }, 120);
        }
        else
        {
            particles.Confetti(new[] { palette.Damage, new Color(0.35f, 0.1f, 0.1f) }, 50);
        }
    }
}
