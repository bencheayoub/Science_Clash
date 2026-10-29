using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image artworkImage;
    [SerializeField] private Button button;

    public CardData Data { get; private set; }

    private void Awake() => button.onClick.AddListener(OnClicked);

    public void Bind(CardData data)
    {
        artworkImage.preserveAspect = true;
        transform.localScale = Vector3.one;
        Data = data;
        nameText.text = data.CardName;
        costText.text = data.Cost.ToString();
        descriptionText.text = data.Description;

        artworkImage.sprite = data.Artwork;
        artworkImage.enabled = data.Artwork != null;   // hide the empty box if no art
    }

    /// <summary>Takes the card out of play: it can't be clicked, leaves the layout, and dissolves away.</summary>
    public void PlayDissolve(Material dissolveMaterial, Color edgeColor, float duration)
    {
        button.interactable = false;

        // Stay exactly where we are while the other cards re-arrange.
        if (!TryGetComponent(out LayoutElement layoutElement))
            layoutElement = gameObject.AddComponent<LayoutElement>();
        layoutElement.ignoreLayout = true;
        transform.SetAsLastSibling();

        gameObject.AddComponent<UiDissolve>().Play(dissolveMaterial, edgeColor, duration, destroyWhenDone: true);
    }

    private void OnClicked() => GameEvents.RaiseCardClicked(Data);
}
