using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconsGiver : MonoBehaviour
{
    [SerializeField] private Image _bigIconPrefab;
    [SerializeField] private Image _smallIconPrefab;

    public static IconsGiver Instance;

    private void Start()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
    }

    public Image GetIcon(IconSize iconSize)
    {
        switch (iconSize)
        {
            case IconSize.Big:
                return Instantiate(_bigIconPrefab);
            case IconSize.Small:
                return Instantiate(_smallIconPrefab);
        }

        return null;
    }

}

public enum IconSize
{
    Big,
    Small
}