using System.Collections;
using UnityEngine;

/// <summary>A quick "pop": the element swells and settles back. Works while the game is paused.</summary>
public class UiPunch : MonoBehaviour
{
    private Vector3 baseScale = Vector3.one;
    private bool hasBase;
    private Coroutine running;

    private void Awake() => CaptureBase();

    private void OnDisable()
    {
        running = null;
        if (hasBase) transform.localScale = baseScale;
    }

    private void CaptureBase()
    {
        if (hasBase) return;
        baseScale = transform.localScale;
        hasBase = true;
    }

    public void Play(float strength = 0.25f, float duration = 0.3f)
    {
        CaptureBase();
        if (!isActiveAndEnabled) return;

        if (running != null) StopCoroutine(running);
        running = StartCoroutine(Run(strength, duration));
    }

    private IEnumerator Run(float strength, float duration)
    {
        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            float n = t / duration;
            float pulse = Mathf.Sin(n * Mathf.PI) * (1f - n);   // up fast, settle slowly
            transform.localScale = baseScale * (1f + strength * pulse);
            yield return null;
        }

        transform.localScale = baseScale;
        running = null;
    }
}
