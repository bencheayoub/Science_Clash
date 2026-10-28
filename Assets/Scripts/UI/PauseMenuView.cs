using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Esc toggles a pause panel (resume / restart / quit).
/// Put this script on the Canvas (or any object that stays active), NOT on the panel itself.
/// </summary>
public class PauseMenuView : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private bool isPaused;
    private bool gameOver;

    private void Awake()
    {
        panel.SetActive(false);
        resumeButton.onClick.AddListener(Resume);
        restartButton.onClick.AddListener(Restart);
        quitButton.onClick.AddListener(Quit);
    }

    private void OnEnable() => GameEvents.OnGameOver += HandleGameOver;
    private void OnDisable() => GameEvents.OnGameOver -= HandleGameOver;

    // Never leave the game frozen when this object goes away (scene reload, quit to editor).
    private void OnDestroy() => Time.timeScale = 1f;

    private void Update()
    {
        if (!gameOver && EscapePressed())
            SetPaused(!isPaused);
    }

    private void HandleGameOver(int winner)
    {
        gameOver = true;
        SetPaused(false);
    }

    private void Resume() => SetPaused(false);

    private void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;
        panel.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;
    }

    private static bool EscapePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }
}
