using UnityEngine;
using UnityEngine.UI;

/// <summary>One flash of lightning between two points. Create it with Spawn; it fades and removes itself.</summary>
public class LightningBolt : MonoBehaviour
{
    private static readonly int IntensityId = Shader.PropertyToID("_Intensity");
    private static readonly int SeedId = Shader.PropertyToID("_Seed");
    private static readonly int ThicknessId = Shader.PropertyToID("_Thickness");

    private const float Lifetime = 0.3f;
    private const float Height = 260f;   // canvas units: how much room the bolt has to zigzag

    private Material materialCopy;
    private float age;

    /// <param name="parent">Use the root canvas, so layout groups never move the bolt.</param>
    public static void Spawn(Transform parent, Material material, Vector3 fromWorld, Vector3 toWorld, Color color, bool heavy)
    {
        var go = new GameObject("LightningBolt", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        go.transform.SetAsLastSibling();

        // A long thin rectangle that starts at "from" and points at "to".
        Vector3 delta = toWorld - fromWorld;
        float canvasScale = parent.lossyScale.x > 0f ? parent.lossyScale.x : 1f;

        var rect = (RectTransform)go.transform;
        rect.pivot = new Vector2(0f, 0.5f);
        rect.position = fromWorld;
        rect.sizeDelta = new Vector2(delta.magnitude / canvasScale, Height);
        rect.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);

        // RawImage (not Image) so the shader receives a clean 0..1 UV across the whole rectangle.
        var image = go.AddComponent<RawImage>();
        image.raycastTarget = false;
        image.color = color;

        Material copy = new Material(material);
        copy.SetFloat(SeedId, Random.Range(0f, 100f));
        copy.SetFloat(ThicknessId, heavy ? 0.035f : 0.02f);
        image.material = copy;

        go.AddComponent<LightningBolt>().materialCopy = copy;
    }

    private void Update()
    {
        age += Time.unscaledDeltaTime;
        float n = Mathf.Clamp01(age / Lifetime);

        float fade = (1f - n) * (1f - n);
        float flicker = 0.75f + 0.25f * Random.value;
        materialCopy.SetFloat(IntensityId, fade * flicker);

        if (n >= 1f) Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (materialCopy != null) Destroy(materialCopy);
    }
}
