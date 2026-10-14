using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneController
{
    public static void LoadScene(SceneIndex sceneIndex)
    {
        SceneManager.LoadScene((int)sceneIndex);
    }
}

public enum SceneIndex
{
    MainMenu,
    GamePlay,
    Hangar
}