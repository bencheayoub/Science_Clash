using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    private const float AppearDuration = 0.3f;
    private const float AppearStartScale = 0.3f;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image artworkImage;
    [SerializeField] private Button button;

    private CardHolo holo;
    private CanvasGroup canvasGroup;
    private Coroutine appearRoutine;

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

    /// <summary>Turns on the hover foil + tilt for this card. Does nothing if no material is given.</summary>
    public void EnableHolo(Material holoMaterial, Color tint)
    {
        if (holoMaterial == null) return;

        if (!TryGetComponent(out holo))
            holo = gameObject.AddComponent<CardHolo>();
        holo.Setup(holoMaterial, tint);
    }

    /// <summary>The card pops in (small and transparent, then bounces to full size) after an optional delay.</summary>
    public void PlayAppear(float delay)
    {
        if (!isActiveAndEnabled) return;

        ApplyAppear(AppearStartScale, 0f);   // hidden right away, so there is no one-frame flash
        appearRoutine = StartCoroutine(AppearRoutine(delay));
    }

    private IEnumerator AppearRoutine(float delay)
    {
        for (float t = 0f; t < delay; t += Time.unscaledDeltaTime)
            yield return null;

        for (float t = 0f; t < AppearDuration; t += Time.unscaledDeltaTime)
        {
            float n = t / AppearDuration;
            float scale = Mathf.LerpUnclamped(AppearStartScale, 1f, EaseOutBack(n));
            ApplyAppear(scale, Mathf.Clamp01(n * 2f));
            yield return null;
        }

        ApplyAppear(1f, 1f);
        appearRoutine = null;
    }

    private void ApplyAppear(float scale, float alpha)
    {
        if (canvasGroup == null && !TryGetComponent(out canvasGroup))
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = alpha;

        if (holo != null) holo.SetAppearScale(scale);   // the foil owns the card's scale
        else transform.localScale = Vector3.one * scale;
    }

    // Overshoots a little past 1 and settles back: the "bounce".
    private static float EaseOutBack(float n)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        float m = n - 1f;
        return 1f + c3 * m * m * m + c1 * m * m;
    }

    /// <summary>Takes the card out of play: it can't be clicked, leaves the layout, and dissolves away.</summary>
    public void PlayDissolve(Material dissolveMaterial, Color edgeColor, float duration)
    {
        button.interactable = false;

        // If the card was still popping in, finish that instantly.
        if (appearRoutine != null)
        {
            StopCoroutine(appearRoutine);
            appearRoutine = null;
            ApplyAppear(1f, 1f);
        }

        // The foil must stop first, so it never fights the dissolve material.
        if (holo != null) holo.Freeze();

        // Stay exactly where we are while the other cards re-arrange.
        if (!TryGetComponent(out LayoutElement layoutElement))
            layoutElement = gameObject.AddComponent<LayoutElement>();
        layoutElement.ignoreLayout = true;
        transform.SetAsLastSibling();

        gameObject.AddComponent<UiDissolve>().Play(dissolveMaterial, edgeColor, duration, destroyWhenDone: true);
    }

    private void OnClicked() => GameEvents.RaiseCardClicked(Data);
}
