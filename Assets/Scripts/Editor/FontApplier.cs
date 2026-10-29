#if UNITY_EDITOR
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Editor tool: gives every TextMeshPro text in the open scene and in your prefabs one consistent font setup.
///   - text inside a Button          -> the "Electronic" font asset (for example Electronic Highway Sign SDF)
///   - big text (size 36 or more)    -> Chakra Petch Bold
///   - everything else               -> Chakra Petch (Regular if you have it, otherwise the same Bold)
/// It also repairs texts whose material does not belong to their font (those draw nothing).
/// Menu: Science Clash > Apply Fonts To Scene And Prefabs.
/// </summary>
public static class FontApplier
{
    private const float TitleSizeThreshold = 36f;
    private const string MainFontKeyword = "ChakraPetch";
    private const string ButtonFontKeyword = "Electronic";

    private struct FontSet
    {
        public TMP_FontAsset Title;
        public TMP_FontAsset Body;
        public TMP_FontAsset Button;
    }

    [MenuItem("Science Clash/Apply Fonts To Scene And Prefabs")]
    public static void Apply()
    {
        TMP_FontAsset titleFont = FindFont(MainFontKeyword, "Bold");
        if (titleFont == null)
        {
            Debug.LogError("No font asset with '" + MainFontKeyword + "' in its name was found. Generate it with Window > TextMeshPro > Font Asset Creator.");
            return;
        }

        TMP_FontAsset bodyFont = FindFont(MainFontKeyword, "Regular");
        TMP_FontAsset buttonFont = FindFont(ButtonFontKeyword, "Sign");
        if (buttonFont == null)
        {
            buttonFont = bodyFont;
            Debug.LogWarning("No font asset with '" + ButtonFontKeyword + "' in its name was found. Buttons will use the body font.");
        }

        var fonts = new FontSet { Title = titleFont, Body = bodyFont, Button = buttonFont };
        Debug.Log("Fonts: title = " + fonts.Title.name + ", body = " + fonts.Body.name + ", buttons = " + fonts.Button.name);

        SetProjectDefaultFont(fonts.Body);

        // Leave prefab editing mode so we work on the real scene.
        StageUtility.GoToMainStage();

        int changedInScene = ApplyToScene(fonts);
        int changedInPrefabs = ApplyToPrefabs(fonts);

        AssetDatabase.SaveAssets();
        Debug.Log("Fonts applied. Scene texts changed: " + changedInScene + ", prefab texts changed: " + changedInPrefabs);
    }

    private static int ApplyToScene(FontSet fonts)
    {
        int count = 0;

        foreach (TMP_Text text in Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (PrefabUtility.IsPartOfPrefabInstance(text)) continue;   // the prefab itself is handled below

            Undo.RecordObject(text, "Apply font");
            if (ApplyTo(text, fonts))
                count++;
        }

        if (count > 0)
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        return count;
    }

    // Edits the prefab assets directly (no instantiating), which avoids a UI package bug with LoadPrefabContents.
    private static int ApplyToPrefabs(FontSet fonts)
    {
        int count = 0;

        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.StartsWith("Assets/TextMesh Pro")) continue;

            foreach (TMP_Text text in AssetDatabase.LoadAllAssetsAtPath(path).OfType<TMP_Text>())
            {
                if (ApplyTo(text, fonts))
                    count++;
            }
        }

        return count;
    }

    // Returns true if anything had to change.
    private static bool ApplyTo(TMP_Text text, FontSet fonts)
    {
        TMP_FontAsset target = ChooseFont(text, fonts);
        bool changed = false;

        if (text.font != target)
        {
            text.font = target;
            changed = true;
        }

        // A material from another font's atlas makes the text invisible.
        Material material = text.fontSharedMaterial;
        if (material == null || material.mainTexture != target.atlasTexture)
        {
            text.fontSharedMaterial = target.material;
            changed = true;
        }

        if (changed)
            EditorUtility.SetDirty(text);

        return changed;
    }

    private static TMP_FontAsset ChooseFont(TMP_Text text, FontSet fonts)
    {
        if (text.GetComponentInParent<Button>() != null) return fonts.Button;
        if (text.fontSize >= TitleSizeThreshold) return fonts.Title;
        return fonts.Body;
    }

    // Makes new texts use the body font too (best effort; if it fails, set it by hand in Project Settings).
    private static void SetProjectDefaultFont(TMP_FontAsset font)
    {
        if (TMP_Settings.instance == null) return;

        var settings = new SerializedObject(TMP_Settings.instance);
        SerializedProperty property = settings.FindProperty("m_defaultFontAsset");
        if (property == null)
        {
            Debug.LogWarning("Could not set the default font automatically. Set it in Project Settings > TextMesh Pro > Settings.");
            return;
        }

        property.objectReferenceValue = font;
        settings.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(TMP_Settings.instance);
    }

    // Finds a font asset whose name contains the keyword, preferring one that also contains 'prefer'.
    private static TMP_FontAsset FindFont(string keyword, string prefer)
    {
        string[] paths = AssetDatabase.FindAssets(keyword + " t:TMP_FontAsset")
            .Select(AssetDatabase.GUIDToAssetPath)
            .ToArray();

        string chosen = paths.FirstOrDefault(p => p.Contains(prefer)) ?? paths.FirstOrDefault();
        return chosen == null ? null : AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(chosen);
    }
}
#endif
