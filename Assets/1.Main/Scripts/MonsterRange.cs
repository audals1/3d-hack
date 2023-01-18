using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRange : MonsterController
{
    #region Filed
    [SerializeField]
    GameObject m_magicObj;
    [SerializeField]
    Transform ShamanHUD;
    MonsterAnimController.AnimState[] m_attackMotions = { MonsterAnimController.AnimState.Attack1, MonsterAnimController.AnimState.Attack2 };
    #endregion
    #region Methods
    void SetAttack()
    {
        SetState(MonsterState.Attack);
        m_monAnimCtr.Play(m_attackMotions[Random.Range(0, m_attackMotions.Length)]);
    }
    protected override void InitStatus()
    {
        m_status = new Status(20, 20f, 20f, 30f, 45f, 20f, 100f);
    }
    public override void InitMonster(MonsterManager.MonsterType type)
    {
        m_attackDist = 3.5f;
        m_detectDist = 25f;
        m_sqrAttackDist = Mathf.Pow(m_attackDist, 2f);
        m_sqrDetectDist = Mathf.Pow(m_detectDist, 2f);
        m_idleDuration = 2f;
        Type = type;
        SetHud(ShamanHUD);
        InitStatus();
    }
    public override void SetDamage(AttackType attackType, float damage, SkillData skilldata)
    {
        if (IsDie) return;

        m_hudCtr.ShowHud();
        m_status.m_hp -= Mathf.CeilToInt(damage);
        m_hudCtr.DisplayDamage(attackType, damage, m_status.m_hp / (float)m_status.m_hpMax);
        if (attackType == AttackType.Miss) return;
        m_navAgent.ResetPath();
        m_navAgent.isStopped = true;
        StartCoroutine(SetColor(Color.red, 0.3f));
        SetState(MonsterState.Damaged);
        m_monAnimCtr.Play(MonsterAnimController.AnimState.Hit, false);
        
        if (m_status.m_hp <= 0)
        {
            SetState(MonsterState.Die);
            m_monAnimCtr.Play(MonsterAnimController.AnimState.Die);
            m_collider.enabled = false;
        }
    }
    protected override void BehaviourProcess()
    {
        switch (m_state)
        {
            case MonsterState.Idle:
                m_idleTime += Time.deltaTime;
                if (m_idleTime >= m_idleDuration)
                {
                    if (FindTarget(m_player.transform.position)) //플레이어 인식 검사
                    {
                        if (CheckArea(m_player.transform.position, m_sqrAttackDist)) // 공격가능거리 도달
                        {
                            var dir = m_player.transform.position - transform.position;
                            dir.y = 0f;
                            transform.forward = dir;
                            SetAttack();
                            return;
                        }
                        SetState(MonsterState.Chase);
                        m_navAgent.isStopped = false;
                        m_monAnimCtr.Play(MonsterAnimController.AnimState.Run);
                        StartCoroutine("Coroutine_SerchTarget");
                        m_navAgent.stoppingDistance = m_attackDist;
                    }
                    else //거리에 플레이어 없음
                    {
                        SetIdle(0f);
                    }
                    m_idleTime = 0f;
                }
                break;
            case MonsterState.Chase:
                if (FindTarget(m_player.transform.position))
                {
                    if (CheckArea(m_player.transform.position, m_sqrAttackDist))
                    {
                        SetIdle(1f);
                    }
                }
                else
                {
                    SetIdle(0f);
                }
                break;
        }
    }
    #endregion
    #region Anim_Event
    void AnimEvent_Magicball()
    {
        MagicManager.Instance.CreateMagic1();
    }
    void AnimEvent_MagicRocet()
    {
        MagicManager.Instance.CreateMagic2();
    }
    #endregion

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        InitMonster(MonsterManager.MonsterType.GoblinShaman);
    }
    protected override void Update()
    {
        BehaviourProcess();
    }
}
