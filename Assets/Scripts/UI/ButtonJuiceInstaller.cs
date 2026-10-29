using UnityEngine;
using UnityEngine.UI;

/// <summary>On start, gives every button in the scene (even hidden ones) the ButtonJuice feel.</summary>
public class ButtonJuiceInstaller : MonoBehaviour
{
    private void Start()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            if (!button.TryGetComponent(out ButtonJuice _))
                button.gameObject.AddComponent<ButtonJuice>();
        }
    }
}
