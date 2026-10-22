using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TemporaryModifier
{
    private TemporaryModifierData _modifierData;
    private TankStatsController _tankStatsController;
    private CancellationTokenSource _cts;

    public TemporaryModifier(TemporaryModifierData modifierData)
    {
        _modifierData = modifierData;
    }

    public void ActivateModifier(TankStatsController tankStatsController)
    {
        _tankStatsController = tankStatsController;
        _tankStatsController.TempModifiers.Add(this);
        _cts?.Cancel();
        _cts?.Dispose();

        _cts = new();
        DurationAsync(_cts.Token).Forget();
    }

    private async UniTaskVoid DurationAsync(CancellationToken ct)
    {
        foreach (var item in _modifierData.StatsModifier)
            _tankStatsController.GetStat(item.ModifierStat).AddModifier(item);

        await UniTask.Delay(TimeSpan.FromSeconds(_modifierData.Duration), cancellationToken: ct);

        foreach (var item in _modifierData.StatsModifier)
            _tankStatsController.GetStat(item.ModifierStat).RemoveModifier(item);

    }

    public TemporaryModifierData GetData()
    {
        return _modifierData;
    }

}
