using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowingHealthBar : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _barText;
    [SerializeField] private Slider _barSlider;
    [SerializeField] private RectTransform _iconsPlace;

    private Transform _targetTransform;
    private HealthSystem _targetHealth;

    private Vector3 _offset = new Vector3(0, 2, -3.5f);

    private Dictionary<TemporaryModifier, Image> _iconsList = new();


    public void Initialize(Transform targetTransform, TankBehaviour tankBehaviour)
    {
        _targetTransform = targetTransform;

        _targetHealth = tankBehaviour.TankHealthSystem;
        _targetHealth.OnHealthChanged += ChangeHealthBar;

        tankBehaviour.OnAddingTemporaryModifier += AddIcon;
    }

    private void Update()
    {
        FollowTarget();
    }

    public void FollowTarget()
    {
        if (_targetTransform != null)
            transform.position = _targetTransform.position + _offset;
    }

    private void ChangeHealthBar()
    {
        _barText.text = _targetHealth.CurrentHealth + " / " + _targetHealth.MaxHealth;
        _barSlider.value = (float)_targetHealth.CurrentHealth / (float)_targetHealth.MaxHealth;
    }

    private void AddIcon(TemporaryModifier modifier)
    {
        var icon = IconsGiver.Instance.GetIcon(IconSize.Small);
        icon.rectTransform.SetParent(_iconsPlace);
        icon.rectTransform.localPosition = Vector3.zero;
        icon.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);

        icon.sprite = modifier.ModifierData.Icon;
        modifier.OnEffectEnd += RemoveIcon;

        _iconsList.Add(modifier, icon);
    }

    private void RemoveIcon(TemporaryModifier modifier)
    {
        modifier.OnEffectEnd -= RemoveIcon;

        var icon = _iconsList[modifier];
        _iconsList.Remove(modifier);

        Destroy(icon);
    }

}
