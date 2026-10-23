using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Science Clash/Card", fileName = "Card_")]
public class CardData : ScriptableObject
{
    [Header("Info")]
    [SerializeField] private string cardName;
    [SerializeField, TextArea] private string description;
    [SerializeField] private Sprite artwork;

    [Header("Rules")]
    [SerializeField, Min(0)] private int cost = 1;
    [SerializeField] private CardTargetType targetType = CardTargetType.Enemy;

    [Header("Effects")]
    [SerializeField] private List<EffectData> effects = new List<EffectData>();

    public string CardName => cardName;
    public string Description => description;
    public Sprite Artwork => artwork;
    public int Cost => cost;
    public CardTargetType TargetType => targetType;
    public IReadOnlyList<EffectData> Effects => effects;

    /// <summary>Applies every effect on this card, in order.</summary>
    public void Resolve(in EffectContext context)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i] != null)
                effects[i].Apply(context);
        }
    }
}