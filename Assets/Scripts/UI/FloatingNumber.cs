using TMPro;
using UnityEngine;

/// <summary>A number that pops, floats upward and fades out, then removes itself. Create it with Spawn.</summary>
public class FloatingNumber : MonoBehaviour
{
    private const float Duration = 0.9f;
    private const float RiseDistance = 90f;   // in canvas units

    private TMP_Text label;
    private Vector3 startPosition;
    private float age;

    /// <param name="parent">Use the root canvas, so layout groups never move the number.</param>
    /// <param name="fontSource">An existing text whose font we copy, so the number matches the game's look.</param>
    public static void Spawn(Transform parent, Vector3 worldPosition, TMP_Text fontSource, string text, Color color, float fontSize = 44f)
    {
        var go = new GameObject("FloatingNumber", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        go.transform.SetAsLastSibling();
        go.transform.position = worldPosition;

        var rect = (RectTransform)go.transform;
        rect.sizeDelta = new Vector2(300f, 80f);
        rect.localPosition += new Vector3(Random.Range(-40f, 40f), Random.Range(-10f, 30f), 0f);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.font = fontSource.font;
        tmp.fontSharedMaterial = fontSource.fontSharedMaterial;
        tmp.fontSize = fontSize;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;
        tmp.color = color;
        tmp.text = text;

        var number = go.AddComponent<FloatingNumber>();
        number.label = tmp;
        number.startPosition = rect.localPosition;
    }

    private void Update()
    {
        age += Time.unscaledDeltaTime;
        float n = Mathf.Clamp01(age / Duration);

        float rise = 1f - Mathf.Pow(1f - n, 3f);   // fast at first, then slows down
        transform.localPosition = startPosition + Vector3.up * (RiseDistance * rise);

        // Quick pop up to 1.3x, then settle at 1x.
        float pop = n < 0.15f
            ? Mathf.Lerp(0.5f, 1.3f, n / 0.15f)
            : Mathf.Lerp(1.3f, 1f, (n - 0.15f) / 0.25f);
        transform.localScale = Vector3.one * pop;

        Color c = label.color;
        c.a = 1f - Mathf.Clamp01((n - 0.6f) / 0.4f);   // fade out during the last 40%
        label.color = c;

        if (n >= 1f) Destroy(gameObject);
    }
}
