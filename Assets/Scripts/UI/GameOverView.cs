using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
    private const float FadeDuration = 0.4f;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        panel.SetActive(false);
        restartButton.onClick.AddListener(Restart);
    }

    private void OnEnable() => GameEvents.OnGameOver += HandleGameOver;
    private void OnDisable() => GameEvents.OnGameOver -= HandleGameOver;

    private void HandleGameOver(int winner)
    {
        resultText.text = winner == 0 ? "You win!" : "Computer wins!";
        panel.SetActive(true);
        StartCoroutine(PopIn());
    }

    // The screen fades in while the result text pops.
    private IEnumerator PopIn()
    {
        if (!panel.TryGetComponent(out CanvasGroup group))
            group = panel.AddComponent<CanvasGroup>();
        group.alpha = 0f;

        if (!resultText.TryGetComponent(out UiPunch punch))
            punch = resultText.gameObject.AddComponent<UiPunch>();
        punch.Play(0.5f, 0.6f);

        for (float t = 0f; t < FadeDuration; t += Time.unscaledDeltaTime)
        {
            group.alpha = t / FadeDuration;
            yield return null;
        }
        group.alpha = 1f;
    }

    private void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
