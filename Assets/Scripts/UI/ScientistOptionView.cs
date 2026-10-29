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
    }

    private void OnClicked() => GameEvents.RaiseScientistChosen(Data);
}
