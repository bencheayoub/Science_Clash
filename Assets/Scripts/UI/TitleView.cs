using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Full-screen title panel shown on top of the scientist selection until Play is pressed.
/// Put this script on the Canvas (or any object that stays active), NOT on the panel itself.
/// </summary>
public class TitleView : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        panel.SetActive(true);
        playButton.onClick.AddListener(Play);
        quitButton.onClick.AddListener(Quit);
    }

    private void Play() => panel.SetActive(false);

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
