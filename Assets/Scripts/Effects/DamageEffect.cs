using UnityEngine;

[CreateAssetMenu(menuName = "Science Clash/Effects/Damage", fileName = "Damage_")]
public class DamageEffect : EffectData
{
    [SerializeField, Min(0)] private int amount = 3;

    public override void Apply(in EffectContext context)
    {
        if (context.Target == null || !context.Target.IsAlive) return;

        context.Target.TakeDamage(amount);
        GameEvents.RaiseDamageDealt(context.Target, amount);
    }
}