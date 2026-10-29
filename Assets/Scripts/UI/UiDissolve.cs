using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dissolves every Image under this object and fades every text.
/// Used for played cards (destroyed afterwards) and defeated portraits (left invisible).
/// </summary>
public class UiDissolve : MonoBehaviour
{
    private static readonly int DissolveId = Shader.PropertyToID("_Dissolve");
    private static readonly int EdgeColorId = Shader.PropertyToID("_EdgeColor");
    private static readonly int NoiseOriginId = Shader.PropertyToID("_NoiseOrigin");
    private static readonly int NoiseScaleId = Shader.PropertyToID("_NoiseScale");

    private const float NoiseCellsAcrossObject = 4f;   // how coarse the burn pattern is
    private const float TextFadeFraction = 0.5f;       // texts fade out during the first part of the effect

    private Material materialCopy;

    public void Play(Material dissolveMaterial, Color edgeColor, float duration, bool destroyWhenDone)
    {
        if (materialCopy != null) return;   // already running

        materialCopy = new Material(dissolveMaterial);
        materialCopy.SetFloat(DissolveId, 0f);
        materialCopy.SetColor(EdgeColorId, edgeColor);

        // Noise is measured in world units, so scale it to this object's size.
        var rect = (RectTransform)transform;
        float worldWidth = rect.TransformVector(Vector3.right * rect.rect.width).magnitude;
        materialCopy.SetVector(NoiseOriginId, rect.TransformPoint(rect.rect.center));
        materialCopy.SetFloat(NoiseScaleId, worldWidth > 0f ? NoiseCellsAcrossObject / worldWidth : 1f);

        foreach (Image image in GetComponentsInChildren<Image>(true))
            image.material = materialCopy;

        StartCoroutine(Run(duration, destroyWhenDone));
    }

    private IEnumerator Run(float duration, bool destroyWhenDone)
    {
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            materialCopy.SetFloat(DissolveId, Mathf.SmoothStep(0f, 1f, t));
            SetTextAlpha(texts, 1f - Mathf.Clamp01(t / TextFadeFraction));
            yield return null;
        }

        materialCopy.SetFloat(DissolveId, 1f);
        SetTextAlpha(texts, 0f);

        if (destroyWhenDone)
            Destroy(gameObject);
    }

    private static void SetTextAlpha(TMP_Text[] texts, float alpha)
    {
        foreach (TMP_Text text in texts)
            text.alpha = alpha;
    }

    private void OnDestroy()
    {
        if (materialCopy != null)
            Destroy(materialCopy);
    }
}
