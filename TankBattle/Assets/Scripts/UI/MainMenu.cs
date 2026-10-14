using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button ExitButton;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        PlayButton.onClick.AddListener(StartGame);
        SettingsButton.onClick.AddListener(OpenSettings);
        ExitButton.onClick.AddListener(ExitGame);

    }

    private void StartGame()
    {
        SceneController.LoadScene(SceneIndex.Hangar);
    }

    private void OpenSettings()
    {
        Debug.Log("Settings");
        //SceneController.LoadScene(SceneIndex.Hangar);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
