using UnityEngine;
using UnityEngine.UI;

// Briefly splits the colors of the background image whenever someone takes damage.
// Works on a private copy of the material so the shared asset is never modified.
[RequireComponent(typeof(Image))]
public class HitAberration : MonoBehaviour
{
    private static readonly int StrengthId = Shader.PropertyToID("_Strength");

    [Tooltip("A material using the ScienceClash/UI/BackgroundAberration shader.")]
    [SerializeField] private Material aberrationMaterial;

    [Header("Feel")]
    [SerializeField, Min(0.05f)] private float duration = 0.3f;
    [SerializeField, Min(0f)] private float minStrength = 0.008f;
    [SerializeField, Min(0f)] private float maxStrength = 0.035f;
    [SerializeField, Min(0f)] private float strengthPerDamage = 0.003f;

    private Material materialCopy;
    private float peak;
    private float timeLeft;

    private void Awake()
    {
        if (aberrationMaterial == null)
        {
            Debug.LogWarning("HitAberration: assign the aberration material.", this);
            enabled = false;
            return;
        }

        materialCopy = new Material(aberrationMaterial);
        GetComponent<Image>().material = materialCopy;
    }

    private void OnEnable() => GameEvents.OnDamageDealt += HandleDamageDealt;
    private void OnDisable() => GameEvents.OnDamageDealt -= HandleDamageDealt;

    private void OnDestroy()
    {
        if (materialCopy != null)
            Destroy(materialCopy);
    }

    private void HandleDamageDealt(ICombatant target, int amount)
    {
        if (amount <= 0) return;

        float hit = Mathf.Clamp(amount * strengthPerDamage, minStrength, maxStrength);
        peak = Mathf.Max(CurrentStrength(), hit);   // a new hit never weakens a running effect
        timeLeft = duration;
    }

    private void Update()
    {
        if (timeLeft <= 0f) return;

        timeLeft -= Time.unscaledDeltaTime;
        materialCopy.SetFloat(StrengthId, timeLeft > 0f ? CurrentStrength() : 0f);
    }

    private float CurrentStrength()
    {
        if (timeLeft <= 0f) return 0f;
        float fade = timeLeft / duration;
        return peak * fade * fade;
    }
}
