using UnityEditor;
using UnityEngine;

/// <summary>
/// Builds the duotone materials from UiPalette so art colors never get hardcoded.
/// Menu: Science Clash > Generate Art Materials
/// </summary>
public static class ArtMaterialGenerator
{
    private const string ShaderName = "ScienceClash/UI Duotone";
    private const string OutputFolder = "Assets/Materials/Generated";
    private const float BackgroundBrightness = 0.55f;

    [MenuItem("Science Clash/Generate Art Materials")]
    public static void Generate()
    {
        UiPalette palette = FindPalette();
        Shader shader = Shader.Find(ShaderName);
        if (palette == null || shader == null)
        {
            Debug.LogError("Missing UiPalette asset or UI Duotone shader.");
            return;
        }

        EnsureFolder();

        Color backgroundLight = Color.Lerp(palette.Background, palette.Primary, BackgroundBrightness);
        CreateOrUpdate("BackgroundDuotone", shader, palette.Background, backgroundLight);
        CreateOrUpdate("PortraitDuotone", shader, palette.Panel, palette.Text);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Art materials generated in " + OutputFolder);
    }

    private static UiPalette FindPalette()
    {
        string[] guids = AssetDatabase.FindAssets("t:UiPalette");
        if (guids.Length == 0) return null;
        return AssetDatabase.LoadAssetAtPath<UiPalette>(AssetDatabase.GUIDToAssetPath(guids[0]));
    }

    private static void EnsureFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder(OutputFolder))
            AssetDatabase.CreateFolder("Assets/Materials", "Generated");
    }

    private static void CreateOrUpdate(string name, Shader shader, Color dark, Color light)
    {
        string path = OutputFolder + "/" + name + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.shader = shader;
        mat.SetColor("_DarkColor", dark);
        mat.SetColor("_LightColor", light);
        EditorUtility.SetDirty(mat);
    }
}
