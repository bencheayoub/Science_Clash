#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor tool: gives every CardData an icon.
/// 1) If Assets/Art/CardIcons has a PNG named like the card (letters and digits only, e.g. RadiumGlow.png), it is used.
/// 2) Otherwise the card gets the icon of its main effect type: Damage, Heal, MultiHit, LifeSteal, Overcharge or Execute.
/// Menu: Science Clash > Assign Card Icons.
/// </summary>
public static class CardIconAssigner
{
    private const string IconFolder = "Assets/Art/CardIcons";

    [MenuItem("Science Clash/Assign Card Icons")]
    public static void Assign()
    {
        int assigned = 0;
        int skipped = 0;

        foreach (string guid in AssetDatabase.FindAssets("t:CardData"))
        {
            CardData card = AssetDatabase.LoadAssetAtPath<CardData>(AssetDatabase.GUIDToAssetPath(guid));
            if (card == null) continue;

            string cardKey = new string(card.CardName.Where(char.IsLetterOrDigit).ToArray());
            Sprite icon = LoadIcon(cardKey);
            if (icon == null)
                icon = LoadIcon(MainEffectKey(card));

            if (icon == null)
            {
                skipped++;
                Debug.LogWarning("No icon found for card '" + card.CardName + "' (looked for " + cardKey + ".png and " + MainEffectKey(card) + ".png in " + IconFolder + ")");
                continue;
            }

            var so = new SerializedObject(card);
            so.FindProperty("artwork").objectReferenceValue = icon;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(card);
            assigned++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Card icons assigned: " + assigned + ", skipped: " + skipped);
    }

    // The first effect that is not plain Damage decides the icon; plain damage cards use "Damage".
    private static string MainEffectKey(CardData card)
    {
        foreach (EffectData effect in card.Effects)
        {
            if (effect == null || effect is DamageEffect) continue;
            if (effect is HealEffect) return "Heal";
            if (effect is MultiHitEffect) return "MultiHit";
            if (effect is LifeStealEffect) return "LifeSteal";
            if (effect is OverchargeEffect) return "Overcharge";
            if (effect is ExecuteEffect) return "Execute";
        }
        return "Damage";
    }

    private static Sprite LoadIcon(string fileName)
    {
        string path = IconFolder + "/" + fileName + ".png";
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return null;

        if (importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
#endif
