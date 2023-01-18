using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Status
{
    public int m_hp;
    public int m_hpMax;
    public float m_hitRate;
    public float m_dodgeRate;
    public float m_criRate;
    public float m_attack;
    public float m_defence;
    public float m_criAttack;

    public Status(int hp, float hitRate, float dodgeRate, float crirate, float attack, float defence, float criAttack)
    {
        m_hp = m_hpMax = hp;
        m_hitRate = hitRate;
        m_dodgeRate = dodgeRate;
        m_criRate = crirate;
        m_attack = attack;
        m_defence = defence;
        m_criAttack = criAttack;
    }
}
