using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TemporaryModifier
{
    public TemporaryModifierData ModifierData
    {
        get;
        private set;
    }

    private TankStatsController _tankStatsController;
    private CancellationTokenSource _cts;

    public Action<TemporaryModifier> OnEffectEnd;

    public TemporaryModifier(TemporaryModifierData modifierData)
    {
        ModifierData = modifierData;
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
        foreach (var item in ModifierData.StatsModifier)
            _tankStatsController.GetStat(item.ModifierStat).AddModifier(item);

        await UniTask.Delay(TimeSpan.FromSeconds(ModifierData.Duration), cancellationToken: ct);

        foreach (var item in ModifierData.StatsModifier)
            _tankStatsController.GetStat(item.ModifierStat).RemoveModifier(item);

        OnEffectEnd?.Invoke(this);

    }
}
