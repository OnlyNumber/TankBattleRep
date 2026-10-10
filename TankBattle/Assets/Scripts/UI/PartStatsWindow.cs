using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartStatsWindow : Window
{
    [SerializeField] private RectTransform layoutForInfo;
    [SerializeField] private DescriptionBehaviour _descriptionBehaviourPrefab;
    [SerializeField] private Button _button;

    public event Action OnButtoPressed;
    private List<DescriptionBehaviour> _descriptions = new();

    void Start()
    {
        _button.onClick.AddListener(PressedButton);
    }

    public void PressedButton()
    {
        OnButtoPressed?.Invoke();
    }

    public void ActivateButton(bool state)
    {
        _button.interactable = state;
    }

    public void AddDescription(params Description[] descriptions)
    {
        foreach (var item in descriptions)
        {
            var descrip = Instantiate(_descriptionBehaviourPrefab, layoutForInfo);
            descrip.Initialize(item);

            _descriptions.Add(descrip);
        }
    }

    public void ClearDescription()
    {
        foreach (var item in _descriptions)
            Destroy(item.gameObject);

        _descriptions.Clear();
    }
}