using UnityEngine;

/// <summary>
/// The single source of truth for the game's colors.
/// UI, shaders and particle scripts read from this asset instead of hardcoding colors.
/// </summary>
[CreateAssetMenu(menuName = "Science Clash/UI Palette", fileName = "UiPalette")]
public class UiPalette : ScriptableObject
{
    [Header("Surfaces")]
    [SerializeField] private Color background = new Color32(0x0B, 0x12, 0x20, 0xFF);
    [SerializeField] private Color panel = new Color32(0x13, 0x1C, 0x2E, 0xFF);

    [Header("Accents")]
    [SerializeField] private Color primary = new Color32(0x3D, 0xD9, 0xFF, 0xFF);
    [SerializeField] private Color secondary = new Color32(0xFF, 0xB3, 0x47, 0xFF);

    [Header("Meaning")]
    [SerializeField] private Color damage = new Color32(0xFF, 0x4D, 0x6D, 0xFF);
    [SerializeField] private Color heal = new Color32(0x5C, 0xFF, 0xA8, 0xFF);

    [Header("Text")]
    [SerializeField] private Color text = new Color32(0xE8, 0xF1, 0xFF, 0xFF);
    [SerializeField] private Color mutedText = new Color32(0x8A, 0x9B, 0xB8, 0xFF);

    public Color Background => background;
    public Color Panel => panel;
    public Color Primary => primary;
    public Color Secondary => secondary;
    public Color Damage => damage;
    public Color Heal => heal;
    public Color Text => text;
    public Color MutedText => mutedText;
}
