using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _barText;
    [SerializeField] private TMPro.TMP_Text _enemiesCountText;
    [SerializeField] private TMPro.TMP_Text _moneyText;
    [SerializeField] private RectTransform _iconsPlace;

    [SerializeField] private Slider _barSlider;

    private HealthSystem _targetHealth;
    private EnemySpawner _enemySpawner;

    private Dictionary<TemporaryModifier, Image> _iconsList = new();


    public void Initialize(TankBehaviour playerTank, EnemySpawner enemySpawner)
    {
        _targetHealth = playerTank.TankHealthSystem;
        _targetHealth.OnHealthChanged += ChangeHealthBar;
        ChangeHealthBar();

        PlayerInstance.Instance.OnChangeMoney += MoneyTextUpdate;
        _moneyText.text = PlayerInstance.Instance.PlayerData.MoneyCount.ToString();

        _enemySpawner = enemySpawner;
        _enemySpawner.OnDeathSpawnedUnit += EnemiesTextUpdate;
        _enemiesCountText.text = (_enemySpawner.CurrentDeadEnemies - _enemySpawner.EnemiesPerWave).ToString() + " / " + _enemySpawner.EnemiesPerWave.ToString();



        playerTank.OnAddingTemporaryModifier += AddIcon;

    }

    private void AddIcon(TemporaryModifier modifier)
    {
        var icon = IconsGiver.Instance.GetIcon(IconSize.Big);
        icon.rectTransform.SetParent(_iconsPlace);
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

    private void EnemiesTextUpdate(TankBehaviour tankBehaviour)
    {
        _enemiesCountText.text = (_enemySpawner.CurrentDeadEnemies - _enemySpawner.EnemiesPerWave).ToString() + " / " + _enemySpawner.EnemiesPerWave.ToString();
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
