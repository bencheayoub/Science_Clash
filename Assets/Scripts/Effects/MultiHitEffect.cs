using UnityEngine;

[CreateAssetMenu(menuName = "Science Clash/Effects/Multi Hit", fileName = "MultiHit_")]
public class MultiHitEffect : EffectData
{
    [SerializeField, Min(1)] private int hits = 3;
    [SerializeField, Min(0)] private int amountPerHit = 2;

    // Hits the target several times. Each hit raises its own event, so VFX can play per hit.
    public override void Apply(in EffectContext context)
    {
        for (int i = 0; i < hits; i++)
        {
            if (context.Target == null || !context.Target.IsAlive) return;

            context.Target.TakeDamage(amountPerHit);
            GameEvents.RaiseDamageDealt(context.Target, amountPerHit);
        }
    }
}
