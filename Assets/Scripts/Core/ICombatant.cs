public interface ICombatant
{
    string DisplayName { get; }
    int CurrentHealth { get; }
    int MaxHealth { get; }
    bool IsAlive { get; }

    void TakeDamage(int amount);
    void Heal(int amount);
}