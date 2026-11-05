using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartStatsWindow : Window
{
    [SerializeField] private RectTransform layoutForInfo;
    [SerializeField] private DescriptionBehaviour _descriptionBehaviourPrefab;
    [SerializeField] private Button _button;
    [SerializeField] private TMPro.TMP_Text _buttonText;

    [SerializeField] private TMPro.TMP_Text _partName;

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

    public void ClearAll()
    {
        _partName.text = "";

        foreach (var item in _descriptions)
            Destroy(item.gameObject);

        _descriptions.Clear();
    }

    public void SetPartName(string name)
    {
        _partName.text = name;
    }
    
    public void SetButtonText(string name)
    {
        _buttonText.text = name;
    }
}