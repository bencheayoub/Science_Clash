using UnityEngine;

/// <summary>Runtime instance of a scientist. The ScientistData asset is never modified.</summary>
public class Scientist : ICombatant
{
    private readonly ScientistData data;

    public string DisplayName => data.ScientistName;
    public int MaxHealth => data.MaxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0;
    public ScientistData Data => data;

    public Scientist(ScientistData data)
    {
        this.data = data;
        CurrentHealth = data.MaxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (!IsAlive) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        if (!IsAlive)
            GameEvents.RaiseScientistDefeated(this);
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;
        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
    }
}