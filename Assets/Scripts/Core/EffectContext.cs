/// <summary>Everything an effect needs to know when it is applied.</summary>
public readonly struct EffectContext
{
    public readonly int SourcePlayer;
    public readonly ICombatant Source;
    public readonly ICombatant Target;

    public EffectContext(int sourcePlayer, ICombatant source, ICombatant target)
    {
        SourcePlayer = sourcePlayer;
        Source = source;
        Target = target;
    }
}