using UnityEngine;

// Shakes a set of UI elements whenever someone takes damage.
// Bigger hits shake harder. Elements return to their exact original position afterwards.
public class HitShake : MonoBehaviour
{
    [Tooltip("The UI elements that should shake (panels, hand, HUD). Not the background, or its edges would show.")]
    [SerializeField] private RectTransform[] targets;

    [Header("Feel")]
    [SerializeField, Min(0.05f)] private float duration = 0.35f;
    [SerializeField, Min(0f)] private float minStrength = 4f;
    [SerializeField, Min(0f)] private float maxStrength = 22f;
    [SerializeField, Min(0f)] private float strengthPerDamage = 1.5f;

    private Vector2[] basePositions;
    private float timeLeft;
    private float strength;
    private bool shaking;

    private void OnEnable() => GameEvents.OnDamageDealt += HandleDamageDealt;

    private void OnDisable()
    {
        GameEvents.OnDamageDealt -= HandleDamageDealt;
        Restore();
    }

    private void HandleDamageDealt(ICombatant target, int amount)
    {
        if (amount <= 0 || targets == null || targets.Length == 0) return;

        if (!shaking) CaptureBasePositions();

        strength = Mathf.Clamp(amount * strengthPerDamage, minStrength, maxStrength);
        timeLeft = duration;
        shaking = true;
    }

    private void Update()
    {
        if (!shaking) return;

        timeLeft -= Time.unscaledDeltaTime;
        if (timeLeft <= 0f)
        {
            Restore();
            return;
        }

        // Shake fades out smoothly as time runs out.
        float fade = timeLeft / duration;
        float power = strength * fade * fade;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null) continue;
            targets[i].anchoredPosition = basePositions[i] + Random.insideUnitCircle * power;
        }
    }

    private void CaptureBasePositions()
    {
        basePositions = new Vector2[targets.Length];
        for (int i = 0; i < targets.Length; i++)
            if (targets[i] != null) basePositions[i] = targets[i].anchoredPosition;
    }

    private void Restore()
    {
        if (!shaking) return;

        for (int i = 0; i < targets.Length; i++)
            if (targets[i] != null) targets[i].anchoredPosition = basePositions[i];

        shaking = false;
    }
}
