using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Soft pulsing glow around a UI element while the pointer is over it.
// Added by script (e.g. from ScientistOptionView), so no prefab editing is needed.
public class SelectionGlow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const int TextureSize = 64;
    private const int GlowSize = 16;   // how far the glow reaches outside the card, in pixels

    private static Sprite glowSprite;

    [SerializeField, Min(1f)] private float hoverScale = 1.05f;
    [SerializeField, Min(1f)] private float fadeSpeed = 8f;
    [SerializeField, Min(0f)] private float pulseSpeed = 4f;
    [SerializeField, Range(0f, 1f)] private float pulseDepth = 0.25f;

    private Image glowImage;
    private Color glowColor = Color.white;
    private float amount;   // 0 = idle, 1 = fully hovered
    private bool hovered;

    private void Awake() => BuildGlowImage();

    private void OnDisable()
    {
        hovered = false;
        amount = 0f;
        ApplyVisuals();
    }

    public void SetColor(Color color) => glowColor = color;

    public void OnPointerEnter(PointerEventData eventData) => hovered = true;
    public void OnPointerExit(PointerEventData eventData) => hovered = false;

    private void Update()
    {
        // Unscaled time so it still works while the game is paused.
        amount = Mathf.MoveTowards(amount, hovered ? 1f : 0f, fadeSpeed * Time.unscaledDeltaTime);
        ApplyVisuals();
    }

    private void ApplyVisuals()
    {
        float pulse = 1f - pulseDepth + pulseDepth * Mathf.Sin(Time.unscaledTime * pulseSpeed);
        Color c = glowColor;
        c.a = amount * pulse;

        glowImage.color = c;
        glowImage.enabled = amount > 0.001f;

        transform.localScale = Vector3.one * Mathf.Lerp(1f, hoverScale, amount);
    }

    private void BuildGlowImage()
    {
        GameObject go = new GameObject("Glow", typeof(RectTransform), typeof(LayoutElement));
        go.transform.SetParent(transform, false);
        go.transform.SetAsFirstSibling();
        go.GetComponent<LayoutElement>().ignoreLayout = true;   // don't let layout groups move it

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(-GlowSize, -GlowSize);
        rect.offsetMax = new Vector2(GlowSize, GlowSize);

        glowImage = go.AddComponent<Image>();
        glowImage.sprite = GetGlowSprite();
        glowImage.type = Image.Type.Sliced;
        glowImage.fillCenter = false;        // only the outer ring, so the card stays visible
        glowImage.raycastTarget = false;     // never blocks clicks
    }

    // Built once in code: a white square ring that fades out toward its outer edge.
    private static Sprite GetGlowSprite()
    {
        if (glowSprite != null) return glowSprite;

        Texture2D tex = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        int innerMin = GlowSize;
        int innerMax = TextureSize - GlowSize - 1;

        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                float dx = Mathf.Max(innerMin - x, 0, x - innerMax);
                float dy = Mathf.Max(innerMin - y, 0, y - innerMax);
                float distance = Mathf.Sqrt(dx * dx + dy * dy);

                float alpha = Mathf.Clamp01(1f - distance / GlowSize);
                alpha *= alpha;   // softer falloff

                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        tex.Apply();

        Vector4 border = new Vector4(GlowSize, GlowSize, GlowSize, GlowSize);
        glowSprite = Sprite.Create(tex, new Rect(0, 0, TextureSize, TextureSize),
            new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
        return glowSprite;
    }
}
