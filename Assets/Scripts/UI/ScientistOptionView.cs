using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScientistOptionView : MonoBehaviour
{
    private const float OutlineThickness = 3f;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Button button;
    [Tooltip("An Outline component on the card background. Tinted with the scientist's accent color.")]
    [SerializeField] private Outline accentOutline;

    private SelectionGlow glow;

    public ScientistData Data { get; private set; }

    private void Awake() => button.onClick.AddListener(OnClicked);

    public void Bind(ScientistData data)
    {
        Data = data;
        nameText.text = data.ScientistName;
        healthText.text = $"{data.MaxHealth} HP";

        portraitImage.sprite = data.Portrait;
        portraitImage.enabled = data.Portrait != null;
        portraitImage.preserveAspect = true;

        if (accentOutline != null)
        {
            accentOutline.effectColor = data.AccentColor;
            accentOutline.effectDistance = new Vector2(OutlineThickness, -OutlineThickness);
        }

        // Hover glow in the scientist's accent color (created by script, no prefab change).
        if (glow == null) glow = gameObject.AddComponent<SelectionGlow>();
        glow.SetColor(data.AccentColor);
    }

    private void OnClicked() => GameEvents.RaiseScientistChosen(Data);
}
