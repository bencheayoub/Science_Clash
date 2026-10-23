using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
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
        resultText.text = $"Player {winner + 1} wins!";
        panel.SetActive(true);
    }

    private void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}