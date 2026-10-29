using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScientistPanelView : MonoBehaviour
{
    private const float OutlineThickness = 3f;

    [SerializeField] private GameManager game;
    [SerializeField] private int playerIndex;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image portraitImage;

    [Header("Look")]
    [SerializeField] private UiPalette palette;
    [Tooltip("Optional: an Outline component on the portrait frame, tinted with the scientist's accent color.")]
    [SerializeField] private Outline accentOutline;

    [Header("Defeat effect")]
    [SerializeField] private Material dissolveMaterial;
    [SerializeField, Min(0.1f)] private float dissolveDuration = 1.2f;

    private bool defeatPlayed;

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

    private void HandleDefeated(ICombatant who)
    {
        Refresh();

        if (defeatPlayed || game == null || !game.IsStarted) return;
        if (!ReferenceEquals(who, GetScientist())) return;   // someone else's scientist

        PlayDefeatEffect();
    }

    private Scientist GetScientist()
    {
        PlayerState player = game.GetPlayer(playerIndex);
        return player.ActiveScientist ?? player.Scientists[0];
    }

    private void Refresh()
    {
        if (game == null || !game.IsStarted) return;

        Scientist scientist = GetScientist();

        nameText.text = scientist.DisplayName;
        healthText.text = scientist.IsAlive
            ? $"{scientist.CurrentHealth} / {scientist.MaxHealth}"
            : "DEFEATED";

        float ratio = (float)scientist.CurrentHealth / scientist.MaxHealth;
        healthBar.value = ratio;
        TintHealthBar(ratio);

        portraitImage.sprite = scientist.Data.Portrait;
        portraitImage.preserveAspect = true;
        portraitImage.enabled = scientist.Data.Portrait != null;

        if (accentOutline != null)
        {
            accentOutline.effectColor = scientist.Data.AccentColor;
            accentOutline.effectDistance = new Vector2(OutlineThickness, -OutlineThickness);
        }
    }

    // Red when low, green when full, using the palette's damage and heal colors.
    private void TintHealthBar(float ratio)
    {
        if (palette == null || healthBar.fillRect == null) return;

        Image fill = healthBar.fillRect.GetComponent<Image>();
        if (fill != null)
            fill.color = Color.Lerp(palette.Damage, palette.Heal, ratio);
    }

    private void PlayDefeatEffect()
    {
        if (dissolveMaterial == null) return;

        defeatPlayed = true;
        portraitImage.gameObject.AddComponent<UiDissolve>()
            .Play(dissolveMaterial, GetScientist().Data.AccentColor, dissolveDuration, destroyWhenDone: false);
    }
}
