using UnityEngine;

/// <summary>
/// Listens for damage and fires a lightning bolt from the attacker's portrait to the target's.
/// Self-damage (recoil) gets no bolt. Big hits get a thicker bolt and a white screen flash.
/// </summary>
public class LightningController : MonoBehaviour
{
    private const int BigHitDamage = 10;

    [SerializeField] private GameManager game;
    [SerializeField] private RectTransform player1Portrait;
    [SerializeField] private RectTransform player2Portrait;
    [SerializeField] private Material lightningMaterial;

    private int cardPlayer = -1;   // who is playing the current card

    private void OnEnable()
    {
        GameEvents.OnTurnStarted += HandleTurnStarted;
        GameEvents.OnCardPlayStarted += HandleCardPlayStarted;
        GameEvents.OnDamageDealt += HandleDamageDealt;
    }

    private void OnDisable()
    {
        GameEvents.OnTurnStarted -= HandleTurnStarted;
        GameEvents.OnCardPlayStarted -= HandleCardPlayStarted;
        GameEvents.OnDamageDealt -= HandleDamageDealt;
    }

    private void HandleTurnStarted(int player) => cardPlayer = -1;
    private void HandleCardPlayStarted(int player, CardData card) => cardPlayer = player;

    private void HandleDamageDealt(ICombatant target, int amount)
    {
        if (amount <= 0 || lightningMaterial == null || game == null || !game.IsStarted) return;
        if (player1Portrait == null || player2Portrait == null) return;

        int targetPlayer = PlayerOf(target);
        if (targetPlayer < 0) return;
        if (targetPlayer == cardPlayer) return;   // recoil: the player hurt themselves

        int attacker = 1 - targetPlayer;
        RectTransform from = PortraitOf(attacker);
        RectTransform to = PortraitOf(targetPlayer);

        Canvas canvas = from.GetComponentInParent<Canvas>();
        if (canvas == null) return;
        Transform root = canvas.rootCanvas.transform;

        bool heavy = amount >= BigHitDamage;
        if (heavy) ScreenFlash.Spawn(root);

        LightningBolt.Spawn(root, lightningMaterial, CenterOf(from), CenterOf(to), AccentColorOf(attacker), heavy);
        AudioManager.Play(Sfx.Zap);
    }

    private int PlayerOf(ICombatant target)
    {
        for (int i = 0; i < 2; i++)
        {
            PlayerState player = game.GetPlayer(i);
            Scientist scientist = player.ActiveScientist ?? player.Scientists[0];
            if (ReferenceEquals(scientist, target)) return i;
        }
        return -1;
    }

    private RectTransform PortraitOf(int player) => player == 0 ? player1Portrait : player2Portrait;

    private static Vector3 CenterOf(RectTransform rect) => rect.TransformPoint(rect.rect.center);

    private Color AccentColorOf(int player)
    {
        Scientist scientist = game.GetPlayer(player).ActiveScientist;
        return scientist != null ? scientist.Data.AccentColor : Color.white;
    }
}
