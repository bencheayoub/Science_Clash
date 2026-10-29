using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScientistPanelView : MonoBehaviour
{
    private const float OutlineThickness = 3f;
    private const float BarSpeed = 6f;          // how fast the health bar slides to its new value
    private const float FlashDuration = 0.35f;  // portrait tint after a hit or a heal

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
    private bool barInitialized;
    private float targetRatio = 1f;
    private float shownRatio = 1f;
    private float flashTimer;
    private Color flashColor = Color.white;

    private Color DamageColor => palette != null ? palette.Damage : Color.red;
    private Color HealColor => palette != null ? palette.Heal : Color.green;

    private void OnEnable()
    {
        GameEvents.OnGameStarted += HandleGameStarted;
        GameEvents.OnTurnStarted += HandleTurnStarted;
        GameEvents.OnDamageDealt += HandleDamageDealt;
        GameEvents.OnHealed += HandleHealed;
        GameEvents.OnScientistDefeated += HandleDefeated;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStarted -= HandleGameStarted;
        GameEvents.OnTurnStarted -= HandleTurnStarted;
        GameEvents.OnDamageDealt -= HandleDamageDealt;
        GameEvents.OnHealed -= HandleHealed;
        GameEvents.OnScientistDefeated -= HandleDefeated;
    }

    private void HandleGameStarted() => Refresh(snapBar: true);
    private void HandleTurnStarted(int player) => Refresh();

    private void HandleDamageDealt(ICombatant target, int amount)
    {
        Refresh();
        if (amount > 0 && IsMine(target)) ShowFeedback(DamageColor, $"-{amount}");
    }

    private void HandleHealed(ICombatant target, int amount)
    {
        Refresh();
        if (amount > 0 && IsMine(target)) ShowFeedback(HealColor, $"+{amount}");
    }

    private void HandleDefeated(ICombatant who)
    {
        Refresh();

        if (defeatPlayed || game == null || !game.IsStarted) return;
        if (!IsMine(who)) return;   // someone else's scientist

        PlayDefeatEffect();
    }

    private bool IsMine(ICombatant target) =>
        game != null && game.IsStarted && ReferenceEquals(target, GetScientist());

    private Scientist GetScientist()
    {
        PlayerState player = game.GetPlayer(playerIndex);
        return player.ActiveScientist ?? player.Scientists[0];
    }

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;

        // The bar slides toward the real value instead of jumping.
        if (!Mathf.Approximately(shownRatio, targetRatio))
        {
            shownRatio = Mathf.Lerp(shownRatio, targetRatio, 1f - Mathf.Exp(-BarSpeed * dt));
            if (Mathf.Abs(shownRatio - targetRatio) < 0.002f) shownRatio = targetRatio;

            healthBar.value = shownRatio;
            TintHealthBar(shownRatio);
        }

        // The portrait is tinted, then fades back to normal.
        if (flashTimer > 0f)
        {
            flashTimer -= dt;
            float k = Mathf.Clamp01(flashTimer / FlashDuration);
            portraitImage.color = Color.Lerp(Color.white, flashColor, k);
        }
    }

    private void Refresh(bool snapBar = false)
    {
        if (game == null || !game.IsStarted) return;

        Scientist scientist = GetScientist();

        nameText.text = scientist.DisplayName;
        healthText.text = scientist.IsAlive
            ? $"{scientist.CurrentHealth} / {scientist.MaxHealth}"
            : "DEFEATED";

        targetRatio = (float)scientist.CurrentHealth / scientist.MaxHealth;
        if (snapBar || !barInitialized)
        {
            barInitialized = true;
            shownRatio = targetRatio;
            healthBar.value = shownRatio;
            TintHealthBar(shownRatio);
        }

        portraitImage.sprite = scientist.Data.Portrait;
        portraitImage.preserveAspect = true;
        portraitImage.enabled = scientist.Data.Portrait != null;

        if (accentOutline != null)
        {
            accentOutline.effectColor = scientist.Data.AccentColor;
            accentOutline.effectDistance = new Vector2(OutlineThickness, -OutlineThickness);
        }
    }

    private void ShowFeedback(Color color, string label)
    {
        if (defeatPlayed) return;

        flashColor = color;
        flashTimer = FlashDuration;

        Canvas canvas = portraitImage.canvas;
        if (canvas == null) return;

        FloatingNumber.Spawn(canvas.rootCanvas.transform, portraitImage.rectTransform.position, healthText, label, color);
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
