using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Feeds the palette colors into the BackgroundFX material.
/// Works on a private copy of the material so the shared asset is never modified.
/// </summary>
[RequireComponent(typeof(Image))]
public class BackgroundFxController : MonoBehaviour
{
    private static readonly int PrimaryColorId = Shader.PropertyToID("_PrimaryColor");
    private static readonly int SecondaryColorId = Shader.PropertyToID("_SecondaryColor");

    [SerializeField] private UiPalette palette;

    private Material materialCopy;

    private void Awake()
    {
        Image image = GetComponent<Image>();
        materialCopy = new Material(image.material);
        image.material = materialCopy;

        if (palette == null) return;

        materialCopy.SetColor(PrimaryColorId, palette.Primary);
        materialCopy.SetColor(SecondaryColorId, palette.Secondary);
    }

    private void OnDestroy()
    {
        if (materialCopy != null)
            Destroy(materialCopy);
    }
}
