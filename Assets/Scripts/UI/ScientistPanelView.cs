using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScientistPanelView : MonoBehaviour
{
    [SerializeField] private GameManager game;
    [SerializeField] private int playerIndex;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image portraitImage;

    private void OnEnable()
    {
        GameEvents.OnGameStarted += Refresh;
        GameEvents.OnTurnStarted += HandleTurnStarted;
        GameEvents.OnDamageDealt += HandleDamageDealt;
        GameEvents.OnHealed += HandleHealed;
        GameEvents.OnScientistDefeated += HandleDefeated;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStarted -= Refresh;
        GameEvents.OnTurnStarted -= HandleTurnStarted;
        GameEvents.OnDamageDealt -= HandleDamageDealt;
        GameEvents.OnHealed -= HandleHealed;
        GameEvents.OnScientistDefeated -= HandleDefeated;
    }

    private void HandleTurnStarted(int player) => Refresh();
    private void HandleDamageDealt(ICombatant target, int amount) => Refresh();
    private void HandleHealed(ICombatant target, int amount) => Refresh();
    private void HandleDefeated(ICombatant who) => Refresh();

    private void Refresh()
    {
        if (game == null || !game.IsStarted) return;

        PlayerState player = game.GetPlayer(playerIndex);
        Scientist scientist = player.ActiveScientist ?? player.Scientists[0];

        nameText.text = scientist.DisplayName;
        healthText.text = scientist.IsAlive
            ? $"{scientist.CurrentHealth} / {scientist.MaxHealth}"
            : "DEFEATED";
        healthBar.value = (float)scientist.CurrentHealth / scientist.MaxHealth;
        portraitImage.sprite = scientist.Data.Portrait;
        portraitImage.preserveAspect = true;
        portraitImage.enabled = scientist.Data.Portrait != null;
    }
}