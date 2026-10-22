using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public static class UtilitiesMath
{
    private const int Base_Critical_Chance = 20;

    private static string TemporaryModifierDataPath = "TemporaryMofiers";
    private static List<TemporaryModifierData> TemporaryModifierDatas = new();

    public static void CalculateHit(RaycastHit hitInfo, ShellInfo shellInfo, Vector3 direction)
    {
        var hullArmor = hitInfo.collider.GetComponentInParent<HullArmor>();
        var tankBehaviour = hitInfo.collider.GetComponentInParent<TankBehaviour>();


        if (hullArmor == null)
            return;

        var armorStat = hullArmor.GetArmorStat(hitInfo.collider);

        float impactAngleRad = Vector3.Angle(hitInfo.normal, -direction) * Mathf.Deg2Rad;

        float effectiveArmor = armorStat.ArmorThickness / Mathf.Max(Mathf.Cos(impactAngleRad), 0.05f);

        int damage = shellInfo.Damage;
        int critChance = Base_Critical_Chance;

        GetCriticalDamage(tankBehaviour, armorStat, critChance);

        if (shellInfo.Penetration >= effectiveArmor)
        {
            tankBehaviour.TankHealthSystem.CurrentHealth -= damage;

            Debug.Log("Last health " + tankBehaviour.TankHealthSystem.CurrentHealth + "Penetrated " + effectiveArmor + "Shell " + shellInfo.Penetration);
        }
        else
        {
            Debug.Log($"Not penetrated" + "Shell " + shellInfo.Penetration + " effective armor: " + effectiveArmor);
        }
    }

    private static void GetCriticalDamage(TankBehaviour tankBehaviour, ArmorStat armorStat, float chance)
    {
        switch (armorStat.Side)
        {
            case ArmorSide.Front:
                chance *= 0.5f;

                break;
            case ArmorSide.Side:
                chance *= 1.3f;

                break;
            case ArmorSide.Rear:
                chance *= 1.8f;
                break;
        }

        float rng = Random.Range(0, 100);

        if (chance < rng)
            return;

        if (TemporaryModifierDatas.Count == 0)
            TemporaryModifierDatas.AddRange(Resources.LoadAll<TemporaryModifierData>(TemporaryModifierDataPath));

        List<TemporaryModifierData> datas = new();

        foreach (var item in TemporaryModifierDatas)
        {
            if(!tankBehaviour.IsContainThisModifier(item))
            datas.Add(item);
        }

        var temporaryModifier = datas[Random.Range(0, datas.Count)].GetModifier();

        tankBehaviour.AddTemporaryModifier(temporaryModifier);

    }
}
