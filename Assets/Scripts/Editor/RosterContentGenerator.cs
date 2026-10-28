#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor tool: creates the effect assets and signature cards for all six scientists
/// and assigns them to the matching ScientistData assets.
/// Menu: Science Clash > Generate Roster Content.
/// Re-running it overwrites the generated assets, so edit numbers in the assets only after you are done generating.
/// </summary>
public static class RosterContentGenerator
{
    private const string Root = "Assets/Data/Generated";
    private const string EffectsFolder = Root + "/Effects";
    private const string CardsFolder = Root + "/Cards";

    [MenuItem("Science Clash/Generate Roster Content")]
    public static void Generate()
    {
        EnsureFolder("Assets", "Data");
        EnsureFolder("Assets/Data", "Generated");
        EnsureFolder(Root, "Effects");
        EnsureFolder(Root, "Cards");

        CardTargetType enemy = CardTargetType.Enemy;
        CardTargetType self = CardTargetType.Self;

        // ---------------- Curie: radiation and sustain ----------------
        AssignToScientist("Curie",
            MakeCard("Radium Glow", "Deal 3 damage.", 1, enemy, Damage(3)),
            MakeCard("Half-Life", "Heal 3 health.", 1, self, Heal(3)),
            MakeCard("Polonium Strike", "Deal 4 damage and heal 4.", 2, enemy, LifeSteal(4)),
            MakeCard("Radioactive Decay", "Hit 3 times for 2 damage each.", 2, enemy, MultiHit(3, 2)),
            MakeCard("Isolation", "Heal 5 health.", 2, self, Heal(5)),
            MakeCard("Nobel Legacy", "Deal 6 damage and heal 6.", 3, enemy, LifeSteal(6)));

        // ---------------- Newton: heavy and reliable ----------------
        AssignToScientist("Newton",
            MakeCard("Apple Drop", "Deal 3 damage.", 1, enemy, Damage(3)),
            MakeCard("Inertia", "Heal 3 health.", 1, self, Heal(3)),
            MakeCard("Gravity", "Deal 6 damage.", 2, enemy, Damage(6)),
            MakeCard("Third Law", "Deal 7 damage. You take 2 damage.", 2, enemy, Overcharge(7, 2)),
            MakeCard("Laws of Motion", "Hit 3 times for 2 damage each.", 2, enemy, MultiHit(3, 2)),
            MakeCard("Principia", "Deal 5 damage, or 10 if the target is at half health or less.", 3, enemy, Execute(5, 5, 0.5f)));

        // ---------------- Tesla: electric multi-hits and risk ----------------
        AssignToScientist("Tesla",
            MakeCard("Spark", "Deal 3 damage.", 1, enemy, Damage(3)),
            MakeCard("Wireless Power", "Heal 3 health.", 1, self, Heal(3)),
            MakeCard("Tesla Coil", "Hit 3 times for 2 damage each.", 2, enemy, MultiHit(3, 2)),
            MakeCard("Alternating Current", "Deal 3 damage, then hit twice for 2 damage each.", 2, enemy, Damage(3), MultiHit(2, 2)),
            MakeCard("Overload", "Deal 10 damage. You take 4 damage.", 3, enemy, Overcharge(10, 4)),
            MakeCard("Chain Lightning", "Hit 4 times for 2 damage each.", 3, enemy, MultiHit(4, 2)));

        // ---------------- Turing: finishers ----------------
        AssignToScientist("Turing",
            MakeCard("Enigma Probe", "Deal 3 damage.", 1, enemy, Damage(3)),
            MakeCard("Decrypt", "Deal 3 damage, or 8 if the target is at half health or less.", 2, enemy, Execute(3, 5, 0.5f)),
            MakeCard("Bombe Machine", "Hit 3 times for 2 damage each.", 2, enemy, MultiHit(3, 2)),
            MakeCard("Turing Test", "Heal 5 health.", 2, self, Heal(5)),
            MakeCard("Imitation Game", "Deal 6 damage.", 2, enemy, Damage(6)),
            MakeCard("Halting Problem", "Deal 4 damage, or 13 if the target is at 40% health or less.", 3, enemy, Execute(4, 9, 0.4f)));

        // ---------------- Lovelace: algorithms and healing ----------------
        AssignToScientist("Lovelace",
            MakeCard("Punch Card", "Deal 3 damage.", 1, enemy, Damage(3)),
            MakeCard("Debug", "Heal 3 health.", 1, self, Heal(3)),
            MakeCard("Loop", "Hit 3 times for 2 damage each.", 2, enemy, MultiHit(3, 2)),
            MakeCard("Compile", "Deal 4 damage and heal 4.", 2, enemy, LifeSteal(4)),
            MakeCard("Analytical Engine", "Hit 4 times for 2 damage each.", 3, enemy, MultiHit(4, 2)),
            MakeCard("Poetical Science", "Heal 8 health.", 3, self, Heal(8)));

        // ---------------- Einstein: high risk, high reward ----------------
        AssignToScientist("Einstein",
            MakeCard("Thought Experiment", "Deal 3 damage.", 1, enemy, Damage(3)),
            MakeCard("Photoelectric Effect", "Hit 3 times for 2 damage each.", 2, enemy, MultiHit(3, 2)),
            MakeCard("Time Dilation", "Heal 5 health.", 2, self, Heal(5)),
            MakeCard("Special Relativity", "Deal 7 damage. You take 2 damage.", 2, enemy, Overcharge(7, 2)),
            MakeCard("E=mc2", "Deal 13 damage. You take 5 damage.", 3, enemy, Overcharge(13, 5)),
            MakeCard("General Relativity", "Deal 6 damage, then hit twice for 2 damage each.", 3, enemy, Damage(6), MultiHit(2, 2)));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Roster content generated in " + Root);
    }

    // ---------- effect factories (one asset per distinct setting, reused by many cards) ----------

    private static EffectData Damage(int amount) =>
        GetEffect<DamageEffect>("Damage_" + amount, ("amount", amount));

    private static EffectData Heal(int amount) =>
        GetEffect<HealEffect>("Heal_" + amount, ("amount", amount));

    private static EffectData LifeSteal(int amount) =>
        GetEffect<LifeStealEffect>("LifeSteal_" + amount, ("amount", amount));

    private static EffectData MultiHit(int hits, int amountPerHit) =>
        GetEffect<MultiHitEffect>("MultiHit_" + hits + "x" + amountPerHit,
            ("hits", hits), ("amountPerHit", amountPerHit));

    private static EffectData Overcharge(int damage, int recoil) =>
        GetEffect<OverchargeEffect>("Overcharge_" + damage + "_" + recoil,
            ("damage", damage), ("recoil", recoil));

    private static EffectData Execute(int baseDamage, int bonusDamage, float threshold) =>
        GetEffect<ExecuteEffect>("Execute_" + baseDamage + "_" + bonusDamage + "_" + Mathf.RoundToInt(threshold * 100f),
            ("baseDamage", baseDamage), ("bonusDamage", bonusDamage), ("threshold", threshold));

    // ---------- helpers ----------

    private static T GetEffect<T>(string assetName, params (string field, object value)[] values) where T : EffectData
    {
        T effect = LoadOrCreate<T>(EffectsFolder + "/" + assetName + ".asset");
        var so = new SerializedObject(effect);

        foreach ((string field, object value) in values)
        {
            SerializedProperty prop = so.FindProperty(field);
            if (prop == null)
            {
                Debug.LogError("Field not found on " + typeof(T).Name + ": " + field);
                continue;
            }

            if (value is int i) prop.intValue = i;
            else if (value is float f) prop.floatValue = f;
        }

        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(effect);
        return effect;
    }

    private static CardData MakeCard(string cardName, string description, int cost, CardTargetType target, params EffectData[] effects)
    {
        string fileName = new string(cardName.Where(char.IsLetterOrDigit).ToArray());
        CardData card = LoadOrCreate<CardData>(CardsFolder + "/Card_" + fileName + ".asset");

        var so = new SerializedObject(card);
        so.FindProperty("cardName").stringValue = cardName;
        so.FindProperty("description").stringValue = description;
        so.FindProperty("cost").intValue = cost;
        so.FindProperty("targetType").enumValueIndex = Array.IndexOf(Enum.GetValues(typeof(CardTargetType)), target);

        SerializedProperty list = so.FindProperty("effects");
        list.ClearArray();
        for (int i = 0; i < effects.Length; i++)
        {
            list.InsertArrayElementAtIndex(i);
            list.GetArrayElementAtIndex(i).objectReferenceValue = effects[i];
        }

        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(card);
        return card;
    }

    private static void AssignToScientist(string keyword, params CardData[] cards)
    {
        foreach (string guid in AssetDatabase.FindAssets("t:ScientistData"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScientistData scientist = AssetDatabase.LoadAssetAtPath<ScientistData>(path);
            if (scientist == null) continue;

            var so = new SerializedObject(scientist);
            string displayName = so.FindProperty("scientistName").stringValue;
            bool matches =
                displayName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                scientist.name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
            if (!matches) continue;

            SerializedProperty list = so.FindProperty("signatureCards");
            list.ClearArray();
            for (int i = 0; i < cards.Length; i++)
            {
                list.InsertArrayElementAtIndex(i);
                list.GetArrayElementAtIndex(i).objectReferenceValue = cards[i];
            }

            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(scientist);
            Debug.Log("Assigned " + cards.Length + " cards to " + scientist.name);
            return;
        }

        Debug.LogWarning("No ScientistData found for '" + keyword + "'. Cards were created but not assigned.");
    }

    private static T LoadOrCreate<T>(string path) where T : ScriptableObject
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset != null) return asset;

        asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }

    private static void EnsureFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + name))
            AssetDatabase.CreateFolder(parent, name);
    }
}
#endif
