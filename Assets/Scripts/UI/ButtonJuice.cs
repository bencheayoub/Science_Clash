using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Makes a button grow on hover, shrink when pressed, and play its sounds.</summary>
[RequireComponent(typeof(Button))]
public class ButtonJuice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float pressScale = 0.94f;
    [SerializeField] private float speed = 18f;

    private Button button;
    private Vector3 baseScale = Vector3.one;
    private float target = 1f;
    private bool hovered;

    private void Awake()
    {
        button = GetComponent<Button>();
        baseScale = transform.localScale;
        button.onClick.AddListener(() => AudioManager.Play(Sfx.ButtonClick));
    }

    private void OnEnable()
    {
        hovered = false;
        target = 1f;
        transform.localScale = baseScale;
    }

    private void Update()
    {
        float current = transform.localScale.x / baseScale.x;
        float next = Mathf.Lerp(current, target, 1f - Mathf.Exp(-speed * Time.unscaledDeltaTime));
        transform.localScale = baseScale * next;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;
        hovered = true;
        target = hoverScale;
        AudioManager.Play(Sfx.ButtonHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        target = 1f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable) target = pressScale;
    }

    public void OnPointerUp(PointerEventData eventData) => target = hovered ? hoverScale : 1f;
}
