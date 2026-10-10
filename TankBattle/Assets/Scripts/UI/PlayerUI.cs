using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _barText;
    [SerializeField] private TMPro.TMP_Text _enemiesCountText;
    [SerializeField] private TMPro.TMP_Text _moneyText;

    [SerializeField] private Slider _barSlider;

    private HealthSystem _targetHealth;

    public void Initialize(HealthSystem healthSystem)
    {
        _targetHealth = healthSystem;
        _targetHealth.OnHealthChanged += ChangeHealthBar;
        ChangeHealthBar();

        PlayerInstance.Instance.OnChangeMoney += MoneyTextUpdate;
        _moneyText.text = PlayerInstance.Instance.PlayerData.MoneyCount.ToString();
    }

    private void MoneyTextUpdate(int moneyCount)
    {
        _moneyText.text = moneyCount.ToString();
    }

    private void ChangeHealthBar()
    {
        _barText.text = _targetHealth.CurrentHealth + " / " + _targetHealth.MaxHealth;
        _barSlider.value = (float)_targetHealth.CurrentHealth / (float)_targetHealth.MaxHealth;
    }
}
