using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    Normal,
    Critical,
    Miss,
    Max
}
public class DamageCal
{
    public static bool AttackDecision(float attackerHit, float defenceDodge)
    {
        if (Mathf.Approximately(attackerHit, 100f) || attackerHit > 100f)
            return true;
        float total = attackerHit + defenceDodge;
        float hitRate = Random.Range(0, total + 1);
        if (hitRate <= attackerHit)
        {
            return true;
        }
        return false;
    }
    public static float NormalDamage(float attackerAtk, float skillAtk, float defenceDef)
    {
        float attack = attackerAtk + (attackerAtk * skillAtk / 100.0f);
        float result = attack - defenceDef;
        if (result < 0)
        {
            result = 1;
        }
        else
        {
            result = attack - defenceDef;
        }
        return result;
    }
    public static bool CriticalDecision(float criRate)
    {
        int value = Random.Range(1, 101);
        if (value <= criRate)
        {
            return true;
        }
        return false;
    }
    public static float CriticalDamage(float damage, float criAtk)
    {
        return damage + (damage * criAtk / 100.0f);
    }
}

