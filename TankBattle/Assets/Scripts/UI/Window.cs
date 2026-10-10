using UnityEngine;

public class Window : MonoBehaviour
{
    [SerializeField] protected GameObject CurrentWindow;

    public void Show()
    {
        CurrentWindow.SetActive(true);
    }
    
    public void Hide()
    {
        CurrentWindow.SetActive(false);
    }
}
