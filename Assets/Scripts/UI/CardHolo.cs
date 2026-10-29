using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Holographic foil on a hand card: while the mouse is over it, a rainbow sheen follows the cursor
/// and the card tilts slightly toward it. Added to each card at runtime by CardView.EnableHolo.
/// </summary>
public class CardHolo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static readonly int AmountId = Shader.PropertyToID("_HoloAmount");
    private static readonly int PointerId = Shader.PropertyToID("_HoloPointer");
    private static readonly int CenterId = Shader.PropertyToID("_HoloCenter");
    private static readonly int SizeId = Shader.PropertyToID("_HoloSize");
    private static readonly int TintId = Shader.PropertyToID("_HoloTint");

    private const float MaxTiltDegrees = 15f;
    // Must stay >= 1 / cos(MaxTiltDegrees) (about 1.035), or tilting shrinks the card
    // under the cursor and the hover flickers at the edges.
    private const float HoverScale = 1.06f;
    private const float FadeSpeed = 6f;      // how fast the foil fades in and out
    private const float FollowSpeed = 14f;   // how fast the sheen and tilt chase the cursor

    private Material materialCopy;
    private RectTransform rect;
    private Camera uiCamera;

    private bool hovered;
    private float amount;            // 0..1 foil strength
    private Vector2 pointer;         // cursor on the card, -0.5..0.5
    private Vector2 smoothPointer;

    /// <summary>Gives every Image on this card the holo material (one private copy per card).</summary>
    public void Setup(Material holoMaterial, Color tint)
    {
        rect = (RectTransform)transform;

        Canvas canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;

        materialCopy = new Material(holoMaterial);
        materialCopy.SetColor(TintId, tint);
        materialCopy.SetFloat(AmountId, 0f);

        foreach (Image image in GetComponentsInChildren<Image>(true))
            image.material = materialCopy;
    }

    /// <summary>Called when the card starts dissolving: stop reacting to the mouse and keep the current pose.</summary>
    public void Freeze()
    {
        hovered = false;
        enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData) => hovered = true;
    public void OnPointerExit(PointerEventData eventData) => hovered = false;

    private void Update()
    {
        if (materialCopy == null) return;

        float dt = Time.unscaledDeltaTime;
        if (hovered) pointer = ReadPointerOnCard();

        amount = Mathf.MoveTowards(amount, hovered ? 1f : 0f, dt * FadeSpeed);
        Vector2 target = hovered ? pointer : Vector2.zero;
        smoothPointer = Vector2.Lerp(smoothPointer, target, 1f - Mathf.Exp(-FollowSpeed * dt));

        bool idle = !hovered && amount <= 0f && smoothPointer.sqrMagnitude < 0.00001f;
        if (idle)
        {
            if (smoothPointer != Vector2.zero)
            {
                smoothPointer = Vector2.zero;
                Apply();   // one last time, so the card ends perfectly flat
            }
            return;
        }

        Apply();
    }

    private void Apply()
    {
        Vector2 tilt = smoothPointer * 2f;   // -1..1
        transform.localRotation = Quaternion.Euler(tilt.y * MaxTiltDegrees, -tilt.x * MaxTiltDegrees, 0f);
        transform.localScale = Vector3.one * Mathf.Lerp(1f, HoverScale, amount);

        Rect r = rect.rect;
        Vector3 center = rect.TransformPoint(r.center);
        Vector3 size = rect.TransformVector(new Vector3(r.width, r.height, 0f));

        materialCopy.SetFloat(AmountId, amount);
        materialCopy.SetVector(PointerId, smoothPointer);
        materialCopy.SetVector(CenterId, center);
        materialCopy.SetVector(SizeId, new Vector4(Mathf.Abs(size.x), Mathf.Abs(size.y), 0f, 0f));
    }

    // Cursor position on the card, from -0.5 (left/bottom) to 0.5 (right/top).
    private Vector2 ReadPointerOnCard()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, ScreenPointer(), uiCamera, out Vector2 local);
        Rect r = rect.rect;
        return new Vector2(
            Mathf.Clamp((local.x - r.center.x) / r.width, -0.5f, 0.5f),
            Mathf.Clamp((local.y - r.center.y) / r.height, -0.5f, 0.5f));
    }

    // Works with the new Input System and with the old Input Manager.
    private static Vector2 ScreenPointer()
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = UnityEngine.InputSystem.Mouse.current;
        return mouse != null ? mouse.position.ReadValue() : Vector2.zero;
#else
        return Input.mousePosition;
#endif
    }

    private void OnDestroy()
    {
        if (materialCopy != null)
            Destroy(materialCopy);
    }
}
