using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class FollowingHealthBar : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _barText;
    [SerializeField] private Slider _barSlider;

    private Transform _targetTransform;
    private HealthSystem _targetHealth;

    private Vector3 _offset = new Vector3(0, 2, -3.5f);

    public void Initialize(Transform targetTransform, HealthSystem healthSystem)
    {
        _targetTransform = targetTransform;

        _targetHealth = healthSystem;
        _targetHealth.OnHealthChanged += ChangeHealthBar;
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
}
