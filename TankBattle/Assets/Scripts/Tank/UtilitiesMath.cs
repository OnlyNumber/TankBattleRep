using UnityEngine;

public static class UtilitiesMath
{
    public static void CalculateHit(RaycastHit hitInfo, ShellInfo shellInfo, Vector3 direction)
    {
        var hullArmor = hitInfo.collider.GetComponentInParent<HullArmor>();

        if (hullArmor == null)
            return;

        var armorStat = hullArmor.GetArmorStat(hitInfo.collider);

        float impactAngleRad = Vector3.Angle(hitInfo.normal, -direction) * Mathf.Deg2Rad;

        float effectiveArmor = armorStat.ArmorThickness / Mathf.Max(Mathf.Cos(impactAngleRad), 0.05f);

        int damage = shellInfo.Damage;

        switch (armorStat.Side)
        {
            case ArmorSide.Front:
                damage *= 1;

                break;
            case ArmorSide.Side:
                damage = (int)((float)damage * 1.25f);

                break;
            case ArmorSide.rear:
                damage = (int)((float)damage * 1.5f);

                break;
        }

        if (shellInfo.Penetration >= effectiveArmor)
        {
            hullArmor.Changehealth(-damage);

            Debug.Log("Last health " + hullArmor.GetCurrentHealth());
        }
        else
        {
            Debug.Log($"Not penetrated, effective armor: " + effectiveArmor);
        }
    }
}
