using UnityEngine;

[CreateAssetMenu(menuName = "Science Clash/Effects/Heal", fileName = "Heal_")]
public class HealEffect : EffectData
{
    [SerializeField, Min(0)] private int amount = 3;

    public override void Apply(in EffectContext context)
    {
        if (context.Target == null || !context.Target.IsAlive) return;

        context.Target.Heal(amount);
        GameEvents.RaiseHealed(context.Target, amount);
    }
}