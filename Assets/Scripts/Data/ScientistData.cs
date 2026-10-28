using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Science Clash/Scientist", fileName = "Scientist_")]
public class ScientistData : ScriptableObject
{
    [SerializeField] private string scientistName;
    [SerializeField, Min(1)] private int maxHealth = 30;
    [SerializeField] private Sprite portrait;

    [Header("Abilities")]
    [Tooltip("Cards only this scientist has. They are added to this scientist's deck on top of the shared cards.")]
    [SerializeField] private List<CardData> signatureCards = new List<CardData>();

    public string ScientistName => scientistName;
    public int MaxHealth => maxHealth;
    public Sprite Portrait => portrait;
    public IReadOnlyList<CardData> SignatureCards => signatureCards;
}
