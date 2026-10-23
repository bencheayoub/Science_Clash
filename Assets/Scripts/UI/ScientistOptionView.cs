using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScientistOptionView : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Button button;

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
    }

    private void OnClicked() => GameEvents.RaiseScientistChosen(Data);
}