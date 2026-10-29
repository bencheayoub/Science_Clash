using UnityEngine;
using UnityEngine.UI;

/// <summary>A quick white flash over the whole screen, like a thunderclap. Removes itself.</summary>
public class ScreenFlash : MonoBehaviour
{
    private Image image;
    private float peakAlpha;
    private float duration;
    private float age;

    public static void Spawn(Transform parent, float peakAlpha = 0.35f, float duration = 0.18f)
    {
        var go = new GameObject("ScreenFlash", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        go.transform.SetAsLastSibling();

        var rect = (RectTransform)go.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var image = go.AddComponent<Image>();
        image.raycastTarget = false;   // never blocks clicks
        image.color = new Color(1f, 1f, 1f, peakAlpha);

        var flash = go.AddComponent<ScreenFlash>();
        flash.image = image;
        flash.peakAlpha = peakAlpha;
        flash.duration = duration;
    }

    private void Update()
    {
        age += Time.unscaledDeltaTime;
        float n = Mathf.Clamp01(age / duration);

        Color c = image.color;
        c.a = peakAlpha * (1f - n) * (1f - n);
        image.color = c;

        if (n >= 1f) Destroy(gameObject);
    }
}
