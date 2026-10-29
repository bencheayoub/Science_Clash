using UnityEngine;

// Spawns particles on scientists' portraits: sparks on damage, sparkles on heal,
// and a stream of sparks flying from the caster to the target when a card is played.
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
        GameEvents.OnCardPlayStarted += HandleCardPlayStarted;
    }

    private void OnDisable()
    {
        GameEvents.OnDamageDealt -= HandleDamage;
        GameEvents.OnHealed -= HandleHeal;
        GameEvents.OnCardPlayStarted -= HandleCardPlayStarted;
    }

    private void HandleCardPlayStarted(int player, CardData card)
    {
        if (particles == null || palette == null) return;
        if (card.TargetType == CardTargetType.Self) return;   // self cards have no target to fly to
        if (portraits == null || portraits.Length < 2) return;

        Vector3 from = portraits[player].position;
        Vector3 to = portraits[1 - player].position;

        particles.Stream(from, to, palette.Primary, 14, 14f, 0.3f);
        particles.Stream(from, to, Color.white, 6, 8f, 0.3f);
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
