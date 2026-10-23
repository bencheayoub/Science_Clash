using UnityEngine;

[CreateAssetMenu(menuName = "Science Clash/Scientist", fileName = "Scientist_")]
public class ScientistData : ScriptableObject
{
    [SerializeField] private string scientistName;
    [SerializeField, Min(1)] private int maxHealth = 30;
    [SerializeField] private Sprite portrait;

    public string ScientistName => scientistName;
    public int MaxHealth => maxHealth;
    public Sprite Portrait => portrait;
}