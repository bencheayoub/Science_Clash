using UnityEngine;

public class ScientistSelectionView : MonoBehaviour
{
    [SerializeField] private GameManager game;
    [SerializeField] private ScientistOptionView optionPrefab;
    [SerializeField] private Transform container;

    private void OnEnable() => GameEvents.OnScientistChosen += HandleChosen;
    private void OnDisable() => GameEvents.OnScientistChosen -= HandleChosen;

    private void Start()
    {
        foreach (ScientistData data in game.Roster)
            Instantiate(optionPrefab, container).Bind(data);
    }

    private void HandleChosen(ScientistData data) => gameObject.SetActive(false);
}
