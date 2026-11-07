using UnityEngine;
using UnityEngine.UI;
using Zenject.SpaceFighter;

public class CompleteWaveWindow : Window
{
    public Button CompleteWaveButton;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        CompleteWaveButton.onClick.AddListener(LoadHangar);
        
    }

    private void LoadHangar()
    {
        ControllerGameObjectPooler.ClearPools();
        PlayerInstance.Instance.Save();
        SceneController.LoadScene(SceneIndex.Hangar);
    }
}
