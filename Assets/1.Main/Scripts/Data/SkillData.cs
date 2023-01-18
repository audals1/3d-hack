using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DebuffType
{
    None = -1,
    Knockdown,
    Stun,
    Max
}
public class SkillData
{
    public static float MaxKnockbackDist = 5f;
    public static float MaxKnockbackDuration = 1f;
    public int m_attackArea;
    public float m_knockbackDist;
    public int m_effectId;
    public DebuffType m_debuff;
    public float m_debuffDration;
    public float m_coolTime;
    public float m_attack;
}

