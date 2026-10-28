using UnityEngine;

[CreateAssetMenu(menuName = "Science Clash/Effects/Overcharge", fileName = "Overcharge_")]
public class OverchargeEffect : EffectData
{
    [SerializeField, Min(0)] private int damage = 9;
    [SerializeField, Min(0)] private int recoil = 3;

    // Big hit on the target, but the caster hurts themselves. Recoil can never kill the caster.
    public override void Apply(in EffectContext context)
    {
        if (context.Target == null || !context.Target.IsAlive) return;

        context.Target.TakeDamage(damage);
        GameEvents.RaiseDamageDealt(context.Target, damage);

        if (context.Source == null || !context.Source.IsAlive) return;

        int safeRecoil = Mathf.Min(recoil, context.Source.CurrentHealth - 1);
        if (safeRecoil <= 0) return;

        context.Source.TakeDamage(safeRecoil);
        GameEvents.RaiseDamageDealt(context.Source, safeRecoil);
    }
}
