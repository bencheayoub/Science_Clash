using UnityEngine;

[CreateAssetMenu(menuName = "Science Clash/Effects/Execute", fileName = "Execute_")]
public class ExecuteEffect : EffectData
{
    [SerializeField, Min(0)] private int baseDamage = 3;
    [SerializeField, Min(0)] private int bonusDamage = 6;
    [Tooltip("The bonus applies when the target's health is at or below this fraction of max health (0.4 = 40%).")]
    [SerializeField, Range(0f, 1f)] private float threshold = 0.4f;

    // Deals extra damage to a weakened target.
    public override void Apply(in EffectContext context)
    {
        if (context.Target == null || !context.Target.IsAlive) return;

        bool weakened = context.Target.CurrentHealth <= context.Target.MaxHealth * threshold;
        int total = baseDamage + (weakened ? bonusDamage : 0);

        context.Target.TakeDamage(total);
        GameEvents.RaiseDamageDealt(context.Target, total);
    }
}
