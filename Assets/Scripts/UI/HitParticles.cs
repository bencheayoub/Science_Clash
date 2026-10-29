using UnityEngine;

// Spawns particles on a scientist's portrait when it takes damage or is healed.
public class HitParticles : MonoBehaviour
{
    [SerializeField] private GameManager game;
    [SerializeField] private UiParticles particles;
    [SerializeField] private UiPalette palette;
    [Tooltip("One portrait per player, in player order (Player 1 first).")]
    [SerializeField] private RectTransform[] portraits;

    private void OnEnable()
    {
        GameEvents.OnDamageDealt += HandleDamage;
        GameEvents.OnHealed += HandleHeal;
    }

    private void OnDisable()
    {
        GameEvents.OnDamageDealt -= HandleDamage;
        GameEvents.OnHealed -= HandleHeal;
    }

    private void HandleDamage(ICombatant target, int amount)
    {
        int index = FindPortraitIndex(target);
        if (index < 0 || amount <= 0) return;

        Vector3 position = portraits[index].position;
        int count = Mathf.Clamp(8 + amount * 3, 10, 40);

        particles.Burst(position, palette.Damage, count, 420f, 700f, 16f, 0.6f);
        particles.Burst(position, Color.white, count / 3, 300f, 500f, 9f, 0.4f);   // bright core sparks
    }

    private void HandleHeal(ICombatant target, int amount)
    {
        int index = FindPortraitIndex(target);
        if (index < 0 || amount <= 0) return;

        RectTransform portrait = portraits[index];
        int count = Mathf.Clamp(8 + amount * 2, 10, 30);

        particles.Rise(portrait.position, portrait.rect.size, palette.Heal, count, 140f, 14f, 1.0f);
    }

    // Which player's scientist is this? (Same lookup rule the scientist panels use.)
    private int FindPortraitIndex(ICombatant who)
    {
        if (game == null || !game.IsStarted || particles == null || palette == null) return -1;

        for (int i = 0; i < portraits.Length; i++)
        {
            PlayerState player = game.GetPlayer(i);
            Scientist scientist = player.ActiveScientist ?? player.Scientists[0];
            if (ReferenceEquals(scientist, who)) return i;
        }
        return -1;
    }
}
