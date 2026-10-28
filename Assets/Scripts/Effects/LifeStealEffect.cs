using UnityEngine;

[CreateAssetMenu(menuName = "Science Clash/Effects/Life Steal", fileName = "LifeSteal_")]
public class LifeStealEffect : EffectData
{
    [SerializeField, Min(0)] private int amount = 4;

    // Damages the target, then heals the caster for the same amount.
    public override void Apply(in EffectContext context)
    {
        if (context.Target == null || !context.Target.IsAlive) return;

        context.Target.TakeDamage(amount);
        GameEvents.RaiseDamageDealt(context.Target, amount);

        if (context.Source == null || !context.Source.IsAlive) return;

        context.Source.Heal(amount);
        GameEvents.RaiseHealed(context.Source, amount);
    }
}
